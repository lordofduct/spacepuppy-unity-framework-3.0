﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Anim;
using com.spacepuppy.Anim.Legacy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Core.PropertyDrawers;

namespace com.spacepuppyeditor.Anim.Legacy
{

    [CustomPropertyDrawer(typeof(SPAnimClip))]
    public class SPAnimClipPropertyDrawer : PropertyDrawer
    {
        private const string PROP_NAME = "_name";
        private const string PROP_CLIP = "_clip";
        private static string[] FULL_DETAIL_PROPS = new string[] {SPAnimClip.PROP_WEIGHT,
                                                            SPAnimClip.PROP_SPEED,
                                                            "ScaledDuration",
                                                            SPAnimClip.PROP_LAYER,
                                                            SPAnimClip.PROP_WRAPMODE,
                                                            SPAnimClip.PROP_BLENDMODE,
                                                            SPAnimClip.PROP_TIMESUPPLIER,
                                                            SPAnimClip.PROP_MASK};

        private bool _nameIsReadOnly;
        private string[] _detailProps;

        private SelectableComponentPropertyDrawer _selectComponentDrawer;

        #region CONSTRUCTOR

        private void Init(SerializedProperty property, bool searchForAttribs)
        {
            _detailProps = FULL_DETAIL_PROPS;
            if (searchForAttribs && this.fieldInfo != null)
            {
                var readonlyNameAttrib = this.fieldInfo.GetCustomAttributes(typeof(SPAnimClip.ReadOnlyNameAttribute), false).FirstOrDefault() as SPAnimClip.ReadOnlyNameAttribute;
                if (readonlyNameAttrib != null)
                {
                    property.FindPropertyRelative(PROP_NAME).stringValue = readonlyNameAttrib.Name;
                    _nameIsReadOnly = true;
                }
                else
                {
                    _nameIsReadOnly = false;
                }

                var configAttrib = this.fieldInfo.GetCustomAttributes(typeof(SPAnimClip.ConfigAttribute), false).FirstOrDefault() as SPAnimClip.ConfigAttribute;
                if (configAttrib != null)
                {
                    if (configAttrib.HideDetailRegion)
                        _detailProps = null;
                    else if (configAttrib.VisibleProps != null && configAttrib.VisibleProps.Length > 0)
                        _detailProps = configAttrib.VisibleProps;
                }
            }
        }

        public float GetDetailHeight(SerializedProperty property)
        {
            this.Init(property, true);

            return this.GetDetailHeight();
        }

        private float GetDetailHeight()
        {
            if (this.HideAllDetails) return 0f;

            float h = _detailProps.Length * EditorGUIUtility.singleLineHeight;
            return h;
        }

        #endregion

        #region Properties

        public bool HideAllDetails
        {
            get { return _detailProps == null || _detailProps.Length == 0; }
        }

        #endregion

        #region OnGUI

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            this.Init(property, true);

            if (!this.HideAllDetails && property.isExpanded)
            {
                return this.GetDetailHeight() + EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            this.Init(property, true);

            //var cache = SPGUI.DisableIfPlaying();

            if (this.HideAllDetails)
            {
                var clipPos = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                //this.DrawClip(EditorGUI.IndentedRect(clipPos), property, label, EditorGUIUtility.labelWidth, GUI.skin.label, _nameIsReadOnly);
                this.DrawClip(clipPos, property, label, EditorGUIUtility.labelWidth, GUI.skin.label, _nameIsReadOnly);
            }
            else
            {
                property.isExpanded = SPEditorGUI.PrefixFoldoutLabel(position, property.isExpanded, GUIContent.none);
                var clipPos = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                //this.DrawClip(EditorGUI.IndentedRect(clipPos), property, label, EditorGUIUtility.labelWidth, GUI.skin.label, _nameIsReadOnly);
                this.DrawClip(clipPos, property, label, EditorGUIUtility.labelWidth, GUI.skin.label, _nameIsReadOnly);

                if (property.isExpanded)
                {
                    var detailRect = new Rect(position.xMin + 5f, position.yMin + EditorGUIUtility.singleLineHeight, position.width - 5f, this.GetDetailHeight());
                    this.DrawDetails(detailRect, property);
                }
            }

            //cache.Reset();
        }

