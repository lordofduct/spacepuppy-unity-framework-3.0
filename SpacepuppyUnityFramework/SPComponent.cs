using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// A base implementation of components used by most of Spacepuppy Framework. It expands on the functionality of MonoBehaviour as well as implements various interfaces from the Spacepuppy framework. 
    /// 
    /// All scripts that are intended to work in tandem with Spacepuppy Unity Framework should inherit from this instead of MonoBehaviour.
    /// </summary>
    public abstract class SPComponent : MonoBehaviour, IEventfulComponent, ISPDisposable
    {

        #region Events

        public event System.EventHandler OnEnabled;
        public event System.EventHandler OnStarted;
        public event System.EventHandler OnDisabled;
        public event System.EventHandler ComponentDestroyed;

        #endregion

#region Fields
        
        [System.NonSerialized()]
        private bool _started = false;

#endregion

#region CONSTRUCTOR

        protected virtual void Awake()
        {
            if (this is IMixin) MixinUtil.Register(this as IMixin);
        }

        protected virtual void Start()
        {
            _started = true;
            if (this.OnStarted != null) this.OnStarted(this, System.EventArgs.Empty);
        }
        
        protected virtual void OnEnable()
        {
            if (this.OnEnabled != null) this.OnEnabled(this, System.EventArgs.Empty);
        }

        protected virtual void OnDisable()
        {
            if (this.OnDisabled != null) this.OnDisabled(this, System.EventArgs.Empty);
        }
        
        protected virtual void OnDestroy()
        {
            if (this.ComponentDestroyed != null) this.ComponentDestroyed(this, System.EventArgs.Empty);
        }

#endregion

#region Properties

        /// <summary>
        /// Start has been called on this component.
        /// </summary>
        public bool started { get { return _started; } }
        
        #endregion

        #region Root Methods
            
        [System.NonSerialized()]
        private GameObject _entityRoot;

        public GameObject entityRoot
        {
            get
            {
                if (object.ReferenceEquals(_entityRoot, null)) _entityRoot = this.FindRoot();
                return _entityRoot;
            }
        }

        /// <summary>
        /// Call this to resync the 'root' property incase the hierarchy of this object has changed. This needs to be performed since 
        /// unity doesn't have an event/message to signal a change in hierarchy.
        /// </summary>
        public virtual void SyncEntityRoot()
        {
            _entityRoot = this.FindRoot();
        }
            
        #endregion
        
#region IComponent Interface

        bool IComponent.enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        Component IComponent.component
        {
            get { return this; }
        }

        //implemented implicitly
        /*
        GameObject IComponent.gameObject { get { return this.gameObject; } }
        Transform IComponent.transform { get { return this.transform; } }
        */

#endregion

#region ISPDisposable Interface
    
        bool ISPDisposable.IsDisposed
        {
            get
            {
                return !ObjUtil.IsObjectAlive(this);
            }
        }

        void System.IDisposable.Dispose()
        {
            ObjUtil.SmartDestroy(this);
        }

#endregion

    }

}
