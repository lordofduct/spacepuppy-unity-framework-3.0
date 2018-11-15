using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public sealed class t_OnEnable : TriggerComponent, IMStartOrEnableReceiver
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

        void IMStartOrEnableReceiver.OnStartOrEnable()
        {
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
