#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public class i_Trigger : SPComponent, ITriggerable, IObservableTrigger
    {

        #region Fields

        [SerializeField()]
        private int _order;

        [SerializeField()]
        private ActivateEvent _activateOn = ActivateEvent.None;

        [SerializeField()]
        private SPEvent _trigger;

        [SerializeField()]
        private bool _passAlongTriggerArg;

        [SerializeField()]
        [TimeUnitsSelector()]
        private float _delay = 0f;

        #endregion

        #region CONSTRUCTOR

        protected override void Start()
        {
            base.Start();

            if (_activateOn.HasFlag(ActivateEvent.OnStart) || _activateOn.HasFlag(ActivateEvent.OnEnable))
            {
                this.ActivateTrigger(null);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_activateOn.HasFlag(ActivateEvent.OnStart) && !this.started) return;

            if (_activateOn.HasFlag(ActivateEvent.OnEnable))
            {
                this.ActivateTrigger(null);
            }
        }

        #endregion

        #region Properties

        public ActivateEvent ActivateOn
        {
            get { return _activateOn; }
            set { _activateOn = value; }
        }

        public SPEvent Trigger
        {
            get
            {
                return _trigger;
            }
        }

        public bool PassAlongTriggerArg
        {
            get { return _passAlongTriggerArg; }
            set { _passAlongTriggerArg = value; }
        }

        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        #endregion

        #region Methods

        private void DoTriggerNext(object arg)
        {
            if (this._passAlongTriggerArg)
                _trigger.ActivateTrigger(this, arg);
            else
                _trigger.ActivateTrigger(this, null);
        }

        #endregion

        #region ITriggerableMechanism Interface

        public int Order
        {
            get { return _order; }
        }

        public bool CanTrigger
        {
            get { return this.isActiveAndEnabled; }
        }

        public void ActivateTrigger()
        {
            this.ActivateTrigger(null);
        }

        public bool ActivateTrigger(object arg)
        {
            if (!this.CanTrigger) return false;

            if (_delay > 0f)
            {
                this.Invoke(() =>
                {
                    this.DoTriggerNext(arg);
                }, _delay);
            }
            else
            {
                this.DoTriggerNext(arg);
            }

            return true;
        }

        bool ITriggerable.Trigger(object sender, object arg)
        {
            return this.ActivateTrigger(arg);
        }

        #endregion

        #region IObservableTrigger Interface

        BaseSPEvent[] IObservableTrigger.GetEvents()
        {
            return new BaseSPEvent[] { _trigger };
        }

        #endregion

    }

}
