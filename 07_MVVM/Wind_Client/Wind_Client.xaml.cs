using System;
using System.Windows;
using System.Windows.Controls;

using System.ComponentModel.Composition;    // Export[]; reqs ref to System.ComponentModel.Composition

using IWx_Client;
using Wind_ViewModel;

namespace Wind_Client
{
    /// <summary>
    /// Interaction logic for WindClient.xaml
    /// </summary>
    public partial class WindClient : UserControl, IWxClient
    {

        WindViewModel vm = null;        

        /// <summary>
        /// Will generally be called by .net MEF runtime.
        /// Called when the MEF GetExportedValues() method is run.
        /// </summary>
        public WindClient()
        {
            InitializeComponent();
            vm = new WindViewModel();
            this.DataContext = vm;
        }

        private void WindClient_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Wx Station
        /// </summary>
        /// <param name="direction"></param>
        void m_station_ChangedWindSpeed(float speed)
        {
            lblSpeed.Dispatcher.BeginInvoke((Action)(() => { lblSpeed.Content = speed.ToString("0.0"); }));
        }

        /// <summary>
        /// Wx Station
        /// </summary>
        /// <param name="direction"></param>
        void m_station_ChangedWindDir(float direction)
        {
            lblDir.Dispatcher.BeginInvoke((Action)(() => { lblDir.Content = direction.ToString("0.0"); }));
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
                case "WindSpeed":
                    lblSpeed.Dispatcher.BeginInvoke((Action)(() => { lblSpeed.Content = val; }));
                    break;
                case "WindDirection":
                    lblDir.Dispatcher.BeginInvoke((Action)(() => { lblDir.Content = val; }));
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
            get { return " Wind Control"; }
        }

        /*****************  MEF   **************************/
    }
}
