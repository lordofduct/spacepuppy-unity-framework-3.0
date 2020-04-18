﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Project
{

    /// <summary>
    /// Exists to make SerializableInterfaceRef drawable. Do not inherit from this, instead inherit from SerializableInterfaceRef.
    /// </summary>
    public abstract class BaseSerializableInterfaceRef
    {

    }

    /// <summary>
    /// Supports easily serializing a UnityEngine.Object as some interface type, while also allowing the assignment of non-UnityEngine.Object's as said interface type as well (but not serialized).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class SerializableInterfaceRef<T> : BaseSerializableInterfaceRef, ISerializationCallbackReceiver where T : class
    {

        #region Fields

        [SerializeField]
        private UnityEngine.Object _obj;
        [System.NonSerialized]
        private T _value;

        #endregion

        #region CONSTRUCTOR

        public SerializableInterfaceRef()
        {

        }

        public SerializableInterfaceRef(T value)
        {
            this.Value = value;
        }

        #endregion

        #region Properties

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _obj = value as UnityEngine.Object;
                _value = value;
            }
        }

        #endregion

        #region ISerializationCallbackReceiver Interface

        public void OnAfterDeserialize()
        {
            _value = _obj as T;
        }

        public void OnBeforeSerialize()
        {
            _obj = _value as UnityEngine.Object;
        }

        #endregion

    }

}
