using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Modifiers;

namespace com.spacepuppyeditor.Decorators
{

    /// <summary>
    /// Inserts a button before or after a field.
    /// 
    /// Though this is technically a decorator, it uses a PropertyModifier to accomplish its task due to its special needs.
    /// </summary>
    [CustomPropertyDrawer(typeof(InsertButtonAttribute))]
    public class InsertButtonDecorator : PropertyModifier
    {

        protected internal override void OnBeforeGUI(SerializedProperty property, ref bool cancelDraw)
        {
            var attrib = this.attribute as InsertButtonAttribute;
            if (attrib.PrecedeProperty && (!attrib.RuntimeOnly || Application.isPlaying))
            {
                this.DrawButton(property, attrib);
            }
        }

        protected internal override void OnPostGUI(SerializedProperty property)
        {
            var attrib = this.attribute as InsertButtonAttribute;
            if (!attrib.PrecedeProperty && (!attrib.RuntimeOnly || Application.isPlaying))
            {
                this.DrawButton(property, attrib);
            }
        }


        private void DrawButton(SerializedProperty property, InsertButtonAttribute attrib)
        {
            if(GUILayout.Button(attrib.Label))
            {
                var obj = EditorHelper.GetTargetObjectWithProperty(property);
                DynamicUtil.InvokeMethod(obj, attrib.OnClick);
            }
        }

    }

}