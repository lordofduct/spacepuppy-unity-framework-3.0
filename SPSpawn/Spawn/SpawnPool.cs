using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    public class SpawnPool : SPComponent, ICollection<IPrefabCache>
    {
        
        #region Static Multiton Interface

        public const string DEFAULT_SPAWNPOOL_NAME = "Spacepuppy.PrimarySpawnPool";

        private static SpawnPool _defaultPool;
        public static readonly MultitonPool<SpawnPool> Pools = new MultitonPool<SpawnPool>();

        public static SpawnPool DefaultPool
        {
            get
            {
                if (_defaultPool == null) CreatePrimaryPool();
                return _defaultPool;
            }
        }
        
        public static void CreatePrimaryPool()
        {
            if (PrimaryPoolExists) return;

            var go = new GameObject(DEFAULT_SPAWNPOOL_NAME);
            _defaultPool = go.AddComponent<SpawnPool>();
        }

        public static bool PrimaryPoolExists
        {
            get
            {
                if (_defaultPool != null) return true;

                _defaultPool = null;
                if(Pools.Count > 0)
                {
                    SpawnPool point = null;
                    var e = Pools.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current.CompareName(DEFAULT_SPAWNPOOL_NAME))
                        {
                            point = e.Current;
                            break;
                        }
                    }

                    if (!object.ReferenceEquals(point, null))
                    {
                        _defaultPool = point;
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion

        #region Fields

        [SerializeField()]
        [ReorderableArray(DrawElementAtBottom = true, ChildPropertyToDrawAsElementLabel = "ItemName", ChildPropertyToDrawAsElementEntry = "_prefab")]
        private List<PrefabCache> _registeredPrefabs = new List<PrefabCache>();

        [System.NonSerialized()]
        private Dictionary<int, PrefabCache> _prefabToCache = new Dictionary<int, PrefabCache>();

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            Pools.AddReference(this);
            if(this.CompareName(DEFAULT_SPAWNPOOL_NAME) && _defaultPool == null)
            {
                _defaultPool = this;
            }
        }

        protected override void Start()
        {
            base.Start();

            var e = _registeredPrefabs.GetEnumerator();
            while(e.MoveNext())
            {
                if (e.Current.Prefab == null) continue;

                e.Current.Load();
                _prefabToCache[e.Current.PrefabID] = e.Current;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (object.ReferenceEquals(this, _defaultPool))
            {
                _defaultPool = null;
            }
            Pools.RemoveReference(this);

            var e = _registeredPrefabs.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Clear();
            }
        }

        #endregion

        #region Properties

        private string _cachedName;
        public new string name
        {
            get
            {
                if (_cachedName == null) _cachedName = this.gameObject.name;
                return _cachedName;
            }
            set
            {
                this.gameObject.name = value;
                _cachedName = value;
            }
        }

        #endregion

        #region Methods

        public IPrefabCache Register(GameObject prefab, string sname, int cacheSize = 0, int resizeBuffer = 1, int limitAmount = 1)
        {
            if (object.ReferenceEquals(prefab, null)) throw new System.ArgumentNullException("prefab");
            if (_prefabToCache.ContainsKey(prefab.GetInstanceID())) throw new System.ArgumentException("Already manages prefab.", "prefab");

            var cache = new PrefabCache(prefab, sname)
            {
                CacheSize = cacheSize,
                ResizeBuffer = resizeBuffer,
                LimitAmount = limitAmount
            };

            _registeredPrefabs.Add(cache);
            _prefabToCache[cache.PrefabID] = cache;
            cache.Load();
            return cache;
        }

        public bool UnRegister(GameObject prefab)
        {
            var cache = this.FindPrefabCache(prefab);
            if (cache == null) return false;

            return this.UnRegister(cache);
        }

        public bool UnRegister(int prefabId)
        {
            PrefabCache cache;
            if (!_prefabToCache.TryGetValue(prefabId, out cache)) return false;

            return this.UnRegister(cache);
        }

        public bool UnRegister(IPrefabCache cache)
        {
            var obj = cache as PrefabCache;
            if (obj == null) return false;
            if (obj.Owner != this) return false;

            obj.Clear();
            _registeredPrefabs.Remove(obj);
            _prefabToCache.Remove(obj.PrefabID);
            return true;
        }

        public bool Contains(int prefabId)
        {
            return _prefabToCache.ContainsKey(prefabId);
        }

        public bool Contains(string sname)
        {
            var e = _registeredPrefabs.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Name == sname)
                {
                    return true;
                }
            }
            return false;
        }





        public GameObject SpawnByIndex(int index, Transform par = null)
        {
            if (index < 0 || index >= _registeredPrefabs.Count) throw new System.IndexOutOfRangeException();

            var cache = _registeredPrefabs[index];
            var pos = (par != null) ? par.position : Vector3.zero;
            var rot = (par != null) ? par.rotation : Quaternion.identity;
            var obj = cache.Spawn(pos, rot, par);
            this.SignalSpawned(obj);
            return obj.gameObject;
        }

        public GameObject SpawnByIndex(int index, Vector3 position, Quaternion rotation, Transform par = null)
        {
            if (index < 0 || index >= _registeredPrefabs.Count) throw new System.IndexOutOfRangeException();

            var cache = _registeredPrefabs[index];
            var obj = cache.Spawn(position, rotation, par);
            this.SignalSpawned(obj);
            return obj.gameObject;
        }

        public GameObject Spawn(string sname, Transform par = null)
        {
            PrefabCache cache = null;
            var e = _registeredPrefabs.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Name == sname)
                {
                    cache = e.Current;
                    break;
                }
            }
            if (cache == null) return null;

            var pos = (par != null) ? par.position : Vector3.zero;
            var rot = (par != null) ? par.rotation : Quaternion.identity;
            var obj = cache.Spawn(pos, rot, par);
            this.SignalSpawned(obj);
            return obj.gameObject;
        }

        public GameObject Spawn(string sname, Vector3 position, Quaternion rotation, Transform par = null)
        {
            PrefabCache cache = null;
            var e = _registeredPrefabs.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Name == sname)
                {
                    cache = e.Current;
                    break;
                }
            }
            if (cache == null) return null;

            var obj = cache.Spawn(position, rotation, par);
            this.SignalSpawned(obj);
            return obj.gameObject;
        }

        public GameObject Spawn(GameObject prefab, Transform par = null)
        {
            var controller = SpawnAsController(prefab, par);
            return (controller != null) ? controller.gameObject : null;
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform par = null)
        {
            var controller = SpawnAsController(prefab, position, rotation, par);
            return (controller != null) ? controller.gameObject : null;
        }

        public SpawnedObjectController SpawnAsController(GameObject prefab, Transform par = null)
        {
            if (prefab == null) return null;

            var cache = this.FindPrefabCache(prefab);
            var pos = (par != null) ? par.position : Vector3.zero;
            var rot = (par != null) ? par.rotation : Quaternion.identity;

            if (cache != null)
            {
                var controller = cache.Spawn(pos, rot, par);
                this.SignalSpawned(controller);
                return controller;
            }
            else
            {
                var obj = Object.Instantiate(prefab, pos, rot, par);
                if (obj != null)
                {
                    var controller = obj.AddOrGetComponent<SpawnedObjectController>();
                    controller.Init(this, prefab.GetInstanceID());
                    controller.SetSpawned();
                    this.SignalSpawned(controller);
                    return controller;
                }
                else
                {
                    return null;
                }
            }
        }

        public SpawnedObjectController SpawnAsController(GameObject prefab, Vector3 position, Quaternion rotation, Transform par = null)
        {
            if (prefab == null) return null;

            var cache = this.FindPrefabCache(prefab);
            if (cache != null)
            {
                var controller = cache.Spawn(position, rotation, par);
                this.SignalSpawned(controller);
                return controller;
            }
            else
            {
                var obj = Object.Instantiate(prefab, position, rotation, par);
                if (obj != null)
                {
                    var controller = obj.AddOrGetComponent<SpawnedObjectController>();
                    controller.Init(this, prefab.GetInstanceID());
                    controller.SetSpawned();
                    this.SignalSpawned(controller);
                    return controller;
                }
                else
                {
                    return null;
                }
            }
        }

        public GameObject SpawnByPrefabId(int prefabId, Transform par = null)
        {
            var controller = SpawnAsControllerByPrefabId(prefabId, par);
            return (controller != null) ? controller.gameObject : null;
        }

        public GameObject SpawnByPrefabId(int prefabId, Vector3 position, Quaternion rotation, Transform par = null)
        {
            var controller = SpawnAsControllerByPrefabId(prefabId, position, rotation, par);
            return (controller != null) ? controller.gameObject : null;
        }

        public SpawnedObjectController SpawnAsControllerByPrefabId(int prefabId, Transform par = null)
        {
            PrefabCache cache;
            if (!_prefabToCache.TryGetValue(prefabId, out cache)) return null;

            var pos = (par != null) ? par.position : Vector3.zero;
            var rot = (par != null) ? par.rotation : Quaternion.identity;
            var controller = cache.Spawn(pos, rot, par);
            this.SignalSpawned(controller);
            return controller;
        }

        public SpawnedObjectController SpawnAsControllerByPrefabId(int prefabId, Vector3 position, Quaternion rotation, Transform par = null)
        {
            PrefabCache cache;
            if (!_prefabToCache.TryGetValue(prefabId, out cache)) return null;
            var controller = cache.Spawn(position, rotation, par);
            this.SignalSpawned(controller);
            return controller;
        }



        internal bool Despawn(SpawnedObjectController cntrl)
        {
            if (Object.ReferenceEquals(cntrl, null)) throw new System.ArgumentNullException("cntrl");

            PrefabCache cache;
            if(!_prefabToCache.TryGetValue(cntrl.PrefabID, out cache) || !cache.ContainsActive(cntrl))
            {
                return false;
            }

            this.gameObject.Broadcast<IOnDespawnHandler, SpawnedObjectController>(cntrl, (o, c) => o.OnDespawn(c));
            cntrl.gameObject.Broadcast<IOnDespawnHandler, SpawnedObjectController>(cntrl, (o, c) => o.OnDespawn(c));
            return cache.Despawn(cntrl);
        }



        public bool Purge(GameObject obj)
        {
            if (object.ReferenceEquals(obj, null)) throw new System.ArgumentNullException("obj");

            var cntrl = obj.GetComponent<SpawnedObjectController>();
            if (cntrl == null) return false;

            return this.Purge(cntrl);
        }

        public bool Purge(SpawnedObjectController cntrl)
        {
            if (object.ReferenceEquals(cntrl, null)) throw new System.ArgumentNullException("cntrl");

            PrefabCache cache;
            if (!_prefabToCache.TryGetValue(cntrl.PrefabID, out cache) || !cache.Contains(cntrl))
            {
                return false;
            }

            return cache.Purge(cntrl);
        }




        /// <summary>
        /// Match an object to its prefab if this pool manages the GameObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private PrefabCache FindPrefabCache(GameObject obj)
        {
            //TODO - figure out the best way to match a gameobject to the cache pool.
            //as it stands this depends on the prefab being a shared instance across all scripts... 
            //I am unsure if that's how unity works, and what the limitations are on that.
            //consider creating a system of relating equal prefabs

            //test if the object is the prefab in question
            int id = obj.GetInstanceID();
            if (_prefabToCache.ContainsKey(id)) return _prefabToCache[id];

            var controller = obj.FindComponent<SpawnedObjectController>();
            if (controller == null || controller.Pool != this) return null;

            id = controller.PrefabID;
            PrefabCache result;
            if (_prefabToCache.TryGetValue(id, out result)) return result;
            
            return null;
        }

        private void SignalSpawned(SpawnedObjectController cntrl)
        {
            this.gameObject.Broadcast<IOnSpawnHandler, SpawnedObjectController>(cntrl, (o, c) => o.OnSpawn(c));
            cntrl.gameObject.Broadcast<IOnSpawnHandler, SpawnedObjectController>(cntrl, (o, c) => o.OnSpawn(c));
        }

        #endregion

        #region ICollection Interface

        public int Count
        {
            get { return _registeredPrefabs.Count; }
        }

        bool ICollection<IPrefabCache>.IsReadOnly
        {
            get { return false; }
        }

        void ICollection<IPrefabCache>.Add(IPrefabCache item)
        {
            throw new System.NotSupportedException();
        }

        public bool Contains(IPrefabCache item)
        {
            var obj = item as PrefabCache;
            if (item == null) return false;

            return _registeredPrefabs.Contains(item);
        }

        public void Clear()
        {
            var e = _registeredPrefabs.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Clear();
            }

            _registeredPrefabs.Clear();
            _prefabToCache.Clear();
        }

        bool ICollection<IPrefabCache>.Remove(IPrefabCache item)
        {
            return this.UnRegister(item);
        }

        void ICollection<IPrefabCache>.CopyTo(IPrefabCache[] array, int arrayIndex)
        {
            for (int i = 0; i < _registeredPrefabs.Count; i++)
            {
                if (i >= array.Length) return;

                array[arrayIndex + i] = _registeredPrefabs[i];
            }
        }
        
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<IPrefabCache> IEnumerable<IPrefabCache>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        private class PrefabCache : IPrefabCache
        {

            #region Fields

            [SerializeField]
            private string _itemName;
            [SerializeField]
            private GameObject _prefab;
            
            [Tooltip("The starting CacheSize.")]
            public int CacheSize = 0;
            [Tooltip("How much should the cache resize by if an empty/used cache is spawned from.")]
            public int ResizeBuffer = 1;
            [Tooltip("The maximum number of instances allowed to be cached, 0 or less means infinite.")]
            public int LimitAmount = 0;

            [System.NonSerialized()]
            private SpawnPool _owner;
            [System.NonSerialized()]
            private HashSet<SpawnedObjectController> _instances;
            [System.NonSerialized()]
            private HashSet<SpawnedObjectController> _activeInstances;

            #endregion

            #region CONSTRUCTOR

            protected PrefabCache()
            {
                _instances = new HashSet<SpawnedObjectController>(ObjectReferenceEqualityComparer<SpawnedObjectController>.Default);
                _activeInstances = new HashSet<SpawnedObjectController>(ObjectReferenceEqualityComparer<SpawnedObjectController>.Default);
            }
            
            public PrefabCache(GameObject prefab, string name)
            {
                _prefab = prefab;
                _itemName = name;
                _instances = new HashSet<SpawnedObjectController>(ObjectReferenceEqualityComparer<SpawnedObjectController>.Default);
                _activeInstances = new HashSet<SpawnedObjectController>(ObjectReferenceEqualityComparer<SpawnedObjectController>.Default);
            }

            public void Init(SpawnPool owner)
            {
                _owner = owner;
            }

            #endregion

            #region Properties
            
            public SpawnPool Owner
            {
                get { return _owner; }
            }

            public string Name
            {
                get { return _itemName; }
            }

            public GameObject Prefab
            {
                get { return _prefab; }
            }

            public int PrefabID
            {
                //use HashCode since it returns the same as GetInstanceID, but it doesn't check thread
                get { return _prefab.GetHashCode(); }
            }

            int IPrefabCache.CacheSize
            {
                get { return this.CacheSize; }
                set { this.CacheSize = value; }
            }

            int IPrefabCache.ResizeBuffer
            {
                get { return this.ResizeBuffer; }
                set { this.ResizeBuffer = value; }
            }

            int IPrefabCache.LimitAmount
            {
                get { return this.LimitAmount; }
                set { this.LimitAmount = value; }
            }

            public int Count
            {
                get { return _instances.Count + _activeInstances.Count; }
            }

            #endregion

            #region Methods

            internal bool Contains(SpawnedObjectController cntrl)
            {
                return _instances.Contains(cntrl) || _activeInstances.Contains(cntrl);
            }

            internal bool ContainsActive(SpawnedObjectController cntrl)
            {
                return _activeInstances.Contains(cntrl);
            }

            internal void Load()
            {
                this.Clear();
                if (_prefab == null) return;

                for(int i = 0; i < this.CacheSize; i++)
                {
                    _instances.Add(this.CreateCachedInstance());
                }
            }

            internal void Clear()
            {
                if(_instances.Count > 0)
                {
                    var e = _instances.GetEnumerator();
                    while(e.MoveNext())
                    {
                        e.Current.DeInit();
                        Object.Destroy(e.Current.gameObject);
                    }
                    _instances.Clear();
                }

                _activeInstances.Clear();
            }

            internal SpawnedObjectController Spawn(Vector3 pos, Quaternion rot, Transform par)
            {
                if(_instances.Count == 0)
                {
                    int cnt = this.Count;
                    int newSize = cnt + this.ResizeBuffer;
                    if (this.LimitAmount > 0) newSize = Mathf.Min(newSize, this.LimitAmount);

                    if(newSize > cnt)
                    {
                        for(int i = cnt; i < newSize; i++)
                        {
                            _instances.Add(this.CreateCachedInstance());
                        }
                    }
                }

                if(_instances.Count > 0)
                {
                    var cntrl = _instances.Pop();

                    _activeInstances.Add(cntrl);

                    cntrl.transform.parent = par;
                    cntrl.transform.position = pos;
                    cntrl.transform.rotation = rot;
                    cntrl.SetSpawned();

                    return cntrl;
                }
                else
                {
                    var obj = Object.Instantiate(this.Prefab, pos, rot, par);
                    if (obj != null)
                    {
                        var controller = obj.AddOrGetComponent<SpawnedObjectController>();
                        controller.Init(_owner, this.Prefab.GetInstanceID());
                        controller.SetSpawned();
                        _owner.SignalSpawned(controller);
                        return controller;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            internal bool Despawn(SpawnedObjectController cntrl)
            {
                if (!_activeInstances.Remove(cntrl)) return false;

                cntrl.SetDespawned();
                cntrl.transform.parent = _owner.transform;
                cntrl.transform.localPosition = Vector3.zero;
                cntrl.transform.rotation = Quaternion.identity;

                _instances.Add(cntrl);
                return true;
            }

            internal bool Purge(SpawnedObjectController cntrl)
            {
                if (_activeInstances.Remove(cntrl))
                    return true;
                if (_instances.Remove(cntrl))
                    return true;

                return false;
            }

            private SpawnedObjectController CreateCachedInstance()
            {
                var obj = Object.Instantiate(this.Prefab, Vector3.zero, Quaternion.identity);
                obj.name = _itemName + "(CachedInstance)";
                var cntrl = obj.AddOrGetComponent<SpawnedObjectController>();
                cntrl.Init(_owner, this.Prefab.GetInstanceID(), _itemName);

                obj.transform.parent = _owner.transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;
                
                obj.SetActive(false);

                return cntrl;
            }

            #endregion

        }
        
        public struct Enumerator : IEnumerator<IPrefabCache>
        {

            private List<PrefabCache>.Enumerator _e;

            public Enumerator(SpawnPool pool)
            {
                if (pool == null) throw new System.ArgumentNullException("pool");

                _e = pool._registeredPrefabs.GetEnumerator();
            }

            public IPrefabCache Current
            {
                get
                {
                    return _e.Current;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return _e.Current;
                }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            public void Dispose()
            {
                _e.Dispose();
            }

            void System.Collections.IEnumerator.Reset()
            {
                //DO NOTHING
            }
        }

        #endregion

        #region Static Methods

        public static void DeSpawn(GameObject go)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            var c = go.GetComponent<SpawnedObjectController>();
            if (c == null) return;

            c.Kill();
        }

        #endregion

    }

}
