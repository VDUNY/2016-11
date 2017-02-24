using System;

namespace IWx_Client
{
    /// <summary>
    /// This interface provides the specs necessary for an application,
    /// which implements the Managed Extensbility Framework, specifically IWx_Client,
    /// to load those modules dynamically at run-time.
    /// 
    /// The application specifies that it will import components that implement this interface.
    /// The component specifies that it will satisfy - export - this interface.
    /// </summary>
    public interface IWxClient
    {
        /// <summary>
        /// The way to access those components that implement this interface
        /// </summary>
        IWxClient Window { get; }

        /// <summary>
        /// Call on the module to close itself.
        /// </summary>
        void Close();

        /// <summary>
        /// Human-readable name
        /// </summary>
        String ServiceName { get; }
    }
}
