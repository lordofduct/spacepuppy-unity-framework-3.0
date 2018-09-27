#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    [Infobox("Watches a set of IOccupiedTriggers (example: t_OnTriggerOccupied) for when all of them are occupied and signals.")]
    public class t_AllTriggersOccupied : SPComponent, IOccupiedTrigger, IMStartOrEnableReceiver
    {

        #region Fields

        [SerializeField]
        [ReorderableArray]
        [DisableOnPlay()]
        [TypeRestriction(typeof(IOccupiedTrigger))]
        private UnityEngine.Object[] _observedTargets;

        [SerializeField]
        private SPEvent _onEnter;

        [SerializeField]
        private SPEvent _onExit;

        [System.NonSerialized]
        private IOccupiedTrigger[] _targets;
        [System.NonSerialized]
        private HashSet<IOccupiedTrigger> _activatedTriggers = new HashSet<IOccupiedTrigger>();
        [System.NonSerialized()]
        private bool _triggered;

        #endregion

        #region CONSTRUCTOR

        protected override void Awake()
        {
            base.Awake();

            _targets = (from o in _observedTargets where o is IOccupiedTrigger select o as IOccupiedTrigger).ToArray();
        }

        void IMStartOrEnableReceiver.OnStartOrEnable()
        {
            _triggered = false;
            _activatedTriggers.Clear();
            foreach (var t in _targets)
            {
                t.EnterEvent.TriggerActivated += this.OnTriggerEntered;
                t.ExitEvent.TriggerActivated += this.OnTriggerExited;
                if (t.IsOccupied) _activatedTriggers.Add(t);
            }
            this.TestSignalOccupied();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _triggered = false;
            _activatedTriggers.Clear();
            foreach (var t in _targets)
            {
                t.EnterEvent.TriggerActivated -= this.OnTriggerEntered;
                t.ExitEvent.TriggerActivated -= this.OnTriggerExited;
            }
        }

        #endregion

        #region Properties

        public SPEvent OnEnter
        {
            get { return _onEnter; }
        }

        public SPEvent OnExit
        {
            get { return _onExit; }
        }

        public bool IsOccupied
        {
            get { return _triggered; }
        }

        #endregion

        #region Methods

        private void OnTriggerEntered(object sender, System.EventArgs ev)
        {
            var targ = sender as IOccupiedTrigger;
            if (targ != null && _activatedTriggers.Add(targ))
            {
                this.TestSignalOccupied();
            }
        }

        private void OnTriggerExited(object sender, System.EventArgs ev)
        {
            var targ = sender as IOccupiedTrigger;
            if (targ != null && _activatedTriggers.Remove(targ))
            {
                if (_triggered)
                {
                    _triggered = false;
                    _onExit.ActivateTrigger(this, null);
                }
            }
        }

        private void TestSignalOccupied()
        {
            if (_triggered) return;

            if (_activatedTriggers.SetEquals(_targets))
            {
                _triggered = true;
                _onEnter.ActivateTrigger(this, null);
            }
        }

        #endregion

        #region IOccupiedObservableTrigger Interface

        BaseSPEvent[] IObservableTrigger.GetEvents()
        {
            return new BaseSPEvent[] { _onEnter, _onExit };
        }

        BaseSPEvent IOccupiedTrigger.EnterEvent
        {
            get { return _onEnter; }
        }

        BaseSPEvent IOccupiedTrigger.ExitEvent
        {
            get { return _onExit; }
        }

        #endregion

    }

}
