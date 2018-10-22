using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Motor
{

    /// <summary>
    /// IMotor interface for a Rigidbody that treats the Rigidbody with out forces.
    /// 
    /// Rigidbody.MovePosition is used to move the Rigidbody around.
    /// </summary>
    public class RigidbodyMotor : SPComponent, IMotor, IUpdateable
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

        [System.NonSerialized()]
        private Vector3 _vel;
        [System.NonSerialized()]
        private Vector3 _lastPos;
        [System.NonSerialized()]
        private Vector3 _lastVel;

        [System.NonSerialized()]
        private bool _moveCalledLastFrame;
        [System.NonSerialized()]
        private Vector3 _talliedMove;
        [System.NonSerialized()]
        private float _lastDt;
        [System.NonSerialized()]
        private Vector3 _fullTalliedMove;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            if(!object.ReferenceEquals(_rigidbody, null) && _colliders == null || _colliders.Length == 0)
            {
                _colliders = _rigidbody.GetComponentsInChildren<Collider>();
            }
        }

        protected override void OnEnable()
        {
            if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("RigidbodyMotor must be initialized with an appropriate Rigidbody.");

            base.OnEnable();

            _rigidbody.isKinematic = false;
            _vel = Vector3.zero;
            _moveCalledLastFrame = false;
            _talliedMove = Vector3.zero;
            _lastDt = 0f;
            _fullTalliedMove = Vector3.zero;
            
            GameLoop.TardyFixedUpdatePump.Add(this);
            GameLoop.TardyFixedUpdate += this.OnTardyLateUpdate;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            GameLoop.TardyFixedUpdatePump.Remove(this);
            GameLoop.TardyFixedUpdate -= this.OnTardyLateUpdate;
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
                for(int i = 0; i < _colliders.Length; i++)
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
            get { return _vel; }
            set { _vel = value; }
        }

        public Vector3 Position
        {
            get
            {
                return !object.ReferenceEquals(_rigidbody, null) ? _rigidbody.position : Vector3.zero;
            }
            set
            {
                if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("RigidbodyMotor must be initialized with an appropriate Rigidbody.");

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
            if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("RigidbodyMotor must be initialized with an appropriate Rigidbody.");

            _fullTalliedMove += mv;
            _talliedMove += mv;
            _vel = _talliedMove / Time.deltaTime;
        }

        public void AtypicalMove(Vector3 mv)
        {
            if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("RigidbodyMotor must be initialized with an appropriate Rigidbody.");

            _fullTalliedMove += mv;
        }

        public void MovePosition(Vector3 pos, bool setVelocityByChangeInPosition = false)
        {
            if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("RigidbodyMotor must be initialized with an appropriate Rigidbody.");

            var mv = (pos - _rigidbody.position);
            _fullTalliedMove += mv;
            _talliedMove += mv;
            _vel = _talliedMove / Time.deltaTime;
        }

        public void AddForce(Vector3 f, ForceMode mode)
        {
            if (object.ReferenceEquals(_rigidbody, null)) throw new System.InvalidOperationException("RigidbodyMotor must be initialized with an appropriate Rigidbody.");

            switch (mode)
            {
                case ForceMode.Force:
                    //force = mass*distance/time^2
                    //distance = force * time^2 / mass
                    this.Move(f * Time.deltaTime * Time.deltaTime / this.Mass);
                    break;
                case ForceMode.Acceleration:
                    //acceleration = distance/time^2
                    //distance = acceleration * time^2
                    this.Move(f * (Time.deltaTime * Time.deltaTime));
                    break;
                case ForceMode.Impulse:
                    //impulse = mass*distance/time
                    //distance = impulse * time / mass
                    this.Move(f * Time.deltaTime / this.Mass);
                    break;
                case ForceMode.VelocityChange:
                    //velocity = distance/time
                    //distance = velocity * time
                    this.Move(f * Time.deltaTime);
                    break;
            }
        }

        public void AddForceAtPosition(Vector3 f, Vector3 pos, ForceMode mode)
        {
            this.AddForce(f, mode);
        }

        #endregion

        #region IPhysicsObject Interface

        public bool TestOverlap(int layerMask, QueryTriggerInteraction query)
        {
            foreach (var c in _colliders)
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

                if (set.Count > 0)
                {
                    var e = set.GetEnumerator();
                    while (e.MoveNext())
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

                if (set.Count > 0)
                {
                    var e = set.GetEnumerator();
                    while (e.MoveNext())
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
            _lastPos = _rigidbody.position;
            _lastVel = _vel;

            if (_moveCalledLastFrame)
            {
                _moveCalledLastFrame = false;

                //we calculate velocity of LAST move in this move
                if (_lastDt != 0f)
                {
                    var actualMove = (_rigidbody.transform.position - _lastPos);

                    actualMove -= (_fullTalliedMove - _talliedMove);

                    //_vel = actualMove / _lastDt;

                    //var n = actualMove.normalized;
                    //_vel = n * MathUtil.Average(actualMove.magnitude, Vector3.Dot(_talliedMove, n)) / _lastDt;

                    var n = _talliedMove.normalized;
                    _vel = n * Mathf.Max(Vector3.Dot(actualMove, n), 0f) / _lastDt;
                }
            }

            _fullTalliedMove = Vector3.zero;
            _talliedMove = Vector3.zero;
        }


        private void OnTardyLateUpdate_Imp(object sender, System.EventArgs e)
        {
            if (_fullTalliedMove != Vector3.zero)
            {
                _rigidbody.MovePosition(_rigidbody.position + _fullTalliedMove);
            }

            _lastDt = Time.deltaTime;

            //zero out rigidbody velocity, as this should ONLY use MovePosition
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
        private System.EventHandler _onTardyLateUpdate;
        private System.EventHandler OnTardyLateUpdate
        {
            get
            {
                if (_onTardyLateUpdate == null) _onTardyLateUpdate = this.OnTardyLateUpdate_Imp;
                return _onTardyLateUpdate;
            }
        }

        #endregion

    }

}
