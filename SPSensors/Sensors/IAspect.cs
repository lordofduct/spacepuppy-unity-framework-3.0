using UnityEngine;

namespace com.spacepuppy.Sensors
{

    /// <summary>
    /// Represents an object that can be recognized by a Sensor.
    /// </summary>
    public interface IAspect : IGameObjectSource
    {

        bool IsActive { get; }

        float Precedence { get; }

        bool OmniPresent { get; }

    }
    
}
