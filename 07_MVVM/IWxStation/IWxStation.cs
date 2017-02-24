using System;

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
