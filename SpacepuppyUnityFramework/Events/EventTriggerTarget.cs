using UnityEngine;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    [System.Serializable()]
    public class EventTriggerTarget
    {

        #region Fields

        [SerializeField()]
        private float _weight = 1f;
        
        [SerializeField()]
        private UnityEngine.Object _triggerable;
        
        [SerializeField()]
        private VariantReference[] _triggerableArgs;
        
        [SerializeField()]
        private TriggerActivationType _activationType;
        
        [SerializeField()]
        private string _methodName;


        [System.NonSerialized()]
        private ITriggerable[] _triggerAllCache;

        #endregion

        #region Properties

        /// <summary>
        /// A value that can represent the probability weight of the TriggerTarget. This is used by Trigger when configured for probability.
        /// </summary>
        public float Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        //public GameObject Target { get { return (this._triggerable != null) ? _triggerable.gameObject : null; } }
        public UnityEngine.Object Target { get { return _triggerable; } }

        public TriggerActivationType ActivationType { get { return this._activationType; } }
        
        #endregion

        #region Configure Methods

        public void Clear()
        {
            this._triggerable = null;
            this._triggerableArgs = null;
            this._activationType = TriggerActivationType.TriggerAllOnTarget;
            this._methodName = null;
            _triggerAllCache = null;
        }

        public void ConfigureTriggerAll(GameObject targ, object arg = null)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            this._triggerable = targ.transform;
            if (arg == null)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this._activationType = TriggerActivationType.TriggerAllOnTarget;
            this._methodName = null;
            _triggerAllCache = null;
        }

        public void ConfigureTriggerAll(ITriggerable mechanism, object arg = null)
        {
            if (mechanism == null) throw new System.ArgumentNullException("mechanism");
            if (GameObjectUtil.IsGameObjectSource(mechanism))
                _triggerable = GameObjectUtil.GetGameObjectFromSource(mechanism).transform;
            else
                _triggerable = mechanism as UnityEngine.Object;
            if (arg == null || _triggerable == null)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this._activationType = TriggerActivationType.TriggerAllOnTarget;
            this._methodName = null;
            _triggerAllCache = null;
        }

        public void ConfigureTriggerTarget(ITriggerable mechanism, object arg = null)
        {
            if (mechanism == null) throw new System.ArgumentNullException("mechanism");

            this._triggerable = mechanism as UnityEngine.Object;
            if (arg == null || _triggerable == null)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this._activationType = TriggerActivationType.TriggerSelectedTarget;
            this._methodName = null;
            _triggerAllCache = null;
        }

        public void ConfigureSendMessage(GameObject targ, string message, object arg = null)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            this._triggerable = targ.transform;
            if (arg == null)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = new VariantReference[] { new VariantReference(arg) };
            }
            this._methodName = message;
            this._activationType = TriggerActivationType.SendMessage;
            _triggerAllCache = null;
        }

        public void ConfigureCallMethod(GameObject targ, string methodName, params object[] args)
        {
            if (targ == null) throw new System.ArgumentNullException("targ");
            this._triggerable = targ.transform;
            if (args == null || args.Length == 0)
            {
                this._triggerableArgs = null;
            }
            else
            {
                this._triggerableArgs = (from a in args select new VariantReference(a)).ToArray();
            }
            this._methodName = methodName;
            this._activationType = TriggerActivationType.CallMethodOnSelectedTarget;
            _triggerAllCache = null;
        }

        #endregion

        #region Trigger Methods
        
        public void Trigger(object sender, object incomingArg)
        {
            try
            {
                if (this._triggerable == null) return;

                var outgoingArg = (this._triggerableArgs != null && this._triggerableArgs.Length > 0) ? this._triggerableArgs[0].Value : incomingArg;

                //imp
                switch (this._activationType)
                {
                    case TriggerActivationType.TriggerAllOnTarget:
                        {
                            if (_triggerAllCache == null)
                            {
                                //_triggerAllCache = _triggerable.GetComponentsAlt<ITriggerableMechanism>();
                                var go = GameObjectUtil.GetGameObjectFromSource(_triggerable);
                                if (go != null)
                                    _triggerAllCache = go.GetComponents<ITriggerable>();
                                else if (_triggerable is ITriggerable)
                                    _triggerAllCache = new ITriggerable[] { _triggerable as ITriggerable };
                                else
                                    _triggerAllCache = ArrayUtil.Empty<ITriggerable>();

                                if (_triggerableArgs.Length > 1)
                                    System.Array.Sort(_triggerableArgs, TriggerableOrderComparer.Default);
                            }

                            foreach (var t in _triggerAllCache)
                            {
                                if (t.CanTrigger)
                                {
                                    t.Trigger(sender, outgoingArg);
                                }
                            }
                        }
                        break;
                    case TriggerActivationType.TriggerSelectedTarget:
                        {
                            //UnityEngine.Object targ = _triggerable;
                            //if (targ is IProxy) targ = (targ as IProxy).GetTarget(incomingArg);
                            //TriggerSelectedTarget(targ, sender, outgoingArg, instruction);
                            TriggerSelectedTarget(_triggerable, sender, outgoingArg);
                        }
                        break;
                    case TriggerActivationType.SendMessage:
                        {
                            object targ = _triggerable;
                            if (targ is IProxy) targ = (targ as IProxy).GetTarget(incomingArg);
                            SendMessageToTarget(targ, _methodName, outgoingArg);
                        }
                        break;
                    case TriggerActivationType.CallMethodOnSelectedTarget:
                        {
                            CallMethodOnSelectedTarget(_triggerable, _methodName, _triggerableArgs);
                        }
                        break;
                    case TriggerActivationType.EnableTarget:
                        {
                            object targ = _triggerable;
                            if (targ is IProxy) targ = (targ as IProxy).GetTarget(incomingArg);
                            EnableTarget(_triggerable, ConvertUtil.ToEnum<EnableMode>(_methodName));
                        }
                        break;
                    case TriggerActivationType.DestroyTarget:
                        {
                            object targ = _triggerable;
                            if (targ is IProxy) targ = (targ as IProxy).GetTarget(incomingArg);
                            DestroyTarget(_triggerable);
                        }
                        break;
                }
            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex, sender as UnityEngine.Object);
            }
        }
        
        #endregion



        #region Static Methods

        public static void TriggerAllOnTarget(object target, object sender, object arg)
        {
            //var go = GameObjectUtil.GetGameObjectFromSource(target);
            //if (go == null) return;

            using (var lst = com.spacepuppy.Collections.TempCollection.GetList<ITriggerable>())
            {
                var go = GameObjectUtil.GetGameObjectFromSource(target);
                if (go != null)
                {
                    go.GetComponents<ITriggerable>(lst);
                    lst.Sort(TriggerableOrderComparer.Default);
                }
                else if (target is ITriggerable)
                    lst.Add(target as ITriggerable);

                var e = lst.GetEnumerator();
                while (e.MoveNext())
                {
                    var t = e.Current;
                    if (t.CanTrigger)
                    {
                        t.Trigger(sender, arg);
                    }
                }
            }
        }

        public static void TriggerSelectedTarget(object target, object sender, object arg)
        {
            if (target != null && target is ITriggerable)
            {
                var t = target as ITriggerable;
                if (t.CanTrigger) t.Trigger(sender, arg);
            }
        }

        public static void SendMessageToTarget(object target, string message, object arg)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(target);
            if (go != null && message != null)
            {
                go.SendMessage(message, arg, SendMessageOptions.DontRequireReceiver);
            }
        }

        public static void CallMethodOnSelectedTarget(object target, string methodName, VariantReference[] methodArgs)
        {
            if (methodName != null)
            {
                //CallMethod does not support using the passed in arg
                //var args = (from a in this._triggerableArgs select (a != null) ? a.Value : null).ToArray();

                object[] args = null;
                if (methodArgs != null && methodArgs.Length > 0)
                {
                    args = new object[methodArgs.Length];
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (methodArgs[i] != null) args[i] = methodArgs[i].Value;
                    }
                }

                if (args != null && args.Length == 1)
                {
                    DynamicUtil.SetValue(target, methodName, args[0]);
                }
                else
                {
                    DynamicUtil.InvokeMethod(target, methodName, args);
                }
            }
        }

        public static void EnableTarget(object target, EnableMode mode)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(target);
            if (go != null)
            {
                switch (mode)
                {
                    case EnableMode.Disable:
                        go.SetActive(false);
                        break;
                    case EnableMode.Enable:
                        go.SetActive(true);
                        break;
                    case EnableMode.Toggle:
                        go.SetActive(!go.activeSelf);
                        break;
                }
            }
        }

        public static void DestroyTarget(object target)
        {
            var go = GameObjectUtil.GetGameObjectFromSource(target);
            if (go != null)
            {
                ObjUtil.SmartDestroy(go);
            }
            else if (target is UnityEngine.Object)
            {
                ObjUtil.SmartDestroy(target as UnityEngine.Object);
            }
        }

        #endregion

    }

}
