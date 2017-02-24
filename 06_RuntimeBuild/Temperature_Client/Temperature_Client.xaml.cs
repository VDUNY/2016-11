using System;
using System.Windows;
using System.Windows.Controls;

using System.ComponentModel.Composition;    // Export[]; reqs ref to System.ComponentModel.Composition
using System.Threading;
using System.Reflection;    // Assembly; compilation reqs ref to Microsoft.CSharp

using IWx_Client;
using IWx_Station;

namespace Wind_Client
{
    /// <summary>
    /// Interaction logic for TemperatureClient.xaml
    /// </summary>
    public partial class TemperatureClient : UserControl, IWxClient
    {

        string[] m_cmdArgs = null;
        dynamic m_station = null;
        ManualResetEvent m_mreRun = null;

        /// <summary>
        /// Will generally be called by .net MEF runtime.
        /// Called when the MEF GetExportedValues() method is run.
        /// </summary>
        public TemperatureClient()
        {
            InitializeComponent();
            m_cmdArgs = Environment.GetCommandLineArgs();
            if ((m_cmdArgs[1].Split(new char[] { ':' })[1].Equals("true")))
            {
                // using dynamics reqs that the build dir for the client have the needed dlls
                // by default, VS copies any ref'd  dlls to the build dir, typically \debug\bin.
                // with dynamics, there is no longer any ref to the dll, so they are NOT copied to the \bin\debug dir.
                Assembly dynamicAssembly = Assembly.LoadFrom("Wx_Station.dll");
                Type type = dynamicAssembly.GetType("Wx_Station.WxStation");
                m_station = Activator.CreateInstance(type);
            }
            else if ((m_cmdArgs[1].Split(new char[] { ':' })[1].Equals("false")))
            {
                // we can remove the hard-coded strings by loading the desired dlls from a config file, cmd line param or data source.
                Assembly dynamicAssembly = Assembly.LoadFrom("Wx_Station_Sim.dll");
                Type type = dynamicAssembly.GetType("Wx_Station_Sim.WxStationSim");
                m_station = Activator.CreateInstance(type);
            }
            // must cast because += operator can not be applied to type dynamic and method group
            ((IWxStation)m_station).ChangedTemperature += m_station_ChangedTemperature;
            ((IWxStation)m_station).ChangedSetting += m_station_ChangedSetting;

        }

        /// <summary>
        /// tell the server that we are ready to receive data updates 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdStart_Click(object sender, RoutedEventArgs e)
        {
            m_mreRun = new ManualResetEvent(false);
            m_station.Initialize();
            (new Thread(new ThreadStart(() => { m_station.Run(); }))).Start();
            cmdStop.IsEnabled = true;
            cmdStart.IsEnabled = false;
        }

        /// <summary>
        /// no longer interested in wx data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdStop_Click(object sender, RoutedEventArgs e)
        {
            Stopping();
            cmdStop.IsEnabled = false;
            cmdStart.IsEnabled = true;
        }

        private void TemperatureClient_Unloaded(object sender, RoutedEventArgs e)
        {
            Stopping();
        }

        /// <summary>
        /// handle whether the program is shutting down or simply not interested in updated wx data
        /// </summary>
        private void Stopping()
        {
            m_station.Quit();
            if (m_mreRun != null) m_mreRun.Set();
        }

        /// <summary>
        /// Wx Station raises event for this handler.
        /// </summary>
        /// <param name="humidity"></param>
        void m_station_ChangedTemperature(float humidity)
        {
            lblTemperature.Dispatcher.BeginInvoke((Action)(() => { lblTemperature.Content = humidity.ToString("0.0"); }));
        }

        /// <summary>
        /// Wx Station raises event for this handler.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="value"></param>
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
            Stopping();
        }

        public string ServiceName
        {
            get { return " Temperature Control"; }
        }

        /*****************  MEF   **************************/

    }   // public partial class TemperatureClient : UserControl, IWxClient
}   // namespace Wind_Client
