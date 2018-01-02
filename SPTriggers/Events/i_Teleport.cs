#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{
    public class i_Teleport : AutoTriggerable
    {

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(Transform))]
        private TriggerableTargetObject _target = new TriggerableTargetObject();

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(Transform))]
        private TriggerableTargetObject _location = new TriggerableTargetObject();

        [SerializeField]
        private bool _orientWithLocationRotation;

        [SerializeField()]
        private bool _teleportEntireEntity = true;

        #endregion

        #region Properties

        public TriggerableTargetObject Target
        {
            get { return _target; }
        }

        public TriggerableTargetObject Location
        {
            get { return _location; }
        }

        public bool TeleportEntireEntity
        {
            get { return _teleportEntireEntity; }
            set { _teleportEntireEntity = value; }
        }

        #endregion

        #region TriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var targ = this._target.GetTarget<Transform>(arg);
            if (targ == null) return false;
            if (_teleportEntireEntity) targ = GameObjectUtil.FindRoot(targ).transform;

            var loc = _location.GetTarget<Transform>(arg);
            if (targ == null || loc == null) return false;

            targ.position = loc.position;
            if (_orientWithLocationRotation)
                targ.rotation = loc.rotation;

            return true;
        }

        #endregion

    }
}
