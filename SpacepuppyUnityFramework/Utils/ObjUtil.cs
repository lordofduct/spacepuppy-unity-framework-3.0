﻿#pragma warning disable 0168 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{

    public static class ObjUtil
    {

        #region Fields

        private static System.Func<UnityEngine.Object, bool> _isObjectAlive;

        private static Dictionary<System.Type, System.Type[]> _interfaceToComponentMap;

        private static System.Type[] GetInterfaceComponentMap(System.Type tp)
        {
            if (!tp.IsInterface) throw new System.ArgumentException("Generic Type is not an interface.");

            System.Type[] arr;
            if(_interfaceToComponentMap != null && _interfaceToComponentMap.TryGetValue(tp, out arr))
                return arr;

            System.Type utp = typeof(UnityEngine.Object);
            arr = (from t in TypeUtil.GetTypesAssignableFrom(tp)
                   where utp.IsAssignableFrom(t)
                   select t).ToArray();
            if (_interfaceToComponentMap == null) _interfaceToComponentMap = new Dictionary<System.Type, System.Type[]>();
            _interfaceToComponentMap[tp] = arr;

            return arr;
        }

        #endregion

        #region CONSTRUCTOR

        static ObjUtil()
        {
            try
            {
                var tp = typeof(UnityEngine.Object);
                var meth = tp.GetMethod("IsNativeObjectAlive", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                if (meth != null)
                {
                    var d = System.Delegate.CreateDelegate(typeof(System.Func<UnityEngine.Object, bool>), meth) as System.Func<UnityEngine.Object, bool>;
                    _isObjectAlive = (a) => !object.ReferenceEquals(a, null) && d(a);
                }
                else
                    _isObjectAlive = (a) => a != null;
            }
            catch
            {
                //incase there was a change to the UnityEngine.dll
                _isObjectAlive = (a) => a != null;
                UnityEngine.Debug.LogWarning("This version of Spacepuppy Framework does not support the version of Unity it's being used with. (ObjUtil)");
                //throw new System.InvalidOperationException("This version of Spacepuppy Framework does not support the version of Unity it's being used with.");
            }
        }

        #endregion

        #region Find Methods
        
        public static UnityEngine.Object Find(SearchBy search, string query)
        {
            switch(search)
            {
                case SearchBy.Nothing:
                    return null;
                case SearchBy.Tag:
                    return GameObjectUtil.FindWithMultiTag(query);
                case SearchBy.Name:
                    return UnityEngine.GameObject.Find(query);
                case SearchBy.Type:
                    return ObjUtil.FindObjectOfType(TypeUtil.FindType(query));
                default:
                    return null;
            }
        }

        public static T Find<T>(SearchBy search, string query) where T : class
        {
            switch (search)
            {
                case SearchBy.Nothing:
                    return null;
                case SearchBy.Tag:
                    return ObjUtil.GetAsFromSource<T>(GameObjectUtil.FindWithMultiTag(query));
                case SearchBy.Name:
                    return ObjUtil.GetAsFromSource<T>(UnityEngine.GameObject.Find(query));
                case SearchBy.Type:
                    return ObjUtil.GetAsFromSource<T>(ObjUtil.FindObjectOfType(TypeUtil.FindType(query)));
                default:
                    return null;
            }
        }


        public static UnityEngine.Object[] FindAll(SearchBy search, string query)
        {
            switch (search)
            {
                case SearchBy.Nothing:
                    return ArrayUtil.Empty<UnityEngine.Object>();
                case SearchBy.Tag:
                    return GameObjectUtil.FindGameObjectsWithMultiTag(query);
                case SearchBy.Name:
                    {
                        using (var tmp = com.spacepuppy.Collections.TempCollection.GetList<UnityEngine.GameObject>())
                        {
                            GameObjectUtil.FindAllByName(query, tmp);
                            return tmp.ToArray();
                        }
                    }
                case SearchBy.Type:
                    return ObjUtil.FindObjectsOfType(TypeUtil.FindType(query));
                default:
                    return null;
            }
        }

        public static UnityEngine.Object[] FindAll(SearchBy search, string query, System.Type tp)
        {
            switch (search)
            {
                case SearchBy.Nothing:
                    return ArrayUtil.Empty<UnityEngine.Object>();
                case SearchBy.Tag:
                    {
                        using (var tmp = com.spacepuppy.Collections.TempCollection.GetList<UnityEngine.GameObject>())
                        using (var results = com.spacepuppy.Collections.TempCollection.GetList<UnityEngine.Object>())
                        {
                            GameObjectUtil.FindGameObjectsWithMultiTag(query, tmp);
                            var e = tmp.GetEnumerator();
                            while (e.MoveNext())
                            {
                                var o = ObjUtil.GetAsFromSource(tp, e.Current) as UnityEngine.Object;
                                if (o != null) results.Add(o);
                            }
                            return results.ToArray();
                        }
                    }
                case SearchBy.Name:
                    {
                        using (var tmp = com.spacepuppy.Collections.TempCollection.GetList<UnityEngine.GameObject>())
                        using (var results = com.spacepuppy.Collections.TempCollection.GetList<UnityEngine.Object>())
                        {
                            GameObjectUtil.FindAllByName(query, tmp);
                            var e = tmp.GetEnumerator();
                            while (e.MoveNext())
                            {
                                var o = ObjUtil.GetAsFromSource(tp, e.Current) as UnityEngine.Object;
                                if (o != null) results.Add(o);
                            }
                            return results.ToArray();
                        }
                    }
                case SearchBy.Type:
                    {
                        using (var results = com.spacepuppy.Collections.TempCollection.GetList<UnityEngine.Object>())
                        {
                            foreach (var o in ObjUtil.FindObjectsOfType(TypeUtil.FindType(query)))
                            {
                                var o2 = ObjUtil.GetAsFromSource(tp, o) as UnityEngine.Object;
                                if (o2 != null) results.Add(o2);
                            }
                            return results.ToArray();
                        }
                    }
                default:
                    return null;
            }
        }

        public static T[] FindAll<T>(SearchBy search, string query) where T : class
        {
            switch (search)
            {
                case SearchBy.Nothing:
                    return ArrayUtil.Empty<T>();
                case SearchBy.Tag:
                    {
                        using (var tmp = com.spacepuppy.Collections.TempCollection.GetList<UnityEngine.GameObject>())
                        using (var results = com.spacepuppy.Collections.TempCollection.GetList<T>())
                        {
                            GameObjectUtil.FindGameObjectsWithMultiTag(query, tmp);
                            var e = tmp.GetEnumerator();
                            while(e.MoveNext())
                            {
                                var o = ObjUtil.GetAsFromSource<T>(e.Current);
                                if (o != null) results.Add(o);
                            }
                            return results.ToArray();
                        }
                    }
                case SearchBy.Name:
                    {
                        using (var tmp = com.spacepuppy.Collections.TempCollection.GetList<UnityEngine.GameObject>())
                        using (var results = com.spacepuppy.Collections.TempCollection.GetList<T>())
                        {
                            GameObjectUtil.FindAllByName(query, tmp);
                            var e = tmp.GetEnumerator();
                            while (e.MoveNext())
                            {
                                var o = ObjUtil.GetAsFromSource<T>(e.Current);
                                if (o != null) results.Add(o);
                            }
                            return results.ToArray();
                        }
                    }
                case SearchBy.Type:
                    {
                        using (var results = com.spacepuppy.Collections.TempCollection.GetList<T>())
                        {
                            foreach(var o in ObjUtil.FindObjectsOfType(TypeUtil.FindType(query)))
                            {
                                var o2 = ObjUtil.GetAsFromSource<T>(o);
                                if (o2 != null) results.Add(o2);
                            }
                            return results.ToArray();
                        }
                    }
                default:
                    return null;
            }
        }


        public static UnityEngine.Object FindObjectOfType(System.Type tp)
        {
            if (tp == null) return null;

            if(tp.IsInterface)
            {
                var map = GetInterfaceComponentMap(tp);
                if (map.Length == 0) return null;

                foreach (var ctp in map)
                {
                    var obj = UnityEngine.Object.FindObjectOfType(ctp);
                    if (obj != null) return obj;
                }
            }
            else
            {
                return UnityEngine.Object.FindObjectOfType(tp);
            }

            return null;
        }

        public static UnityEngine.Object[] FindObjectsOfType(System.Type tp)
        {
            if (tp == null) return ArrayUtil.Empty<UnityEngine.Object>();

            if (tp.IsInterface)
            {
                var map = GetInterfaceComponentMap(tp);
                using (var lst = TempCollection.GetList<UnityEngine.Object>())
                {
                    foreach (var ctp in map)
                    {
                        lst.AddRange(UnityEngine.Object.FindObjectsOfType(ctp));
                    }
                    return lst.ToArray();
                }
            }
            else
            {
                return UnityEngine.Object.FindObjectsOfType(tp);
            }
        }

        public static int FindObjectsOfType(System.Type tp, ICollection<UnityEngine.Object> lst)
        {
            if (tp == null) return 0;

            if (tp.IsInterface)
            {
                var map = GetInterfaceComponentMap(tp);
                int cnt = 0;
                foreach (var ctp in map)
                {
                    var arr = UnityEngine.Object.FindObjectsOfType(ctp);
                    cnt += arr.Length;
                    lst.AddRange(arr);
                }
                return cnt;
            }
            else
            {
                var arr = UnityEngine.Object.FindObjectsOfType(tp);
                foreach (var obj in arr)
                {
                    lst.Add(obj);
                }
                return arr.Length;
            }
        }


        public static T FindObjectOfInterface<T>() where T : class
        {
            var tp = typeof(T);
            var map = GetInterfaceComponentMap(tp);
            if (map.Length == 0) return null;

            foreach (var ctp in map)
            {
                var obj = UnityEngine.Object.FindObjectOfType(ctp);
                if (obj != null) return obj as T;
            }

            return null;
        }

        public static T[] FindObjectsOfInterface<T>() where T : class
        {
            var tp = typeof(T);
            var map = GetInterfaceComponentMap(tp);
            using (var lst = TempCollection.GetSet<T>())
            {
                foreach(var ctp in map)
                {
                    foreach(var obj in UnityEngine.Object.FindObjectsOfType(ctp))
                    {
                        lst.Add(obj as T);
                    }
                }
                return lst.ToArray();
            }
        }

        public static int FindObjectsOfInterface<T>(ICollection<T> lst) where T : class
        {
            var tp = typeof(T);
            var map = GetInterfaceComponentMap(tp);
            int cnt = 0;
            foreach (var ctp in map)
            {
                var arr = UnityEngine.Object.FindObjectsOfType(ctp);
                cnt += arr.Length;
                foreach (var obj in arr)
                {
                    lst.Add(obj as T);
                }
            }
            return cnt;
        }

        #endregion

        #region Casting

        /// <summary>
        /// Returns true null if the UnityEngine.Object is dead, otherwise returns itself.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T SanitizeRef<T>(this T obj) where T : class
        {
            return (obj is UnityEngine.Object && (obj as UnityEngine.Object) == null) ? null : obj;
        }

        public static object ReduceIfProxy(this object obj)
        {
            if (obj is IProxy) return (obj as IProxy).GetTarget();

            return obj.SanitizeRef();
        }

        public static object ReduceIfProxy(this object obj, object arg)
        {
            if (obj is IProxy) return (obj as IProxy).GetTarget(arg);

            return obj.SanitizeRef();
        }


        public static T GetAsFromSource<T>(object obj) where T : class
        {
            obj = obj.SanitizeRef();
            if (obj == null) return null;
            if (obj is T) return obj as T;
            if (obj is IComponent)
            {
                var c = (obj as IComponent).component;
                if (c is T) return c as T;
            }
            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go is T) return go as T;

            //if (go != null && ComponentUtil.IsAcceptableComponentType(typeof(T))) return go.GetComponentAlt<T>();
            if (go != null)
            {
                var tp = typeof(T);
                if (typeof(SPEntity).IsAssignableFrom(tp))
                    return SPEntity.Pool.GetFromSource(tp, go) as T;
                else if (ComponentUtil.IsAcceptableComponentType(tp))
                    return go.GetComponent(tp) as T;
            }

            return null;
        }

        public static bool GetAsFromSource<T>(object obj, out T result, bool respectProxy = false) where T : class
        {
            obj = obj.SanitizeRef();
            result = null;
            if (obj == null) return false;

            if (respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return false;
            }

            if (obj is T)
            {
                result = obj as T;
                return true;
            }
            if (obj is IComponent)
            {
                var c = (obj as IComponent).component;
                if (c is T)
                {
                    result = c as T;
                    return true;
                }
            }
            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go is T)
            {
                result = go as T;
                return true;
            }

            //if (go != null && ComponentUtil.IsAcceptableComponentType(typeof(T))) return go.GetComponentAlt<T>();
            if (go != null)
            {
                var tp = typeof(T);
                if (typeof(SPEntity).IsAssignableFrom(tp))
                {
                    var uobj = SPEntity.Pool.GetFromSource(tp, go);
                    if (uobj == null) return false;

                    result = uobj as T;
                    return result != null;
                }
                else if (ComponentUtil.IsAcceptableComponentType(tp))
                {
                    var uobj = go.GetComponent(tp);
                    if (uobj == null) return false;

                    result = uobj as T;
                    return result != null;
                }
            }

            return false;
        }

        public static object GetAsFromSource(System.Type tp, object obj)
        {
            obj = obj.SanitizeRef();
            if (obj == null) return null;

            var otp = obj.GetType();
            if (TypeUtil.IsType(otp, tp)) return obj;
            if (obj is IComponent)
            {
                var c = (obj as IComponent).component;
                if (!object.ReferenceEquals(c, null) && TypeUtil.IsType(c.GetType(), tp)) return c;
            }

            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (tp == typeof(UnityEngine.GameObject)) return go;

            if (go != null)
            {
                if (typeof(SPEntity).IsAssignableFrom(tp))
                    return SPEntity.Pool.GetFromSource(tp, go);
                else if (ComponentUtil.IsAcceptableComponentType(tp))
                    return go.GetComponent(tp);
            }

            return null;
        }

        public static bool GetAsFromSource(System.Type tp, object obj, out object result, bool respectProxy = false)
        {
            obj = obj.SanitizeRef();
            result = null;
            if (obj == null) return false;

            if (respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return false;
            }

            var otp = obj.GetType();
            if (TypeUtil.IsType(otp, tp))
            {
                result = obj;
                return true;
            }
            if (obj is IComponent)
            {
                var c = (obj as IComponent).component;
                if (!object.ReferenceEquals(c, null) && TypeUtil.IsType(c.GetType(), tp))
                {
                    result = c;
                    return true;
                }
            }

            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (tp == typeof(UnityEngine.GameObject))
            {
                result = go;
                return true;
            }

            if (go != null)
            {
                if (typeof(SPEntity).IsAssignableFrom(tp))
                {
                    var uobj = SPEntity.Pool.GetFromSource(tp, go);
                    if (uobj == null) return false;

                    result = uobj;
                    return true;
                }
                else if (ComponentUtil.IsAcceptableComponentType(tp))
                {
                    var uobj = go.GetComponent(tp);
                    if (uobj == null) return false;

                    result = uobj;
                    return true;
                }
            }

            return false;
        }


        public static T GetAsFromSource<T>(object obj, bool respectProxy) where T : class
        {
            obj = obj.SanitizeRef();
            if (obj == null) return null;

            if (respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return null;
            }

            if (obj is T) return obj as T;
            if (obj is IComponent)
            {
                var c = (obj as IComponent).component;
                if (c is T) return c as T;
            }
            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (go is T) return go as T;

            //if (go != null && ComponentUtil.IsAcceptableComponentType(typeof(T))) return go.GetComponentAlt<T>();
            if (go != null)
            {
                var tp = typeof(T);
                if (typeof(SPEntity).IsAssignableFrom(tp))
                    return SPEntity.Pool.GetFromSource(tp, go) as T;
                else if (ComponentUtil.IsAcceptableComponentType(tp))
                    return go.GetComponent(tp) as T;
            }

            return null;
        }

        public static object GetAsFromSource(System.Type tp, object obj, bool respectProxy)
        {
            obj = obj.SanitizeRef();
            if (obj == null) return null;

            if (respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return null;
            }

            var otp = obj.GetType();
            if (TypeUtil.IsType(otp, tp)) return obj;
            if (obj is IComponent)
            {
                var c = (obj as IComponent).component;
                if (!object.ReferenceEquals(c, null) && TypeUtil.IsType(c.GetType(), tp)) return c;
            }

            var go = GameObjectUtil.GetGameObjectFromSource(obj);
            if (tp == typeof(UnityEngine.GameObject)) return go;

            if (go != null)
            {
                if (typeof(SPEntity).IsAssignableFrom(tp))
                    return SPEntity.Pool.GetFromSource(tp, go);
                else if (ComponentUtil.IsAcceptableComponentType(tp))
                    return go.GetComponent(tp);
            }

            return null;
        }


        public static T[] GetAllFromSource<T>(object obj, bool includeChildren = false) where T : class
        {
            obj = obj.SanitizeRef();
            if (obj == null) return ArrayUtil.Empty<T>();

            using (var set = TempCollection.GetSet<T>())
            {
                if (obj is T) set.Add(obj as T);
                if (obj is IComponent)
                {
                    var c = (obj as IComponent).component;
                    if (c is T) set.Add(c as T);
                }

                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go is T) set.Add(go as T);

                //if (go != null && ComponentUtil.IsAcceptableComponentType(typeof(T))) go.GetComponentsAlt<T>(set);
                if (go != null)
                {
                    var tp = typeof(T);
                    if (typeof(SPEntity).IsAssignableFrom(tp))
                    {
                        var entity = SPEntity.Pool.GetFromSource(tp, go) as T;
                        if (entity != null) set.Add(entity);
                    }
                    else if (typeof(UnityEngine.GameObject).IsAssignableFrom(tp))
                    {
                        if (includeChildren)
                        {
                            using (var lst = TempCollection.GetList<UnityEngine.Transform>())
                            {
                                go.GetComponentsInChildren<UnityEngine.Transform>(lst);

                                var e = lst.GetEnumerator();
                                while (e.MoveNext())
                                {
                                    set.Add(e.Current.gameObject as T);
                                }
                            }
                        }
                    }
                    if (ComponentUtil.IsAcceptableComponentType(tp))
                    {
                        if (includeChildren)
                            go.GetChildComponents<T>(set, true);
                        else
                            go.GetComponents<T>(set);
                    }
                }

                return set.Count > 0 ? set.ToArray() : ArrayUtil.Empty<T>();
            }
        }

        public static object[] GetAllFromSource(System.Type tp, object obj, bool includeChildren = false)
        {
            obj = obj.SanitizeRef();
            if (obj == null) return ArrayUtil.Empty<object>();

            using (var set = TempCollection.GetSet<object>())
            {
                var otp = obj.GetType();
                if (TypeUtil.IsType(otp, tp)) set.Add(obj);
                if (obj is IComponent)
                {
                    var c = (obj as IComponent).component;
                    if (!object.ReferenceEquals(c, null) && TypeUtil.IsType(c.GetType(), tp)) set.Add(c);
                }

                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go != null)
                {
                    if (typeof(SPEntity).IsAssignableFrom(tp))
                    {
                        var entity = SPEntity.Pool.GetFromSource(tp, go);
                        if (entity != null) set.Add(entity);
                    }
                    else if (typeof(UnityEngine.GameObject).IsAssignableFrom(tp))
                    {
                        if (includeChildren)
                        {
                            using (var lst = TempCollection.GetList<UnityEngine.Transform>())
                            {
                                go.GetComponentsInChildren<UnityEngine.Transform>(lst);

                                var e = lst.GetEnumerator();
                                while (e.MoveNext())
                                {
                                    set.Add(e.Current.gameObject);
                                }
                            }
                        }
                        else
                        {
                            set.Add(go);
                        }
                    }
                    else if (ComponentUtil.IsAcceptableComponentType(tp))
                    {
                        using (var lst = TempCollection.GetList<UnityEngine.Component>())
                        {
                            if (includeChildren)
                                ComponentUtil.GetChildComponents(go, tp, lst, true);
                            else
                                go.GetComponents(tp, lst);

                            var e = lst.GetEnumerator();
                            while (e.MoveNext())
                            {
                                set.Add(e.Current);
                            }
                        }
                    }
                }

                return set.Count > 0 ? set.ToArray() : ArrayUtil.Empty<object>();
            }
        }

        public static bool IsType(object obj, System.Type tp)
        {
            if (obj == null) return false;

            return TypeUtil.IsType(obj.GetType(), tp);
        }

        public static bool IsType(object obj, System.Type tp, bool respectProxy)
        {
            if (obj == null) return false;

            if (respectProxy && obj is IProxy)
            {
                return TypeUtil.IsType((obj as IProxy).GetTargetType(), tp);
            }
            else
            {
                return TypeUtil.IsType(obj.GetType(), tp);
            }
        }

        #endregion

        #region Destruction Methods

        public static void SmartDestroy(UnityEngine.Object obj)
        {
            if (obj.IsNullOrDestroyed()) return;

            if (UnityEngine.Application.isEditor && !UnityEngine.Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            else
            {
                //UnityEngine.Object.Destroy(obj);
                if (obj is UnityEngine.GameObject)
                    (obj as UnityEngine.GameObject).Kill();
                else if (obj is UnityEngine.Transform)
                    (obj as UnityEngine.Transform).gameObject.Kill();
                else
                    UnityEngine.Object.Destroy(obj);
            }
        }

        public static System.Func<UnityEngine.Object, bool> IsObjectAlive
        {
            get { return _isObjectAlive; }
        }

        public static bool IsDisposed(this ISPDisposable obj)
        {
            if (object.ReferenceEquals(obj, null)) return true;

            return obj.IsDisposed;
        }

        /// <summary>
        /// Returns true if the object is either a null reference or has been destroyed by unity. 
        /// This will respect ISPDisposable over all else.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrDestroyed(this System.Object obj)
        {
            if (object.ReferenceEquals(obj, null)) return true;

            if (obj is ISPDisposable)
                return (obj as ISPDisposable).IsDisposed;
            else if (obj is UnityEngine.Object)
                return !_isObjectAlive(obj as UnityEngine.Object);
            else if (obj is UnityEngine.TrackedReference)
                return (obj as UnityEngine.TrackedReference) == null;
            else if (obj is IComponent)
                return !_isObjectAlive((obj as IComponent).component);
            else if (obj is IGameObjectSource)
                return !_isObjectAlive((obj as IGameObjectSource).gameObject);
            
            return false;
        }

        /// <summary>
        /// Unlike IsNullOrDestroyed, this only returns true if the managed object half of the object still exists, 
        /// but the unmanaged half has been destroyed by unity. 
        /// This will respect ISPDisposable over all else.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDestroyed(this System.Object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;

            if (obj is ISPDisposable)
                return (obj as ISPDisposable).IsDisposed;
            else if (obj is UnityEngine.Object)
                return !_isObjectAlive(obj as UnityEngine.Object);
            else if (obj is UnityEngine.TrackedReference)
                return (obj as UnityEngine.TrackedReference) == null;
            else if (obj is IComponent)
                return !_isObjectAlive((obj as IComponent).component);
            else if (obj is IGameObjectSource)
                return !_isObjectAlive((obj as IGameObjectSource).gameObject);


            //if (obj is UnityEngine.Object)
            //    return (obj as UnityEngine.Object) == null;
            //else if (obj is IComponent)
            //    return (obj as IComponent).component == null;
            //else if (obj is IGameObjectSource)
            //    return (obj as IGameObjectSource).gameObject == null;

            return false;
        }

        public static bool IsAlive(this System.Object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;

            if (obj is ISPDisposable)
                return !(obj as ISPDisposable).IsDisposed;
            else if (obj is UnityEngine.Object)
                return _isObjectAlive(obj as UnityEngine.Object);
            else if (obj is IComponent)
                return _isObjectAlive((obj as IComponent).component);
            else if (obj is IGameObjectSource)
                return _isObjectAlive((obj as IGameObjectSource).gameObject);


            //if (obj is UnityEngine.Object)
            //    return (obj as UnityEngine.Object) != null;
            //else if (obj is IComponent)
            //    return (obj as IComponent).component != null;
            //else if (obj is IGameObjectSource)
            //    return (obj as IGameObjectSource).gameObject != null;

            return true;
        }

        /// <summary>
        /// Returns true if the object passed in is one of the many UnityEngine.Object's that unity will create that are invalid and report being null, but ReferenceEquals reports not being null. 
        /// This is opposed to 'IsDestroyed' since this object should have never existed in the first place.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsInvalidObject(UnityEngine.Object obj)
        {
            //note - we use 'GetHashCode' because it returns the instanceId BUT doesn't do the stupid main thread test
            return !object.ReferenceEquals(obj, null) && obj.GetHashCode() == 0;
        }

        /// <summary>
        /// Returns true if the Unity object passed in is not null but isn't necessarily alive/destroyed. 
        /// This is a compliment to IsInvalidObject for use when you want to determine if some value was ever once valid. 
        /// For example if in the inspector you leave a UnityEngin.Object field null, the serializer will actually populate it with an 'Invalid Object'. 
        /// If you ever needed to differentiate between if that value was ever a value but since destroyed VS being an invalid object, this will help.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsValidObject(UnityEngine.Object obj)
        {
            //note - we use 'GetHashCode' because it returns the instanceId BUT doesn't do the stupid main thread test
            return !object.ReferenceEquals(obj, null) && obj.GetHashCode() != 0;
        }

        #endregion

        #region Misc

        /// <summary>
        /// Returns true if 'obj' is 'parent', attached to 'parent', or part of the child hiearchy of 'parent'.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsRelatedTo(UnityEngine.Object parent, UnityEngine.Object obj)
        {
            if (parent == null || obj == null) return false;
            if (parent == obj) return true;

            var go = GameObjectUtil.GetGameObjectFromSource(parent);
            if (go == null) return false;

            var child = GameObjectUtil.GetGameObjectFromSource(obj);
            if (child == null) return false;

            if (go == child) return true;
            return child.transform.IsChildOf(go.transform);
        }

        public static bool EqualsAny(System.Object obj, params System.Object[] others)
        {
            return System.Array.IndexOf(others, obj) >= 0;
        }
        
        #endregion

    }

}
