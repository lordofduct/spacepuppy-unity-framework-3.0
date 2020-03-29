﻿using System.Collections.Generic;

namespace com.spacepuppy.StateMachine
{

    /// <summary>
    /// Acts as a basic base state machine that manages a single state at a time. All one must do is wire up the triggers.
    /// </summary>
    public class StateMachine<T> : IStateMachine<T> where T : class
    {

        #region Fields

        private IStateSupplier<T> _states;
        private T _current;

        #endregion

        #region CONSTRUCTOR

        public StateMachine(IStateSupplier<T> supplier)
        {
            if (supplier == null) throw new System.ArgumentNullException("supplier");
            _states = supplier;
        }

        #endregion

        #region Properties

        public IStateSupplier<T> StateSupplier { get { return _states; } }

        #endregion

        #region IStateMachine Interface

        public event StateChangedEventHandler<T> StateChanged;
        protected void OnStateChanged(StateChangedEventArgs<T> e)
        {
            if (this.StateChanged != null) this.StateChanged(this, e);
        }

        public T Current { get { return _current; } }

        public bool Contains(T state)
        {
            return _states.Contains(state);
        }

        public T ChangeState(T newState)
        {
            if (object.Equals(newState, _current)) return _current;

            var oldState = _current;
            _current = newState;

            this.OnStateChanged(new StateChangedEventArgs<T>(oldState, newState));

            return _current;
        }

        public void GetStates(ICollection<T> coll)
        {
            _states.GetStates(coll);
        }
        
        public void Foreach(System.Action<T> callback)
        {
            _states.Foreach(callback);
        }

        #endregion

        #region IEnumerable Interface

        public IEnumerator<T> GetEnumerator()
        {
            return _states.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _states.GetEnumerator();
        }

        #endregion

        #region Static Factory

        public static StateMachine<T> CreateFromComponentSource(UnityEngine.GameObject source)
        {
            return new StateMachine<T>(new ComponentStateSupplier<T>(source));
        }

        /// <summary>
        /// Create a ParentComponentStateSupplier
        /// </summary>
        /// <param name="source"></param>
        /// <param name="includeStatesOnContainer"></param>
        /// <param name="isStatic">Set true if the hierarchy doesn't change (a changing hierarchy requires extra processing and is slower)</param>
        /// <returns></returns>
        public static StateMachine<T> CreateFromParentComponentSource(UnityEngine.GameObject source, bool includeStatesOnContainer, bool isStatic)
        {
            return new StateMachine<T>(new ParentComponentStateSupplier<T>(source, includeStatesOnContainer, isStatic));
        }

        #endregion

    }
}
