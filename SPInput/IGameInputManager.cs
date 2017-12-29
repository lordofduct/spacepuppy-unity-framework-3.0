using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.UserInput;

namespace com.spacepuppy
{

    public interface IGameInputManager : IService, IEnumerable<IPlayerInputDevice>
    {

        int Count { get; }
        IPlayerInputDevice this[string id] { get; set; }
        IPlayerInputDevice Main { get; }

        IPlayerInputDevice GetDevice(string id);
        T GetDevice<T>(string id) where T : IPlayerInputDevice;

    }

}
