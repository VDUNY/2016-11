using System;
using System.Threading;
using IWx_Station;

namespace Wx_Station
{
    /* With an interface, the user does not access a concrete implementation of this assy. */
    /* Any changes to this class will require re-compilation of the any assys that reference this assy. */
    public class WxStation : IWxStation
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

        public WxStation() { }

        // What if user forgets to call the Initialize method? */
        public void Initialize()  { m_mreRun = new ManualResetEvent(false); }

        /* Any changes to the Run() method require us to test the client for side effects. */
        /* Since the client must call the Run() method, there is no opportunity for the server to acquire indepedently. */
        public void Run()
        {
            while (!m_mreRun.WaitOne(1000))
            {
                /* Altho' we need to handle exceptions, it need not propagate back to the client.
                /* An unhandled exception here could crash the overall app. */
                /* If we catch and handle, then we need some error mechanism to inform the client. */
                m_temperature = ((Single)(new Random((int)(System.DateTime.Now.Ticks << 20))).NextDouble()) * 100.0f;
                if (ChangedTemperature != null) ChangedTemperature(m_temperature);
                m_windSpeed = ((Single)(new Random((int)(System.DateTime.Now.Ticks << 16))).NextDouble()) * 30.0f;
                if (ChangedWindSpeed != null) ChangedWindSpeed(m_windSpeed);
                m_windDirection = ((Single)(new Random((int)(System.DateTime.Now.Ticks << 12))).NextDouble()) * 360.0f;
                if (ChangedWindDirection != null) ChangedWindDirection(m_windDirection);
            }
        }

        /// <summary>
        /// no longer interested in collecting data
        /// </summary>
        public void Quit() { m_mreRun.Set(); }

    }   //     public class WxStation : IWxStation
}   // namespace Wx_Station
