﻿#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Events;
using com.spacepuppy.Tween;

namespace com.spacepuppy.Render.Events
{
    public class i_TweenMaterial : AutoTriggerable
    {

        #region Fields
        
        [SerializeField()]
        private VariantReference _autoKillId;

        [SerializeField()]
        private bool _killOnDisable;

        [SerializeField()]
        private SPTimePeriod _duration;

        [SerializeField()]
        private EaseStyle _ease;

        [SerializeField()]
        [DisableOnPlay()]
        [EnumPopupExcluding((int)TweenHash.AnimMode.AnimCurve, (int)TweenHash.AnimMode.Curve, (int)TweenHash.AnimMode.By, (int)TweenHash.AnimMode.RedirectTo)]
        private TweenHash.AnimMode _mode;

        [SerializeField()]
        private MaterialTransition _transition;

        [SerializeField()]
        private SPEvent _onComplete;

        [SerializeField()]
        private SPEvent _onTick;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();


            switch (_mode)
            {
                case TweenHash.AnimMode.To:
                    _transition.Values.Insert(0, new VariantReference());
                    break;
                case TweenHash.AnimMode.From:
                    _transition.Values.Add(new VariantReference());
                    break;
            }

        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_killOnDisable)
            {
                SPTween.Find((t) =>
                {
                    if (t is ObjectTweener && (t as ObjectTweener).Target == _transition)
                    {
                        t.Kill();
                        return true;
                    }
                    return false;
                });
            }
        }

        #endregion

        #region Properties

        public object AutoKillId
        {
            get
            {
                var id = _autoKillId.Value;
                if (id == null) id = this;
                return id;
            }
            set
            {
                if (Object.Equals(value, this))
                    _autoKillId.Value = null;
                else
                    _autoKillId.Value = value;
            }
        }

        #endregion

        #region ITriggerable Interface

        public override bool CanTrigger
        {
            get
            {
                return base.CanTrigger && _transition.Material != null && _transition.Values.Count != 0;
            }
        }

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;
            
            switch (_mode)
            {
                case TweenHash.AnimMode.To:
                    _transition.Values[0].Value = _transition.GetValue();
                    break;
                case TweenHash.AnimMode.From:
                    _transition.Values[_transition.Values.Count - 1].Value = _transition.GetValue();
                    break;
            }

            var twn = SPTween.Tween(_transition)
                             .FromTo("Position", EaseMethods.GetEase(_ease), _duration.Seconds, 0f, 1f)
                             .SetId(this.AutoKillId)
                             .Use(_duration.TimeSupplier);
            
            if (_onComplete?.HasReceivers ?? false)
                twn.OnFinish((t) => _onComplete.ActivateTrigger(this, null));

            if (_onTick?.HasReceivers ?? false)
                twn.OnStep((t) => _onTick.ActivateTrigger(this, null));

            twn.Play(true);

            return true;
        }

        #endregion

    }
}
