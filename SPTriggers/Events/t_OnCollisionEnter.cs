#pragma warning disable 0168, 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public class t_OnCollisionEnter : TriggerComponent
    {

        #region Fields

        [SerializeField]
        private EventActivatorMaskRef _mask;
        [SerializeField]
        private float _cooldownInterval = 1.0f;
        [SerializeField]
        private bool _includeColliderAsTriggerArg = true;

        [System.NonSerialized()]
        private bool _coolingDown;

        #endregion

        #region Properties

        public IEventActivatorMask Mask
        {
            get { return _mask.Value; }
            set { _mask.Value = value; }
        }

        public float CooldownInterval
        {
            get { return _cooldownInterval; }
            set { _cooldownInterval = value; }
        }

        public bool IncludeCollidersAsTriggerArg
        {
            get { return _includeColliderAsTriggerArg; }
            set { _includeColliderAsTriggerArg = value; }
        }

        #endregion

        #region Methods

        private void OnCollisionEnter(Collision c)
        {
            if (_coolingDown) return;

            if (_mask.Value == null || _mask.Value.Intersects(c.collider))
            {
                if (_includeColliderAsTriggerArg)
                {
                    this.ActivateTrigger(c.collider);
                }
                else
                {
                    this.ActivateTrigger();
                }

                _coolingDown = true;
                //use global incase this gets disabled
                this.InvokeGuaranteed(() =>
                {
                    _coolingDown = false;
                }, _cooldownInterval);
            }
        }

        #endregion

    }

}
