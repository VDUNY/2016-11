using System;

using System.Threading;
using System.Windows.Input;
using System.Reflection;    // Assembly; compilation reqs ref to Microsoft.CSharp

using IWx_Station;
using IWx_Station_Extended;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;

/*
 * cmd line param: 
 *  /ActualStation:true -> real weather data
 *  /ActualStation:false -> simulated weather data
*/


namespace Humidity_ViewModel
{
    public class HumidityViewModel :BindableBase
    {

        string[] m_cmdArgs = null;
        dynamic m_station = null;

        private Single m_humidity = -9999.0f;
        private bool m_isRunning = false;
        public HumidityViewModel() 
        {
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
            ((IWxStationExtended)m_station).ChangedHumidity += m_station_ChangedHumidity;
            ((IWxStation)m_station).ChangedSetting += m_station_ChangedSetting;

            StartAcq = new DelegateCommand<object>(param => { this.OnStartAcq(param); }, this.CanStartAcq);
            StopAcq =  new DelegateCommand<object>(param => { this.OnStopAcq(param);  }, this.CanStopAcq);

            m_station.Initialize();

        }

        public Single Humidity
        {
            get { return m_humidity; }
            set { SetProperty<Single>(ref m_humidity, value); OnPropertyChanged( () => Humidity); }
        }

        public ICommand StartAcq { get; private set; }

        public bool CanStartAcq(object obj) 
        {
            if (m_isRunning) return false;
            else return true;
        }

        private void OnStartAcq(object arg) 
        {
            (new Thread(new ThreadStart(() => { m_station.Run(); }))).Start();
            m_isRunning = true;
            ((DelegateCommand<object>)StartAcq).RaiseCanExecuteChanged();   // tell ui that status has changed
            ((DelegateCommand<object>)StopAcq).RaiseCanExecuteChanged();    // tell ui that status has changed
        }

        public ICommand StopAcq { get; private set; }

        private void OnStopAcq(object arg) 
        {
            (new Thread(new ThreadStart(() => { m_station.Quit(); }))).Start();
            m_isRunning = false;
            ((DelegateCommand<object>)StartAcq).RaiseCanExecuteChanged();   // tell ui that status has changed
            ((DelegateCommand<object>)StopAcq).RaiseCanExecuteChanged();    // tell ui that status has changed
        }

        public bool CanStopAcq(object obj)
        {
            if (m_isRunning) return true;
            else return false;
        }

        /// <summary>
        /// Wx Station
        /// </summary>
        /// <param name="direction"></param>
        void m_station_ChangedHumidity(float humidity)
        {
            Humidity = humidity;
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
                case "Humidity":
                    Humidity = Convert.ToSingle(value);
                    break;
            }
        }



    }
}
