using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Events;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Components;

namespace com.spacepuppyeditor.Base.Events
{

    [CustomPropertyDrawer(typeof(ObservableTargetData))]
    public class ObservableTargetDataPropertyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2f + 2f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.Box(new Rect(position.xMin, position.yMin + 1f, position.width, position.height - 2f), GUIContent.none);

            var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(position.xMin, r0.yMax, position.width, EditorGUIUtility.singleLineHeight);

            var targProp = property.FindPropertyRelative("_target");
            var indexProp = property.FindPropertyRelative("_triggerIndex");

            EditorGUI.BeginChangeCheck();
            SPEditorGUI.PropertyField(r0, targProp);
            if (EditorGUI.EndChangeCheck())
                indexProp.intValue = 0;

            if (targProp.objectReferenceValue is IObservableTrigger)
            {
                var targ = targProp.objectReferenceValue as IObservableTrigger;
                var owner = new SerializedObject(targProp.objectReferenceValue);

                int i = 0;
                var events = (from e in targ.GetEvents() select GetTriggerTargsId(owner, e, ++i)).ToArray();
                indexProp.intValue = EditorGUI.Popup(r1, "Trigger Event", indexProp.intValue, events);
            }
            else
            {
                EditorGUI.LabelField(r1, "Trigger Event", "Select Target First");
            }
        }


        private static string GetTriggerTargsId(SerializedObject owner, BaseSPEvent e, int index)
        {
            var child = owner.GetIterator();
            child.Next(true);
            do
            {
                var v = EditorHelper.GetTargetObjectOfProperty(child);
                if (v is BaseSPEvent && v == e) return child.displayName;
            }
            while (child.Next(false));

            if (string.IsNullOrEmpty(e.ObservableTriggerId))
                return "Trigger (" + index.ToString() + ")";
            else
                return e.ObservableTriggerId + "(" + index.ToString() + ")";
        }

    }

}
