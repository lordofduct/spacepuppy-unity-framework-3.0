using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public class t_OnStart : TriggerComponent
    {

        #region Fields

        [SerializeField()]
        private SPTimePeriod _delay;

        #endregion

        #region Properties

        public SPTimePeriod Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        #endregion

        #region Messages

        protected override void Start()
        {
            base.Start();

            if (_delay.Seconds > 0f)
            {
                this.InvokeGuaranteed(() =>
                {
                    this.ActivateTrigger(this);
                }, _delay.Seconds, _delay.TimeSupplier);
            }
            else
            {
                this.ActivateTrigger(this);
            }
        }

        #endregion

    }

}
