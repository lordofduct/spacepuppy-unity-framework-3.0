using System;

namespace com.spacepuppy
{

    /// <summary>
    /// A serializable semi-unique id. It is not universely unique, but will be system unique at the least, and should be team unique confidently.
    /// The id is based on the exact moment in time it was generated, and the idea that 2 team members both generate them simultaneously is absurd.
    /// </summary>
    [System.Serializable]
    public struct ShortUid
    {

        public static ShortUid Zero { get { return new ShortUid(); } }

        #region Fields

        //has to be stored with uint's
        //unity has a bug at the time of writing this where long doesn't serialize in prefabs correctly
        //there is a fix in unity beta 2017.1, but we are unsure as to when the full release will be out
        //so stuck with this hack fix
        [UnityEngine.SerializeField]
        private uint _low;
        [UnityEngine.SerializeField]
        private uint _high;

        #endregion

        #region CONSTRUCTOR

        public ShortUid(long value)
        {
            _low = (uint)(value & uint.MaxValue);
            _high = (uint)(value >> 32);
        }

        public static ShortUid NewId()
        {
            return new ShortUid(System.DateTime.UtcNow.Ticks);
        }

        #endregion

        #region Properties

        public long Value
        {
            get
            {
                return ((long)_high << 32) | (long)_low;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the base64 encoded guid as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value.ToString("X16");
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a
        /// specified Object represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ShortUid)
            {
                var uid = (ShortUid)obj;
                return uid._high == _high && uid._low == _low;
            }
            return false;
        }

        /// <summary>
        /// Returns the HashCode for underlying Guid.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)(_high ^ _low);
        }

        #endregion

        #region Conversion

        public static System.Guid ToGuid(ShortUid uid)
        {
            long val = uid.Value;
            int low = (int)(val & uint.MaxValue);
            short mid = (short)((val >> 32) & ushort.MaxValue);
            short high = (short)(val >> 48);
            return new System.Guid(low, mid, high, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        public static ShortUid ToShortUid(System.Guid uid)
        {
            var arr = uid.ToByteArray();
            long low = (int)(arr[3] << 24) | (int)(arr[2] << 16) | (int)(arr[1] << 8) | (int)arr[0];
            long high = (long)((int)(arr[7] << 24) | (int)(arr[6] << 16) | (int)(arr[5] << 8) | (int)arr[4]) << 32;
            return new ShortUid(high | low);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Determines if both ShortGuids have the same underlying
        /// Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(ShortUid x, ShortUid y)
        {
            return x._high == y._high && x._low == y._low;
        }

        /// <summary>
        /// Determines if both ShortGuids do not have the
        /// same underlying Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(ShortUid x, ShortUid y)
        {
            return x._high != y._high || x._low != y._low;
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's string equivilent
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static implicit operator string(ShortUid uid)
        {
            return uid.ToString();
        }

        public static implicit operator long(ShortUid uid)
        {
            return uid.Value;
        }

        #endregion

        #region Special Types

        public class ConfigAttribute : Attribute
        {
            public bool ReadOnly;
            public bool AllowZero;
        }

        #endregion

    }

}
