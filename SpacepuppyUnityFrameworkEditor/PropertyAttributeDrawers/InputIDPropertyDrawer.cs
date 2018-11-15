using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Settings;

namespace com.spacepuppyeditor.PropertyAttributeDrawers
{

    [CustomPropertyDrawer(typeof(InputIDAttribute))]
    public class InputIDPropertyDrawer : PropertyDrawer
    {

        private string[] _inputIds;

        private void Init()
        {
            _inputIds = InputSettings.GetGlobalInputIds();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                if (_inputIds == null) this.Init();

                property.stringValue = SPEditorGUI.OptionPopupWithCustom(position, label, property.stringValue, _inputIds);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

    }

}
