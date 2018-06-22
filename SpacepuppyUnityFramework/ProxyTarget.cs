#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Events;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// An IProxy that locates an object by name/tag/type in a scene or entity.
    /// </summary>
    public class ProxyTarget : Triggerable, IDynamic, IProxy
    {

        #region Fields

        [SerializeField]
        private TriggerableTargetObject _target;
        [SerializeField]
        [TypeReference.Config(typeof(Component), allowAbstractClasses = true, allowInterfaces = true)]
        private TypeReference _componentTypeOnTarget = new TypeReference();

        [Space()]
        [SerializeField]
        [Tooltip("Cache the target when it's first retrieved. This is useful for speeding up any 'Find' commands if called repeatedly, but is hindered if the target is changing.")]
        private bool _cache;
        [System.NonSerialized]
        private UnityEngine.Object _object;

        [Space()]
        [SerializeField]
        private bool _treatAsTriggerable = true;
        [SerializeField]
        [EnumPopupExcluding((int)TriggerActivationType.SendMessage, (int)TriggerActivationType.CallMethodOnSelectedTarget, (int)TriggerActivationType.EnableTarget)]
        private TriggerActivationType _triggerAction;

        #endregion

        #region Properties

        public TriggerActivationType TriggerAction
        {
            get { return _triggerAction; }
            set
            {
                switch (value)
                {
                    case TriggerActivationType.SendMessage:
                    case TriggerActivationType.CallMethodOnSelectedTarget:
                    case TriggerActivationType.EnableTarget:
                        throw new System.ArgumentOutOfRangeException("TriggerActivationType not supported.");
                    default:
                        _triggerAction = value;
                        break;
                }
            }
        }

        #endregion

        #region IProxy Interface

        public object GetTarget()
        {
            if (_cache)
            {
                if (_object != null) return _object;

                _object = _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), null) as UnityEngine.Object;
                return _object;
            }
            else
            {
                return _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), null) as UnityEngine.Object;
            }
        }

        public object GetTarget(object arg)
        {
            if (_cache)
            {
                if (_object != null) return _object;

                if (_componentTypeOnTarget == null) return null;
                _object = _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), arg) as UnityEngine.Object;
                return _object;
            }
            else
            {
                if (_componentTypeOnTarget == null) return null;
                return _target.GetTarget(_componentTypeOnTarget.Type ?? typeof(UnityEngine.Object), arg) as UnityEngine.Object;
            }
        }

        public System.Type GetTargetType()
        {
            if (_componentTypeOnTarget.Type != null) return _componentTypeOnTarget.Type;
            return (_cache && _object != null) ? _object.GetType() : typeof(UnityEngine.Object);
        }

        #endregion

        #region TriggerableMechanism Interface

        public override bool CanTrigger
        {
            get { return _treatAsTriggerable && base.CanTrigger; }
        }

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this.GetTarget(arg);
            if (targ == null) return false;

            switch (_triggerAction)
            {
                case TriggerActivationType.TriggerAllOnTarget:
                    EventTriggerEvaluator.Current.TriggerAllOnTarget(targ, sender, arg);
                    return true;
                case TriggerActivationType.TriggerSelectedTarget:
                    EventTriggerEvaluator.Current.TriggerSelectedTarget(targ, sender, arg);
                    return true;
                case TriggerActivationType.DestroyTarget:
                    EventTriggerEvaluator.Current.DestroyTarget(targ);
                    return true;
            }

            return false;
        }

        #endregion

        #region IDynamic Interface

        object IDynamic.this[string sMemberName]
        {
            get
            {
                return (this as IDynamic).GetValue(sMemberName);
            }
            set
            {
                (this as IDynamic).SetValue(sMemberName, value);
            }
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            var targ = this.GetTarget();
            if (targ == null) return false;
            return targ.SetValue(sMemberName, value, index);
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            var targ = this.GetTarget();
            if (targ == null) return false;
            return targ.GetValue(sMemberName, args);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            var targ = this.GetTarget();
            if (targ == null)
            {
                result = null;
                return false;
            }
            return targ.TryGetValue(sMemberName, out result, args);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            var targ = this.GetTarget();
            if (targ == null) return false;
            return targ.InvokeMethod(sMemberName, args);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            var targ = this.GetTarget();
            if (targ == null) return false;
            return DynamicUtil.HasMember(targ, sMemberName, includeNonPublic);
        }

        IEnumerable<System.Reflection.MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            return DynamicUtil.GetMembers(this.GetTarget(), includeNonPublic);
        }

        IEnumerable<string> IDynamic.GetMemberNames(bool includeNonPublic)
        {
            return DynamicUtil.GetMemberNames(this.GetTarget(), includeNonPublic);
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            return DynamicUtil.GetMember(this.GetTarget(), sMemberName, includeNonPublic);
        }

        #endregion

    }

}
