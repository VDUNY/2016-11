using RuntimeComponent = IWx_Client.IWxClient; // for those times when the namespace and class name are identical,

/*
 * cmd line param: 
 *  /ActualStation:true -> real weather data
 *  /ActualStation:false -> simulated weather data
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Collections.Generic;       // IEnumerable<T>

using System.ComponentModel.Composition;    // MEF; reqs ref to System.ComponentModel.Composition
using System.ComponentModel.Composition.Hosting;    // MEF; AggregateCatalog

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

        [Import(typeof(RuntimeComponent))]  // this is the IWx_Client interface
        // we want MEF to find user controls that satisfy this specification
        public RuntimeComponent m_runtimePart { get; set; }
        IEnumerable<RuntimeComponent> m_runtimeParts = null;   // all the parts that MEF brings in

        public MainWindow()
        {
            InitializeComponent();
            m_cmdArgs = Environment.GetCommandLineArgs();

            /*************   MEF  *********************/
            try
            {       // let MEF build the app out of discovered parts
                var catalog = new AggregateCatalog();   // var type can not be init'd to null
                // BadFormatImageException occurs as a first chance exception if attempting to load a flat dll
                // as they are w/o a manifest, some32bitstd.dll. It can be safely ignored. 
                // The code will continue to load .net dlls relevant to using the Managed Extension Framework (MEF).
                // 'relevant' means the component exports the IWx_Client interface
                catalog = new AggregateCatalog(new DirectoryCatalog("."),
                        new AssemblyCatalog(Assembly.GetExecutingAssembly()));
                var container = new CompositionContainer(catalog);
                m_runtimeParts = container.GetExportedValues<RuntimeComponent>();  // ctors of the individual user components for the views called here
            }
            catch (ReflectionTypeLoadException)
            {
            }
            catch (Exception)
            {
            }
            foreach (RuntimeComponent win in m_runtimeParts)    // add the component to the ui grid + a separator line
            {
                RowDefinition gridRow = new RowDefinition();
                gridMain.RowDefinitions.Add(gridRow);
                StackPanel stackPnl = new StackPanel();
                stackPnl.Children.Add((UIElement)win);
                Grid.SetRow(stackPnl, gridMain.RowDefinitions.Count-1);
                gridMain.Children.Add(stackPnl);
                RowDefinition gridRowBorder = new RowDefinition();
                gridMain.RowDefinitions.Add(gridRowBorder);
                Border border = new Border();
                Grid.SetRow(border, gridMain.RowDefinitions.Count - 1);
                border.Background = System.Windows.Media.Brushes.LemonChiffon;
                border.Height = 10;
                gridMain.Children.Add(border);
            }
            /*************   MEF  *********************/
        }

        /// <summary>
        /// each of the dynamically loaded ui components need to be closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WxClient_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (RuntimeComponent win in m_runtimeParts)
            {
                win.Close();
            }
        }

    }   // public partial class MainWindow : Window
}   // namespace Wx_Client_Extended
