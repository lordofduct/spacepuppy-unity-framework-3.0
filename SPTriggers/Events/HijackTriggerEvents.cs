#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Events
{

    public sealed class HijackTriggerEvents : SPComponent, IMStartOrEnableReceiver, IObservableTrigger
    {

        #region Fields

        [SerializeField]
        [ReorderableArray]
        private ObservableTargetData[] _targets;

        [SerializeField]
        private SPEvent _onHijacked;

        [SerializeField]
        [Tooltip("If true the target won't be purged of its listeners when hijacked.")]
        private bool _dontOverrideTargets;

        #endregion

        #region CONSTRUCTOR

        void IMStartOrEnableReceiver.OnStartOrEnable()
        {
            foreach (var t in _targets)
            {
                t.Init();
                t.TriggerActivated += this.OnHijackedEventActivated;
                if (!_dontOverrideTargets) t.BeginHijack();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var t in _targets)
            {
                t.TriggerActivated -= this.OnHijackedEventActivated;
                t.EndHijack();
                t.DeInit();
            }
        }

        #endregion

        #region Properties

        public SPEvent OnHijacked
        {
            get { return _onHijacked; }
        }

        public bool DontOverrideTargets
        {
            get { return _dontOverrideTargets; }
            set { _dontOverrideTargets = value; }
        }

        #endregion

        #region Methods

        private void OnHijackedEventActivated(object sender, TempEventArgs e)
        {
            _onHijacked.ActivateTrigger(this, e.Value);
        }

        #endregion

        #region IObservableTrigger Interface

        BaseSPEvent[] IObservableTrigger.GetEvents()
        {
            return new BaseSPEvent[] { _onHijacked };
        }

        #endregion

    }

}
