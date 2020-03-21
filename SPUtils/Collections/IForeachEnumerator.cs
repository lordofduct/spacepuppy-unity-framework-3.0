using System;
using System.Collections.Generic;

namespace com.spacepuppy.Collections
{
    public interface IForeachEnumerator<T>
    {

        void Foreach(System.Action<T> callback);

    }
}
