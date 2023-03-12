using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace AppUpload.Model
{
    public class ApplicationDetails
    {

        #region Fields
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger
                (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Properties
        public string ApplicationName { get; set; }
        public string UriSource { get; set; }
        public string PathToApp { get; set; }
        public ICommand ButtonRunApps { get; set; }

        public OperationEventArgs OperationEventArg { get; set; }

        public event EventHandler<OperationEventArgs> OnProcessStarted;

        #endregion

        #region CTOR
        public ApplicationDetails(string pApplicationName, string pUriSource, string pPathToApp)
        {
            OperationEventArg = new OperationEventArgs();
            ButtonRunApps = new RelayCommand((parameter) => true, RunProcess);
            ApplicationName = pApplicationName;
            UriSource = pUriSource;
            PathToApp = pPathToApp;
        }

        #endregion

        #region Methods

        public void RunProcess(object parameter)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = PathToApp
                    }
                };

                OperationEventArg.ToMaximizeWindow = false;

                OnProcessStarted?.Invoke(this, OperationEventArg);

                process.Start();

                process.WaitForExit();

                OperationEventArg.ToMaximizeWindow = true;

                OnProcessStarted?.Invoke(this, OperationEventArg);

            }
            catch (Exception ex)
            {
                OperationEventArg.ToMaximizeWindow = true;

                OnProcessStarted?.Invoke(this, OperationEventArg);

                log.Error(@$"ApplicationName:{ApplicationName}
                                    UriSource:{UriSource}
                                    PathToApp:{PathToApp}
                                    Exception Message: {ex}");

                MessageBox.Show($"Error in upload process.\nProcess Path:\n{PathToApp}\nFor more information go to log",
                    "Upload Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override string ToString()
        {
            return ApplicationName;
        }

        #endregion
    }

    public class OperationEventArgs : EventArgs
    {
        public bool ToMaximizeWindow { get; set; }
    }
}
