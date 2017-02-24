using System;
using System.Threading;
using IWx_Station;
using IWx_Station_Extended;

namespace Wx_Station
{
    /* With no interface, the user must make a concrete implementation of this assy. */
    /* Any changes to this class will require re-compilation of the any assys that reference this assy. */
    public class WxStation : IWxStation, IWxStationExtended
    {
        private ManualResetEvent m_mreRun = null;
        private Single m_temperature = -1.0f;
        private Single m_windSpeed = -1.0f;
        private Single m_windDirection = -1.0f;
        private Single m_humidity = -1.0f;

        public event DelChangedTemperature ChangedTemperature;
        public event DelChangedWindDirection ChangedWindDirection;
        public event DelChangedWindSpeed ChangedWindSpeed;
        public event DelChangedHumidity ChangedHumidity;
        public event DelChangedSetting ChangedSetting;
        public event DelExceptionRaised ExceptionRaised;

        public WxStation() { }

        // What if user forgets to call the Initialize method? */
        public void Initialize()  { m_mreRun = new ManualResetEvent(false); }

        /* Any changes to the Run() method require us to test the client for side effects. */
        /* Since the client must call the Run() method, there is no opportunity for the server to acquire indepedently. */
        public void Run()
        {
            while (!m_mreRun.WaitOne(1000))
            {
                /* An exception here could crash the user's app. */
                /* If we catch and handle, then we need some error mechanism to inform the client. */
                m_temperature = ((Single)(new Random((int)(System.DateTime.Now.Ticks << 20))).NextDouble()) * 100.0f;
                if (ChangedTemperature != null) ChangedTemperature(m_temperature);
                m_windSpeed = ((Single)(new Random((int)(System.DateTime.Now.Ticks << 16))).NextDouble()) * 30.0f;
                if (ChangedWindSpeed != null) ChangedWindSpeed(m_windSpeed);
                m_windDirection = ((Single)(new Random((int)(System.DateTime.Now.Ticks << 12))).NextDouble()) * 360.0f;
                if (ChangedWindDirection != null) ChangedWindDirection(m_windDirection);
                m_humidity = ((Single)(new Random((int)(System.DateTime.Now.Ticks << 8))).NextDouble()) * 100.0f;
                if (ChangedHumidity != null) ChangedHumidity(m_humidity);
            }
        }

        public void Quit() { m_mreRun.Set(); }

    }	/ public class WxStation : IWxStation, IWxStationExtended
}	// namespace Wx_Station
