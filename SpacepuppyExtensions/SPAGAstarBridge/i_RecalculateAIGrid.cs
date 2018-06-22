using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Events;
using com.spacepuppy.Utils;

using Pathfinding;

namespace com.spacepuppy.Scenario
{

    public class i_RecalculateAIGrid : AutoTriggerable
    {

        public enum RecalculateMode
        {
            All = 0,
            Region = 1,
            BoundsOfCollider = 2
        }

        #region Fields

        [SerializeField]
        private RecalculateMode _mode;
        [SerializeField]
        private UnityEngine.Object _objectRef;

        [SerializeField]
        private float _delay;

        #endregion

        #region Properties

        public RecalculateMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public Collider Collider
        {
            get { return _objectRef as Collider; }
            set { _objectRef = value; }
        }

        #endregion

        #region Methods

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            switch(_mode)
            {
                case RecalculateMode.All:
                    {
                        if (_delay > 0f)
                            this.InvokeGuaranteed(() => AstarPath.active.ScanAsync(), _delay);
                        else
                            AstarPath.active.ScanAsync();
                        return true;
                    }
                case RecalculateMode.Region:
                    {
                        var bounds = new Bounds(this.transform.position, this.transform.localScale);
                        var guo = new GraphUpdateObject(bounds)
                        {
                            updatePhysics = true
                        };
                        if (_delay > 0f)
                            this.InvokeGuaranteed(() => AstarPath.active.UpdateGraphs(guo), _delay);
                        else
                            AstarPath.active.UpdateGraphs(guo);
                    }
                    break;
                case RecalculateMode.BoundsOfCollider:
                    {
                        if (_objectRef == null || !(_objectRef is Collider)) return false;
                        var bounds = (_objectRef as Collider).bounds;
                        var guo = new GraphUpdateObject(bounds)
                        {
                            updatePhysics = true
                        };
                        if (_delay > 0f)
                            this.InvokeGuaranteed(() => AstarPath.active.UpdateGraphs(guo), _delay);
                        else
                            AstarPath.active.UpdateGraphs(guo);
                    }
                    break;
            }

            return false;
        }

        #endregion

    }

}