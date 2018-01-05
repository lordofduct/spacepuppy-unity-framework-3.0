using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Events;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn.Events
{

    public class t_OnSpawned : TriggerComponent, IOnSpawnHandler
    {

        #region IOnSpawnHandler Interface

        void IOnSpawnHandler.OnSpawn(SpawnedObjectController cntrl)
        {
            this.ActivateTrigger(cntrl);
        }

        #endregion

    }

}
