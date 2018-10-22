#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Events;

namespace com.spacepuppy.SPInput.Events
{

    public class t_OnAnyInput : SPComponent, IObservableTrigger
    {

        [System.Flags]
        public enum TrackingOptions
        {
            All = -1,
            None = 0,
            TrackUnityKeyboard = 1,
            TrackMouseScroll = 2,
            TrackMousePosition = 4,
            TrackSPInputs = 8
        }

        #region Fields

        [SerializeField]
        [EnumFlags]
        private TrackingOptions _options = TrackingOptions.All;

        [SerializeField]
        private SPEvent _onInput;

        private Vector3 _lastMouse;

        #endregion

        #region Properties

        public TrackingOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        public SPEvent OnInput
        {
            get { return _onInput; }
        }

        #endregion

        #region Methods

        protected override void OnEnable()
        {
            base.OnEnable();

            _lastMouse = Input.mousePosition;
        }

        // Update is called once per frame
        void Update()
        {
            var mpos = _lastMouse;
            _lastMouse = Input.mousePosition;

            if (((_options & TrackingOptions.TrackUnityKeyboard) != 0 && Input.anyKeyDown) ||
                ((_options & TrackingOptions.TrackMouseScroll) != 0 && Input.mouseScrollDelta.sqrMagnitude > 0.0001f) ||
                ((_options & TrackingOptions.TrackMousePosition) != 0 && (mpos - _lastMouse).sqrMagnitude > 0.0001f))
            {
                _onInput.ActivateTrigger(this, null);
                return;
            }

            if ((_options & TrackingOptions.TrackSPInputs) != 0)
            {
                var inputManager = Services.Get<IInputManager>();
                foreach (var dev in inputManager)
                {
                    if (dev.AnyInputActivated)
                    {
                        _onInput.ActivateTrigger(this, null);
                        return;
                    }
                }
            }
        }

        #endregion

        #region IObservableTrigger Interface

        BaseSPEvent[] IObservableTrigger.GetEvents()
        {
            return new BaseSPEvent[] { _onInput };
        }

        #endregion

    }

}
