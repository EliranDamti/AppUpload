using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using AppUpload.Model;

namespace AppUpload.ViewModel
{
    public class AppUploadViewModel : INotifyPropertyChanged
    {
        #region Fields
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger
                (System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        #endregion

        #region Properties

        public ICommand Exit { get; set; }
        public ObservableCollection<ApplicationDetails> ApplicationObservableCollection { get; set; }
        public WindowState AppWindowState { get; set; }

        #endregion

        #region CTOR

        public AppUploadViewModel()
        {
            AppWindowState = WindowState.Maximized;

            Exit = new RelayCommand((_) => true, Close);

            try
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(@"./Config/AppUploadConfig.xml");

                var components = doc.DocumentElement?.SelectSingleNode("/configuration/Components");

                ApplicationObservableCollection = new ObservableCollection<ApplicationDetails>();

                foreach (XmlNode node in components!)
                {
                    var item = new ApplicationDetails(
                        node["ApplicationName"]?.InnerText,
                        node["UriSource"]?.InnerText,
                        node["PathToApp"]?.InnerText
                    );

                    item.OnProcessStarted += Item_OnProcessStarted;

                    ApplicationObservableCollection.Add(item);
                }

            }
            catch (Exception ex)
            {
                Log.Error(@$"Config File Error, CTOR - [AppUploadViewModel]
                                    Exception Message: {ex}");

                MessageBox.Show("Error in parsing the config file",
                    "Config File Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion


        #region Methods


        private void Close(object pParameter)
        {
            Log.Info("[close] EXIT button was pressed");

            var result = MessageBox.Show("Are you sure you want to exit?",
                "Question", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                ShutdownAndLogoffApplication();
            }
        }

        public static void ShutdownAndLogoffApplication()
        {
            try
            {
                [DllImport("wtsapi32.dll", SetLastError = true)]
                static extern bool WTSDisconnectSession(IntPtr hServer, int sessionId, bool bWait);

                const int wtsCurrentSession = -1;

                if (!WTSDisconnectSession(IntPtr.Zero,
                        wtsCurrentSession, false))
                {
                    throw new Win32Exception();
                }
                
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                Log.Error($"[ShutdownApplication] Shutdown failure\n{ex}");
            }
        }

        private void Item_OnProcessStarted(object sender, OperationEventArgs e)
        {
            AppWindowState = e.ToMaximizeWindow ? WindowState.Maximized : WindowState.Minimized;

            OnPropertyChanged(nameof(AppWindowState));
        }

        #region  Implementation INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion


    }
}
