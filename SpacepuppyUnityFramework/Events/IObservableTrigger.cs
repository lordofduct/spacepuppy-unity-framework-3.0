using System;

namespace com.spacepuppy.Events
{

    public interface IObservableTrigger : IComponent
    {
        BaseSPEvent[] GetEvents();
    }

}
