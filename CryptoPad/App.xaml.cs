using System.Text;
using System.Windows;
using CryptoPad.Views;

namespace CryptoPad
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            base.OnStartup(e);

            this.MainWindow = new MainWindow();
            MainWindow.Show();
        }
    }
}
