using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Sensors
{

    public abstract class Sensor : SPEntityComponent
    {

        #region Fields

        [System.NonSerialized()]
        private CompositeSensor _compositeSensorParent;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            this.SyncCompositeSensorParent();
        }

        protected override void Start()
        {
            base.Start();

            if (_compositeSensorParent != null && !_compositeSensorParent.Contains(this)) _compositeSensorParent.SyncChildSensors();
        }

        private void SyncCompositeSensorParent()
        {
            if (this is CompositeSensor)
            {
                _compositeSensorParent = null;
                return;
            }

            _compositeSensorParent = this.GetComponent<CompositeSensor>();
            if (_compositeSensorParent == null) _compositeSensorParent = this.GetComponentInParent<CompositeSensor>();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        /// Since Sensors may filter out objects based on masks, this returns true if the sensor would even bother attempting to see a GameObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool ConcernedWith(UnityEngine.Object obj);

        public abstract bool Visible(IAspect aspect);

        public abstract bool SenseAny(System.Func<IAspect, bool> predicate = null);

        public abstract IAspect Sense(System.Func<IAspect, bool> predicate = null);

        public abstract int SenseAll(ICollection<IAspect> lst, System.Func<IAspect, bool> predicate = null);

        public abstract IEnumerable<IAspect> SenseAll(System.Func<IAspect, bool> predicate = null);

        public abstract int SenseAll<T>(ICollection<T> lst, System.Func<T, bool> predicate = null) where T : class, IAspect;
        
        #endregion

    }

}
