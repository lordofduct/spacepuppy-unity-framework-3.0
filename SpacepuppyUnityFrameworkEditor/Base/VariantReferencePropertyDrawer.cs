using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

using com.spacepuppyeditor.Core.PropertyDrawers;

namespace com.spacepuppyeditor.Core
{

    [CustomPropertyDrawer(typeof(VariantReference))]
    public class VariantReferencePropertyDrawer : PropertyDrawer
    {

        public const string PROP_MODE = "_mode";
        public const string PROP_TYPE = "_type";
        public const string PROP_X = "_x";
        public const string PROP_Y = "_y";
        public const string PROP_Z = "_z";
        public const string PROP_W = "_w";
        public const string PROP_STRING = "_string";
        public const string PROP_OBJREF = "_unityObjectReference";

        #region Fields

        const float REF_SELECT_WIDTH = 70f;

        public bool RestrictVariantType = false;


        private VariantType _variantTypeRestrictedTo;
        private System.Type _typeRestrictedTo = typeof(UnityEngine.Object);
        private System.Type _forcedObjectType = typeof(UnityEngine.Object);

        private VariantReference.EditorHelper _helper = new VariantReference.EditorHelper(new VariantReference());
        private SelectableComponentPropertyDrawer _selectComponentDrawer = new SelectableComponentPropertyDrawer();

        #endregion

        #region Properties

        public VariantType VariantTypeRestrictedTo
        {
            get { return _variantTypeRestrictedTo; }
        }

        public System.Type TypeRestrictedTo
        {
            get { return _typeRestrictedTo; }
            set
            {
                _typeRestrictedTo = value ?? typeof(UnityEngine.Object);
                _variantTypeRestrictedTo = VariantReference.GetVariantType(_typeRestrictedTo);
            }
        }

        /// <summary>
        /// Unity Object type restriction, if the restricted type is a UnityEngine.Object.
        /// </summary>
        public System.Type ForcedObjectType
        {
            get { return _forcedObjectType; }
            set
            {
                if (ComponentUtil.IsAcceptableComponentType(value))
                    _forcedObjectType = value;
                else if (TypeUtil.IsType(value, typeof(UnityEngine.Object)))
                    _forcedObjectType = value;
                else
                    _forcedObjectType = typeof(UnityEngine.Object);
            }
        }

        #endregion

