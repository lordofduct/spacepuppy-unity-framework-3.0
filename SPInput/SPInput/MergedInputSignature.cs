using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.SPInput
{

    /// <summary>
    /// This allows multiple input signatures to register for a single input id. So say 2 buttons can perform the same action. 
    /// The order of precedence is in the order of the signatures in the list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MergedInputSignature<T> : BaseInputSignature where T : IInputSignature
    {

        #region Fields

        private HashSet<T> _signatures = new HashSet<T>();

        #endregion

        #region CONSTRUCTOR

        public MergedInputSignature(string id)
            : base(id)
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
            while (e.MoveNext())
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

        public override void Reset()
        {
            var e = _signatures.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Reset();
            }
        }

        #endregion

    }

    public class MergedButtonInputSignature : MergedInputSignature<IButtonInputSignature>, IButtonInputSignature
    {

        private ButtonState _current;
        private ButtonState _currentFixed;
        private float _lastDown;

        #region CONSTRUCTOR

        public MergedButtonInputSignature(string id)
            : base(id)
        {
        }

        public MergedButtonInputSignature(string id, IButtonInputSignature a, IButtonInputSignature b)
            : base(id)
        {
            this.Signatures.Add(a);
            this.Signatures.Add(b);
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

        public void Consume()
        {
            if (GameLoop.CurrentSequence == UpdateSequence.FixedUpdate)
            {
                _currentFixed = InputUtil.ConsumeButtonState(_currentFixed);
            }
            else
            {
                _current = InputUtil.ConsumeButtonState(_current);
            }
        }

        public float LastDownTime
        {
            get { return _lastDown; }
        }

        public override void Update()
        {
            base.Update();

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
            _current = InputUtil.GetNextButtonState(_current, down);

            if (_current == ButtonState.Down)
                _lastDown = Time.realtimeSinceStartup;
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

        public override void Reset()
        {
            base.Reset();

            _current = ButtonState.None;
            _currentFixed = ButtonState.None;
            _lastDown = 0f;
        }

    }

    public class MergedAxleInputSignature : MergedInputSignature<IAxleInputSignature>, IAxleInputSignature
    {

        private MergedAxlePrecedence _axlePrecedence;

        #region CONSTRUCTOR

        public MergedAxleInputSignature(string id)
            : base(id)
        {
        }

        public MergedAxleInputSignature(string id, IAxleInputSignature a, IAxleInputSignature b)
            : base(id)
        {
            this.Signatures.Add(a);
            this.Signatures.Add(b);
        }

        #endregion

        public MergedAxlePrecedence AxlePrecedence
        {
            get { return _axlePrecedence; }
            set { _axlePrecedence = value; }
        }

        public float DeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff Cutoff
        {
            get;
            set;
        }

        public float CurrentState
        {
            get
            {
                float result = 0f;

                switch (_axlePrecedence)
                {
                    case MergedAxlePrecedence.Largest:
                        if (this.SignaturesSet.Count > 0)
                        {
                            var e = this.SignaturesSet.GetEnumerator();
                            while (e.MoveNext())
                            {
                                if (e.Current.CurrentState >= 0f) result = e.Current.CurrentState;
                            }
                        }
                        break;
                    case MergedAxlePrecedence.Smallest:
                        if (this.Signatures.Count > 0)
                        {
                            result = float.PositiveInfinity;
                            var e = this.SignaturesSet.GetEnumerator();
                            while (e.MoveNext())
                            {
                                float s = e.Current.CurrentState;
                                if (s > 0f && s < result) result = e.Current.CurrentState;
                            }
                            if (float.IsPositiveInfinity(result)) result = 0f;
                        }
                        break;
                }

                return InputUtil.CutoffAxis(result, this.DeadZone, this.Cutoff);
            }
        }

    }

    public class MergedDualAxleInputSignature : MergedInputSignature<IDualAxleInputSignature>, IDualAxleInputSignature
    {

        private MergedAxlePrecedence _axlePrecedence;

        #region CONSTRUCTOR

        public MergedDualAxleInputSignature(string id)
            : base(id)
        {
        }

        public MergedDualAxleInputSignature(string id, IDualAxleInputSignature a, IDualAxleInputSignature b)
            : base(id)
        {
            this.Signatures.Add(a);
            this.Signatures.Add(b);
        }

        #endregion

        public MergedAxlePrecedence AxlePrecedence
        {
            get { return _axlePrecedence; }
            set { _axlePrecedence = value; }
        }

        public float DeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff Cutoff
        {
            get;
            set;
        }

        public float RadialDeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff RadialCutoff
        {
            get;
            set;
        }

        public Vector2 CurrentState
        {
            get
            {
                Vector2 result = Vector2.zero;
                switch (_axlePrecedence)
                {
                    case MergedAxlePrecedence.Largest:
                        if (this.Signatures.Count > 0)
                        {
                            float mag = 0f;
                            var e = this.SignaturesSet.GetEnumerator();
                            while (e.MoveNext())
                            {
                                if (e.Current.CurrentState.sqrMagnitude > mag)
                                {
                                    result = e.Current.CurrentState;
                                    mag = result.sqrMagnitude;
                                }
                            }
                        }
                        break;
                    case MergedAxlePrecedence.Smallest:
                        if (this.Signatures.Count > 0)
                        {
                            float mag = float.PositiveInfinity;
                            var e = this.SignaturesSet.GetEnumerator();
                            while (e.MoveNext())
                            {
                                if (e.Current.CurrentState.sqrMagnitude < mag)
                                {
                                    result = e.Current.CurrentState;
                                    mag = result.sqrMagnitude;
                                }
                            }
                        }
                        break;
                }

                return InputUtil.CutoffDualAxis(result, this.DeadZone, this.Cutoff, this.RadialDeadZone, this.RadialCutoff);
            }
        }
    }

}
