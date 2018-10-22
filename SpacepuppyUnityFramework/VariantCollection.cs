using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using com.spacepuppy.Dynamic;

namespace com.spacepuppy
{

    [System.Serializable()]
    public class VariantCollection : IStateToken, ISerializationCallbackReceiver, ISerializable, IEnumerable<KeyValuePair<string, object>>
    {

        #region Fields

        [System.NonSerialized()]
        private Dictionary<string, VariantReference> _table = new Dictionary<string, VariantReference>();

        [SerializeField()]
        private string[] _keys;
        [SerializeField()]
        private VariantReference[] _values;

        #endregion

        #region CONSTRUCTOR

        public VariantCollection()
        {
        }

        #endregion

        #region Properties

        public object this[string key]
        {
            get
            {
                VariantReference v;
                if (_table.TryGetValue(key, out v))
                {
                    return v.Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                VariantReference v;
                if (_table.TryGetValue(key, out v))
                {
                    v.Value = value;
                }
                else
                {
                    _table.Add(key, new VariantReference(value));
                }
            }
        }

        public IEnumerable<string> Names { get { return _table.Keys; } }

        public int Count
        {
            get { return _table.Count; }
        }

        #endregion

        #region Methods

        public object GetValue(string key)
        {
            return this[key];
        }

        public bool TryGetValue(string skey, out object result)
        {
            VariantReference v;
            if (_table.TryGetValue(skey, out v))
            {
                result = v.Value;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public void SetValue(string key, object value)
        {
            this[key] = value;
        }

        public VariantReference GetVariant(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v;
            else
                return null;
        }

        public VariantReference GetVariant(string key, bool createIfNotExist)
        {
            if (createIfNotExist)
            {
                VariantReference v;
                if (_table.TryGetValue(key, out v))
                {
                    return v;
                }
                else
                {
                    v = new VariantReference();
                    _table.Add(key, v);
                    return v;
                }
            }
            else
            {
                return _table[key];
            }
        }

        public bool GetBool(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.BoolValue;
            else
                return false;
        }

        public void SetBool(string key, bool value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.BoolValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    BoolValue = value
                });
            }
        }

        public int GetInt(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.IntValue;
            else
                return 0;
        }

