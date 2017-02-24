﻿using System;
using System.Threading;
using IWx_Station;

namespace Wx_Station_Sim
{
    /* With an interface, the user does not access a concrete implementation of this assy. */
    /* Any changes to this class will require re-compilation of the any assys that reference this assy. */
    public class WxStationSim : IWxStation
    {
        // ManualResetEvent is a good way to avoid the bool bContinue = true; while (bContinue) { ... } snippet.
        // WaitOne(int msec) returns false if reset/unsignaled and true if set/signaled.
        private ManualResetEvent m_mreRun = null;
        private Single m_temperature = -1.0f;
        private Single m_windSpeed = -1.0f;
        private Single m_windDirection = -1.0f;

        // client subscribes to these events, if interested in data. Server does not pump data unless requested.
        public event DelChangedTemperature ChangedTemperature;
        public event DelChangedWindDirection ChangedWindDirection;
        public event DelChangedWindSpeed ChangedWindSpeed;
        public event DelChangedSetting ChangedSetting;
        public event DelExceptionRaised ExceptionRaised;

        public WxStationSim() { }

        // What if user forgets to call the Initialize method? */
        public void Initialize() { m_mreRun = new ManualResetEvent(false); }

        /* Any changes to the Run() method require us to test the client for side effects. */
        /* Since the client must call the Run() method, there is no opportunity for the server to acquire indepedently. */
        public void Run()
        {
            int i = 0;
            while (!m_mreRun.WaitOne(1000))
            {
                /* Altho' we need to handle exceptions, it need not propagate back to the client.
                /* An exception here could crash the user's app. */
                /* If we catch and handle, then we need some error mechanism to inform the client. */
                m_temperature = (Single)i * -1.0f;
                m_windSpeed = (Single)i * -1.0f;
                m_windDirection = (Single)i * -1.0f;
                if (ChangedSetting != null)
                {
                    ChangedSetting("Temperature", m_temperature.ToString("0.0"));
                    ChangedSetting("WindSpeed", m_windSpeed.ToString("0.0"));
                    ChangedSetting("WindDirection", m_windDirection.ToString("0.0"));
                }
                i++; if (i > 9) { i = 0; }
            }
        }

        /// <summary>
        /// no longer interested in collecting data
        /// </summary>
        public void Quit() { m_mreRun.Set(); }

    }   //     public class WxStationSim : IWxStation
}   // namespace Wx_Station_Sim

