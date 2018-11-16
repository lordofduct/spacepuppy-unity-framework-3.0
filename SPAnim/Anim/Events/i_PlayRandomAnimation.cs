#pragma warning disable 0414 // variable declared but not used.
#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Anim.Legacy;
using com.spacepuppy.Events;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Anim.Events
{

    public class i_PlayRandomAnimation : AutoTriggerable, IObservableTrigger
    {

        private const string TRG_ONANIMCOMPLETE = "OnAnimComplete";
        
        #region Fields

        [SerializeField]
        [TriggerableTargetObject.Config(typeof(UnityEngine.Object))]
        private TriggerableTargetObject _targetAnimator;

        [SerializeField]
        private List<PlayAnimInfo> _clips;
        
        [SerializeField()]
        private SPEvent _onAnimComplete = new SPEvent(TRG_ONANIMCOMPLETE);
        [SerializeField()]
        [Tooltip("If an animation doesn't play, should we signal complete. This is useful if the animation is supposed to be chaining to another i_ that MUST run.")]
        private bool _triggerCompleteIfNoAnim = true;

        #endregion

        #region Methods
        
        private object PlayClip(SPLegacyAnimController controller, UnityEngine.Object clip, PlayAnimInfo info)
        {
            if (clip is AnimationClip)
            {
                if (info.CrossFadeDur > 0f)
                    return controller.CrossFadeAuxiliary(clip as AnimationClip,
                                                         (info.SettingsMask != 0) ? AnimSettings.Intersect(AnimSettings.Default, info.Settings, info.SettingsMask) : AnimSettings.Default,
                                                         info.CrossFadeDur, info.QueueMode, info.PlayMode);
                else
                    return controller.PlayAuxiliary(clip as AnimationClip,
                                                    (info.SettingsMask != 0) ? AnimSettings.Intersect(AnimSettings.Default, info.Settings, info.SettingsMask) : AnimSettings.Default,
                                                    info.QueueMode, info.PlayMode);
            }
            else if (clip is IScriptableAnimationClip)
            {
                return controller.Play(clip as IScriptableAnimationClip);
            }

            return null;
        }

        private object PlayClip(Animation controller, UnityEngine.Object clip, PlayAnimInfo info)
        {
            if (clip is AnimationClip)
            {
                var animController = controller as Animation;
                var id = "aux*" + clip.GetInstanceID();
                var a = animController[id];
                if (a == null || a.clip != clip)
                {
                    animController.AddClip(clip as AnimationClip, id);
                }

                AnimationState anim;
                if (info.CrossFadeDur > 0f)
                    anim = animController.CrossFadeQueued(id, info.CrossFadeDur, info.QueueMode, info.PlayMode);
                else
                    anim = animController.PlayQueued(id, info.QueueMode, info.PlayMode);
                if (info.SettingsMask != 0) info.Settings.Apply(anim, info.SettingsMask);
                return anim;
            }

            return null;
        }

        private object TryPlay(object controller, PlayAnimInfo info)
        {
            if (controller is SPLegacyAnimController)
            {
                switch (info.Mode)
                {
                    case i_PlayAnimation.PlayByMode.PlayAnim:
                        return PlayClip(controller as SPLegacyAnimController, info.Clip, info);
                    case i_PlayAnimation.PlayByMode.PlayAnimByID:
                        var anim = (controller as SPLegacyAnimController).GetAnim(info.Id);
                        if (anim != null)
                        {
                            if (info.CrossFadeDur > 0f)
                                anim.CrossFade(info.CrossFadeDur, info.QueueMode, info.PlayMode);
                            else
                                anim.Play(info.QueueMode, info.PlayMode);
                            if (info.SettingsMask != 0) info.Settings.Apply(anim, info.SettingsMask);
                            return anim;
                        }
                        return null;
                    case i_PlayAnimation.PlayByMode.PlayAnimFromResource:
                        return this.PlayClip(controller as SPLegacyAnimController, Resources.Load<UnityEngine.Object>(info.Id), info);
                }
            }
            else if (controller is Animation)
            {
                switch (info.Mode)
                {
                    case i_PlayAnimation.PlayByMode.PlayAnim:
                        return PlayClip(controller as Animation, info.Clip, info);
                    case i_PlayAnimation.PlayByMode.PlayAnimByID:
                        var comp = controller as Animation;
                        if (comp[info.Id] != null)
                        {
                            AnimationState anim;
                            if (info.CrossFadeDur > 0f)
                                anim = comp.CrossFadeQueued(info.Id, info.CrossFadeDur, info.QueueMode, info.PlayMode);
                            else
                                anim = comp.PlayQueued(info.Id, info.QueueMode, info.PlayMode);
                            if (info.SettingsMask != 0) info.Settings.Apply(anim, info.SettingsMask);
                            return anim;
                        }
                        return null;
                    case i_PlayAnimation.PlayByMode.PlayAnimFromResource:
                        return this.PlayClip(controller as Animation, Resources.Load<UnityEngine.Object>(info.Id), info);
                }
            }
            else if (controller is ISPAnimationSource)
            {
                if (info.Mode == i_PlayAnimation.PlayByMode.PlayAnimByID)
                {
                    var anim = (controller as ISPAnimationSource).GetAnim(info.Id);
                    if (anim != null)
                    {
                        if (info.CrossFadeDur > 0f)
                            anim.CrossFade(info.CrossFadeDur, info.QueueMode, info.PlayMode);
                        else
                            anim.Play(info.QueueMode, info.PlayMode);
                        if (info.SettingsMask != 0) info.Settings.Apply(anim, info.SettingsMask);
                        return anim;
                    }
                    return null;
                }
            }
            else if (controller is ISPAnimator)
            {
                if (string.IsNullOrEmpty(info.Id)) return null;
                return com.spacepuppy.Dynamic.DynamicUtil.InvokeMethod(controller, info.Id);
            }

            return null;
        }

        private object ResolveTargetAnimator(object arg)
        {
            var obj = _targetAnimator.GetTarget<UnityEngine.Object>(arg);

            ISPAnimationSource src = null;
            ISPAnimator spanim = null;
            Animation anim = null;

            if (ObjUtil.GetAsFromSource<ISPAnimationSource>(obj, out src))
                return src;
            if (ObjUtil.GetAsFromSource<ISPAnimator>(obj, out spanim))
                return spanim;
            if (ObjUtil.GetAsFromSource<Animation>(obj, out anim))
                return anim;

            if (obj is SPEntity || _targetAnimator.ImplicityReducesEntireEntity)
            {
                var go = obj is SPEntity ? (obj as SPEntity).gameObject : GameObjectUtil.FindRoot(GameObjectUtil.GetGameObjectFromSource(obj));
                if (go == null) return null;

                SPLegacyAnimController spcont;
                if (go.FindComponent<SPLegacyAnimController>(out spcont))
                    return spcont;

                if (go.FindComponent<Animation>(out anim))
                    return anim;
            }

            return null;
        }




        public override bool CanTrigger
        {
            get
            {
                return base.CanTrigger && _clips.Count > 0;
            }
        }

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this.ResolveTargetAnimator(arg);
            if (targ == null) return false;

            var info = _clips.Count == 1 ? _clips[0] : _clips.PickRandom((e) => e != null ? e.Weight : 0f);
            if (info == null) return false;

            var anim = this.TryPlay(targ, info);
            if (anim == null)
            {
                if (_triggerCompleteIfNoAnim) this.Invoke(() => { _onAnimComplete.ActivateTrigger(this, arg); }, 0f);
                return false;
            }

            if (_onAnimComplete.Count > 0)
            {
                if (anim is ISPAnim)
                {
                    (anim as ISPAnim).Schedule((s) =>
                    {
                        _onAnimComplete.ActivateTrigger(this, arg);
                    });
                }
                else if (anim is AnimationState)
                {
                    GameLoop.Hook.StartCoroutine((anim as AnimationState).ScheduleLegacy((a) =>
                    {
                        _onAnimComplete.ActivateTrigger(this, arg);
                    }));
                }
            }

            return false;
        }

        #endregion

        #region IObservableTrigger Interface

        BaseSPEvent[] IObservableTrigger.GetEvents()
        {
            return new BaseSPEvent[] { _onAnimComplete };
        }

        #endregion

        #region Static Interface

        public static bool IsAcceptibleAnimator(object obj)
        {
            return obj is ISPAnimationSource || obj is ISPAnimator || obj is Animation;
        }

        #endregion
        
    }

}
