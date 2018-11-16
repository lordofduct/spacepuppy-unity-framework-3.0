using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public class i_Enable : AutoTriggerable
    {

        public enum EnableMode
        {
            TriggerArg = -1,
            Enable = 0,
            Disable = 1,
            Toggle = 2
        }

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(UnityEngine.Object))]
        private TriggerableTargetObject _targetObject = new TriggerableTargetObject();

        [SerializeField()]
        private EnableMode _mode;

        [SerializeField()]
        private SPTimePeriod _delay = 0f;

        #endregion

        #region Properties

        public TriggerableTargetObject TargetObject
        {
            get { return _targetObject; }
        }

        public EnableMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public SPTimePeriod Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        #endregion

        #region Methods

        private void SetEnabledByMode(object arg)
        {
            var targ = _targetObject.GetTarget<UnityEngine.Object>(arg);
            if (targ == null) return;
            
            switch (_mode)
            {
                case EnableMode.Enable:
                    if(targ is GameObject)
                    {
                        (targ as GameObject).SetActive(true);
                    }
                    else if(targ is Component)
                    {
                        (targ as Component).SetEnabled(true);
                    }
                    else
                    {
                        //do nothing
                    }
                    break;
                case EnableMode.Disable:
                    if (targ is GameObject)
                    {
                        (targ as GameObject).SetActive(false);
                    }
                    else if (targ is Component)
                    {
                        (targ as Component).SetEnabled(false);
                    }
                    else
                    {
                        //do nothing
                    }
                    break;
                case EnableMode.Toggle:
                    if (targ is GameObject)
                    {
                        (targ as GameObject).SetActive(!(targ as GameObject).activeSelf);
                    }
                    else if (targ is Component)
                    {
                        (targ as Component).SetEnabled(!(targ as Component).IsEnabled());
                    }
                    else
                    {
                        //do nothing
                    }
                    break;
            }
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
                    this.SetEnabledByMode(arg);
                }, _delay.Seconds, _delay.TimeSupplier);
            }
            else
            {
                this.SetEnabledByMode(arg);
            }

            return true;
        }

        #endregion

    }

}
