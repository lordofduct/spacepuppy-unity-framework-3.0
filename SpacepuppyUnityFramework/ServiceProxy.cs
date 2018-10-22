using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy
{

    /// <summary>
    /// Inherit from this class, and add a 'CreateAssetMenuAttribute' to create a asset that can be used as a proxy to a Service.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceProxy<T> : ScriptableObject, IProxy where T : class, IService
    {

        bool IProxy.QueriesTarget
        {
            get { return false; }
        }

        public object GetTarget()
        {
            return Services.Get<T>();
        }

        public object GetTarget(object arg)
        {
            return Services.Get<T>();
        }

        public System.Type GetTargetType()
        {
            return typeof(T);
        }

    }

}
