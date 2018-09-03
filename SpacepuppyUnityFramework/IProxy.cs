using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Dynamic;
using com.spacepuppy.Utils;

namespace com.spacepuppy
{

    /// <summary>
    /// Interface to define an object that allows pass through of another object.
    /// 
    /// IProxy's are good for retrieving references to objects that can't otherwise be referenced directly in the inspector.
    /// 
    /// ObjUtil.GetAsFromSource respects IProxy.
    /// 
    /// This is useful at editor time when you may need to reference something in a scene that doesn't yet exist at editor time (an uninstantiated prefab for instance). 
    /// An IProxy may let you reference said object by name, tag, layer, type, etc.
    /// 
    /// For examples see:
    /// ProxyTarget
    /// </summary>
    public interface IProxy
    {
        System.Type GetTargetType();

        object GetTarget();
        object GetTarget(object arg);
    }

    /// <summary>
    /// A serializable IProxy struct that will search the scene for an object by name/tag/type.
    /// </summary>
    [System.Serializable]
    public struct QueryProxy : IProxy
    {

        #region Fields

        [SerializeField()]
        private UnityEngine.Object _target;
        [SerializeField()]
        private SearchBy _searchBy;
        [SerializeField()]
        private string _queryString;

        #endregion

        #region CONSTRUCTOR

        public QueryProxy(UnityEngine.Object target)
        {
            _target = target;
            _searchBy = SearchBy.Nothing;
            _queryString = null;
        }

        public QueryProxy(SearchBy searchBy)
        {
            _target = null;
            _searchBy = searchBy;
            _queryString = null;
        }

        public QueryProxy(SearchBy searchBy, string query)
        {
            _target = null;
            _searchBy = searchBy;
            _queryString = query;
        }

        #endregion

        #region Properties

        public UnityEngine.Object Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public SearchBy SearchBy
        {
            get { return _searchBy; }
            set { _searchBy = value; }
        }

        public string SearchByQuery
        {
            get { return _queryString; }
            set { _queryString = value; }
        }

        #endregion

        #region Methods

        public object GetTarget()
        {
            if (_searchBy == SearchBy.Nothing)
            {
                return (_target is IProxy) ? (_target as IProxy).GetTarget() : _target;
            }
            else
            {
                return ObjUtil.Find(_searchBy, _queryString);
            }
        }

        public object[] GetTargets()
        {
            if (_searchBy == SearchBy.Nothing)
            {
                return new object[] { (_target is IProxy) ? (_target as IProxy).GetTarget() : _target };
            }
            else
            {
                return ObjUtil.FindAll(_searchBy, _queryString);
            }
        }

        public T GetTarget<T>() where T : class
        {
            if (_searchBy == SearchBy.Nothing)
            {
                return ObjUtil.GetAsFromSource<T>(_target, true);
            }
            else
            {
                return ObjUtil.Find<T>(_searchBy, _queryString);
            }
        }

        public T[] GetTargets<T>() where T : class
        {
            if (_searchBy == SearchBy.Nothing)
            {
                var targ = ObjUtil.GetAsFromSource<T>(_target, true);
                return targ != null ? new T[] { targ } : ArrayUtil.Empty<T>();
            }
            else
            {
                return ObjUtil.FindAll<T>(_searchBy, _queryString);
            }
        }

        #endregion

        #region IProxy Interface

        object IProxy.GetTarget()
        {
            return this.GetTarget();
        }

        object IProxy.GetTarget(object arg)
        {
            return this.GetTarget();
        }

        public System.Type GetTargetType()
        {
            if (_target == null) return typeof(object);
            return (_target is IProxy) ? (_target as IProxy).GetTargetType() : _target.GetType();
        }

        #endregion

        #region Special Types

        public class ConfigAttribute : System.Attribute
        {

            public System.Type TargetType;
            public bool AllowProxy = true;

            public ConfigAttribute()
            {
                this.TargetType = typeof(GameObject);
            }

            public ConfigAttribute(System.Type targetType)
            {
                //if (targetType == null || 
                //    (!TypeUtil.IsType(targetType, typeof(UnityEngine.Object)) && !TypeUtil.IsType(targetType, typeof(IComponent)))) throw new TypeArgumentMismatchException(targetType, typeof(UnityEngine.Object), "targetType");
                if (targetType == null ||
                    (!TypeUtil.IsType(targetType, typeof(UnityEngine.Object)) && !targetType.IsInterface))
                    throw new TypeArgumentMismatchException(targetType, typeof(UnityEngine.Object), "targetType");

                this.TargetType = targetType;
            }

        }

        #endregion

    }

    /// <summary>
    /// A serializable IProxy struct that will access a target's member/property for an object.
    /// </summary>
    [System.Serializable]
    public struct MemberProxy : IProxy
    {

        #region Fields

        [SerializeField()]
        [SelectableObject]
        [DefaultFromSelf(HandleOnce = true)]
        private UnityEngine.Object _target;
        [SerializeField()]
        private string _memberName;

        #endregion

        #region Properties

        public UnityEngine.Object Target
        {
            get { return _target; }
            set { _target = value; }
        }

        public string MemberName
        {
            get { return _memberName; }
            set { _memberName = value; }
        }

        public object Value
        {
            get { return this.GetValue(); }
            set { this.SetValue(value); }
        }

        #endregion

        #region Methods

        public object GetValue()
        {
            if (_target == null)
                return null;
            else
                return DynamicUtil.GetValue(_target, _memberName);
        }

        public T GetValue<T>()
        {
            if (_target == null)
                return default(T);
            else
            {
                var result = DynamicUtil.GetValue(_target, _memberName);
                if (result is T)
                    return (T)result;
                else if (ConvertUtil.IsSupportedType(typeof(T)))
                    return ConvertUtil.ToPrim<T>(result);
                else
                    return default(T);
            }
        }

        public bool SetValue(object value)
        {
            return DynamicUtil.SetValue(_target, _memberName, value);
        }

        #endregion

        #region IProxy Interface

        object IProxy.GetTarget()
        {
            return this.GetValue();
        }

        object IProxy.GetTarget(object arg)
        {
            return this.GetValue();
        }

        System.Type IProxy.GetTargetType()
        {
            if (_memberName == null) return typeof(object);
            return DynamicUtil.GetReturnType(DynamicUtil.GetMember(_target, _memberName, false)) ?? typeof(object);
        }

        #endregion

        #region Config Attrib

        public class ConfigAttribute : System.Attribute
        {
            public DynamicMemberAccess MemberAccessLevel;

            public ConfigAttribute(DynamicMemberAccess memberAccessLevel)
            {
                this.MemberAccessLevel = memberAccessLevel;
            }

        }

        #endregion

    }

}
