using System;
using IWx_Station;

/* Events allow the client to remove direct calls on the server. By providing events thru the server,
 * the client subscribes and unsubscribes according to its needs. 
 * Further, events do not need exception handling for the server's responses, if a contract with
 * pre-reqs and post-reqs is provided.
 * 
*/
namespace IWx_Station_Extended
{
    // events are declared in the implementing class
    public delegate void DelChangedHumidity(Single humidity);

    public interface IWxStationExtended : IWxStation
    {
        // an event is just another way of specifying a method. So spec it in the interface.
        event DelChangedHumidity ChangedHumidity;

    }

}