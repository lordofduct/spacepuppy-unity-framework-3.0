using UnityEngine;
using UnityEditor;

using com.spacepuppy.Anim;
using com.spacepuppy.Anim.Events;
using com.spacepuppy.Anim.Legacy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Base.Events;

namespace com.spacepuppyeditor.Anim.Events
{

    [CustomPropertyDrawer(typeof(PlayAnimInfo))]
    public class PlayAnimInfoPropertyDrawer : PropertyDrawer
    {
        public const string PROP_WEIGHT = "Weight";
        public const string PROP_MODE = "_mode";
        public const string PROP_ID = "_id";
        public const string PROP_CLIP = "_clip";
        public const string PROP_SETTINGSMASK = "SettingsMask";
        public const string PROP_SETTINGS = "Settings";
        public const string PROP_QUEUEMODE = "QueueMode";
        public const string PROP_PLAYMODE = "PlayMode";
        public const string PROP_CROSSFADEDUR = "CrossFadeDur";
        private static string[] PROPS_ANIMSETTINGS = new string[] { "weight", "speed", "layer", "wrapMode", "blendMode", "timeSupplier" };

        public bool DrawFlat;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(property.serializedObject.targetObject is i_PlayRandomAnimation)
            {
                this.DrawFlat = true;

                var controller = property.serializedObject.FindProperty(i_PlayRandomAnimationInspector.PROP_TARGETANIMATOR).FindPropertyRelative(TriggerableTargetObjectPropertyDrawer.PROP_TARGET).objectReferenceValue;
                if (controller is Animation || controller is SPLegacyAnimController)
                {
                    float h = EditorGUIUtility.singleLineHeight * 6f;

                    var propSettings = property.FindPropertyRelative(PROP_SETTINGS);
                    h += EditorGUIUtility.singleLineHeight;
                    int mask = property.FindPropertyRelative(PROP_SETTINGSMASK).intValue;
                    for (int i = 0; i < PROPS_ANIMSETTINGS.Length; i++)
                    {
                        if (propSettings.isExpanded || (mask & (1 << i)) != 0) h += EditorGUIUtility.singleLineHeight;
                    }

                    return h;
                }
                else if (controller is ISPAnimator)
                {
                    return EditorGUIUtility.singleLineHeight * 2f;
                }
                else if (controller is ISPAnimationSource)
                {
                    float h = EditorGUIUtility.singleLineHeight * 5f;
                    if (!this.DrawFlat) h += EditorGUIUtility.singleLineHeight;

                    var propSettings = property.FindPropertyRelative(PROP_SETTINGS);
                    h += EditorGUIUtility.singleLineHeight;
                    int mask = property.FindPropertyRelative(PROP_SETTINGSMASK).intValue;
                    for (int i = 0; i < PROPS_ANIMSETTINGS.Length; i++)
                    {
                        if (propSettings.isExpanded || (mask & (1 << i)) != 0) h += EditorGUIUtility.singleLineHeight;
                    }

                    return h;
                }
                else
                {
                    return EditorGUIUtility.singleLineHeight;
                }
            }
            else
            {
                if (!this.DrawFlat && !property.isExpanded) return EditorGUIUtility.singleLineHeight;
                
                float h = EditorGUIUtility.singleLineHeight * 6f;
                if (!this.DrawFlat) h += EditorGUIUtility.singleLineHeight;

                var propSettings = property.FindPropertyRelative(PROP_SETTINGS);
                h += EditorGUIUtility.singleLineHeight;
                int mask = property.FindPropertyRelative(PROP_SETTINGSMASK).intValue;
                for (int i = 0; i < PROPS_ANIMSETTINGS.Length; i++)
                {
                    if (propSettings.isExpanded || (mask & (1 << i)) != 0) h += EditorGUIUtility.singleLineHeight;
                }

                return h;
            }


        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.targetObject is i_PlayRandomAnimation)
            {
                this.DrawForPlayRandomAnimation(position, property, label);
            }
            else
            {
                this.DrawGenerically(position, property, label);
            }
        }

        private void DrawForPlayRandomAnimation(Rect position, SerializedProperty property, GUIContent label)
        {
            var controller = property.serializedObject.FindProperty(i_PlayRandomAnimationInspector.PROP_TARGETANIMATOR).FindPropertyRelative(TriggerableTargetObjectPropertyDrawer.PROP_TARGET).objectReferenceValue;
            if (controller is Animation || controller is SPLegacyAnimController)
            {
                this.DrawFlat = true;
                this.DrawGenerically(position, property, label);
            }
            else if (controller is ISPAnimator)
            {
                var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                var r1 = new Rect(r0.xMin, r0.yMax, r0.width, EditorGUIUtility.singleLineHeight);

                SPEditorGUI.PropertyField(r0, property.FindPropertyRelative(PROP_WEIGHT));
                var propId = property.FindPropertyRelative(PROP_ID);
                propId.stringValue = i_PlayAnimationInspector.DrawSPAnimatorFunctionPopup(r1, controller as ISPAnimator, propId.stringValue);

            }
            else if (controller is ISPAnimationSource)
            {
                var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                var r1 = new Rect(r0.xMin, r0.yMax, r0.width, EditorGUIUtility.singleLineHeight);
                var r3 = new Rect(r1.xMin, r1.yMax, r1.width, position.yMax - r1.yMax - EditorGUIUtility.singleLineHeight * 3f);
                var r4 = new Rect(r3.xMin, r3.yMax, r3.width, EditorGUIUtility.singleLineHeight);
                var r5 = new Rect(r4.xMin, r4.yMax, r4.width, EditorGUIUtility.singleLineHeight);
                var r6 = new Rect(r5.xMin, r5.yMax, r5.width, EditorGUIUtility.singleLineHeight);

                SPEditorGUI.PropertyField(r0, property.FindPropertyRelative(PROP_WEIGHT));
                property.FindPropertyRelative(PROP_CLIP).objectReferenceValue = null;
                SPEditorGUI.PropertyField(r1, property.FindPropertyRelative(PROP_ID));

                this.DrawSettings(r3, property);

                SPEditorGUI.PropertyField(r4, property.FindPropertyRelative(PROP_QUEUEMODE));
                SPEditorGUI.PropertyField(r5, property.FindPropertyRelative(PROP_PLAYMODE));
                SPEditorGUI.PropertyField(r6, property.FindPropertyRelative(PROP_CROSSFADEDUR));
            }
            else
            {
                EditorGUI.LabelField(position, "Attach a target animator before configuring animations.");
            }
        }

        private void DrawGenerically(Rect position, SerializedProperty property, GUIContent label)
        {
            if(!this.DrawFlat)
            {
                var rh = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                position = new Rect(position.xMin, rh.yMax, position.width, position.height - rh.height);

                property.isExpanded = EditorGUI.Foldout(rh, property.isExpanded, label);

                if (!property.isExpanded) return;
            }

            var r0 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(r0.xMin, r0.yMax, r0.width, EditorGUIUtility.singleLineHeight);
            var r2 = new Rect(r1.xMin, r1.yMax, r1.width, EditorGUIUtility.singleLineHeight);
            var r3 = new Rect(r2.xMin, r2.yMax, r2.width, position.yMax - r2.yMax - EditorGUIUtility.singleLineHeight * 3f);
            var r4 = new Rect(r3.xMin, r3.yMax, r3.width, EditorGUIUtility.singleLineHeight);
            var r5 = new Rect(r4.xMin, r4.yMax, r4.width, EditorGUIUtility.singleLineHeight);
            var r6 = new Rect(r5.xMin, r5.yMax, r5.width, EditorGUIUtility.singleLineHeight);

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
                    }
                    break;
                case i_PlayAnimation.PlayByMode.PlayAnimByID:
                    {
                        property.FindPropertyRelative(PROP_CLIP).objectReferenceValue = null;

                        SPEditorGUI.PropertyField(r2, property.FindPropertyRelative(PROP_ID));
                    }
                    break;
                case i_PlayAnimation.PlayByMode.PlayAnimFromResource:
                    {
                        property.FindPropertyRelative(PROP_CLIP).objectReferenceValue = null;

                        SPEditorGUI.PropertyField(r2, property.FindPropertyRelative(PROP_ID));
                    }
                    break;
            }

            this.DrawSettings(r3, property);
            SPEditorGUI.PropertyField(r4, property.FindPropertyRelative(PROP_QUEUEMODE));
            SPEditorGUI.PropertyField(r5, property.FindPropertyRelative(PROP_PLAYMODE));
            SPEditorGUI.PropertyField(r6, property.FindPropertyRelative(PROP_CROSSFADEDUR));
        }

        private void DrawSettings(Rect position, SerializedProperty property)
        {
            var propMask = property.FindPropertyRelative(PROP_SETTINGSMASK);
            var propSettings = property.FindPropertyRelative(PROP_SETTINGS);

            int mask = propMask.intValue;

            var r1 = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
            propSettings.isExpanded = EditorGUI.Foldout(r1, propSettings.isExpanded, mask != 0 ? "Custom Settings : Active" : "Custom Settings");

            EditorGUI.BeginChangeCheck();
            int row = 0;
            for (int i = 0; i < PROPS_ANIMSETTINGS.Length; i++)
            {
                int m = 1 << i;
                bool active = (mask & m) != 0;
                if (!propSettings.isExpanded && !active) continue;

                var propSet = propSettings.FindPropertyRelative(PROPS_ANIMSETTINGS[i]);
                var r2 = new Rect(position.xMin, r1.yMax + row * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                var r2a = new Rect(r2.xMin, r2.yMin, 30f, r2.height);
                var r2b = new Rect(r2a.xMax, r2.yMin, r2.width - r2a.width, r2.height);
                if(EditorGUI.Toggle(r2a, active))
                {
                    mask |= m;
                    EditorGUI.PropertyField(r2b, propSet);
                }
                else
                {
                    mask &= ~m;
                    EditorGUI.PrefixLabel(r2b, EditorHelper.TempContent(propSet.displayName, propSet.tooltip));
                }
                row++;
            }
            if (EditorGUI.EndChangeCheck())
                propMask.intValue = mask;
        }
        
    }

}
