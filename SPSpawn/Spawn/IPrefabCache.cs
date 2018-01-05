using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Spawn
{

    public interface IPrefabCache
    {

        string Name { get; }
        GameObject Prefab { get; }
        int PrefabID { get; }
        int CacheSize { get; set; }
        int ResizeBuffer { get; set; }
        int LimitAmount { get; set; }

    }

}
