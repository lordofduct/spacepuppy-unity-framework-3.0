using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Anim.Legacy
{

    public abstract class SPLegacyAnimator : SPComponent, ISPAnimator
    {

        #region Fields

        [SerializeField()]
        [DefaultFromSelf(UseEntity = true)]
        private SPLegacyAnimator _controller;

        [System.NonSerialized]
        private bool _initialized;

        #endregion

        #region CONSTRUCTOR
        
        protected override void Start()
        {
            base.Start();

            var entity = SPEntity.Pool.GetFromSource<SPEntity>(this);
            if (!_initialized && entity != null && entity.IsAwake)
            {
                _initialized = true;
                this.Init(entity, _controller);
            }
        }

        public void Configure(SPLegacyAnimator controller)
        {
            if (_initialized) throw new System.InvalidOperationException("Can not change the Controller of an SPAnimator once it's been initialized.");
            _controller = controller;
        }

        protected abstract void Init(SPEntity entity, SPLegacyAnimator controller);

        #endregion

        #region Properties

        public SPLegacyAnimator Controller
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

        public abstract void Play(string id, QueueMode queuMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer);

        #endregion

    }

}
