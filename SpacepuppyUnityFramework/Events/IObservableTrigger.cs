using System;

namespace com.spacepuppy.Events
{

    /// <summary>
    /// A trigger that can be observed by other triggers for activation. 
    /// </summary>
    public interface IObservableTrigger : IComponent
    {
        BaseSPEvent[] GetEvents();
    }

    /// <summary>
    /// A trigger that can be observed by other triggers. Of which 2 of the events that may trigger are an Enter and Exit state. 
    /// Think like a collider enter/exit, or a mouse enter/exit, or other event where there is a start and end.
    /// </summary>
    public interface IOccupiedTrigger : IObservableTrigger
    {

        BaseSPEvent EnterEvent { get; }
        BaseSPEvent ExitEvent { get; }
        bool IsOccupied { get; }

    }

}
