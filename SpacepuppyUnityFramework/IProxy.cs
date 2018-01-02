using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    /// <summary>
    /// Interface to define an object that allows pass through of another object.
    /// 
    /// IProxy's are good for retrieving references to objects that can't otherwise be referenced directly in the inspector.
    /// 
    /// ObjUtil.GetAsFromSource respects IProxy.
    /// 
    /// This is useful at editor time when you may need to reference something in a scene that doesn't yet exist at editor time (an uninstantiated prefab for instance). 
    /// An IProxy may let you reference said object by name, tag, layer, type, etc.
    /// 
    /// For examples see:
    /// ProxyTarget
    /// </summary>
    public interface IProxy
    {
        System.Type GetTargetType();

        object GetTarget();
        object GetTarget(object arg);
    }

}
