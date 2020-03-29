﻿#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using com.spacepuppy.Utils;

using com.spacepuppy.Tween;

namespace com.spacepuppy.Events
{
    public class i_LookAt : AutoTriggerable
    {

        public enum SlerpStyle
        {
            None = 0,
            Speed = 1,
            Time = 2
        }

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(UnityEngine.Transform))]
        private TriggerableTargetObject _observer = new TriggerableTargetObject();
        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(UnityEngine.Transform))]
        private TriggerableTargetObject _target = new TriggerableTargetObject();

        [SerializeField()]
        [Tooltip("The axis to rotate around.")]
        private CartesianAxis _axis = CartesianAxis.Y;
        [SerializeField()]
        [Tooltip("Rotate around the axis as defined on the transform this is attache to. Otherwise rotate around the axis in global terms.")]
        private bool _axisIsRelative;
        [SerializeField()]
        [Tooltip("Flatten the look direction on the defined axis.")]
        private bool _flattenOnAxis = true;

        [SerializeField]
        private SlerpStyle _slerp;
        [SerializeField()]
        [Tooltip("If greater than 0, then it will slerp.")]
        private float _slerpValue;

        [SerializeField]
        [Tooltip("Only fires if slerp'd")]
        private SPEvent _onSlerpComplete;

        #endregion

        #region Properties

        public TriggerableTargetObject Observer
        {
            get
            {
                return _observer;
            }
        }

        public TriggerableTargetObject Target
        {
            get
            {
                return _observer;
            }
        }

        /// <summary>
        /// Axis of the transform to use for rotating around.
        /// </summary>
        public CartesianAxis Axis
        {
            get { return _axis; }
            set { _axis = value; }
        }

        /// <summary>
        /// Rotate around the axis as defined on teh transform that is attached to. Otherwise rotate around the axis in global terms.
        /// </summary>
        public bool AxisIsRelative
        {
            get { return _axisIsRelative; }
            set { _axisIsRelative = value; }
        }

        /// <summary>
        /// Flatten the look direction on the defined axis.
        /// </summary>
        public bool FlattenOnAxis
        {
            get { return _flattenOnAxis; }
            set { _flattenOnAxis = value; }
        }

        public SlerpStyle Slerp
        {
            get { return _slerp; }
            set { _slerp = value; }
        }

        public float SlerpValue
        {
            get { return _slerpValue; }
            set { _slerpValue = value; }
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var observer = this._observer.GetTarget<Transform>(arg);
            if (observer == null) return false;
            var targ = this._target.GetTarget<Transform>(arg);
            if (targ == null) return false;

            var dir = targ.position - observer.position;
            var ax = (this._axisIsRelative) ? this.transform.GetAxis(this._axis) : TransformUtil.GetAxis(this._axis);
            if (this._flattenOnAxis) dir = dir.SetLengthOnAxis(ax, 0f);
            var q = Quaternion.LookRotation(dir, ax);

            var a = Quaternion.Angle(observer.rotation, q);
            if (a <= MathUtil.EPSILON) return true; //don't bother tweening if we are already looking at it

            if (_slerp > SlerpStyle.None && _slerpValue > 0)
            {
                switch (_slerp)
                {
                    case SlerpStyle.Speed:
                        {
                            var dur = a / _slerpValue;
                            var twn = com.spacepuppy.Tween.SPTween.Tween(observer).To("rotation", dur, q);
                            if (_onSlerpComplete?.HasReceivers ?? false) twn.OnFinish((s, e) => _onSlerpComplete.ActivateTrigger(this, null));
                            twn.Play(true);
                        }
                        break;
                    case SlerpStyle.Time:
                        {
                            var twn = com.spacepuppy.Tween.SPTween.Tween(observer).To("rotation", _slerpValue, q);
                            if (_onSlerpComplete?.HasReceivers ?? false) twn.OnFinish((s, e) => _onSlerpComplete.ActivateTrigger(this, null));
                            twn.Play(true);
                        }
                        break;
                }
            }
            else
            {
                observer.rotation = q;
                if (_onSlerpComplete?.HasReceivers ?? false) _onSlerpComplete.ActivateTrigger(this, null);
            }
            return true;
        }

        #endregion

    }
}
