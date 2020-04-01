﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Core.PropertyDrawers
{

    [CustomPropertyDrawer(typeof(DisplayFlatAttribute))]
    public class DisplayFlatPropertyDrawer : PropertyDrawer
    {

        private static readonly float TOP_PAD = 2f + EditorGUIUtility.singleLineHeight;
        private const float BOTTOM_PAD = 2f;
        private const float MARGIN = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool canShrink = this.attribute is DisplayFlatAttribute ? (this.attribute as DisplayFlatAttribute).CanShrinkAndExpand : false;

            bool cache = property.isExpanded;
            if (!canShrink)
            {
                property.isExpanded = true;
            }

            try
            {
                if (property.isExpanded)
                {
                    if (property.hasChildren)
                    {
                        return SPEditorGUI.GetDefaultPropertyHeight(property, label, true) + BOTTOM_PAD + TOP_PAD - EditorGUIUtility.singleLineHeight;
                    }
                    else
                    {
                        return SPEditorGUI.GetDefaultPropertyHeight(property, label);
                    }
                }
                else
                {
                    return EditorGUIUtility.singleLineHeight + BOTTOM_PAD;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                property.isExpanded = cache;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool canShrink = this.attribute is DisplayFlatAttribute ? (this.attribute as DisplayFlatAttribute).CanShrinkAndExpand : false;

            bool cache = property.isExpanded;
            if (!canShrink)
            {
                property.isExpanded = true;
            }
            try
            {
                if (!property.hasChildren)
                {
                    SPEditorGUI.DefaultPropertyField(position, property, label);
                    return;
                }

                if (canShrink) cache = SPEditorGUI.PrefixFoldoutLabel(position, property.isExpanded, GUIContent.none);

                if (property.isExpanded)
                {
                    float h = SPEditorGUI.GetDefaultPropertyHeight(property, label, true) + BOTTOM_PAD + TOP_PAD - EditorGUIUtility.singleLineHeight;
                    var area = new Rect(position.x, position.yMax - h, position.width, h);
                    var drawArea = new Rect(area.x, area.y + TOP_PAD, area.width - MARGIN, area.height - TOP_PAD);

                    GUI.BeginGroup(area, label, GUI.skin.box);
                    GUI.EndGroup();

                    EditorGUI.indentLevel++;
                    SPEditorGUI.FlatChildPropertyField(drawArea, property);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    GUI.BeginGroup(position, label, GUI.skin.box);
                    GUI.EndGroup();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                property.isExpanded = cache;
            }
        }

    }
}
