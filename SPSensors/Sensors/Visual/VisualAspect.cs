using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Sensors.Visual
{

    public class VisualAspect : AbstractAspect
    {

        #region Static Multiton Interface

        private static MultitonPool<IAspect> _pool = new MultitonPool<IAspect>();

        public static MultitonPool<IAspect> Pool { get { return _pool; } }
        
        #endregion
        
        #region Fields

        [SerializeField()]
        private float _precedence;

        [SerializeField()]
        private Color _aspectColor = Color.blue;

        #endregion

        #region CONSTRUCTOR

        protected override void OnEnable()
        {
            _pool.AddReference(this);

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _pool.RemoveReference(this);
        }

        #endregion

        #region IAspect Interface
        
        public override float Precedence
        {
            get { return _precedence; }
            set { _precedence = value; }
        }

        public Color AspectColor
        {
            get { return _aspectColor; }
            set { _aspectColor = value; }
        }

        #endregion

    }

}