        #region Methods

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelWidthCache = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = Mathf.Max(0f, labelWidthCache - REF_SELECT_WIDTH);
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);
            this.DrawValueField(position, property);

            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = labelWidthCache;
        }

        public void DrawValueField(Rect position, SerializedProperty property)
        {
            CopyValuesToHelper(property, _helper);

            //draw ref selection
            position = this.DrawRefModeSelectionDropDown(position, property, _helper);

            //draw value
            switch (_helper._mode)
            {
                case VariantReference.RefMode.Value:
                    {
                        EditorGUI.BeginChangeCheck();
                        this.DrawValueFieldInValueMode(position, property, _helper);
                        if (EditorGUI.EndChangeCheck())
                        {
                            CopyValuesFromHelper(property, _helper);
                        }
                    }
                    break;
                case VariantReference.RefMode.Property:
                    this.DrawValueFieldInPropertyMode(position, property, _helper);
                    break;
                case VariantReference.RefMode.Eval:
                    this.DrawValueFieldInEvalMode(position, property, _helper);
                    break;
            }
        }

        private Rect DrawRefModeSelectionDropDown(Rect position, SerializedProperty property, VariantReference.EditorHelper helper)
        {
            var r0 = new Rect(position.xMin, position.yMin, Mathf.Min(REF_SELECT_WIDTH, position.width), position.height);

            EditorGUI.BeginChangeCheck();
            var mode = (VariantReference.RefMode)EditorGUI.EnumPopup(r0, GUIContent.none, _helper._mode);
            if (EditorGUI.EndChangeCheck())
            {
                _helper.PrepareForRefModeChange(mode);
                CopyValuesFromHelper(property, helper);
            }

            return new Rect(r0.xMax, r0.yMin, position.width - r0.width, r0.height);
        }

        private void DrawValueFieldInValueMode(Rect position, SerializedProperty property, VariantReference.EditorHelper helper)
        {
            if (helper.Target == null) return;
            var variant = helper.Target;

            if (this.RestrictVariantType && helper._type != this.VariantTypeRestrictedTo)
            {
                helper.PrepareForValueTypeChange(this.VariantTypeRestrictedTo);
                GUI.changed = true; //force change
            }

            var r0 = new Rect(position.xMin, position.yMin, 90.0f, EditorGUIUtility.singleLineHeight);
            var r1 = new Rect(r0.xMax, position.yMin, position.xMax - r0.xMax, EditorGUIUtility.singleLineHeight);

            var cache = SPGUI.DisableIf(this.RestrictVariantType);
            EditorGUI.BeginChangeCheck();
            var valueType = variant.ValueType;
            valueType = (VariantType)EditorGUI.EnumPopup(r0, GUIContent.none, valueType);
            if (EditorGUI.EndChangeCheck())
            {
                helper.PrepareForValueTypeChange(valueType);
            }
            cache.Reset();

            if (_typeRestrictedTo.IsEnum)
            {
                variant.IntValue = ConvertUtil.ToInt(EditorGUI.EnumPopup(r1, ConvertUtil.ToEnumOfType(_typeRestrictedTo, variant.IntValue)));
            }
            else
            {
                switch (valueType)
                {
                    case VariantType.Null:
                        cache = SPGUI.Disable();
                        EditorGUI.TextField(r1, "Null");
                        cache.Reset();
                        break;
                    case VariantType.String:
                        variant.StringValue = EditorGUI.TextField(r1, variant.StringValue);
                        break;
                    case VariantType.Boolean:
                        variant.BoolValue = EditorGUI.Toggle(r1, variant.BoolValue);
                        break;
                    case VariantType.Integer:
                        variant.IntValue = EditorGUI.IntField(r1, variant.IntValue);
                        break;
                    case VariantType.Float:
                        variant.FloatValue = EditorGUI.FloatField(r1, variant.FloatValue);
                        break;
                    case VariantType.Double:
                        //variant.DoubleValue = ConvertUtil.ToDouble(EditorGUI.TextField(r1, variant.DoubleValue.ToString()));
                        variant.DoubleValue = EditorGUI.DoubleField(r1, variant.DoubleValue);
                        break;
                    case VariantType.Vector2:
                        variant.Vector2Value = EditorGUI.Vector2Field(r1, GUIContent.none, variant.Vector2Value);
                        break;
                    case VariantType.Vector3:
                        variant.Vector3Value = EditorGUI.Vector3Field(r1, GUIContent.none, variant.Vector3Value);
                        break;
                    case VariantType.Vector4:
                        variant.Vector4Value = EditorGUI.Vector4Field(r1, (string)null, variant.Vector4Value);
                        break;
                    case VariantType.Quaternion:
                        variant.QuaternionValue = SPEditorGUI.QuaternionField(r1, GUIContent.none, variant.QuaternionValue);
                        break;
                    case VariantType.Color:
                        variant.ColorValue = EditorGUI.ColorField(r1, variant.ColorValue);
                        break;
                    case VariantType.DateTime:
                        variant.DateValue = ConvertUtil.ToDate(EditorGUI.TextField(r1, variant.DateValue.ToString()));
                        break;
                    case VariantType.GameObject:
                        variant.GameObjectValue = EditorGUI.ObjectField(r1, variant.GameObjectValue, typeof(GameObject), true) as GameObject;
                        break;
                    case VariantType.Component:
                        {
                            _selectComponentDrawer.AllowNonComponents = false;
                            _selectComponentDrawer.RestrictionType = ComponentUtil.IsAcceptableComponentType(_forcedObjectType) ? _forcedObjectType : typeof(Component);
                            _selectComponentDrawer.ShowXButton = true;
                            var targProp = property.FindPropertyRelative(PROP_OBJREF);
                            EditorGUI.BeginChangeCheck();
                            _selectComponentDrawer.OnGUI(r1, targProp);
                            if (EditorGUI.EndChangeCheck())
                            {
                                variant.ComponentValue = targProp.objectReferenceValue as Component;
                            }
                        }
                        break;
                    case VariantType.Object:
                        {
                            var obj = variant.ObjectValue;
                            if (ComponentUtil.IsAcceptableComponentType(_forcedObjectType))
                            {
                                if (obj is GameObject || obj is Component)
                                {
                                    _selectComponentDrawer.AllowNonComponents = false;
                                    _selectComponentDrawer.RestrictionType = _forcedObjectType;
                                    _selectComponentDrawer.ShowXButton = true;
                                    var targProp = property.FindPropertyRelative(PROP_OBJREF);
                                    EditorGUI.BeginChangeCheck();
                                    _selectComponentDrawer.OnGUI(r1, targProp);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        variant.ObjectValue = targProp.objectReferenceValue as Component;
                                    }
                                }
                                else
                                {
                                    EditorGUI.BeginChangeCheck();
                                    obj = EditorGUI.ObjectField(r1, obj, typeof(UnityEngine.Object), true);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        if (obj == null)
                                        {
                                            variant.ObjectValue = null;
                                        }
                                        else if (_forcedObjectType.IsInstanceOfType(obj))
                                        {
                                            variant.ObjectValue = obj;
                                        }
                                        else
                                        {
                                            var go = GameObjectUtil.GetGameObjectFromSource(obj);
                                            if (go != null)
                                                variant.ObjectValue = go.GetComponent(_forcedObjectType);
                                            else
                                                variant.ObjectValue = null;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                variant.ObjectValue = EditorGUI.ObjectField(r1, obj, _forcedObjectType, true);
                            }
                        }
                        break;
                    case VariantType.LayerMask:
                        {
                            variant.LayerMaskValue = SPEditorGUI.LayerMaskField(r1, GUIContent.none, (int)variant.LayerMaskValue);
                        }
                        break;
                    case VariantType.Rect:
                        {
                            variant.RectValue = EditorGUI.RectField(r1, variant.RectValue);
                        }
                        break;
                    case VariantType.Numeric:
                        {
                            //we just treat numeric types as double and let the numeric deal with it
                            var tp = this.TypeRestrictedTo;
                            if (tp != null && typeof(INumeric).IsAssignableFrom(tp))
                            {
                                var n = variant.NumericValue;
                                double d = n != null ? n.ToDouble(null) : 0d;
                                EditorGUI.BeginChangeCheck();
                                d = EditorGUI.DoubleField(r1, d);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    variant.NumericValue = Numerics.CreateNumeric(tp, d);
                                }
                            }
                            else
                            {
                                variant.DoubleValue = EditorGUI.DoubleField(r1, variant.DoubleValue);
                            }
                        }
                        break;
                }

            }
        }

        private void DrawValueFieldInPropertyMode(Rect position, SerializedProperty property, VariantReference.EditorHelper helper)
        {
            _selectComponentDrawer.AllowNonComponents = true;
            _selectComponentDrawer.RestrictionType = null;
            _selectComponentDrawer.ShowXButton = false;
            var targProp = property.FindPropertyRelative(PROP_OBJREF);
            var memberProp = property.FindPropertyRelative(PROP_STRING);
            var vtypeProp = property.FindPropertyRelative(PROP_TYPE);

            if (targProp.objectReferenceValue == null)
            {
                _selectComponentDrawer.OnGUI(position, targProp);
            }
            else
            {
                var r1 = new Rect(position.xMin, position.yMin, position.width * 0.4f, position.height);
                var r2 = new Rect(r1.xMax, position.yMin, position.width - r1.width, position.height);
                _selectComponentDrawer.OnGUI(r1, targProp);

                System.Reflection.MemberInfo selectedMember;
                EditorGUI.BeginChangeCheck();
                memberProp.stringValue = SPEditorGUI.ReflectedPropertyField(r2, targProp.objectReferenceValue, memberProp.stringValue, com.spacepuppy.Dynamic.DynamicMemberAccess.Read, out selectedMember);
                if (EditorGUI.EndChangeCheck())
                    vtypeProp.SetEnumValue<VariantType>(selectedMember != null ? VariantReference.GetVariantType(DynamicUtil.GetReturnType(selectedMember)) : VariantType.Null);
            }
        }

        private void DrawValueFieldInEvalMode(Rect position, SerializedProperty property, VariantReference.EditorHelper helper)
        {
            _selectComponentDrawer.AllowNonComponents = true;
            _selectComponentDrawer.RestrictionType = null;
            _selectComponentDrawer.ShowXButton = false;
            var targProp = property.FindPropertyRelative(PROP_OBJREF);
            var evalProp = property.FindPropertyRelative(PROP_STRING);
            var vtypeProp = property.FindPropertyRelative(PROP_TYPE);

            var r1 = new Rect(position.xMin, position.yMin, position.width * 0.4f, position.height);
            var r2 = new Rect(r1.xMax, position.yMin, position.width - r1.width, position.height);
            _selectComponentDrawer.OnGUI(r1, targProp);
            evalProp.stringValue = EditorGUI.TextField(r2, evalProp.stringValue);
            vtypeProp.SetEnumValue<VariantType>(VariantType.Null);
        }

        #endregion

        #region Static Utils

        public static void CopyValuesToHelper(SerializedProperty property, VariantReference.EditorHelper helper)
        {
            helper._mode = property.FindPropertyRelative(PROP_MODE).GetEnumValue<VariantReference.RefMode>();
            helper._type = property.FindPropertyRelative(PROP_TYPE).GetEnumValue<VariantType>();
            helper._x = property.FindPropertyRelative(PROP_X).floatValue;
            helper._y = property.FindPropertyRelative(PROP_Y).floatValue;
            helper._z = property.FindPropertyRelative(PROP_Z).floatValue;
            helper._w = property.FindPropertyRelative(PROP_W).doubleValue;
            helper._string = property.FindPropertyRelative(PROP_STRING).stringValue;
            helper._unityObjectReference = property.FindPropertyRelative(PROP_OBJREF).objectReferenceValue;
        }

        public static void CopyValuesFromHelper(SerializedProperty property, VariantReference.EditorHelper helper)
        {
            property.FindPropertyRelative(PROP_MODE).SetEnumValue(helper._mode);
            property.FindPropertyRelative(PROP_TYPE).SetEnumValue(helper._type);
            property.FindPropertyRelative(PROP_X).floatValue = helper._x;
            property.FindPropertyRelative(PROP_Y).floatValue = helper._y;
            property.FindPropertyRelative(PROP_Z).floatValue = helper._z;
            property.FindPropertyRelative(PROP_W).doubleValue = helper._w;
            property.FindPropertyRelative(PROP_STRING).stringValue = helper._string;
            property.FindPropertyRelative(PROP_OBJREF).objectReferenceValue = helper._unityObjectReference;
        }

        public static void SetSerializedProperty(SerializedProperty property, object obj)
        {
            if (property == null) throw new System.ArgumentNullException(nameof(property));

            var variant = new VariantReference();
            variant.Value = obj;
            CopyValuesFromHelper(property, new VariantReference.EditorHelper(variant));
        }

        public static object GetFromSerializedProperty(SerializedProperty property)
        {
            if (property == null) throw new System.ArgumentNullException(nameof(property));

            var helper = new VariantReference.EditorHelper();
            CopyValuesToHelper(property, helper);
            return helper.Target.Value;
        }

        #endregion

    }

}