        public void SetInt(string key, int value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.IntValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    IntValue = value
                });
            }
        }

        public float GetFloat(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.FloatValue;
            else
                return 0f;
        }

        public void SetFloat(string key, float value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.FloatValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    FloatValue = value
                });
            }
        }

        public double GetDouble(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.DoubleValue;
            else
                return 0f;
        }

        public void SetDouble(string key, double value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.DoubleValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    DoubleValue = value
                });
            }
        }

        public Vector2 GetVector2(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.Vector2Value;
            else
                return Vector2.zero;
        }

        public void SetVector2(string key, Vector2 value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.Vector2Value = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    Vector2Value = value
                });
            }
        }

        public Vector3 GetVector3(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.Vector3Value;
            else
                return Vector3.zero;
        }

        public void SetVector3(string key, Vector3 value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.Vector3Value = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    Vector3Value = value
                });
            }
        }

        public Quaternion GetQuaternion(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.QuaternionValue;
            else
                return Quaternion.identity;
        }

        public void SetQuaternion(string key, Quaternion value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.QuaternionValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    QuaternionValue = value
                });
            }
        }

        public Color GetColor(string key)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
                return v.ColorValue;
            else
                return Color.black;
        }

        public void SetColor(string key, Color value)
        {
            VariantReference v;
            if (_table.TryGetValue(key, out v))
            {
                v.ColorValue = value;
            }
            else
            {
                _table.Add(key, new VariantReference()
                {
                    ColorValue = value
                });
            }
        }





        public bool HasMember(string key)
        {
            return _table.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _table.Remove(key);
        }

        #endregion

        #region IToken Interface

        /// <summary>
        /// Iterates over members of the collection and attempts to set them to an object as if they 
        /// were property names on that object.
        /// </summary>
        /// <param name="obj"></param>
        public void CopyTo(object obj)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                DynamicUtil.SetValue(obj, e.Current.Key, e.Current.Value.Value);
            }
        }

        /// <summary>
        /// Iterates over keys in this collection and attempts to update the values associated with that 
        /// key to the value pulled from a property on object.
        /// </summary>
        /// <param name="obj"></param>
        public void SyncFrom(object obj)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Value.Value = DynamicUtil.GetValue(obj, e.Current.Key);
            }
        }
        /// <summary>
        /// Lerp the target objects values to the state of the VarianteCollection. If the member doesn't have a current state/undefined, 
        /// then the member is set to the current state in this VariantCollection.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t"></param>
        public void Lerp(object obj, float t)
        {
            var e = _table.GetEnumerator();
            while (e.MoveNext())
            {
                object value;
                if (DynamicUtil.TryGetValue(obj, e.Current.Key, out value))
                {
                    value = Evaluator.TryLerp(value, e.Current.Value.Value, t);
                    DynamicUtil.SetValue(obj, e.Current.Key, value);
                }
                else
                {
                    DynamicUtil.SetValue(obj, e.Current.Key, e.Current.Value.Value);
                }
            }
        }

        #endregion

        #region ITokenizable Interface

        public object CreateStateToken()
        {
            if (_table.Count == 0) return com.spacepuppy.Utils.ArrayUtil.Empty<KeyValuePair<string, VariantReference>>();
            KeyValuePair<string, VariantReference>[] arr = new KeyValuePair<string, VariantReference>[_table.Count];
            var e = _table.GetEnumerator();
            int i = 0;
            while (e.MoveNext())
            {
                arr[i] = e.Current;
                i++;
            }
            return arr;
        }

        public void RestoreFromStateToken(object token)
        {
            if (token is KeyValuePair<string, VariantReference>[])
            {
                _table.Clear();
                var arr = token as KeyValuePair<string, VariantReference>[];
                foreach (var pair in arr)
                {
                    _table[pair.Key] = pair.Value;
                }
            }
            else
            {
                DynamicUtil.CopyState(this, token);
            }
        }

        #endregion

        #region IDynamic Interface

        object IDynamic.this[string sMemberName]
        {
            get { return (this as IDynamic).GetValue(sMemberName); }
            set { (this as IDynamic).SetValue(sMemberName, value); }
        }

        bool IDynamic.SetValue(string sMemberName, object value, params object[] index)
        {
            if (_table.ContainsKey(sMemberName))
            {
                this[sMemberName] = value;
                return true;
            }
            else if (DynamicUtil.HasMemberDirect(this, sMemberName, true))
                return DynamicUtil.SetValueDirect(this, sMemberName, value, index);
            else
            {
                this[sMemberName] = value;
                return true;
            }
        }

        object IDynamic.GetValue(string sMemberName, params object[] args)
        {
            if (_table.ContainsKey(sMemberName))
                return this[sMemberName];
            else
                return DynamicUtil.GetValueDirect(this, sMemberName, args);
        }

        bool IDynamic.TryGetValue(string sMemberName, out object result, params object[] args)
        {
            if (_table.ContainsKey(sMemberName))
            {
                result = this[sMemberName];
                return true;
            }
            else
                return DynamicUtil.TryGetValueDirect(this, sMemberName, out result, args);
        }

        object IDynamic.InvokeMethod(string sMemberName, params object[] args)
        {
            //throw new System.NotSupportedException();
            return DynamicUtil.InvokeMethodDirect(this, sMemberName, args);
        }

        bool IDynamic.HasMember(string sMemberName, bool includeNonPublic)
        {
            if (_table.ContainsKey(sMemberName))
                return true;
            else
                return DynamicUtil.TypeHasMember(this.GetType(), sMemberName, includeNonPublic);
        }

        IEnumerable<System.Reflection.MemberInfo> IDynamic.GetMembers(bool includeNonPublic)
        {
            var tp = this.GetType();
            if (Application.isEditor && !Application.isPlaying)
            {
                var ptp = typeof(Variant);
                for (int i = 0; i < _keys.Length; i++)
                {
                    yield return new DynamicPropertyInfo(_keys[i], tp, ptp);
                }
            }
            else
            {
                var ptp = typeof(Variant);
                var e = _table.GetEnumerator();
                while (e.MoveNext())
                {
                    yield return new DynamicPropertyInfo(e.Current.Key, tp, ptp);
                }
            }

            foreach (var p in DynamicUtil.GetMembersFromType(tp, includeNonPublic))
            {
                if (p.Name != "_table" && p.Name != "_values" && p.Name != "_keys")
                    yield return p;
            }
        }

        IEnumerable<string> IDynamic.GetMemberNames(bool includeNonPublic)
        {
            return _table.Keys;
        }

        System.Reflection.MemberInfo IDynamic.GetMember(string sMemberName, bool includeNonPublic)
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                if (_keys.Contains(sMemberName)) return new DynamicPropertyInfo(sMemberName, this.GetType(), typeof(Variant));
            }
            else if (_table.ContainsKey(sMemberName))
            {
                return new DynamicPropertyInfo(sMemberName, this.GetType(), typeof(Variant));
            }

            return DynamicUtil.GetMemberFromType(this.GetType(), sMemberName, includeNonPublic);
        }

        #endregion

        #region ISerializationCallbackReceiver Interface

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _table.Clear();
            var cnt = Mathf.Min(_values.Length, _keys.Length);
            for (int i = 0; i < cnt; i++)
            {
                _table.Add(_keys[i], _values[i]);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _keys = _table.Keys.ToArray();
            _values = _table.Values.ToArray();
        }

        #endregion

        #region ISerializable Interface

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.GetObjectData(info, context);
        }

        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            (this as ISerializationCallbackReceiver).OnBeforeSerialize();
            info.AddValue("_keys", _keys);
            info.AddValue("_values", _values);
        }

        protected VariantCollection(SerializationInfo info, StreamingContext context)
        {
            _keys = info.GetValue("_keys", typeof(string[])) as string[];
            _values = info.GetValue("_values", typeof(VariantReference[])) as VariantReference[];

            (this as ISerializationCallbackReceiver).OnAfterDeserialize();
        }

        #endregion

        #region IEnumerable Interface

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator : IEnumerator<KeyValuePair<string, object>>
        {

            private Dictionary<string, VariantReference>.Enumerator _e;
            private KeyValuePair<string, object> _current;

            internal Enumerator(VariantCollection coll)
            {
                _e = coll._table.GetEnumerator();
                _current = default(KeyValuePair<string, object>);
            }



            public KeyValuePair<string, object> Current
            {
                get
                {
                    return _current;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return _current;
                }
            }

            public void Dispose()
            {
                _e.Dispose();
                _current = default(KeyValuePair<string, object>);
            }

            public bool MoveNext()
            {
                if (_e.MoveNext())
                {
                    _current = new KeyValuePair<string, object>(_e.Current.Key, _e.Current.Value.Value);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                (_e as System.Collections.IEnumerator).Reset();
            }
        }

        #endregion

        #region Special Types

        [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
        public class AsPropertyListAttribute : System.Attribute
        {

            private System.Type _tp;

            public AsPropertyListAttribute(System.Type tp)
            {
                _tp = tp;
            }

            public System.Type TargetType { get { return _tp; } }

        }
        
        #endregion

    }

}
