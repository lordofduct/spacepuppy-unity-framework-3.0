using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Events;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Facilitates sending events between assets/prefabs. By relying on a ScriptableObject to mediate an event between 2 other assets 
    /// you can have communication between those assets setup at dev time. For example communicating an event between 2 prefabs, or a prefab 
    /// and a scene, or any other similar situation. 
    /// 
    /// To use:
    /// Create a ProxyMediator as an asset and name it something unique.
    /// Any SPEvent/Trigger can target this asset just by dragging it into the target field.
    /// Now any script can accept a ProxyMediator and listen for the 'OnTriggered' event to receive a signal that the mediator had been triggered elsewhere.
    /// You can also attach a T_OnProxyMediatorTriggered, and drag the ProxyMediator asset in question into the 'Mediator' field. This T_ will fire when the mediator is triggered elsewhere.
    /// </summary>
    [CreateAssetMenu(fileName = "ProxyMediator", menuName = "Spacepuppy/ProxyMediator")]
    public class ProxyMediator : ScriptableObject, ITriggerable
    {

        public System.EventHandler OnTriggered;

        public void Trigger()
        {
            if (this.OnTriggered != null) this.OnTriggered(this, System.EventArgs.Empty);
        }

        #region ITriggerableMechanism Interface

        bool ITriggerable.CanTrigger
        {
            get
            {
                return true;
            }
        }

        int ITriggerable.Order
        {
            get
            {
                return 0;
            }
        }

        bool ITriggerable.Trigger(object sender, object arg)
        {
            this.Trigger();
            return true;
        }

        #endregion

    }

}
