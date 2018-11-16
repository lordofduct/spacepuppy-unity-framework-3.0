using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public abstract class TriggerComponent : SPComponent, IObservableTrigger
    {

        #region Fields

        [SerializeField()]
        private SPEvent _trigger = new SPEvent();

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _trigger.ObservableTriggerId = this.GetType().Name;
        }

        #endregion

        #region Properties

        public SPEvent Trigger
        {
            get
            {
                return _trigger;
            }
        }

        #endregion

        #region Methods

        public virtual void ActivateTrigger()
        {
            _trigger.ActivateTrigger(this, null);
        }

        public virtual void ActivateTrigger(object arg)
        {
            _trigger.ActivateTrigger(this, arg);
        }

        #endregion

        #region IObservableTrigger Interface

        BaseSPEvent[] IObservableTrigger.GetEvents()
        {
            return new BaseSPEvent[] { _trigger };
        }

        #endregion

    }

    public abstract class AutoTriggerComponent : TriggerComponent
    {

        #region Fields

        [SerializeField()]
        private ActivateEvent _activateOn = ActivateEvent.OnStartOrEnable;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if ((_activateOn & ActivateEvent.Awake) != 0)
            {
                this.OnTriggerActivate();
            }
        }

        protected override void Start()
        {
            base.Start();

            if ((_activateOn & ActivateEvent.OnStart) != 0 || (_activateOn & ActivateEvent.OnEnable) != 0)
            {
                this.OnTriggerActivate();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!this.started) return;

            if ((_activateOn & ActivateEvent.OnEnable) != 0)
            {
                this.OnTriggerActivate();
            }
        }

        #endregion

        #region Properties

        public ActivateEvent ActivateOn
        {
            get { return _activateOn; }
            set { _activateOn = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Override this to start the trigger sequence on start/enable, depending configuration.
        /// </summary>
        protected abstract void OnTriggerActivate();

        #endregion

    }

}
