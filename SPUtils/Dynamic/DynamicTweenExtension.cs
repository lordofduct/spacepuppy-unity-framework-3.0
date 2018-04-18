using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using com.spacepuppy.Tween;

namespace com.spacepuppy.Dynamic
{
    public static class DynamicTweenExtension
    {
        
        public static TweenHash TweenTo(TweenHash hash, StateToken token, com.spacepuppy.Tween.Ease ease, float dur)
        {
            if (hash == null || token == null) return hash;

            var e = token.GetEnumerator();
            while (e.MoveNext())
            {
                var value = e.Current.Value;
                if (value == null) continue;

                switch (VariantReference.GetVariantType(value.GetType()))
                {
                    case VariantType.Integer:
                    case VariantType.Float:
                    case VariantType.Double:
                    case VariantType.Vector2:
                    case VariantType.Vector3:
                    case VariantType.Vector4:
                    case VariantType.Quaternion:
                    case VariantType.Color:
                    case VariantType.Rect:
                        hash.To(e.Current.Key, ease, value, dur);
                        break;
                }
            }

            return hash;
        }

        public static TweenHash TweenTo(this TweenHash hash, VariantCollection coll, com.spacepuppy.Tween.Ease ease, float dur)
        {
            if (hash == null || coll == null) return hash;

            var e = coll.GetEnumerator();
            while (e.MoveNext())
            {
                var value = e.Current.Value;
                if (value == null) continue;

                switch (VariantReference.GetVariantType(value.GetType()))
                {
                    case VariantType.Integer:
                    case VariantType.Float:
                    case VariantType.Double:
                    case VariantType.Vector2:
                    case VariantType.Vector3:
                    case VariantType.Vector4:
                    case VariantType.Quaternion:
                    case VariantType.Color:
                    case VariantType.Rect:
                        hash.To(e.Current.Key, ease, value, dur);
                        break;
                }
            }

            return hash;
        }

    }
}
