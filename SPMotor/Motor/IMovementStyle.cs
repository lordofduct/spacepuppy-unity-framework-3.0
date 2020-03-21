﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.Motor
{

    public interface IMovementStyle : IComponent
    {
        
        /// <summary>
        /// Returns true if UpdateMovement should be called during FixedUpdate. If the movementstyle relies on an IMotor, just return the PrefersFixedUpdate property of the IMotor.
        /// </summary>
        bool PrefersFixedUpdate { get; }

        /// <summary>
        /// Called when the style becomes the currently active state.
        /// </summary>
        /// <param name="stateIsStacking">True if this state is being stacked.</param>
        void OnActivate(IMovementStyle lastStyle, ActivationReason reason);
        /// <summary>
        /// Called when the style is leaving being the currently active state.
        /// </summary>
        /// <param name="stateIsStacking">True if the next state is going to stack.</param>
        void OnDeactivate(IMovementStyle nextStyle, ActivationReason reason);
        /// <summary>
        /// Called if the stack is dumped out with out popping through the entries. This allows a MovementStyle that goes 
        /// into a dormant state if another state is stacked ontop of it, to clean its dormant state up if the stack is 
        /// dumped completely by either a call to ChangeState or PopAllStates.
        /// </summary>
        void OnPurgedFromStack();

        void UpdateMovement();

    }

}
