using System;
using System.Threading;

namespace Wx_Station
{
    /* With no interface, the user must make a concrete implementation of this assy. */
    /* Any changes to this class will require re-compilation of the any assys that reference this assy. */
    public class WxStation
    {
        // ManualResetEvent is a good way to avoid the bool bContinue = true; while (bContinue) { ... } snippet.
        // WaitOne(int msec) returns false if reset/unsignaled and true if set/signaled.
        private ManualResetEvent m_mreRun = null;

        public WxStation()
        {
        }

        // What if user forgets to call the Initialize method? */
        public void Initialize()
        {
            // set the manual reset event to unsignaled
            m_mreRun = new ManualResetEvent(false); // Sets the state of the event to not signaled, causing one or more threads to block.
        }

        /* public properties force the user to create a reference to this assy and query for the values. */
        /* What if we change the way one of the properties is calculated? */
        public Single Temperature { get; set; }
        public Single WindSpeed { get; set; }
        public Single WindDirection { get; set; }

        /* Any changes to the Run() method require us to test the client for side effects. */
        /* Since the client must call the Run() method, there is no opportunity for the server to acquire indepedently. */
        public void Run()
        {
            while (!m_mreRun.WaitOne(1000)) // if signaled, WaitOne(int) returns true without blocking.
            {
                /* An exception here could crash the user's app. */
                /* If we catch and handle, then we need some error mechanism to inform the client. */
                Temperature = ((Single)(new Random((int)(System.DateTime.Now.Ticks << 20))).NextDouble()) * 100.0f;
                WindSpeed = ((Single)(new Random((int)(System.DateTime.Now.Ticks << 16))).NextDouble()) * 30.0f;
                WindDirection = ((Single)(new Random((int)(System.DateTime.Now.Ticks << 12))).NextDouble()) * 360.0f;

            }
        }

        /// <summary>
        /// no longer interested in collecting data
        /// </summary>
        public void Quit()
        {
            m_mreRun.Set(); // Sets the state of the event to signaled, allowing one or more waiting threads to proceed.
        }

    }   // public class WxStation
}   // namespace Wx_Station
