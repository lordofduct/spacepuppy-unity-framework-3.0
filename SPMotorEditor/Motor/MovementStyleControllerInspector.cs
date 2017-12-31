using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Motor;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Motor
{

    [CustomEditor(typeof(MovementStyleController), true)]
    public class MovementStyleControllerInspector : SPEditor
    {

        public const string PROP_DEFAULTMOVEMENTSTYLE = "_defaultMovementStyle";

        #region Properties

        public new MovementStyleController target { get { return base.target as MovementStyleController; } }

        #endregion

        protected override void OnSPInspectorGUI()
        {
            this.serializedObject.Update();
            this.DrawDefaultInspectorExcept(PROP_DEFAULTMOVEMENTSTYLE);
            this.serializedObject.ApplyModifiedProperties();




            var ids = new List<string>();
            ids.Add("None");
            ids.AddRange(from c in this.target.GetComponents<IMovementStyle>() select c.GetType().Name);

            int i = (target.DefaultMovementStyle != null) ? ids.IndexOf(target.DefaultMovementStyle.GetType().Name) : -1;
            if (i < 0) i = 0;

            EditorGUI.BeginChangeCheck();
            i = EditorGUILayout.Popup("Default Movement Style", i, ids.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                if (i < 0) i = 0;

                if (i == 0)
                {
                    target.DefaultMovementStyle = null;
                }
                else
                {
                    var id = ids[i];
                    target.DefaultMovementStyle = (from c in this.target.GetComponents<IMovementStyle>() where c.GetType().Name == id select c).FirstOrDefault();
                }

                this.serializedObject.Update();
            }






            if (Application.isPlaying)
            {
                if (this.target.Current != null)
                {
                    EditorGUILayout.HelpBox("Currently active style is '" + this.target.Current.GetType().Name + "'.", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Currently active style is null.", MessageType.Info);
                }
            }

        }

        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

    }

}
