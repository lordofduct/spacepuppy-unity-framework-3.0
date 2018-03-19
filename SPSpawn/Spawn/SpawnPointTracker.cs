#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Events;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Spawn
{

    public sealed class SpawnPointTracker : SPComponent, IMStartOrEnableReceiver
    {

        #region Fields

        [SerializeField]
        [ReorderableArray]
        [TypeRestriction(typeof(ISpawnPoint))]
        private List<Component> _spawnPoints;

        [SerializeField]
        private SPEvent _onSpawnedObject;
        [SerializeField]
        private SPEvent _onKilledObject;


        [System.NonSerialized]
        private HashSet<SpawnedObjectController> _objects = new HashSet<SpawnedObjectController>();

        #endregion

        #region CONSTRUCTOR

        void IMStartOrEnableReceiver.OnStartOrEnable()
        {
            var e = _spawnPoints.GetEnumerator();
            while(e.MoveNext())
            {
                var s = e.Current as ISpawnPoint;
                if (s != null)
                {
                    s.OnSpawned.TriggerActivated -= this.OnSpawnedObjectHandler;
                    s.OnSpawned.TriggerActivated += this.OnSpawnedObjectHandler;
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            var e = _spawnPoints.GetEnumerator();
            while (e.MoveNext())
            {
                var s = e.Current as ISpawnPoint;
                if (s != null)
                {
                    s.OnSpawned.TriggerActivated -= this.OnSpawnedObjectHandler;
                }
            }

            var e2 = _objects.GetEnumerator();
            while(e2.MoveNext())
            {

            }
            _objects.Clear();
        }

        #endregion

        #region Properties

        public SPEvent OnSpawnedObject
        {
            get { return _onSpawnedObject; }
        }

        public SPEvent OnKilledObject
        {
            get { return _onKilledObject; }
        }

        [ShowNonSerializedProperty("Active Count:", Readonly = true)]
        public int ActiveObjectCount
        {
            get { return _objects.Count; }
        }

        #endregion

        #region Methods

        public IEnumerable<ISpawnPoint> GetSpawnPoints()
        {
            return (from p in _spawnPoints select p as ISpawnPoint);
        }

        #endregion

        #region Event Handlers

        private System.EventHandler<TempEventArgs> _onSpawnedObjectHandler;
        private System.EventHandler<TempEventArgs> OnSpawnedObjectHandler
        {
            get
            {
                if (_onSpawnedObjectHandler == null) _onSpawnedObjectHandler = this.OnSpawnedObjectHandler_Imp;
                return _onSpawnedObjectHandler;
            }
        }
        private void OnSpawnedObjectHandler_Imp(object sender, TempEventArgs e)
        {
            var obj = ObjUtil.GetAsFromSource<SpawnedObjectController>(e.Value);
            if(obj != null)
            {
                obj.OnKilled -= this.OnKilledObjectHandler_Imp;
                obj.OnKilled += this.OnKilledObjectHandler_Imp;
                _onSpawnedObject.ActivateTrigger(this, obj);
            }
        }


        private System.EventHandler _onKilledObjectHandler;
        private System.EventHandler OnKilledObjectHandler
        {
            get
            {
                if (_onKilledObjectHandler == null) _onKilledObjectHandler = this.OnKilledObjectHandler_Imp;
                return _onKilledObjectHandler;
            }
        }
        private void OnKilledObjectHandler_Imp(object sender, System.EventArgs e)
        {
            var obj = ObjUtil.GetAsFromSource<SpawnedObjectController>(sender);
            if (obj != null)
            {
                _objects.Remove(obj);
                obj.OnKilled -= this.OnKilledObjectHandler;
                _onKilledObject.ActivateTrigger(this, obj);
            }
        }

        #endregion

    }

}
