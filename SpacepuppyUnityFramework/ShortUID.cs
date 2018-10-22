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

    /// <summary>
    /// Similar to ShortUid in that it can store the same numeric value, but can also be customized to be a unique string instead. 
    /// ShortUid can be implicitly converted to TokenId.
    /// </summary>
    [System.Serializable]
    public struct TokenId
    {

        public static readonly TokenId Empty = new TokenId();

        #region Fields

        [UnityEngine.SerializeField]
        private uint _low;
        [UnityEngine.SerializeField]
        private uint _high;
        [UnityEngine.SerializeField]
        private string _id;

        #endregion

        #region CONSTRUCTOR

        public TokenId(long value)
        {
            _low = (uint)(value & uint.MaxValue);
            _high = (uint)(value >> 32);
            _id = null;
        }

        public TokenId(string value)
        {
            _low = 0;
            _high = 0;
            _id = value;
        }

        public static TokenId NewId()
        {
            return new TokenId(System.DateTime.UtcNow.Ticks);
        }

        #endregion

        #region Properties

        public bool HasValue
        {
            get { return !string.IsNullOrEmpty(_id) || _low != 0 || _high != 0; }
        }

        public long LongValue
        {
            get
            {
                return ((long)_high << 32) | (long)_low;
            }
        }

        public bool IsLong
        {
            get
            {
                return string.IsNullOrEmpty(_id);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the id as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.IsLong)
                return this.LongValue.ToString("X16");
            else
                return _id ?? string.Empty;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a
        /// specified Object represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is TokenId)
            {
                return this.Equals((TokenId)obj);
            }
            else if (obj is ShortUid)
            {
                return this.IsLong && ((ShortUid)obj).Value == this.LongValue;
            }
            return false;
        }

        public bool Equals(TokenId id)
        {
            if (this.IsLong)
            {
                return id.IsLong && this._high == id._high && this._low == id._low;
            }
            else
            {
                return !id.IsLong && this._id == id._id;
            }
        }

        public bool Equals(ShortUid uid)
        {
            return this.IsLong && this.LongValue == uid.Value;
        }

        /// <summary>
        /// Returns the HashCode for underlying id.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (this.IsLong)
                return (int)(_high ^ _low);
            else
                return _id.GetHashCode();
        }

        #endregion

        #region Operators

        /// <summary>
        /// Determines if both TokenId have the same underlying
        /// id value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(TokenId x, TokenId y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Determines if both TokenId do not have the
        /// same underlying id value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(TokenId x, TokenId y)
        {
            return !x.Equals(y);
        }

        /// <summary>
        /// Implicitly converts the TokenId to it's string equivilent
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static implicit operator string(TokenId uid)
        {
            return uid.ToString();
        }

        /// <summary>
        /// Implicitly converts from a ShortUid to a TokenId
        /// </summary>
        /// <param name="uid"></param>
        public static implicit operator TokenId(ShortUid uid)
        {
            return new TokenId(uid.Value);
        }

        #endregion

        #region Special Types

        public class ConfigAttribute : System.Attribute
        {
            public bool ReadOnly;
            public bool AllowZero;
        }

        #endregion

    }

}
