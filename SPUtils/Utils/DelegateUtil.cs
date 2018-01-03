using System;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Utils
{

    public static class DelegateUtil
    {

        public static T ExtractDelegate<T>(object obj, string name, bool ignoreCase = false, bool throwOnBindFailure = false) where T : class
        {
            if (obj == null) throw new System.ArgumentNullException("obj");
            var delegateType = typeof(T);
            if (!delegateType.IsSubclassOf(typeof(System.Delegate))) throw new TypeArgumentMismatchException(delegateType, typeof(System.Delegate), "T");

            return System.Delegate.CreateDelegate(delegateType, obj, name, ignoreCase, throwOnBindFailure) as T;
        }

        public static System.Delegate ExtractDelegate(System.Type delegateType, object obj, string name, bool ignoreCase = false, bool throwOnBindFailure = false)
        {
            if (delegateType == null) throw new System.ArgumentNullException("delegateType");
            if (obj == null) throw new System.ArgumentNullException("obj");
            if (!delegateType.IsSubclassOf(typeof(System.Delegate))) throw new TypeArgumentMismatchException(delegateType, typeof(System.Delegate), "delegateType");

            return System.Delegate.CreateDelegate(delegateType, obj, name, ignoreCase, throwOnBindFailure);
        }

    }

}
