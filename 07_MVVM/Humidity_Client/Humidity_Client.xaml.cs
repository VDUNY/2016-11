using System;
using System.Windows;
using System.Windows.Controls;

using System.ComponentModel.Composition;    // Export[]; reqs ref to System.ComponentModel.Composition

using IWx_Client;
using Humidity_ViewModel;

namespace Wind_Client
{
    /// <summary>
    /// Interaction logic for HumidityClient.xaml
    /// </summary>
    public partial class HumidityClient : UserControl, IWxClient
    {
        string[] m_cmdArgs = null;
        HumidityViewModel vm = null;

        /// <summary>
        /// Will generally be called by .net MEF runtime.
        /// Called when the MEF GetExportedValues() method is run.
        /// </summary>
        public HumidityClient()
        {
            InitializeComponent();
            vm = new HumidityViewModel();
            this.DataContext = vm;
        }

        private void HumidityClient_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Stopping() { }

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
            get { return " Humidity Control"; }
        }

        /*****************  MEF   **************************/
    }
}
