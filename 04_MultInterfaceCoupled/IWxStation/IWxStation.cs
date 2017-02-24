using System;

/* Events allow the client to remove direct calls on the server. By providing events thru the server,
 * the client subscribes and unsubscribes according to its needs. 
 * Further, events do not need exception handling for the server's responses, if a contract with
 * pre-reqs and post-reqs is provided.
 * 
*/
namespace IWx_Station
{
    // events are declared in the implementing class
    public delegate void DelChangedSetting(string settting, string value); //new value notification
    public delegate void DelExceptionRaised(string msg);    // notice of unexpected issue
    public delegate void DelChangedTemperature(Single temperature);
    public delegate void DelChangedWindSpeed(Single speed);
    public delegate void DelChangedWindDirection(Single direction);

    public interface IWxStation
    {
        // an event is just another way of specifying a method. So spec it in the interface.
        event DelChangedSetting ChangedSetting;
        event DelExceptionRaised ExceptionRaised;
        event DelChangedTemperature ChangedTemperature;
        event DelChangedWindDirection ChangedWindDirection;
        event DelChangedWindSpeed ChangedWindSpeed;

        void Initialize();
        void Quit();
        void Run();

    }

}
