using System;

namespace IWx_Station
{
    /* for polymorphism, this interface lets very different code modules appear identical.
        Interfaces provide NO implementation. That is the role of the implementation class.
        Both our real-world weather service and the simulator implement this interface.
        Altho' either or both can implement additional public methods, they will not be available
        to any code that creates a type IWx_Station. Such an attempt violates the principle of
        the interface providing loose-coupling.
    */
    public interface IWxStation
    {
        void Initialize();
        void Quit();
        void Run();

        Single Temperature();   /* the compiler turns these properties into methods */
        Single WindSpeed();
        Single WindDirection();

    }
}
