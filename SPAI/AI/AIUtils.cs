using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy.AI
{

    public static class AIUtils
    {
        
        public static ComplexTarget GetAsComplexTarget(this VariantCollection coll, string key)
        {
            var v = coll.GetVariant(key);
            if (v == null) return ComplexTarget.Null;

            switch (v.ValueType)
            {
                case VariantType.Object:
                    return ComplexTarget.FromObject(v.Value);
                case VariantType.Vector2:
                    return new ComplexTarget(v.Vector2Value);
                case VariantType.Vector3:
                case VariantType.Vector4:
                    return new ComplexTarget(v.Vector3Value);
                case VariantType.GameObject:
                case VariantType.Component:
                    return ComplexTarget.FromObject(v.Value);
                default:
                    return ComplexTarget.Null;
            }
        }
        
    }

}
