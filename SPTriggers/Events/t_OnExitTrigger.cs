using UnityEngine;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public class t_OnExitTrigger : TriggerComponent, ICompoundTriggerResponder
    {

        #region Fields

        [SerializeField]
        private EventActivatorMask _mask = new EventActivatorMask(-1);
        [SerializeField]
        private float _cooldownInterval = 1.0f;
        [SerializeField]
        private bool _includeColliderAsTriggerArg = true;

        [System.NonSerialized()]
        private bool _coolingDown;

        #endregion

        #region Properties

        public EventActivatorMask Mask
        {
            get { return _mask; }
            set { _mask = value; }
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

        private void DoTestTriggerExit(Collider other)
        {
            if (_coolingDown) return;

            if (_mask.Intersects(other))
            {
                if (_includeColliderAsTriggerArg)
                {
                    this.ActivateTrigger(other);
                }
                else
                {
                    this.ActivateTrigger();
                }

                _coolingDown = true;
                //use global incase this gets disable
                GameLoop.Hook.Invoke(() =>
                {
                    _coolingDown = false;
                }, _cooldownInterval);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (this.HasComponent<CompoundTrigger>()) return;

            this.DoTestTriggerExit(other);
        }


        void ICompoundTriggerResponder.OnCompoundTriggerEnter(Collider other)
        {
            //do nothing
        }

        void ICompoundTriggerResponder.OnCompoundTriggerExit(Collider other)
        {
            this.DoTestTriggerExit(other);
        }

        #endregion

    }

}
