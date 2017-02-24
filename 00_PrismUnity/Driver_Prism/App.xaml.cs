using System;
using System.Configuration;
using System.Windows;

namespace Driver_Prism
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Override because this is the method that starts a wpf app.
        /// Instead of starting the uri (which we removed), start the bootstrapper instead.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DriverBootstrapper app = new DriverBootstrapper();
            app.Run();
        }
    }
}
