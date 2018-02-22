using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Motor
{

    public interface IMotor : IComponent, IPhysicsObject
    {

        bool PrefersFixedUpdate { get; }
        float Mass { get; set; }
        float StepOffset { get; set; }
        float SkinWidth { get; set; }
        bool CollisionEnabled { get; set; }

        Vector3 Velocity { get; set; }
        Vector3 Position { get; set; }
        Vector3 LastPosition { get; }
        Vector3 LastVelocity { get; }
        
        void AtypicalMove(Vector3 mv);
        void MovePosition(Vector3 pos, bool setVelocityByChangeInPosition = false);
        void Move(Vector3 mv);
        void AddForce(Vector3 f, ForceMode mode);
        void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode);

    }

}
