using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Place on the root of a GameObject hierarchy, or a prefab, to signify that it is a complete entity.
    /// 
    /// If this class is derived from, make sure to set its execution order to the last executed script! 
    /// Failure to do so will result in IEntityAwakeHandler receivers to be messaged out of order.
    /// </summary>
    [DisallowMultipleComponent()]
    public class SPEntity : SPComponent
    {

        #region Multiton Interface

        private static UniqueToEntityMultitonPool<SPEntity> _pool = new UniqueToEntityMultitonPool<SPEntity>();
        public static UniqueToEntityMultitonPool<SPEntity> Pool { get { return _pool; } }

        #endregion

        #region Fields

        [System.NonSerialized()]
        private bool _isAwake;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            this.AddTag(SPConstants.TAG_ROOT);
            _pool.AddReference(this);

            base.Awake();

            _isAwake = true;

            Messaging.Broadcast<IEntityAwakeHandler>(this.gameObject, (h) => h.OnEntityAwake(this));
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            _pool.RemoveReference(this);
        }

        #endregion

        #region Properties

        public bool IsAwake { get { return _isAwake; } }

        #endregion

        #region Methods

        private string _cachedName;
        public bool CompareName(string value)
        {
            if (_cachedName == null)
            {
                _cachedName = this.gameObject.name;
            }
            return _cachedName == value;
        }

        #endregion

    }
}
