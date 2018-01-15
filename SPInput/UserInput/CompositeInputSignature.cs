using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.UserInput
{

    /// <summary>
    /// This allows multiple input signatures to register for a single input id. So say 2 buttons can perform the same action. 
    /// The order of precedence is in the order of the signatures in the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CompositeInputSignature<T> : BaseInputSignature where T : IInputSignature
    {

        #region Fields

        private HashSet<T> _signatures = new HashSet<T>();

        #endregion

        #region CONSTRUCTOR

        public CompositeInputSignature(string id)
            : base(id)
        {
        }

        public CompositeInputSignature(string id, int hash)
            : base(id, hash)
        {
        }

        #endregion

        #region Properties

        public ICollection<T> Signatures { get { return _signatures; } }

        protected HashSet<T> SignaturesSet { get { return _signatures; } }

        #endregion

        #region Methods

        public override void Update()
        {
            var e = _signatures.GetEnumerator();
            while(e.MoveNext())
            {
                e.Current.Update();
            }
        }

        public override void FixedUpdate()
        {
            var e = _signatures.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.FixedUpdate();
            }
        }

        #endregion

    }

    public class CompositeButtonInputSignature : CompositeInputSignature<IButtonInputSignature>, IButtonInputSignature
    {

        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        #region CONSTRUCTOR

        public CompositeButtonInputSignature(string id)
            : base(id)
        {
        }

        public CompositeButtonInputSignature(string id, int hash)
            : base(id, hash)
        {
        }

        #endregion

        public ButtonState CurrentState
        {
            get
            {
                if (GameLoop.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return _currentFixed;
                }
                else
                {
                    return _current;
                }
            }
        }

        public ButtonState GetCurrentState(bool getFixedState)
        {
            return (getFixedState) ? _currentFixed : _current;
        }

        public bool GetPressed(float duration, bool getFixedState)
        {
            if (getFixedState)
            {
                return _currentFixed == ButtonState.Released && Time.unscaledTime - _lastDown <= duration;
            }
            else
            {
                return _current == ButtonState.Released && Time.unscaledTime - _lastDown <= duration;
            }
        }

        public override void Update()
        {
            base.Update();

            bool down = false;
            var e = this.SignaturesSet.GetEnumerator();
            while(e.MoveNext())
            {
                if(e.Current.GetCurrentState(false) >= ButtonState.Down)
                {
                    down = true;
                    break;
                }
            }
            _current = InputUtil.GetNextButtonState(_current, down);

            if (_current == ButtonState.Down)
                _lastDown = Time.unscaledTime;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            bool down = false;
            var e = this.SignaturesSet.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.GetCurrentState(false) >= ButtonState.Down)
                {
                    down = true;
                    break;
                }
            }
            _currentFixed = InputUtil.GetNextButtonState(_currentFixed, down);
        }
    }

    public class CompositeAxleInputSignature : CompositeInputSignature<IAxleInputSignature>, IAxleInputSignature
    {

        private CompositeAxlePrecedence _axlePrecedence;

        #region CONSTRUCTOR

        public CompositeAxleInputSignature(string id)
            : base(id)
        {
        }

        public CompositeAxleInputSignature(string id, int hash)
            : base(id, hash)
        {
        }

        #endregion

        public CompositeAxlePrecedence AxlePrecedence
        {
            get { return _axlePrecedence; }
            set { _axlePrecedence = value; }
        }

        public float CurrentState
        {
            get
            {
                switch (_axlePrecedence)
                {
                    case CompositeAxlePrecedence.Largest:
                        if (this.SignaturesSet.Count > 0)
                        {
                            float v = 0f;
                            var e = this.SignaturesSet.GetEnumerator();
                            while (e.MoveNext())
                            {
                                if (e.Current.CurrentState >= 0f) v = e.Current.CurrentState;
                            }
                            return v;
                        }
                        break;
                    case CompositeAxlePrecedence.Smallest:
                        if (this.Signatures.Count > 0)
                        {
                            float v = float.PositiveInfinity;
                            var e = this.SignaturesSet.GetEnumerator();
                            while (e.MoveNext())
                            {
                                float s = e.Current.CurrentState;
                                if (s > 0f && s < v) v = e.Current.CurrentState;
                            }
                            return (float.IsPositiveInfinity(v)) ? 0f : v;
                        }
                        break;
                }
                return 0f;
            }
        }
    }

    public class CompositeDualAxleInputSignature : CompositeInputSignature<IDualAxleInputSignature>, IDualAxleInputSignature
    {

        private CompositeAxlePrecedence _axlePrecedence;

        #region CONSTRUCTOR

        public CompositeDualAxleInputSignature(string id)
            : base(id)
        {
        }

        public CompositeDualAxleInputSignature(string id, int hash)
            : base(id, hash)
        {
        }

        #endregion

        public CompositeAxlePrecedence AxlePrecedence
        {
            get { return _axlePrecedence; }
            set { _axlePrecedence = value; }
        }

        public Vector2 CurrentState
        {
            get
            {
                switch (_axlePrecedence)
                {
                    case CompositeAxlePrecedence.Largest:
                        if (this.Signatures.Count > 0)
                        {
                            Vector2 v = Vector2.zero;
                            float mag = 0f;
                            var e = this.SignaturesSet.GetEnumerator();
                            while(e.MoveNext())
                            {
                                if(e.Current.CurrentState.sqrMagnitude > mag)
                                {
                                    v = e.Current.CurrentState;
                                    mag = v.sqrMagnitude;
                                }
                            }
                            return v;
                        }
                        break;
                    case CompositeAxlePrecedence.Smallest:
                        if (this.Signatures.Count > 0)
                        {
                            Vector2 v = Vector2.zero;
                            float mag = float.PositiveInfinity;
                            var e = this.SignaturesSet.GetEnumerator();
                            while (e.MoveNext())
                            {
                                if (e.Current.CurrentState.sqrMagnitude < mag)
                                {
                                    v = e.Current.CurrentState;
                                    mag = v.sqrMagnitude;
                                }
                            }
                            return v;
                        }
                        break;
                }
                return Vector2.zero;
            }
        }
    }

}

