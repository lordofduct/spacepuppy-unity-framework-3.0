using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Spawn
{

    public interface IOnSpawnHandler
    {

        void OnSpawn(SpawnedObjectController cntrl);

    }

}
