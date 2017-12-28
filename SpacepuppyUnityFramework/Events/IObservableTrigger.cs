using System;

namespace com.spacepuppy.Events
{

    public interface IObservableTrigger : IComponent
    {
        SPEvent[] GetEvents();
    }

}
