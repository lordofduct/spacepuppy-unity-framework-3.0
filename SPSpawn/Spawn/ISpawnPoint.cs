using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Events;

namespace com.spacepuppy.Spawn
{

    public interface ISpawnPoint
    {

        BaseSPEvent OnSpawned { get; }

        void Spawn();

    }

}
