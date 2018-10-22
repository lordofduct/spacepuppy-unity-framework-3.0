using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Anim.Legacy
{

    /// <summary>
    /// It is usually more convenient to break Animation scripts into several parts for the various tasks they handle. Each script should inherit from this class as it handles a lot of boilerplate.
    /// </summary>
    public abstract class SPLegacyAnimator : SPComponent, ISPAnimator
    {

        #region Fields

        [SerializeField()]
        [DefaultFromSelf(Relativity = EntityRelativity.Entity)]
        private SPLegacyAnimController _controller;

        [System.NonSerialized]
        private bool _initialized;

        #endregion

        #region CONSTRUCTOR
        
        protected override void Start()
        {
            var entity = SPEntity.Pool.GetFromSource<SPEntity>(this);
            if (!_initialized && entity != null && entity.IsAwake)
            {
                _initialized = true;
                this.Init(entity, _controller);
            }

            base.Start();
        }

        public void Configure(SPLegacyAnimController controller)
        {
            if (_initialized) throw new System.InvalidOperationException("Can not change the Controller of an SPAnimator once it's been initialized.");
            _controller = controller;
        }

        protected abstract void Init(SPEntity entity, SPLegacyAnimController controller);

        #endregion

        #region Properties

        public SPLegacyAnimController Controller
        {
            get
            {
                return _controller;
            }
        }

        public bool IsInitialized
        {
            get { return _initialized; }
        }

        #endregion

        #region ISPAnimator Interface
        
        #endregion

    }

}
