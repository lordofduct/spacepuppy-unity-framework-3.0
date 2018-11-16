#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using com.spacepuppy.Events;

namespace com.spacepuppy.Tween.Events
{

    public class i_TweenValue : AutoTriggerable, IObservableTrigger
    {

        #region Fields

        [SerializeField()]
        private SPTime _timeSupplier;

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(UnityEngine.Object))]
        private TriggerableTargetObject _target;

        [SerializeField()]
        private TweenData[] _data;

        [SerializeField()]
        private SPEvent _onComplete;

        [SerializeField()]
        private SPEvent _onTick;

        [SerializeField()]
        [Tooltip("Leave blank for tweens to be unique to this component.")]
        private string _tweenToken;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if (string.IsNullOrEmpty(_tweenToken)) _tweenToken = "i_Tween*" + this.GetInstanceID().ToString();
        }

        /*
         * TODO - if want to kill these tweens, need to store each tween that was started. Can't kill all on '_target' since we changed over to TriggerableTargetObject.
         * 
        protected override void OnDisable()
        {
            base.OnDisable();

            //SPTween.KillAll(_target, _tweenToken);
        }
        */

        #endregion

        #region Methods

        #endregion

        #region ITriggerable Interface

        public override bool CanTrigger
        {
            get
            {
                return base.CanTrigger && _data.Length > 0;
            }
        }

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var target = _target.GetTarget<UnityEngine.Object>(arg);
            if (target == null) return false;

            var twn = SPTween.Tween(target);
            for (int i = 0; i < _data.Length; i++)
            {
                twn.ByAnimMode(_data[i].Mode, _data[i].MemberName, EaseMethods.GetEase(_data[i].Ease), _data[i].Duration, _data[i].ValueS.Value, _data[i].ValueE.Value, _data[i].Option);
            }
            twn.Use(_timeSupplier.TimeSupplier);
            twn.SetId(target);

            if (_onComplete.Count > 0)
                twn.OnFinish((t) => _onComplete.ActivateTrigger(this, null));

            if (_onTick.Count > 0)
                twn.OnStep((t) => _onTick.ActivateTrigger(this, null));

            twn.Play(true, _tweenToken);
            return true;
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        public class TweenData
        {
            [SerializeField()]
            [EnumPopupExcluding((int)TweenHash.AnimMode.AnimCurve, (int)TweenHash.AnimMode.Curve)]
            public TweenHash.AnimMode Mode;
            [SerializeField()]
            public string MemberName;
            [SerializeField()]
            public EaseStyle Ease;
            [SerializeField()]
            public VariantReference ValueS;
            [SerializeField()]
            public VariantReference ValueE;
            [SerializeField()]
            [TimeUnitsSelector()]
            public float Duration;
            [SerializeField]
            public int Option;
        }

        #endregion

        #region IObservable Interface

        BaseSPEvent[] IObservableTrigger.GetEvents()
        {
            return new BaseSPEvent[] { _onTick, _onComplete };
        }

        #endregion

    }
}
