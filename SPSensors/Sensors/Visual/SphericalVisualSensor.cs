using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Geom;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Sensors.Visual
{

    public class SphericalVisualSensor : VisualSensor
    {

        public enum Modes
        {
            Conical = 0,
            Frustum = 1
        }

        #region Fields

        [MinRange(0f)]
        [SerializeField()]
        private float _radius = 5.0f;
        [Tooltip("An optional radius to carve out the center.")]
        [MinRange(0f)]
        [SerializeField()]
        private float _innerRadius = 0f;

        [SerializeField]
        private Modes _mode;
        [Range(0f, 360f)]
        [SerializeField()]
        private float _horizontalAngle = 360f;
        [Range(0f, 180f)]
        [SerializeField()]
        private float _verticalAngle = 180f;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public float InnerRadius
        {
            get { return _innerRadius; }
            set { _innerRadius = Mathf.Clamp(value, 0f, _radius); }
        }

        public Modes Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public float HorizontalAngle
        {
            get { return _horizontalAngle; }
            set { _horizontalAngle = Mathf.Clamp(value, 0, 360f); }
        }

        public float VerticalAngle
        {
            get { return _verticalAngle; }
            set { _verticalAngle = Mathf.Clamp(value, 0f, 180f); }
        }

        #endregion

        #region Methods

        public override BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(this.transform.position, _radius);
        }

        protected override bool TestVisibility(VisualAspect aspect)
        {
            float aspRad = aspect.Radius;
            float sqrRadius = _radius * _radius;
            Vector3 v = aspect.transform.position - this.transform.position;
            float sqrDist = v.sqrMagnitude;

            if (sqrDist - (aspRad * aspRad) > sqrRadius) return false;
            if (this._innerRadius > aspRad && sqrDist < this._innerRadius * this._innerRadius) return false;
            
            switch (_mode)
            {
                case Modes.Conical:
                    if (this._horizontalAngle < 360.0f)
                    {
                        var directionOfAspectInLocalSpace = this.transform.InverseTransformDirection(v);
                        if (aspRad > MathUtil.EPSILON)
                        {
                            float k = 2f * Mathf.Asin(aspRad / (Mathf.Sqrt(sqrDist + (aspRad * aspRad) / 4f))) * Mathf.Rad2Deg;
                            float a = VectorUtil.AngleBetween(Vector3.forward, directionOfAspectInLocalSpace);
                            if (a > (this._horizontalAngle / 2f) - k)
                                return false;
                        }
                        else
                        {
                            float a = VectorUtil.AngleBetween(Vector3.forward, directionOfAspectInLocalSpace);
                            if (a > this._horizontalAngle / 2f)
                                return false;
                        }
                    }
                    break;
                case Modes.Frustum:
                    if (this._horizontalAngle < 360.0f && this._verticalAngle < 360.0f)
                    {
                        var directionOfAspectInLocalSpace = this.transform.InverseTransformDirection(v);
                        if (aspRad > MathUtil.EPSILON)
                        {
                            float k = 2f * Mathf.Asin(aspRad / (Mathf.Sqrt(sqrDist + (aspRad * aspRad) / 4f))) * Mathf.Rad2Deg;
                            float a = VectorUtil.AngleBetween(Vector2.right, new Vector2(directionOfAspectInLocalSpace.z, directionOfAspectInLocalSpace.x));

                            if (a > (this._horizontalAngle / 2f) - k)
                                return false;
                            a = VectorUtil.AngleBetween(Vector2.right, new Vector2(directionOfAspectInLocalSpace.z, directionOfAspectInLocalSpace.y));
                            if (a > (this._verticalAngle / 2f) - k)
                                return false;
                        }
                        else
                        {
                            float a = VectorUtil.AngleBetween(Vector2.right, new Vector2(directionOfAspectInLocalSpace.z, directionOfAspectInLocalSpace.x));
                            if (a > this._horizontalAngle / 2f)
                                return false;
                            a = VectorUtil.AngleBetween(Vector2.right, new Vector2(directionOfAspectInLocalSpace.z, directionOfAspectInLocalSpace.y));
                            if (a > this._verticalAngle / 2f)
                                return false;
                        }
                    }
                    break;
            }


            if (this.LineOfSightMask.value != 0)
            {
                using (var lst = com.spacepuppy.Collections.TempCollection.GetList<RaycastHit>())
                {
                    int cnt = PhysicsUtil.RaycastAll(this.transform.position, v, lst, v.magnitude, this.LineOfSightMask);
                    for (int i = 0; i < cnt; i++)
                    {
                        //we ignore ourself
                        var r = lst[i].collider.FindRoot();
                        if (r != aspect.entityRoot && r != this.entityRoot) return false;
                    }
                }
            }

            return true;
        }

        #endregion

    }

}
