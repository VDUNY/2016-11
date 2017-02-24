/*
 * cmd line param: 
 *  /ActualStation:true -> real weather data
 *  /ActualStation:false -> simulated weather data
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading; /* ManualResetEvent */
using IWx_Station;  /* handle both wx server and wx simulator */
using Wx_Station;   /* requires reference to the assy */
using Wx_Station_Sim;   /* requires reference to the assy */

namespace Wx_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] m_cmdArgs = null;
        ManualResetEvent m_mreRun = null;
        IWxStation m_station = null; /* reference to assy */

        /* testing is now much easier because we swap in the simulator with a cmd line option */
        public MainWindow()
        {
            InitializeComponent();
            m_cmdArgs = Environment.GetCommandLineArgs();   // check to see if we want wx server or wx simulator
            if ((m_cmdArgs[1].Split(new char[]{ ':'})[1].Equals("true")))
            {
                m_station = new WxStation(); /* concrete instantiation; must consider thrown exceptions */
            }
            else if ((m_cmdArgs[1].Split(new char[] { ':' })[1].Equals("false")))
            {
                m_station = new WxStationSim(); /* concrete instantiation; must consider thrown exceptions */
            }
            m_station.Initialize();
            m_mreRun = new ManualResetEvent(false); // Sets the state of the event to not signaled, causing one or more threads to block.
        }

        /// <summary>
        /// tell the server that we will be polling for wx data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdStart_Click(object sender, RoutedEventArgs e)
        {
            m_mreRun.Reset();   // now unsignaled
            m_station.Initialize();
            // a lambda expression is an anonymous function that can be used to create delegates
            // ThreadStart ctor takes a target method.
            (new Thread(new ThreadStart(() => { m_station.Run(); }))).Start();
            (new Thread
                (new ThreadStart(() =>
                    {
                        // if reset/unsignaled, then wait and enter loop else skip loop.
                        while (!m_mreRun.WaitOne(100))
                        {
                            /* each of these loosely couple the user interface to the interface, but we still need concrete model(s) as references */
                            /* must consider thrown exceptions and/or error msgs */
                            /* concrete server implementation is bound to context switch onto the UI Dispatcher thread. */
                            /* interface implementation is bound to context switch onto the UI Dispatcher thread. */
                            /* BeginInvoke() takes a delegate ... not an anonymous function. */
                            /* Without using MVVM, we are storing UI design and UI state in the same assy. */
                            lblTemperature.Dispatcher.BeginInvoke((Action)(() => { lblTemperature.Content = m_station.Temperature().ToString("0.0"); }));
                            lblWindSpeed.Dispatcher.BeginInvoke((Action)(() => { lblWindSpeed.Content = m_station.WindSpeed().ToString("0.0"); }));
                            lblWindDirection.Dispatcher.BeginInvoke((Action)(() => { lblWindDirection.Content = m_station.WindDirection().ToString("0.0"); }));
                        }
                    }
                    )
                )
            ).Start();
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

    }   // public partial class MainWindow : Window
}   // namespace Wx_Client
