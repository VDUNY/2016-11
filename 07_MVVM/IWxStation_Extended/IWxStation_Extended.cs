using System;
using IWx_Station;

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