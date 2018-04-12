#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Events
{

    public class t_OnTriggerOccupied : SPComponent, ICompoundTriggerEnterResponder, ICompoundTriggerExitResponder, IObservableTrigger
    {

        #region Fields

        [SerializeField]
        private SPEvent _onTriggerOccupied;

        [SerializeField]
        private SPEvent _onTriggerLastExited;

        [SerializeField]
        private bool _useEntity;

        [SerializeField]
        private EventActivatorMask _mask = new EventActivatorMask(-1);

        [SerializeField]
        private HashSet<GameObject> _activeObjects = new HashSet<GameObject>();

        #endregion

        #region Properties

        public SPEvent OnTriggerOccupied
        {
            get { return _onTriggerOccupied; }
        }

        public SPEvent OnTriggerLastExited
        {
            get { return _onTriggerLastExited; }
        }

        public bool UseEntity
        {
            get { return _useEntity; }
            set { _useEntity = value; }
        }

        public EventActivatorMask Mask
        {
            get { return _mask; }
            set { _mask = value; }
        }

        #endregion

        #region Methods

        private void AddObject(GameObject obj)
        {
            if (_useEntity)
            {
                var entity = SPEntity.Pool.GetFromSource(obj);
                if (entity == null) return;

                obj = entity.gameObject;
            }

            if (!_mask.Intersects(obj)) return;

            if (_activeObjects.Count == 0)
            {
                _activeObjects.Add(obj);
                _onTriggerOccupied.ActivateTrigger(this, obj);
            }
            else
            {
                _activeObjects.Add(obj);
            }
        }

        private void RemoveObject(GameObject obj)
        {
            if (_activeObjects.Count == 0) return;

            if (_useEntity)
            {
                var entity = SPEntity.Pool.GetFromSource(obj);
                if (entity == null) return;

                obj = entity.gameObject;
            }

            _activeObjects.Remove(obj);
            if (_activeObjects.Count == 0)
            {
                _onTriggerLastExited.ActivateTrigger(this, obj);
            }
        }

        #endregion

        #region Messages

        void OnTriggerEnter(Collider other)
        {
            if (this.HasComponent<CompoundTrigger>() || other == null) return;

            this.AddObject(other.gameObject);
        }

        void OnTriggerExit(Collider other)
        {
            if (this.HasComponent<CompoundTrigger>() || other == null) return;

            this.RemoveObject(other.gameObject);
        }

        void ICompoundTriggerEnterResponder.OnCompoundTriggerEnter(Collider other)
        {
            if (other == null) return;
            this.AddObject(other.gameObject);
        }

        void ICompoundTriggerExitResponder.OnCompoundTriggerExit(Collider other)
        {
            if (other == null) return;
            this.RemoveObject(other.gameObject);
        }

        #endregion

        #region IObservableTrigger Interface

        BaseSPEvent[] IObservableTrigger.GetEvents()
        {
            return new BaseSPEvent[] { _onTriggerOccupied, _onTriggerLastExited };
        }

        #endregion

    }

}
