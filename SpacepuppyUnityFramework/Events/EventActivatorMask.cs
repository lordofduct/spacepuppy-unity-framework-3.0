using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    [System.Serializable()]
    public struct EventActivatorMask
    {

        #region Fields

        private bool _testRoot;

        [SerializeField()]
        private LayerMask _layerMask;

        [SerializeField()]
        [ReorderableArray]
        [TagSelector()]
        private string[] _tags;

        #endregion

        #region CONSTRUCTOR
        
        public EventActivatorMask(LayerMask mask)
        {
            _testRoot = false;
            _layerMask = mask;
            _tags = null;
        }

        public EventActivatorMask(string[] tags)
        {
            _testRoot = false;
            _layerMask = -1;
            _tags = tags;
        }

        public EventActivatorMask(LayerMask mask, string[] tags)
        {
            _testRoot = false;
            _layerMask = mask;
            _tags = tags;
        }

        public EventActivatorMask(LayerMask mask, bool testRoot)
        {
            _testRoot = testRoot;
            _layerMask = mask;
            _tags = null;
        }

        public EventActivatorMask(string[] tags, bool testRoot)
        {
            _testRoot = testRoot;
            _layerMask = -1;
            _tags = tags;
        }

        public EventActivatorMask(LayerMask mask, string[] tags, bool testRoot)
        {
            _testRoot = testRoot;
            _layerMask = mask;
            _tags = tags;
        }

        #endregion

        #region Properties

        public bool TestRoot
        {
            get { return _testRoot; }
            set { _testRoot = value; }
        }

        public LayerMask LayerMask
        {
            get { return _layerMask; }
            set { _layerMask = value; }
        }

        public string[] Tags
        {
            get { return _tags; }
            set { _tags = value; }
        }

        #endregion

        #region Methods

        public bool Intersects(GameObject go)
        {
            if (go == null) return false;

            if (_testRoot) go = go.FindRoot();

            return go.IntersectsLayerMask(_layerMask) && (_tags == null || _tags.Length == 0 || go.HasTag(_tags));
        }

        public bool Intersects(Component comp)
        {
            if (comp == null) return false;
            return Intersects(comp.gameObject);
        }

        #endregion

    }

}
