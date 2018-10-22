using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Motor
{

    /// <summary>
    /// IMotor interface for a Rigidbody that treats the Rigidbody as a simulation of forces.
    /// 
    /// Velocity/Forces are used to move.
    /// </summary>
    [RequireComponentInEntity(typeof(Rigidbody))]
    public class SimulatedRigidbodyMotor : SPComponent, IMotor, IUpdateable
    {

        #region Fields

        [SerializeField]
        [DefaultFromSelf(Relativity = EntityRelativity.Entity)]
        private Rigidbody _rigidbody;
        [SerializeField]
        [OneOrMany]
        [Tooltip("Colliders considered associated with this Motor, leave empty if this should be auto-associated at Awake.")]
        private Collider[] _colliders;

        [SerializeField()]
        private float _stepOffset;
        [SerializeField()]
        private float _skinWidth;
        [SerializeField()]
        [Tooltip("When false Velocity is reset to 0 if Move is not called in FixedUpdate.")]
        private bool _freeMovement;

        [System.NonSerialized()]
        private Vector3 _lastPos;
        [System.NonSerialized()]
        private Vector3 _lastVel;
        [System.NonSerialized()]
        private Vector3 _talliedMove;
        [System.NonSerialized]
        private bool _moveCalled;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if (!object.ReferenceEquals(_rigidbody, null) && _colliders == null || _colliders.Length == 0)
            {
                _colliders = _rigidbody.GetComponentsInChildren<Collider>();
            }
        }

        protected override void OnEnable()
        {
            if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("SimulatedRigidbodyMotor must be initialized with an appropriate Rigidbody.");

            base.OnEnable();

            _rigidbody.isKinematic = false;

            _lastPos = _rigidbody.position;
            _lastVel = Vector3.zero;
            _talliedMove = Vector3.zero;
            _moveCalled = false;
            
            GameLoop.TardyFixedUpdatePump.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            GameLoop.TardyFixedUpdatePump.Remove(this);
        }

        #endregion

        #region Properties

        public Rigidbody Rigidbody
        {
            get { return _rigidbody; }
            set { _rigidbody = value; }
        }

        public Collider[] Colliders
        {
            get { return _colliders; }
            set { _colliders = value ?? ArrayUtil.Empty<Collider>(); }
        }

        public bool FreeMovement
        {
            get { return _freeMovement; }
            set { _freeMovement = value; }
        }

        #endregion

        #region IMotor Interface

        public bool PrefersFixedUpdate
        {
            get { return true; }
        }

        public float Mass
        {
            get
            {
                return !object.ReferenceEquals(_rigidbody, null) ? _rigidbody.mass : 0f;
            }
            set
            {
                if (!object.ReferenceEquals(_rigidbody, null)) _rigidbody.mass = value;
            }
        }

        public float StepOffset
        {
            get
            {
                return _stepOffset;
            }
            set
            {
                _stepOffset = Mathf.Max(value, 0f);
            }
        }

        public float SkinWidth
        {
            get
            {
                return _skinWidth;
            }
            set
            {
                _skinWidth = Mathf.Max(value, 0f);
            }
        }

        public bool CollisionEnabled
        {
            get
            {
                for (int i = 0; i < _colliders.Length; i++)
                {
                    if (!_colliders[i].enabled) return false;
                }
                return true;
            }
            set
            {
                for (int i = 0; i < _colliders.Length; i++)
                {
                    _colliders[i].enabled = value;
                }
            }
        }

        public Vector3 Velocity
        {
            get
            {
                return !object.ReferenceEquals(_rigidbody, null) ? _rigidbody.velocity : Vector3.zero;
            }
            set
            {
                if (!object.ReferenceEquals(_rigidbody, null)) _rigidbody.velocity = value;
                _talliedMove = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                return !object.ReferenceEquals(_rigidbody, null) ? _rigidbody.position : Vector3.zero;
            }
            set
            {
                if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("SimulatedRigidbodyMotor must be initialized with an appropriate Rigidbody.");

                _rigidbody.position = value;
            }
        }

        public Vector3 LastPosition
        {
            get { return _lastPos; }
        }

        public Vector3 LastVelocity
        {
            get { return _lastVel; }
        }



        public void Move(Vector3 mv)
        {
            if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("SimulatedRigidbodyMotor must be initialized with an appropriate Rigidbody.");

            _talliedMove += mv;

            Vector3 v = _talliedMove / Time.deltaTime;
            //v -= _owner.LastVelocity; //remove the old velocity so it's setting to, not adding to
            //_rigidbody.AddForce(v, ForceMode.VelocityChange);
            _rigidbody.velocity = v;

            _moveCalled = true;
        }

        public void AtypicalMove(Vector3 mv)
        {
            if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("SimulatedRigidbodyMotor must be initialized with an appropriate Rigidbody.");

            //_rigidbody.MovePosition(_rigidbody.position + mv);
            _rigidbody.position += mv; //for some reason moveposition doesn't work with moving platforms
        }

        public void MovePosition(Vector3 pos, bool setVelocityByChangeInPosition = false)
        {
            if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("SimulatedRigidbodyMotor must be initialized with an appropriate Rigidbody.");

            if (setVelocityByChangeInPosition)
            {
                var v = (pos - _rigidbody.position);
                v /= Time.deltaTime;
                _rigidbody.velocity = v;
                _moveCalled = true;
            }
            _rigidbody.MovePosition(pos);
        }

        public void AddForce(Vector3 f, ForceMode mode)
        {
            if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("SimulatedRigidbodyMotor must be initialized with an appropriate Rigidbody.");

            _rigidbody.AddForce(f, mode);
            _moveCalled = true;
        }

        public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
        {
            if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("SimulatedRigidbodyMotor must be initialized with an appropriate Rigidbody.");

            _rigidbody.AddForceAtPosition(f, pos, mode);
            _moveCalled = true;
        }

        #endregion

        #region IPhysicsObject Interface

        public bool TestOverlap(int layerMask, QueryTriggerInteraction query)
        {
            foreach(var c in _colliders)
            {
                if (GeomUtil.GetGeom(c).TestOverlap(layerMask, query)) return true;
            }

            return false;
        }

        public int Overlap(ICollection<Collider> results, int layerMask, QueryTriggerInteraction query)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            using (var set = TempCollection.GetSet<Collider>())
            {
                foreach (var c in _colliders)
                {
                    GeomUtil.GetGeom(c).Overlap(set, layerMask, query);
                }

                if(set.Count > 0)
                {
                    var e = set.GetEnumerator();
                    while(e.MoveNext())
                    {
                        results.Add(e.Current);
                    }
                    return set.Count;
                }
            }

            return 0;
        }

        public bool Cast(Vector3 direction, out RaycastHit hitinfo, float distance, int layerMask, QueryTriggerInteraction query)
        {
            foreach (var c in _colliders)
            {
                if (GeomUtil.GetGeom(c).Cast(direction, out hitinfo, distance, layerMask, query)) return true;
            }

            hitinfo = default(RaycastHit);
            return false;
        }

        public int CastAll(Vector3 direction, ICollection<RaycastHit> results, float distance, int layerMask, QueryTriggerInteraction query)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            using (var set = TempCollection.GetSet<RaycastHit>())
            {
                foreach (var c in _colliders)
                {
                    GeomUtil.GetGeom(c).CastAll(direction, set, distance, layerMask, query);
                }

                if(set.Count > 0)
                {
                    var e = set.GetEnumerator();
                    while(e.MoveNext())
                    {
                        results.Add(e.Current);
                    }
                    return set.Count;
                }
            }

            return 0;
        }

        #endregion

        #region IUpdatable Interface

        void IUpdateable.Update()
        {
            if (!_freeMovement && !_moveCalled)
            {
                _rigidbody.velocity = Vector3.zero;
                _lastVel = Vector3.zero;
            }
            else
            {
                _lastVel = _rigidbody.velocity;
            }

            _lastPos = _rigidbody.position;
            _talliedMove = Vector3.zero;
            _moveCalled = false;
        }
        
        #endregion
        
    }

}