        public void DrawClip(Rect area, SerializedProperty property, GUIContent label, float labelWidth, GUIStyle labelStyle, bool nameIsReadonly)
        {
            //Draw Name
            var nameProp = property.FindPropertyRelative(PROP_NAME);

            var labelRect = new Rect(area.xMin, area.yMin, labelWidth, EditorGUIUtility.singleLineHeight);
            var textRect = new Rect(labelRect.xMax, area.yMin, (area.width - labelWidth) * 0.4f, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginProperty(area, label, nameProp);
            //EditorGUI.LabelField(labelRect, label, labelStyle);
            GUI.Label(labelRect, label, labelStyle);
            if (nameIsReadonly || _nameIsReadOnly || Application.isPlaying)
            {
                //EditorGUI.LabelField(textRect, nameProp.stringValue, GUI.skin.textField);
                GUI.Label(textRect, nameProp.stringValue, GUI.skin.textField);
            }
            else
            {
                //nameProp.stringValue = EditorGUI.TextField(textRect, nameProp.stringValue);
                nameProp.stringValue = GUI.TextField(textRect, nameProp.stringValue);
            }
            //EditorGUI.EndProperty();

            var cache = SPGUI.DisableIfPlaying();

            //Draw Animation Clip Reference
            var clipProp = property.FindPropertyRelative(PROP_CLIP);
            var xmin = textRect.xMax + 2f;
            var clipRect = new Rect(xmin, area.yMin, area.xMax - xmin, EditorGUIUtility.singleLineHeight);
            var clipLabel = GUIContent.none;
            //EditorGUI.BeginProperty(clipRect, clipLabel, clipProp);
            //clipProp.objectReferenceValue = EditorGUI.ObjectField(clipRect, clipProp.objectReferenceValue, typeof(AnimationClip), false);
            var obj = clipProp.objectReferenceValue;
            if (GameObjectUtil.IsGameObjectSource(obj))
            {
                if (_selectComponentDrawer == null)
                {
                    _selectComponentDrawer = new SelectableComponentPropertyDrawer();
                    _selectComponentDrawer.RestrictionType = typeof(IScriptableAnimationClip);
                    _selectComponentDrawer.ShowXButton = true;
                }
                _selectComponentDrawer.OnGUI(clipRect, clipProp, GUIContent.none);
            }
            else
            {
                obj = EditorGUI.ObjectField(clipRect, obj, typeof(UnityEngine.Object), true);
                if (obj == null || obj is AnimationClip || obj is IScriptableAnimationClip)
                    clipProp.objectReferenceValue = obj;
                else if (GameObjectUtil.IsGameObjectSource(obj))
                    clipProp.objectReferenceValue = ObjUtil.GetAsFromSource<IScriptableAnimationClip>(obj) as UnityEngine.Object;
            }
            EditorGUI.EndProperty();

            cache.Reset();



            if (Application.isPlaying && !property.hasMultipleDifferentValues && property.serializedObject.targetObject is SPLegacyAnimController)
            {
                if (GUI.Button(new Rect(area.xMin, area.yMin, 20f, EditorGUIUtility.singleLineHeight), ">"))
                {
                    var targ = property.serializedObject.targetObject as SPLegacyAnimController;
                    targ.Play(nameProp.stringValue);
                }
            }
        }

        public void DrawDetails(Rect position, SerializedProperty property)
        {
            var cache = SPGUI.DisableIfPlaying();

            //draw basic details
            var yMin = position.yMin;
            for (int i = 0; i < _detailProps.Length; i++)
            {
                //var r = new Rect(position.xMin, yMin + i * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                switch (_detailProps[i])
                {
                    case SPAnimClip.PROP_WRAPMODE:
                        {
                            var r = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                            position = new Rect(position.xMin, r.yMax, position.width, Mathf.Max(position.height - r.height, 0f));

                            var wrapModeProp = property.FindPropertyRelative(SPAnimClip.PROP_WRAPMODE);
                            wrapModeProp.SetEnumValue(SPEditorGUI.WrapModeField(r, wrapModeProp.displayName, wrapModeProp.GetEnumValue<WrapMode>(), true));
                        }
                        break;
                    case "ScaledDuration":
                        {
                            var r = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                            position = new Rect(position.xMin, r.yMax, position.width, Mathf.Max(position.height - r.height, 0f));

                            var label = EditorHelper.TempContent("Scaled Duration", "The duration of the clip with the speed applied. Modifying this alters the 'speed' property.");
                            var clip = property.FindPropertyRelative(PROP_CLIP).objectReferenceValue as AnimationClip;
                            if (clip == null)
                            {
                                EditorGUI.FloatField(r, label, 0f);
                                continue;
                            }

                            var speedProp = property.FindPropertyRelative(SPAnimClip.PROP_SPEED);
                            float dur = (speedProp.floatValue == 0f) ? float.PositiveInfinity : Mathf.Abs(clip.length / speedProp.floatValue);
                            EditorGUI.BeginChangeCheck();

                            dur = EditorGUI.FloatField(r, label, dur);

                            if (EditorGUI.EndChangeCheck())
                                speedProp.floatValue = (dur <= 0f) ? 0f : clip.length / dur;
                        }
                        break;
                    default:
                        {
                            var r = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
                            position = new Rect(position.xMin, r.yMax, position.width, Mathf.Max(position.height - r.height, 0f));

                            EditorGUI.PropertyField(r, property.FindPropertyRelative(_detailProps[i]));
                        }
                        break;
                }
            }

            cache.Reset();
        }

        #endregion

    }

}
