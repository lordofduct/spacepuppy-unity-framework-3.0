using System;
using System.Collections.Generic;

namespace com.spacepuppy.Events
{

    public interface ITriggerable
    {
        int Order { get; }
        bool CanTrigger { get; }
        bool Trigger(object sender, object arg);
    }
    
}
