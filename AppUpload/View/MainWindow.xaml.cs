using System.Windows;
using AppUpload.ViewModel;

namespace AppUpload.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new AppUploadViewModel();
            InitializeComponent();
        }
    }
}
