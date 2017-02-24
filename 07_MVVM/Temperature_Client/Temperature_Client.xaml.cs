using System;
using System.Windows;
using System.Windows.Controls;

using System.ComponentModel.Composition;    // Export[]; reqs ref to System.ComponentModel.Composition

using IWx_Client;
using Temperature_ViewModel;

namespace Wind_Client
{
    /// <summary>
    /// Interaction logic for TemperatureClient.xaml
    /// </summary>
    public partial class TemperatureClient : UserControl, IWxClient
    {

        string[] m_cmdArgs = null;
        TemperatureViewModel vm = null;

        /// <summary>
        /// Will generally be called by .net MEF runtime.
        /// Called when the MEF GetExportedValues() method is run.
        /// </summary>
        public TemperatureClient()
        {
            InitializeComponent();
            vm = new TemperatureViewModel();
            this.DataContext = vm;
        }

        private void TemperatureClient_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Wx Station
        /// </summary>
        /// <param name="direction"></param>
        void m_station_ChangedTemperature(float humidity)
        {
            lblTemperature.Dispatcher.BeginInvoke((Action)(() => { lblTemperature.Content = humidity.ToString("0.0"); }));
        }

        /// <summary>
        /// Wx Station simulator
        /// </summary>
        /// <param name="direction"></param>
        void m_station_ChangedSetting(string setting, string value)
        {
            string val = value.Substring(0, value.IndexOf('.') + 2);
            switch (setting)
            {
                case "Temperature":
                    lblTemperature.Dispatcher.BeginInvoke((Action)(() => { lblTemperature.Content = val; }));
                    break;
            }
        }

        /*****************  MEF   **************************/
        /// <summary>
        /// allows MEF to make this WPF control/MVVM view available to other apps by discovery at runtime
        /// </summary>
        [Export(typeof(IWxClient))]
        public IWxClient Window
        {
            get { return this; }
        }

        public void Close()
        {
            // could call Dispose() if IDispose implemented
            cmdStop.Command.Execute(null);
        }

        public string ServiceName
        {
            get { return " Temperature Control"; }
        }

        /*****************  MEF   **************************/
    }
}
