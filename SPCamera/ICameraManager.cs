using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Cameras;

namespace com.spacepuppy
{

    public interface ICameraManager : IService
    {

        ICamera Main { get; set; }

    }

}
