#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using UnityEngine.EventSystems;

namespace com.spacepuppy.Events
{

    public class i_SetActiveUIElement : AutoTriggerable
    {

        #region Fields

        [SerializeField]
        private GameObject _element;

        #endregion

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;


            if (_element != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(_element);
                return true;
            }

            return false;
        }

    }

}
