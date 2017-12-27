using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy
{

    public interface IProxy
    {
        System.Type GetTargetType();

        object GetTarget();
        object GetTarget(object arg);
    }

}
