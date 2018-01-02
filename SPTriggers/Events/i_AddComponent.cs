#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public class i_AddComponent : AutoTriggerable
    {

        #region Fields

        [SerializeField()]
        [TypeReference.Config(typeof(Component), dropDownStyle = TypeDropDownListingStyle.ComponentMenu)]
        private TypeReference _componentType;

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(GameObject))]
        private TriggerableTargetObject _target;

        [SerializeField()]
        [Tooltip("Add a new component even if one already exists.")]
        private bool _addMultipleIfExists;

        #endregion

        #region Properties

        public System.Type ComponentType
        {
            get { return _componentType.Type; }
            set
            {
                if (!TypeUtil.IsType(value, typeof(Component))) throw new System.ArgumentException("Type must inherit from UnityEngine.Component");
                _componentType.Type = value;
            }
        }

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        public bool AddMultipleIfExists
        {
            get { return _addMultipleIfExists; }
            set { _addMultipleIfExists = value; }
        }

        #endregion

        #region TriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = _target.GetTarget<GameObject>(arg);
            if (targ == null) return false;

            try
            {
                if (!_addMultipleIfExists && targ.HasComponent(_componentType.Type)) return false;

                var comp = targ.AddComponent(_componentType.Type);
                return true;
            }
            catch
            {
            }

            return false;
        }

        #endregion

    }

}
