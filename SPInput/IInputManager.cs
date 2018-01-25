using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.SPInput;

namespace com.spacepuppy
{

    public interface IInputManager : IService, IEnumerable<IInputDevice>
    {

        int Count { get; }
        IInputDevice this[string id] { get; }
        IInputDevice Main { get; }

        IInputDevice GetDevice(string id);
        T GetDevice<T>(string id) where T : IInputDevice;

    }

}
