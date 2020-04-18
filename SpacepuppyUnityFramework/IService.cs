﻿#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using com.spacepuppy.Utils;
using Type = System.Type;

namespace com.spacepuppy
{

    /// <summary>
    /// A special type of Singleton accessible by the Services static interface.
    /// 
    /// A service should implement some interface and is registered with Services as that interface. 
    /// This allows for a service to be accessed like a singleton, but implemented as an interface. 
    /// 
    /// See IInputManager, ICameraManager, and ISceneManager for examples.
    /// </summary>
    public interface IService
    {
        event System.EventHandler ServiceUnregistered;
        void SignalServiceUnregistered();
    }

    /// <summary>
    /// Access point for all registered Services.
    /// 
    /// Note - the reflected System.Type versions of these methods will not work on AOT systems.
    /// </summary>
    public static class Services
    {

        #region Methods

        public static bool Exists<T>() where T : class, IService
        {
            return !Entry<T>.Instance.IsNullOrDestroyed();
        }

        public static T Get<T>() where T : class, IService
        {
            var result = Entry<T>.Instance;
            if (!object.ReferenceEquals(result, null) && result.IsNullOrDestroyed())
            {
                result = null;
                Entry<T>.Instance = null;
            }
            return result;
        }

        public static void Register<T>(T service) where T : class, IService
        {
            var other = Entry<T>.Instance;
            if (!other.IsNullOrDestroyed() && !object.ReferenceEquals(other, service)) throw new System.InvalidOperationException("You must first unregister a service before registering a new one.");
            Entry<T>.Instance = service;
        }

        public static void Unregister<T>(bool destroyIfCan = false, bool donotSignalUnregister = false) where T : class, IService
        {
            var inst = Entry<T>.Instance;
            if (!inst.IsNullOrDestroyed())
            {
                Entry<T>.Instance = null;
                if (!donotSignalUnregister)
                    inst.SignalServiceUnregistered();
                
                if (destroyIfCan && inst is UnityEngine.Object)
                    ObjUtil.SmartDestroy(inst as UnityEngine.Object);
            }
        }

        public static object Get(Type tp)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(IService).IsAssignableFrom(tp)) throw new System.ArgumentException("Type is not a IService");

            object result = null;
            try
            {
                var klass = typeof(Entry<>);
                klass.MakeGenericType(tp);
                var field = klass.GetField("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                result = field.GetValue(null);

                if (!object.ReferenceEquals(result, null) && result.IsNullOrDestroyed())
                {
                    result = null;
                    field.SetValue(null, null);
                }
            }
            catch (System.Exception ex)
            {
                throw new System.InvalidOperationException("Failed to resolve type '" + tp.Name + "'.", ex);
            }

            return result;
        }

        public static void Register(System.Type tp, IService service)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!tp.IsClass || tp.IsAbstract || !typeof(IService).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must be a concrete class that implements IService.", "tp");


            System.Reflection.FieldInfo field;
            try
            {
                var klass = typeof(Entry<>);
                klass = klass.MakeGenericType(tp);
                field = klass.GetField("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            }
            catch (System.Exception ex)
            {
                throw new System.InvalidOperationException("Failed to resolve type '" + tp.Name + "'.", ex);
            }

            if (field == null) throw new System.InvalidOperationException("Failed to resolve type '" + tp.Name + "'.");

            var other = field.GetValue(null);
            if (!other.IsNullOrDestroyed() && !object.ReferenceEquals(other, service)) throw new System.InvalidOperationException("You must first unregister a service before registering a new one.");
            field.SetValue(null, service);
        }

        public static void Unregister(System.Type tp, bool destroyIfCan = false, bool donotSignalUnregister = false)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!tp.IsClass || tp.IsAbstract || !typeof(IService).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must be a concrete class that implements IService.", "tp");

            IService inst;
            try
            {
                var klass = typeof(Entry<>);
                klass = klass.MakeGenericType(tp);
                var field = klass.GetField("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                inst = field.GetValue(null) as IService;
                if (inst != null)
                    field.SetValue(null, null);
            }
            catch (System.Exception ex)
            {
                throw new System.InvalidOperationException("Failed to resolve type '" + tp.Name + "'.", ex);
            }

            if (!donotSignalUnregister && inst != null)
                inst.SignalServiceUnregistered();

            if (destroyIfCan && inst is UnityEngine.Object)
                ObjUtil.SmartDestroy(inst as UnityEngine.Object);
        }


        public static TConcrete Create<TServiceType, TConcrete>(bool persistent = false, string name = null) where TServiceType : class, IService
                                                                                                                where TConcrete : class, TServiceType
        {
            var inst = Services.Get<TServiceType>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(TServiceType).Name));

