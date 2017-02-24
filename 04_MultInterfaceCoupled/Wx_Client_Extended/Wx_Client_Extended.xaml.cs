/*
 * cmd line param: 
 *  /ActualStation:true -> real weather data
 *  /ActualStation:false -> simulated weather data
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using IWx_Station;
using IWx_Station_Extended;

using Wx_Station;   // requires reference to the assy 
using Wx_Station_Sim;   // requires reference to the assy 


using System.Reflection;    // Assembly; compilation reqs ref to Microsoft.CSharp


namespace Wx_Client_Extended
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] m_cmdArgs = null;
        ManualResetEvent m_mreRun = null;
        dynamic m_station = null;

        /* testing is now much easier because we swap in the simulator with a cmd line option */
        public MainWindow()
        {
            InitializeComponent();
            m_cmdArgs = Environment.GetCommandLineArgs();
            if ((m_cmdArgs[1].Split(new char[]{ ':'})[1].Equals("true")))
            {
                m_station = new WxStation(); // concrete instantiation; must consider thrown exceptions 
            }
            else if ((m_cmdArgs[1].Split(new char[] { ':' })[1].Equals("false")))
            {
                m_station = new WxStationSim(); // concrete instantiation; must consider thrown exceptions
            }
            m_station.Initialize();
            // ManualResetEvent is a good way to avoid the bool bContinue = true; while (bContinue) { ... } snippet.
            // WaitOne(int msec) returns false if reset/unsignaled and true if set/signaled.
            m_mreRun = new ManualResetEvent(false);
            ((IWxStation)m_station).ChangedTemperature += m_station_ChangedTemperature;
            ((IWxStation)m_station).ChangedWindDirection += m_station_ChangedWindDirection;
            ((IWxStation)m_station).ChangedWindSpeed +=m_station_ChangedWindSpeed;
            ((IWxStationExtended)m_station).ChangedHumidity += m_station_ChangedHumidity;
            ((IWxStation)m_station).ChangedSetting += m_station_ChangedSetting;
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
                case "WindSpeed":
                    lblWindSpeed.Dispatcher.BeginInvoke((Action)(() => { lblWindSpeed.Content = val; }));
                    break;
                case "WindDirection":
                    lblWindDirection.Dispatcher.BeginInvoke((Action)(() => { lblWindDirection.Content = val; }));
                    break;
                case "Humidity":
                    lblHumidity.Dispatcher.BeginInvoke((Action)(() => { lblHumidity.Content = val; }));
                    break;
            }
        }

        /// <summary>
        /// Wx Station raises event for this handler.
        /// </summary>
        /// <param name="direction"></param>
        void m_station_ChangedWindDirection(float direction)
        {
            lblWindDirection.Dispatcher.BeginInvoke((Action)(() => { lblWindDirection.Content = direction.ToString("0.0"); }));
        }

        /// <summary>
        /// Wx Station raises event for this handler.
        /// </summary>
        /// <param name="speed"></param>
        void m_station_ChangedWindSpeed(float speed)
        {
            lblWindSpeed.Dispatcher.BeginInvoke((Action)(() => { lblWindSpeed.Content = speed.ToString("0.0"); }));
        }

        /// <summary>
        /// Wx Station raises event for this handler.
        /// </summary>
        /// <param name="temperature"></param>
        void m_station_ChangedTemperature(float temperature)
        {
            lblTemperature.Dispatcher.BeginInvoke((Action)(() => { lblTemperature.Content = temperature.ToString("0.0"); }));
        }

        /// <summary>
        /// Wx Station raises event for this handler.
        /// </summary>
        /// <param name="humidity"></param>
        void m_station_ChangedHumidity(float humidity)
        {
            lblHumidity.Dispatcher.BeginInvoke((Action)(() => { lblHumidity.Content = humidity.ToString("0.0"); }));
        }

        /// <summary>
        /// tell the server that we are ready to receive data updates 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdStart_Click(object sender, RoutedEventArgs e)
        {
            m_mreRun.Reset();
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

        /// <summary>
        /// client shutting down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WxClient_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Stopping();
        }

        /// <summary>
        /// handle whether the program is shutting down or simply not interested in updated wx data
        /// </summary>
        private void Stopping()
        {
            m_station.Quit();
            m_mreRun.Set();
        }

    }   //     public partial class MainWindow : Window

}   // namespace Wx_Client_Extended
