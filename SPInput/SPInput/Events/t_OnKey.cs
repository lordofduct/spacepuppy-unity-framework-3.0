#pragma warning disable 0649 // variable declared but not used.
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Events;

namespace com.spacepuppy.SPInput.Events
{

    public class t_OnKey : SPComponent, IObservableTrigger
    {

        #region Fields

        [SerializeField]
        private List<KeyInfo> _keys;

        #endregion

        #region Properties

        public List<KeyInfo> Keys
        {
            get
            {
                return _keys;
            }
        }

        #endregion

        #region Methods

        private void Update()
        {
            foreach (var key in _keys)
            {
                if (key == null) continue;

                switch (key.State)
                {
                    case ButtonState.Released:
                        if (Input.GetKeyUp(key.Key))
                        {
                            key.OnKey.ActivateTrigger(this, null);
                        }
                        break;
                    case ButtonState.None:
                        if (!Input.GetKey(key.Key) && !Input.GetKeyUp(key.Key))
                        {
                            key.OnKey.ActivateTrigger(this, null);
                        }
                        break;
                    case ButtonState.Down:
                        if (Input.GetKeyDown(key.Key))
                        {
                            key.OnKey.ActivateTrigger(this, null);
                        }
                        break;
                    case ButtonState.Held:
                        if (Input.GetKey(key.Key) && !Input.GetKeyDown(key.Key))
                        {
                            key.OnKey.ActivateTrigger(this, null);
                        }
                        break;
                }
            }
        }

        #endregion

        #region IObservableTrigger Interface
        
        BaseSPEvent[] IObservableTrigger.GetEvents()
        {
            return (from k in _keys where k != null select k.OnKey).ToArray();
        }

        #endregion

        #region Special Types

        [System.Serializable]
        public class KeyInfo
        {
            public KeyCode Key = KeyCode.Return;
            public ButtonState State = ButtonState.Down;
            [SerializeField]
            [SPEvent.Config(AlwaysExpanded = true)]
            private SPEvent _onKey = new SPEvent();

            public SPEvent OnKey
            {
                get { return _onKey; }
            }
        }

        #endregion

    }

}
