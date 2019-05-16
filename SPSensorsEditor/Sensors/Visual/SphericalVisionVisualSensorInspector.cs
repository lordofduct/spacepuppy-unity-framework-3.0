using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Sensors;
using com.spacepuppy.Sensors.Visual;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Sensors.Visual
{

    [CustomEditor(typeof(SphericalVisualSensor))]
    public class SphericalVisualSensorInspector : VisualSensorInspector
    {
        
        #region OnInspector

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawDefaultInspectorExcept("_mode", "_horizontalAngle", "_verticalAngle");

            var modeProp = this.serializedObject.FindProperty("_mode");
            EditorGUILayout.PropertyField(modeProp);
            switch(modeProp.GetEnumValue<SphericalVisualSensor.Modes>())
            {
                case SphericalVisualSensor.Modes.Conical:
                    this.DrawPropertyField("_horizontalAngle");
                    break;
                case SphericalVisualSensor.Modes.Frustum:
                    this.DrawPropertyField("_horizontalAngle");
                    this.DrawPropertyField("_verticalAngle");
                    break;
            }

            //fix radii
            var radiusProp = this.serializedObject.FindProperty("_radius");
            var innerRadProp = this.serializedObject.FindProperty("_innerRadius");
            if (innerRadProp.floatValue < 0f) innerRadProp.floatValue = 0f;
            else if (innerRadProp.floatValue > radiusProp.floatValue) innerRadProp.floatValue = radiusProp.floatValue;

            this.serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region OnSceneGUI
        
        void OnSceneGUI()
        {
            var targ = this.target as SphericalVisualSensor;
            if (targ == null) return;
            if (!targ.enabled) return;
            
            Handles.color = targ.SensorColor;
            Handles.matrix = Matrix4x4.TRS(targ.transform.position, targ.transform.rotation, Vector3.one);
            
            switch(targ.Mode)
            {
                case SphericalVisualSensor.Modes.Conical:
                    {
                        HandlesHelper.DrawWireSphere(Vector3.zero, Quaternion.identity, targ.Radius, targ.HorizontalAngle);
                        if(targ.InnerRadius > 0f) HandlesHelper.DrawWireSphere(Vector3.zero, Quaternion.identity, targ.InnerRadius, targ.HorizontalAngle);
                        
                        if(targ.HorizontalAngle < 360f)
                        {
                            float dx = Mathf.Cos(targ.HorizontalAngle * Mathf.Deg2Rad / 2f);
                            float dy = Mathf.Sqrt(1f - dx * dx);
                            var v = new Vector3(0f, dy, dx);
                            Handles.DrawLine(v * targ.InnerRadius, v * targ.Radius);
                            v = new Vector3(0f, -dy, dx);
                            Handles.DrawLine(v * targ.InnerRadius, v * targ.Radius);
                            v = new Vector3(dy, 0f, dx);
                            Handles.DrawLine(v * targ.InnerRadius, v * targ.Radius);
                            v = new Vector3(-dy, 0f, dx);
                            Handles.DrawLine(v * targ.InnerRadius, v * targ.Radius);
                        }
                    }
                    break;
                case SphericalVisualSensor.Modes.Frustum:
                    if(targ.HorizontalAngle >= 360f && targ.VerticalAngle >= 180f)
                    {
                        HandlesHelper.DrawWireSphere(Vector3.zero, Quaternion.identity, targ.Radius);
                        if(targ.InnerRadius > 0f) HandlesHelper.DrawWireSphere(Vector3.zero, Quaternion.identity, targ.InnerRadius);
                    }
                    else
                    {
                        HandlesHelper.DrawWireSphereFrustum(targ.InnerRadius, targ.Radius, targ.HorizontalAngle, targ.VerticalAngle);
                    }
                    break;
            }
        }

        #endregion

    }
}
