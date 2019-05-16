using System;
using System.Collections.Generic;

namespace com.spacepuppy.Events
{

    public interface ITriggerable
    {
        int Order { get; }
        /// <summary>
        /// For consistent behaviour this should return false if 'isActiveAndEnabled' is false.
        /// </summary>
        bool CanTrigger { get; }
        bool Trigger(object sender, object arg);
    }
    
}
