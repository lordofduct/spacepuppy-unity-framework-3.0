using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    public class SpawnPool : SPComponent, IEnumerable<IPrefabCache>
    {
        
        #region Static Multiton Interface

        public const string DEFAULT_SPAWNPOOL_NAME = "Spacepuppy.PrimarySpawnPool";

        private static SpawnPool _defaultPool;
        private static HashSet<SpawnPool> _pools = new HashSet<SpawnPool>();

        public static SpawnPool DefaultPool
        {
            get
            {
                if (_defaultPool == null) CreatePrimaryPool();
                return _defaultPool;
            }
        }
        
        public static SpawnPool Pool(string name)
        {
            if (_defaultPool != null && _defaultPool.CachedName == name) return _defaultPool;

            //TODO - should cache 'name' for access so this doesn't generate garbage
            var e = _pools.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.CachedName == name) return e.Current;
            }
            return null;
        }

        public static int PoolCount { get { return _pools.Count; } }

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
                var point = (from p in GameObject.FindObjectsOfType<SpawnPool>() where p.CachedName == DEFAULT_SPAWNPOOL_NAME select p).FirstOrDefault();
                if (!object.ReferenceEquals(point, null))
                {
                    _defaultPool = point;
                    return true;
                }
                else
                {
                    return false;
                }
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

            _pools.Add(this);
            if(this.CachedName == DEFAULT_SPAWNPOOL_NAME && _defaultPool == null)
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
            _pools.Remove(this);

            var e = _registeredPrefabs.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Clear();
            }
        }

        #endregion

        #region Properties

        private string _cachedName;
        public string CachedName
        {
            get
            {
                if (_cachedName == null) _cachedName = this.name;
                return _cachedName;
            }
            set
            {
                this.name = value;
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





        public GameObject Spawn(int index, Transform par = null)
        {
            if (index < 0 || index >= _registeredPrefabs.Count) throw new System.IndexOutOfRangeException();

            var cache = _registeredPrefabs[index];
            var pos = (par != null) ? par.position : Vector3.zero;
            var rot = (par != null) ? par.rotation : Quaternion.identity;
            var obj = cache.Spawn(pos, rot, par);
            this.SignalSpawned(obj);
            return obj.gameObject;
        }

        public GameObject Spawn(int index, Vector3 position, Quaternion rotation, Transform par = null)
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

        internal bool Purge(SpawnedObjectController cntrl)
        {
            if (Object.ReferenceEquals(cntrl, null)) throw new System.ArgumentNullException("cntrl");

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
            if (controller == null && controller.Pool != this) return null;

            id = controller.PrefabID;
            if (_prefabToCache.ContainsKey(id)) return _prefabToCache[id];
            
            return null;
        }

        private void SignalSpawned(SpawnedObjectController cntrl)
        {
            this.gameObject.Broadcast<IOnSpawnHandler, SpawnedObjectController>(cntrl, (o, c) => o.OnSpawn(c));
            cntrl.gameObject.Broadcast<IOnSpawnHandler, SpawnedObjectController>(cntrl, (o, c) => o.OnSpawn(c));
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<IPrefabCache> GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
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
            [Tooltip("The maximum number of instances allowed to be cached.")]
            public int LimitAmount = 0;

            [System.NonSerialized()]
            private SpawnPool _owner;
            [System.NonSerialized()]
            private HashSet<SpawnedObjectController> _instances = new HashSet<SpawnedObjectController>(com.spacepuppy.Collections.ObjectReferenceEqualityComparer<SpawnedObjectController>.Default);
            [System.NonSerialized()]
            private HashSet<SpawnedObjectController> _activeInstances = new HashSet<SpawnedObjectController>(com.spacepuppy.Collections.ObjectReferenceEqualityComparer<SpawnedObjectController>.Default);

            #endregion

            #region CONSTRUCTOR

            public PrefabCache(GameObject prefab, string name)
            {
                _prefab = prefab;
                _itemName = name;
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
                    _instances.Add(this.CreateCachedInstance(i));
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
                            _instances.Add(this.CreateCachedInstance(i));
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

            private SpawnedObjectController CreateCachedInstance(int index)
            {
                var obj = Object.Instantiate(this.Prefab, Vector3.zero, Quaternion.identity);
                obj.name = _itemName + (index + 1).ToString("000");
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
