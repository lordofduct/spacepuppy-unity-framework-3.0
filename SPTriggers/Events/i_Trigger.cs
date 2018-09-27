#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public class i_Trigger : AutoTriggerable, IObservableTrigger
    {

        #region Fields
        
        [SerializeField()]
        private SPEvent _trigger;

        [SerializeField()]
        private bool _passAlongTriggerArg;

        [SerializeField()]
        private SPTimePeriod _delay = 0f;

        #endregion

        #region CONSTRUCTOR
        
        #endregion

        #region Properties
        
        public SPEvent TriggerEvent
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

        public SPTimePeriod Delay
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
        
        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            if (_delay.Seconds > 0f)
            {
                this.InvokeGuaranteed(() =>
                {
                    this.DoTriggerNext(arg);
                }, _delay.Seconds, _delay.TimeSupplier);
            }
            else
            {
                this.DoTriggerNext(arg);
            }

            return true;
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
