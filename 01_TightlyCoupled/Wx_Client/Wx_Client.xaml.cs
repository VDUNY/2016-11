using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading; /* ManualResetEvent */
using Wx_Station;   /* requires reference to the assy */

namespace Wx_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ManualResetEvent m_mreRun = null;
        WxStation m_station = null; /* reference to assy */

        public MainWindow()
        {
            InitializeComponent();
            m_station = new WxStation(); /* concrete instantiation; must consider thrown exceptions and error handling */
            m_station.Initialize();
            // ManualResetEvent is a good way to avoid the bool bContinue = true; while (bContinue) { ... } snippet.
            // WaitOne(int msec) returns false if reset/unsignaled and true if set/signaled.
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
                // a lambda expression is an anonymous function that can be used to create delegates
                // ThreadStart ctor takes a target method.
            (new Thread(new ThreadStart( () => { m_station.Run(); } ) )).Start();
            // we need a thread to be able to exit the event handler and ui state to update.
            (new Thread
                (new ThreadStart(() =>
                    {
                            // if reset/unsignaled, then wait and enter loop else skip loop.
                        while (!m_mreRun.WaitOne(100))  // while (bContinue) { ... }

                        {
                            /* each of these tightly couple the user interface to the reference'd assy */
                            /* must consider thrown exceptions and/or error msgs */
                            /* concrete server implementation is bound to context switch onto the UI Dispatcher thread. */
                            /* Without using MVVM, we are storing UI design and UI state in the same assy. */
                            /* BeginInvoke() takes a delegate ... not an anonymous function. */
                            /* Action delegate allows one to pass a method as a parameter without having create a custom delegate. */
                            lblTemperature.Dispatcher.BeginInvoke((Action)(() => { lblTemperature.Content = m_station.Temperature.ToString("0.0"); }));
                            lblWindSpeed.Dispatcher.BeginInvoke((Action)(() => { lblWindSpeed.Content = m_station.WindSpeed.ToString("0.0"); }));
                            lblWindDirection.Dispatcher.BeginInvoke((Action)(() => { lblWindDirection.Content = m_station.WindDirection.ToString("0.0"); }));
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
