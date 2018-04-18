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
            if (token == null) return hash;

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


        /// <summary>
        /// Lerp the target objects values to the state of the StateToken. If the member doesn't have a current state/undefined, 
        /// then the member is set to the current state in this StateToken.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        public static void Lerp(this StateToken token, object obj, float t)
        {
            if (token == null) return;

            var e = token.GetEnumerator();
            while (e.MoveNext())
            {
                object value;
                if (DynamicUtil.TryGetValue(obj, e.Current.Key, out value))
                {
                    value = Evaluator.TryLerp(value, e.Current.Value, t);
                    DynamicUtil.SetValue(obj, e.Current.Key, value);
                }
                else
                {
                    DynamicUtil.SetValue(obj, e.Current.Key, e.Current.Value);
                }
            }
        }

    }
}
