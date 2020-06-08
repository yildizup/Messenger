using System.Windows;
using Telefonico.Core;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Benutzerdefinierter Startup, sodass wir den IoC zuallererst laden können.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IoCContainer.Setup();

            // Main Window anzeigen
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }

    }
}
