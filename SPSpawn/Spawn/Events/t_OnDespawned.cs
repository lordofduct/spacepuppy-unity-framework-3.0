using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Events;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn.Events
{

    public class t_OnDespawned : TriggerComponent, IOnDespawnHandler
    {

        #region IOnDespawnHandler Interface

        void IOnDespawnHandler.OnDespawn(SpawnedObjectController cntrl)
        {
            this.ActivateTrigger(cntrl);
        }

        #endregion

    }

}
