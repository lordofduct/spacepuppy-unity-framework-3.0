using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.spacepuppy.Events
{

    public interface ITriggerable
    {
        int Order { get; }
        bool CanTrigger { get; }
        bool Trigger(object sender, object arg);
    }
    
}
