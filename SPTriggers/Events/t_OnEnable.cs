using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public sealed class t_OnEnable : TriggerComponent, IMStartOrEnableReceiver
    {

        #region Fields

        [SerializeField()]
        [TimeUnitsSelector()]
        private float _delay;

        #endregion

        #region Properties

        public float Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        #endregion

        #region Messages

        void IMStartOrEnableReceiver.OnStartOrEnable()
        {
            if (_delay > 0f)
            {
                this.InvokeGuaranteed(() =>
                {
                    this.ActivateTrigger(this);
                }, _delay);
            }
            else
            {
                this.ActivateTrigger(this);
            }

        }

        #endregion


    }

}
