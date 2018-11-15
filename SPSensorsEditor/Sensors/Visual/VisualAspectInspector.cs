using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Sensors;
using com.spacepuppy.Sensors.Visual;
using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Sensors.Visual
{

    [InitializeOnLoad]
    [CustomEditor(typeof(VisualAspect), true)]
    public class VisualAspectInspector : SPEditor
    {
        
        static VisualAspectInspector()
        {
            SceneView.onSceneGUIDelegate -= OnGlobalSceneGUI;
            SceneView.onSceneGUIDelegate += OnGlobalSceneGUI;
        }
        private static void OnGlobalSceneGUI(SceneView view)
        {
            var go = Selection.activeGameObject;
            if (go == null) return;

            using (var lst = TempCollection.GetList<VisualAspect>())
            {
                go.FindComponents<VisualAspect>(lst);
                if(lst.Count > 0)
                {
                    var e = lst.GetEnumerator();
                    while(e.MoveNext())
                    {
                        DrawIcon(e.Current);
                    }
                }
            }
        }

        private static void DrawIcon(VisualAspect targ)
        {
            SensorRenderUtil.AspectMaterial.SetColor("_colorSolid", Color.black);
            SensorRenderUtil.AspectMaterial.SetFloat("_colorAlpha", 0.4f);

            Matrix4x4 mat = Matrix4x4.TRS(targ.transform.position, Quaternion.identity, Vector3.one * 0.1f);

            for (int i = 0; i < SensorRenderUtil.AspectMaterial.passCount; ++i)
            {
                SensorRenderUtil.AspectMaterial.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.GetMesh(PrimitiveType.Sphere), mat);
            }

            SensorRenderUtil.AspectMaterial.SetColor("_colorSolid", targ.AspectColor);
            SensorRenderUtil.AspectMaterial.SetFloat("_colorAlpha", 0.8f);

            mat = Matrix4x4.TRS(targ.transform.position, Quaternion.identity, Vector3.one * 0.08f);

            for (int i = 0; i < SensorRenderUtil.AspectMaterial.passCount; ++i)
            {
                SensorRenderUtil.AspectMaterial.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.GetMesh(PrimitiveType.Sphere), mat);
            }
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.InSelectionHierarchy | GizmoType.Active)]
        private static void DrawSphereGizmo(VisualAspect aspect, GizmoType gizmoType)
        {
            if (aspect.Radius > 0f)
            {
                Gizmos.color = aspect.AspectColor;
                Gizmos.DrawWireSphere(aspect.transform.position, aspect.Radius);
            }
        }


        /*
         * Obsolete method
         * 
        #region OnSceneGUI
        
        protected virtual void OnSceneGUI()
        {
            var targ = this.target as VisualAspect;
            if (targ == null) return;
            if (!targ.enabled) return;

            VisualAspectInspector.Material.SetColor("_colorSolid", Color.black);
            VisualAspectInspector.Material.SetFloat("_colorAlpha", 0.4f);

            Matrix4x4 mat = Matrix4x4.TRS(targ.transform.position, Quaternion.identity, Vector3.one * 0.1f);

            for(int i = 0; i < VisualAspectInspector.Material.passCount; ++i)
            {
                VisualAspectInspector.Material.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.GetMesh(PrimitiveType.Sphere), mat);
            }

            VisualAspectInspector.Material.SetColor("_colorSolid", targ.AspectColor);
            VisualAspectInspector.Material.SetFloat("_colorAlpha", 0.8f);

            mat = Matrix4x4.TRS(targ.transform.position, Quaternion.identity, Vector3.one * 0.08f);

            for (int i = 0; i < VisualAspectInspector.Material.passCount; ++i)
            {
                VisualAspectInspector.Material.SetPass(i);
                Graphics.DrawMeshNow(PrimitiveUtil.GetMesh(PrimitiveType.Sphere), mat);
            }

        }

        #endregion
        */
    }

}