            var tp = typeof(TConcrete);
            if (typeof(Component).IsAssignableFrom(tp))
            {
                var obj = ServiceComponent<TServiceType>.Create(tp, persistent, name) as TConcrete;
                Services.Register<TServiceType>(obj);
                return obj;
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(tp))
            {
                var obj = ServiceScriptableObject<TServiceType>.Create(tp, persistent, name) as TConcrete;
                Services.Register<TServiceType>(obj);
                return obj;
            }
            else
            {
                try
                {
                    var obj = System.Activator.CreateInstance<TConcrete>();
                    Services.Register<TServiceType>(obj);
                    return obj;
                }
                catch (System.Exception ex)
                {
                    throw new System.InvalidOperationException("Supplied concrete service type failed to construct.", ex);
                }
            }
        }

        public static T Create<T>(bool persistent = false, string name = null) where T : class, IService
        {
            var inst = Services.Get<T>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(T).Name));

            var tp = typeof(T);
            if (typeof(Component).IsAssignableFrom(tp))
            {
                var obj = ServiceComponent<T>.Create(tp, persistent, name);
                Services.Register<T>(obj);
                return obj;
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(tp))
            {
                var obj = ServiceScriptableObject<T>.Create(tp, persistent, name);
                Services.Register<T>(obj);
                return obj;
            }
            else
            {
                try
                {
                    var obj = System.Activator.CreateInstance<T>();
                    Services.Register<T>(obj);
                    return obj;
                }
                catch (System.Exception ex)
                {
                    throw new System.InvalidOperationException("Supplied concrete service type failed to construct.", ex);
                }
            }
        }

        public static TServiceType Create<TServiceType>(System.Type concreteType, bool persistent = false, string name = null) where TServiceType : class, IService
        {
            if (concreteType == null) throw new System.ArgumentNullException("concreteType");
            if (!typeof(TServiceType).IsAssignableFrom(concreteType)) throw new System.ArgumentException("Type must implement " + typeof(TServiceType).Name);

            var inst = Services.Get<TServiceType>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(TServiceType).Name));

            if (typeof(Component).IsAssignableFrom(concreteType))
            {
                var obj = ServiceComponent<TServiceType>.Create(concreteType, persistent, name);
                Services.Register<TServiceType>(obj);
                return obj;
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(concreteType))
            {
                var obj = ServiceScriptableObject<TServiceType>.Create(concreteType, persistent, name);
                Services.Register<TServiceType>(obj);
                return obj;
            }
            else
            {
                try
                {
                    var obj = System.Activator.CreateInstance(concreteType) as TServiceType;
                    Services.Register<TServiceType>(obj);
                    return obj;
                }
                catch (System.Exception ex)
                {
                    throw new System.InvalidOperationException("Supplied concrete service type failed to construct.", ex);
                }
            }
        }

        public static TServiceType GetOrCreate<TServiceType, TConcrete>(bool persistent = false, string name = null) where TServiceType : class, IService
                                                                                                                     where TConcrete : TServiceType
        {
            var inst = Services.Get<TServiceType>();
            if (inst != null) return inst;

            var tp = typeof(TConcrete);
            if (typeof(Component).IsAssignableFrom(tp))
            {
                return ServiceComponent<TServiceType>.GetOrCreate(tp, persistent, name);
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(tp))
            {
                return ServiceScriptableObject<TServiceType>.GetOrCreate(tp, persistent, name);
            }
            else
            {
                try
                {
                    var obj = System.Activator.CreateInstance<TConcrete>();
                    Services.Register<TServiceType>(obj);
                    return obj;
                }
                catch (System.Exception ex)
                {
                    throw new System.InvalidOperationException("Supplied concrete service type failed to construct.", ex);
                }
            }
        }

        public static T GetOrCreate<T>(bool persistent = false, string name = null) where T : class, IService
        {
            var inst = Services.Get<T>();
            if (inst != null) return inst;

            var tp = typeof(T);
            if (typeof(Component).IsAssignableFrom(tp))
            {
                return ServiceComponent<T>.GetOrCreate(tp, persistent, name);
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(tp))
            {
                return ServiceScriptableObject<T>.GetOrCreate(tp, persistent, name);
            }
            else
            {
                try
                {
                    var obj = System.Activator.CreateInstance<T>();
                    Services.Register<T>(obj);
                    return obj;
                }
                catch (System.Exception ex)
                {
                    throw new System.InvalidOperationException("Supplied concrete service type failed to construct.", ex);
                }
            }
        }

        public static TServiceType GetOrCreate<TServiceType>(System.Type concreteType, bool persistent = false, string name = null) where TServiceType : class, IService
        {
            if (concreteType == null) throw new System.ArgumentNullException("concreteType");
            if (!typeof(TServiceType).IsAssignableFrom(concreteType)) throw new System.ArgumentException("Type must implement " + typeof(TServiceType).Name);

            var inst = Services.Get<TServiceType>();
            if (inst != null) return inst;

            if (typeof(Component).IsAssignableFrom(concreteType))
            {
                var obj = ServiceComponent<TServiceType>.Create(concreteType, persistent, name);
                Services.Register<TServiceType>(obj);
                return obj;
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(concreteType))
            {
                var obj = ServiceScriptableObject<TServiceType>.Create(concreteType, persistent, name);
                Services.Register<TServiceType>(obj);
                return obj;
            }
            else
            {
                try
                {
                    var obj = System.Activator.CreateInstance(concreteType) as TServiceType;
                    Services.Register<TServiceType>(obj);
                    return obj;
                }
                catch (System.Exception ex)
                {
                    throw new System.InvalidOperationException("Supplied concrete service type failed to construct.", ex);
                }
            }
        }

        #endregion

        #region Special Types

        private static class Entry<T> where T : class, IService
        {

            public static T Instance;

        }

        #endregion

    }

    /// <summary>
    /// Abstract component for implementing a Service Component. 
    /// 
    /// When implementing pass in the interface as the generic T parameter to denote as what type this service 
    /// should be accessed when calling Service.Get&ltT&gt.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ServiceComponent<T> : SPComponent, IService where T : class, IService
    {

        #region Fields

        [SerializeField]
        private bool _autoRegisterService;
        [SerializeField]
        private bool _destroyIfMultiple;

        #endregion

        #region CONSTRUCTOR

        public ServiceComponent()
        {

        }

        public ServiceComponent(bool autoRegister, bool destroyIfMultiple)
        {
            _autoRegisterService = autoRegister;
            _destroyIfMultiple = destroyIfMultiple;
        }

        protected override void Awake()
        {
            base.Awake();

            if (!(this is T))
            {
                if (_destroyIfMultiple)
                {
                    ObjUtil.SmartDestroy(this);
                }
                return;
            }

            if (this.ValidateService())
            {
                this.OnValidAwake();
            }
        }

        private bool ValidateService()
        {
            var inst = Services.Get<T>();
            if (inst == null)
            {
                if (_autoRegisterService)
                {
                    Services.Register<T>(this as T);
                }

                return true;
            }
            else if (object.ReferenceEquals(this, inst))
            {
                return true;
            }
            else
            {
                if (_destroyIfMultiple)
                {
                    ObjUtil.SmartDestroy(this);
                }
                return false;
            }
        }

        protected virtual void OnValidAwake()
        {

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            var inst = Services.Get<T>();
            if (object.ReferenceEquals(this, inst))
            {
                Services.Unregister<T>();
            }
        }

        #endregion

        #region Properties

        public bool AutoRegisterService
        {
            get { return _autoRegisterService; }
            set
            {
                _autoRegisterService = value;
                if (value && this.started) this.ValidateService();
            }
        }

        public bool DestroyIfMultiple
        {
            get { return _destroyIfMultiple; }
            set
            {
                _destroyIfMultiple = value;
                if (value && this.started) this.ValidateService();
            }
        }

        #endregion

        #region IService Interface

        public event System.EventHandler ServiceUnregistered;

        void IService.SignalServiceUnregistered()
        {
            this.OnServiceUnregistered();
            this.ServiceUnregistered?.Invoke(this, System.EventArgs.Empty);
        }

        protected virtual void OnServiceUnregistered()
        {
        }

        #endregion

        #region Static Factory

        public static T Get()
        {
            return Services.Get<T>();
        }

        public static TConcrete Create<TConcrete>(bool persistent = false, string name = null) where TConcrete : Component, T
        {
            var inst = Services.Get<T>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(T).Name));

            if (name == null)
                name = "Service." + typeof(T).Name;
            var go = new GameObject(name);
            if (persistent)
            {
                GameObject.DontDestroyOnLoad(go);
            }
            return go.AddComponent<TConcrete>();
        }

        public static T Create(System.Type tp, bool persistent = false, string name = null)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(Component).IsAssignableFrom(tp) || !typeof(T).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must be a Component that implements " + typeof(T).Name);

            var inst = Services.Get<T>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(T).Name));

            if (name == null)
                name = "Service." + typeof(T).Name;
            var go = new GameObject(name);
            if (persistent)
            {
                GameObject.DontDestroyOnLoad(go);
            }
            return go.AddComponent(tp) as T;
        }

        public static T GetOrCreate<TConcrete>(bool persistent = false, string name = null) where TConcrete : Component, T
        {
            var inst = Services.Get<T>();
            if (inst != null) return inst;

            if (name == null)
                name = "Service." + typeof(T).Name;
            var go = new GameObject(name);
            if (persistent)
            {
                GameObject.DontDestroyOnLoad(go);
            }
            return go.AddComponent<TConcrete>();
        }

        public static T GetOrCreate(System.Type tp, bool persistent = false, string name = null)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(Component).IsAssignableFrom(tp) || !typeof(T).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must be a Component that implements " + typeof(T).Name);

            var inst = Services.Get<T>();
            if (inst != null) return inst;

            if (name == null)
                name = "Service." + typeof(T).Name;
            var go = new GameObject(name);
            if (persistent)
            {
                GameObject.DontDestroyOnLoad(go);
            }
            return go.AddComponent(tp) as T;
        }

        #endregion

    }

    /// <summary>
    /// Abstract component for implementing a Service Component. 
    /// 
    /// When implementing pass in the interface as the generic T parameter to denote as what type this service 
    /// should be accessed when calling Service.Get&ltT&gt.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ServiceScriptableObject<T> : ScriptableObject, IService where T : class, IService
    {

        #region Fields

        [SerializeField]
        private bool _autoRegisterService;
        [SerializeField]
        private bool _destroyIfMultiple;

        #endregion

        #region CONSTRUCTOR

        protected virtual void OnEnable() //NOTE - using OnEnable now since it appears Awake doesn't occur on SOs that are created as an asset and loaded that way.
        {
            if (!(this is T))
            {
                this.AutoDestruct();
                return;
            }

            var inst = Services.Get<T>();
            if (inst == null)
            {
                if (_autoRegisterService)
                    Services.Register<T>(this as T);
            }
            else if (object.ReferenceEquals(this, inst))
            {
                //do nothing
            }
            else
            {
                this.AutoDestruct();
                return;
            }

            this.OnValidAwake();
        }

        private void AutoDestruct()
        {
            if (_destroyIfMultiple)
            {
                ObjUtil.SmartDestroy(this);
            }
        }

        protected virtual void OnValidAwake()
        {

        }

        protected virtual void OnDestroy()
        {
            var inst = Services.Get<T>();
            if (object.ReferenceEquals(this, inst))
            {
                Services.Unregister<T>();
            }
        }

        #endregion

        #region IService Interface

        public event System.EventHandler ServiceUnregistered;

        void IService.SignalServiceUnregistered()
        {
            this.OnServiceUnregistered();
            if (this.ServiceUnregistered != null) this.ServiceUnregistered(this, System.EventArgs.Empty);
        }

        protected virtual void OnServiceUnregistered()
        {
        }

        #endregion

        #region Static Factory

        public static T Get()
        {
            return Services.Get<T>();
        }

        public static TConcrete Create<TConcrete>(bool persistent = false, string name = null) where TConcrete : ScriptableObject, T
        {
            var inst = Services.Get<T>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(T).Name));

            if (name == null)
                name = "Service." + typeof(T).Name;
            var obj = ScriptableObject.CreateInstance<TConcrete>();
            obj.name = name;
            return obj;
        }

        public static T Create(System.Type tp, bool persistent = false, string name = null)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(ScriptableObject).IsAssignableFrom(tp) || !typeof(T).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must be a Component that implements " + typeof(T).Name);

            var inst = Services.Get<T>();
            if (inst != null) throw new System.InvalidOperationException(string.Format("A service of type '{0}' already exists.", typeof(T).Name));

            if (name == null)
                name = "Service." + typeof(T).Name;
            var obj = ScriptableObject.CreateInstance(tp) as ScriptableObject;
            obj.name = name;
            return obj as T;
        }

        public static T GetOrCreate<TConcrete>(bool persistent = false, string name = null) where TConcrete : ScriptableObject, T
        {
            var inst = Services.Get<T>();
            if (inst != null) return inst;

            if (name == null)
                name = "Service." + typeof(T).Name;
            var obj = ScriptableObject.CreateInstance<TConcrete>();
            obj.name = name;
            return obj;
        }

        public static T GetOrCreate(System.Type tp, bool persistent = false, string name = null)
        {
            if (tp == null) throw new System.ArgumentNullException("tp");
            if (!typeof(ScriptableObject).IsAssignableFrom(tp) || !typeof(T).IsAssignableFrom(tp)) throw new System.ArgumentException("Type must be a Component that implements " + typeof(T).Name);

            var inst = Services.Get<T>();
            if (inst != null) return inst;

            if (name == null)
                name = "Service." + typeof(T).Name;
            var obj = ScriptableObject.CreateInstance(tp) as ScriptableObject;
            obj.name = name;
            return obj as T;
        }

        #endregion


    }

}
