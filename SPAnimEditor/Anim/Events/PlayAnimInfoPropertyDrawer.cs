using UnityEngine;
using UnityEditor;

using com.spacepuppy.Anim;
using com.spacepuppy.Anim.Events;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Anim.Events
{

    [CustomPropertyDrawer(typeof(i_PlayRandomAnimation.PlayAnimInfo))]
    public class PlayAnimInfoPropertyDrawer : PropertyDrawer
    {
        public const string PROP_WEIGHT = "Weight";
        public const string PROP_MODE = "_mode";
        public const string PROP_ID = "_id";
        public const string PROP_CLIP = "_clip";
        public const string PROP_SETTINGS = "Settings";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float h = EditorGUIUtility.singleLineHeight * 3f;

            var settingsProp = property.FindPropertyRelative(PROP_SETTINGS);
            if(settingsProp.isExpanded)
            {
                h += SPEditorGUI.GetPropertyHeight(settingsProp, EditorHelper.TempContent(settingsProp.displayName));
            }
            else
            {
                h += EditorGUIUtility.singleLineHeight;
            }

            return h;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(r0.xMin, r0.yMax, r0.width, EditorGUIUtility.singleLineHeight);
            var r2 = new Rect(r1.xMin, r1.yMax, r1.width, EditorGUIUtility.singleLineHeight);
            var r3 = new Rect(r2.xMin, r2.yMax, r2.width, position.yMax - r2.yMax);

            SPEditorGUI.PropertyField(r0, property.FindPropertyRelative(PROP_WEIGHT));

            var propMode = property.FindPropertyRelative(PROP_MODE);
            SPEditorGUI.PropertyField(r1, propMode);

            switch (propMode.GetEnumValue<i_PlayAnimation.PlayByMode>())
            {
                case i_PlayAnimation.PlayByMode.PlayAnim:
                    {
                        property.FindPropertyRelative(PROP_ID).stringValue = string.Empty;

                        var clipProp = property.FindPropertyRelative(PROP_CLIP);
                        var obj = EditorGUI.ObjectField(r2, EditorHelper.TempContent(clipProp.displayName), clipProp.objectReferenceValue, typeof(UnityEngine.Object), true);
                        if (obj == null || obj is AnimationClip || obj is IScriptableAnimationClip)
                            clipProp.objectReferenceValue = obj;
                        else if (GameObjectUtil.IsGameObjectSource(obj))
                            clipProp.objectReferenceValue = ObjUtil.GetAsFromSource<IScriptableAnimationClip>(obj) as UnityEngine.Object;
                        
                        SPEditorGUI.PropertyField(r3, property.FindPropertyRelative(PROP_SETTINGS));
                    }
                    break;
                case i_PlayAnimation.PlayByMode.PlayAnimByID:
                    {
                        property.FindPropertyRelative(PROP_CLIP).objectReferenceValue = null;

                        SPEditorGUI.PropertyField(r2, property.FindPropertyRelative(PROP_ID));

                        SPEditorGUI.PropertyField(r3, property.FindPropertyRelative(PROP_SETTINGS));
                    }
                    break;
                case i_PlayAnimation.PlayByMode.PlayAnimFromResource:
                    {
                        property.FindPropertyRelative(PROP_CLIP).objectReferenceValue = null;

                        SPEditorGUI.PropertyField(r2, property.FindPropertyRelative(PROP_ID));

                        SPEditorGUI.PropertyField(r3, property.FindPropertyRelative(PROP_SETTINGS));
                    }
                    break;
            }
        }
        
    }

}
