using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using com.spacepuppy;
using com.spacepuppy.Scenario;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Scenario
{

    [CustomEditor(typeof(i_RecalculateAIGrid))]
    public class i_RecalculateAIGridInspector : SPEditor
    {

        public const string PROP_MODE = "_mode";
        private const string PROP_OBJREF = "_objectRef";

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();

            this.DrawPropertyField(EditorHelper.PROP_SCRIPT);
            this.DrawPropertyField(EditorHelper.PROP_ORDER);

            var modeProp = this.serializedObject.FindProperty(PROP_MODE);
            SPEditorGUILayout.PropertyField(modeProp);
            switch(modeProp.GetEnumValue<i_RecalculateAIGrid.RecalculateMode>())
            {
                case i_RecalculateAIGrid.RecalculateMode.All:
                    //draw nothing else
                    break;
                case i_RecalculateAIGrid.RecalculateMode.Region:
                    //draw nothing else
                    break;
                case i_RecalculateAIGrid.RecalculateMode.BoundsOfCollider:
                    var objProp = this.serializedObject.FindProperty(PROP_OBJREF);
                    objProp.objectReferenceValue = EditorGUILayout.ObjectField("Collider", objProp.objectReferenceValue, typeof(Collider), true);
                    break;
            }

            this.DrawDefaultInspectorExcept(EditorHelper.PROP_SCRIPT, EditorHelper.PROP_ORDER, PROP_MODE, PROP_OBJREF);

            this.serializedObject.ApplyModifiedProperties();
        }
        
        [DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.InSelectionHierarchy)]
        private static void DrawGizmoBounds(i_RecalculateAIGrid targ, GizmoType gizmoType)
        {
            switch(targ.Mode)
            {
                case i_RecalculateAIGrid.RecalculateMode.Region:
                    {
                        var pos = targ.transform.position;
                        var sc = targ.transform.localScale;

                        Gizmos.color = Color.red;
                        Gizmos.DrawWireCube(pos, sc);
                    }
                    break;
                case i_RecalculateAIGrid.RecalculateMode.BoundsOfCollider:
                    {
                        var c = targ.Collider;
                        if (c == null) return;

                        var bounds = c.bounds;
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireCube(bounds.center, bounds.size);
                    }
                    break;
            }
        }

    }

}