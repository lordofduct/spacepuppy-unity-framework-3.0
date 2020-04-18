﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{
    public static class ComponentUtil
    {

        public static T SanitizeActiveAndEnabled<T>(T comp) where T : Component
        {
            if (comp == null) return null;
            if (comp is Behaviour) return (comp as Behaviour).isActiveAndEnabled ? comp : null;
            if (!comp.gameObject.activeInHierarchy) return null;
            if (comp is Collider && !(comp as Collider).enabled) return null;
            return comp;
        }

        public static bool IsComponentType(System.Type tp)
        {
            if (tp == null) return false;
            return typeof(Component).IsAssignableFrom(tp) || typeof(IComponent).IsAssignableFrom(tp);
        }

        public static bool IsAcceptableComponentType(System.Type tp)
        {
            if (tp == null) return false;
            return tp.IsInterface || typeof(Component).IsAssignableFrom(tp);
        }

        public static bool IsComponentSource(object obj)
        {
            return (obj is GameObject || obj is Component);
        }
        
        public static bool IsEnabled(this Component comp)
        {
            if (comp == null) return false;
            if (comp is Behaviour) return (comp as Behaviour).enabled;
            if (comp is Collider) return (comp as Collider).enabled;
            return true;
        }

        /// <summary>
        /// Implementation of 'Behaviour.isActiveAnEnabled' that also works for all Components (including Colliders).
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool IsActiveAndEnabled(this Component comp)
        {
            if (comp == null) return false;
            if (comp is Behaviour) return (comp as Behaviour).isActiveAndEnabled;
            if (!comp.gameObject.activeInHierarchy) return false;
            if (comp is Collider) return (comp as Collider).enabled;
            return true;
        }

        public static void SetEnabled(this Component comp, bool enabled)
        {
            if (comp == null) return;
            else if (comp is Behaviour) (comp as Behaviour).enabled = enabled;
            else if (comp is Collider) (comp as Collider).enabled = enabled;
        }





        #region HasComponent

        public static bool HasComponent<T>(this GameObject obj, bool testIfEnabled = false) where T : class
        {
            return HasComponent(obj, typeof(T), testIfEnabled);
        }
        public static bool HasComponent<T>(this Component obj, bool testIfEnabled = false) where T : class
        {
            return HasComponent(obj, typeof(T), testIfEnabled);
        }

        public static bool HasComponent(this GameObject obj, System.Type tp, bool testIfEnabled = false)
        {
            if (obj == null) return false;

            if (testIfEnabled)
            {
                foreach (var c in obj.GetComponents(tp))
                {
                    if (c.IsEnabled()) return true;
                }
                return false;
            }
            else
            {
                return (obj.GetComponent(tp) != null);
            }
        }
        public static bool HasComponent(this Component obj, System.Type tp, bool testIfEnabled = false)
        {
            if (obj == null) return false;

            if (testIfEnabled)
            {
                foreach (var c in obj.GetComponents(tp))
                {
                    if (c.IsEnabled()) return true;
                }
                return false;
            }
            else
            {
                return (obj.GetComponent(tp) != null);
            }
        }

        #endregion

        #region AddComponent

        public static T AddComponent<T>(this Component c) where T : Component
        {
            if (c == null) return null;
            return c.gameObject.AddComponent<T>();
        }
        public static Component AddComponent(this Component c, System.Type tp)
        {
            if (c == null) return null;
            return c.gameObject.AddComponent(tp);
        }

        public static T AddOrGetComponent<T>(this GameObject obj) where T : Component
        {
            if (obj == null) return null;

            T comp = obj.GetComponent<T>();
            if (comp == null)
            {
                comp = obj.AddComponent<T>();
            }

            return comp;
        }

        public static T AddOrGetComponent<T>(this Component obj) where T : Component
        {
            if (obj == null) return null;

            T comp = obj.GetComponent<T>();
            if (comp == null)
            {
                comp = obj.gameObject.AddComponent<T>();
            }

            return comp;
        }

        public static Component AddOrGetComponent(this GameObject obj, System.Type tp)
        {
            if (obj == null) return null;
            if (!TypeUtil.IsType(tp, typeof(Component))) return null;

            var comp = obj.GetComponent(tp);
            if (comp == null)
            {
                comp = obj.AddComponent(tp);
            }

            return comp;
        }

        public static Component AddOrGetComponent(this Component obj, System.Type tp)
        {
            if (obj == null) return null;
            if (!TypeUtil.IsType(tp, typeof(Component))) return null;

            var comp = obj.GetComponent(tp);
            if (comp == null)
            {
                comp = obj.gameObject.AddComponent(tp);
            }

            return comp;
        }

        
        public static T AddOrFindComponent<T>(this GameObject obj) where T : Component
        {
            if (obj == null) return null;

            T comp = obj.FindComponent<T>();
            if (comp == null)
            {
                comp = obj.AddComponent<T>();
            }

            return comp;
        }

        public static T AddOrFindComponent<T>(this Component obj) where T : Component
        {
            if (obj == null) return null;

            T comp = obj.FindComponent<T>();
            if (comp == null)
            {
                comp = obj.gameObject.AddComponent<T>();
            }

            return comp;
        }

        public static Component AddOrFindComponent(this GameObject obj, System.Type tp)
        {
            if (obj == null) return null;
            if (!TypeUtil.IsType(tp, typeof(Component))) return null;

            var comp = obj.FindComponent(tp);
            if (comp == null)
            {
                comp = obj.AddComponent(tp);
            }

            return comp;
        }

        public static Component AddOrFindComponent(this Component obj, System.Type tp)
        {
            if (obj == null) return null;
            if (!TypeUtil.IsType(tp, typeof(Component))) return null;

            var comp = obj.FindComponent(tp);
            if (comp == null)
            {
                comp = obj.gameObject.AddComponent(tp);
            }

            return comp;
        }
        
        #endregion

        #region GetComponent
        
        public static bool GetComponent<T>(this GameObject obj, out T comp) where T : class
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
            comp = obj.GetComponent<T>();
            return ObjUtil.IsObjectAlive(comp as UnityEngine.Object);
        }
        public static bool GetComponent<T>(this Component obj, out T comp) where T : class
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
            comp = obj.GetComponent<T>();
            return ObjUtil.IsObjectAlive(comp as UnityEngine.Object);
        }

        public static bool GetComponent(this GameObject obj, System.Type tp, out Component comp)
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
            comp = obj.GetComponent(tp);
            return ObjUtil.IsObjectAlive(comp as UnityEngine.Object);
        }
        public static bool GetComponent(this Component obj, System.Type tp, out Component comp)
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
            comp = obj.GetComponent(tp);
            return ObjUtil.IsObjectAlive(comp as UnityEngine.Object);
        }

        #endregion

        #region GetComponents
        
        /// <summary>
        /// Generic access of GetComponents that supports collections other than just List<T>/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void GetComponents<T>(this GameObject obj, ICollection<T> lst) where T : class
        {
            if (obj == null) return;

            using (var tmpLst = TempCollection.GetList<Component>())
            {
                obj.GetComponents(typeof(T), tmpLst);
                var e = tmpLst.GetEnumerator();
                T c = null;
                while (e.MoveNext())
                {
                    c = e.Current as T;
                    if (ObjUtil.IsObjectAlive(c as UnityEngine.Object)) lst.Add(c);
                }
            }
        }

        /// <summary>
        /// Generic access of GetComponents that supports collections other than just List<T>/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void GetComponents<T>(this Component obj, ICollection<T> lst) where T : class
        {
            if (obj == null) return;

            GetComponents<T>(obj.gameObject, lst);
        }
       
        public static void GetComponents<T>(this GameObject obj, ICollection<T> lst, System.Func<Component, T> filter) where T : class
        {
            if (obj == null) return;

            using (var tmpLst = TempCollection.GetList<Component>())
            {
                obj.GetComponents(typeof(Component), tmpLst);
                var e = tmpLst.GetEnumerator();
                T c;
                while (e.MoveNext())
                {
                    c = (filter != null) ? filter(e.Current) : e.Current as T;
                    if (ObjUtil.IsObjectAlive(c as UnityEngine.Object)) lst.Add(c);
                }
            }
        }

        public static void GetComponents<T>(this Component obj, ICollection<T> lst, System.Func<Component, T> filter) where T : class
        {
            if (obj == null) return;

            GetComponents(obj.gameObject, lst, filter);
        }


        public static Component[] GetComponents(this GameObject obj, params System.Type[] types)
        {
            if (obj == null) return ArrayUtil.Empty<Component>();

            using (var tmpLst = TempCollection.GetList<Component>())
            using (var set = ReduceLikeTypes(types))
            {
                var e = set.GetEnumerator();
                while(e.MoveNext())
                {
                    obj.GetComponents(e.Current, tmpLst);
                }
                return tmpLst.ToArray();
            }
        }

        public static void GetComponents(this GameObject obj, System.Type[] types, ICollection<Component> lst)
        {
            if (obj == null) return;

            using (var tmpLst = TempCollection.GetList<Component>())
            using (var set = ReduceLikeTypes(types))
            {
                var e = set.GetEnumerator();
                while (e.MoveNext())
                {
                    obj.GetComponents(e.Current, tmpLst);
                }

                var e2 = tmpLst.GetEnumerator();
                while(e2.MoveNext())
                {
                    lst.Add(e2.Current);
                }
            }
        }

#endregion

        #region Child Component

        public static bool GetComponentInChildren<T>(this GameObject obj, out T comp) where T : class
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
            comp = obj.GetComponentInChildren(typeof(T)) as T;
            return ObjUtil.IsObjectAlive(comp as UnityEngine.Object);
        }

        public static bool GetComponentInChildren<T>(this Component obj, out T comp) where T : class
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
            comp = obj.GetComponentInChildren(typeof(T)) as T;
            return ObjUtil.IsObjectAlive(comp as UnityEngine.Object);
        }

        public static bool GetComponentInChildren<T>(this GameObject obj, System.Type tp, out Component comp)
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
            comp = obj.GetComponentInChildren(tp);
            return comp != null;
        }

        public static bool GetComponentInChildren<T>(this Component obj, System.Type tp, out Component comp)
        {
            if (obj == null)
            {
                comp = null;
                return false;
            }
            comp = obj.GetComponentInChildren(tp);
            return comp != null;
        }

        public static T GetComponentInChildrenAlt<T>(this GameObject obj) where T : class
        {
            if (obj == null) return null;
            return obj.GetComponentInChildren(typeof(T)) as T;
        }

        public static T GetComponentInChildrenAlt<T>(this Component obj) where T : class
        {
            if (obj == null) return null;
            return obj.GetComponentInChildren(typeof(T)) as T;
        }

        #endregion

        #region Child Components

        public static IEnumerable<T> GetChildComponents<T>(this GameObject obj, bool bIncludeSelf = false, bool bIncludeInactive = false) where T : class
        {
            if (obj == null) return Enumerable.Empty<T>();
            if(bIncludeSelf)
            {
                return obj.GetComponentsInChildren(typeof(T), bIncludeInactive).Cast<T>();
            }
            else
            {
                using (var temp = TempCollection.GetList<T>())
                {
                    GetChildComponents<T>(obj, temp, false, bIncludeInactive);
                    return temp.ToArray();
                }
            }
        }

        public static IEnumerable<T> GetChildComponents<T>(this Component obj, bool bIncludeSelf = false, bool bIncludeInactive = false) where T : class
        {
            if (obj == null) return Enumerable.Empty<T>();
            return GetChildComponents<T>(obj.gameObject, bIncludeSelf, bIncludeInactive);
        }

        public static void GetChildComponents<T>(this GameObject obj, ICollection<T> coll, bool bIncludeSelf = false, bool bIncludeInactive = false) where T : class
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            if (obj == null) return;

            using (var tmpLst = TempCollection.GetList<T>())
            {
                if(bIncludeSelf)
                {
                    obj.GetComponentsInChildren<T>(bIncludeInactive, tmpLst);
                    var e = tmpLst.GetEnumerator();
                    while(e.MoveNext())
                    {
                        coll.Add(e.Current);
                    }
                }
                else
                {
                    obj.GetComponentsInChildren<T>(bIncludeInactive, tmpLst);
                    var e = tmpLst.GetEnumerator();
                    while(e.MoveNext())
                    {
                        if ((e.Current as Component).gameObject != obj) coll.Add(e.Current);
                    }
                }
            }
        }

        public static void GetChildComponents<T>(this Component obj, ICollection<T> coll, bool bIncludeSelf = false, bool bIncludeInactive = false) where T : class
        {
            if (obj == null) return;
            GetChildComponents<T>(obj.gameObject, coll, bIncludeSelf, bIncludeInactive);
        }




        public static IEnumerable<Component> GetChildComponents(this GameObject obj, System.Type tp, bool bIncludeSelf = false, bool bIncludeInactive = false)
        {
            if (obj == null) return Enumerable.Empty<Component>();

            if (bIncludeSelf)
            {
                return obj.GetComponentsInChildren(tp, bIncludeInactive);
            }
            else
            {
                using (var temp = TempCollection.GetList<Component>())
                {
                    GetChildComponents(obj, tp, temp, false, bIncludeInactive);
                    return temp.ToArray();
                }
            }
        }

        public static IEnumerable<Component> GetChildComponents(this Component obj, System.Type tp, bool bIncludeSelf = false, bool bIncludeInactive = false)
        {
            if (obj == null) return Enumerable.Empty<Component>();
            return GetChildComponents(obj.gameObject, tp, bIncludeSelf, bIncludeInactive);
        }

        public static void GetChildComponents(this GameObject obj, System.Type tp, ICollection<Component> coll, bool bIncludeSelf = false, bool bIncludeInactive = false)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");
            if (obj == null) return;

            using (var tmpLst = TempCollection.GetList<Component>())
            {
                if (bIncludeSelf)
                {
                    obj.GetComponentsInChildren<Component>(bIncludeInactive, tmpLst);
                    var e = tmpLst.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (TypeUtil.IsType(e.Current.GetType(), tp)) coll.Add(e.Current);
                    }
                }
                else
                {
                    obj.GetComponentsInChildren<Component>(bIncludeInactive, tmpLst);
                    var e = tmpLst.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current.gameObject != obj && TypeUtil.IsType(e.Current.GetType(), tp)) coll.Add(e.Current);
                    }
                }
            }
        }

        public static void GetChildComponents(this Component obj, System.Type tp, ICollection<Component> coll, bool bIncludeSelf = false, bool bIncludeInactive = false)
        {
            if (obj == null) return;
            GetChildComponents(obj.gameObject, tp, coll, bIncludeSelf, bIncludeInactive);
        }

        #endregion

        #region RemoveComponent
        
        public static void RemoveComponents<T>(this GameObject obj) where T : class
        {
            RemoveComponents(obj, typeof(T));
        }

        public static void RemoveComponents<T>(this Component obj) where T : class
        {
            RemoveComponents(obj, typeof(T));
        }

        public static void RemoveComponents(this GameObject obj, System.Type tp)
        {
            if (obj == null) return;
            var arr = obj.GetComponents(tp);
            for (int i = 0; i < arr.Length; i++)
            {
                ObjUtil.SmartDestroy(arr[i]);
            }
        }

        public static void RemoveComponents(this Component obj, System.Type tp)
        {
            if (obj == null) return;
            var arr = obj.GetComponents(tp);
            for (int i = 0; i < arr.Length; i++)
            {
                ObjUtil.SmartDestroy(arr[i]);
            }
        }

        public static void Remove(this Component comp)
        {
            if (comp != null)
            {
                ObjUtil.SmartDestroy(comp);
            }
        }

        #endregion
        
        #region EntityHasComponent

        public static bool EntityHasComponent<T>(this GameObject obj, bool testIfEnabled = false) where T : class
        {
            return EntityHasComponent(obj, typeof(T), testIfEnabled);
        }

        public static bool EntityHasComponent<T>(this Component obj, bool testIfEnabled = false) where T : class
        {
            return EntityHasComponent(obj.gameObject, typeof(T), testIfEnabled);
        }

        public static bool EntityHasComponent(this GameObject obj, System.Type tp, bool testIfEnabled = false)
        {
            if (obj == null) return false;
            var root = obj.FindRoot();

            var c = root.GetComponentInChildren(tp);
            if (c == null) return false;
            return (testIfEnabled) ? c.IsEnabled() : true;
        }

        public static bool EntityHasComponent(this Component obj, System.Type tp, bool testIfEnabled = false)
        {
            return EntityHasComponent(obj.gameObject, tp, testIfEnabled);
        }

        #endregion

        #region FindComponent

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        public static T FindComponent<T>(this GameObject go) where T : class
        {
            if (go == null) return null;

            var entity = SPEntity.Pool.GetFromSource(go);
            if (entity != null) return entity.GetComponentInChildren<T>();
            else
            {
                var root = go.FindRoot();
                return root.GetComponentInChildren<T>();
            }
        }

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <returns></returns>
        public static T FindComponent<T>(this Component c) where T : class
        {
            if (c == null) return null;
            return FindComponent<T>(c.gameObject);
        }

        public static bool FindComponent<T>(this GameObject go, out T comp) where T : class
        {
            comp = FindComponent<T>(go);
            return ObjUtil.IsObjectAlive(comp as UnityEngine.Object);
        }

        public static bool FindComponent<T>(this Component c, out T comp) where T : class
        {
            if (c == null)
            {
                comp = null;
                return false;
            }
            comp = FindComponent<T>(c.gameObject);
            return ObjUtil.IsObjectAlive(comp as UnityEngine.Object);
        }

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static Component FindComponent(this GameObject go, System.Type tp)
        {
            if (go == null) return null;

            var entity = SPEntity.Pool.GetFromSource(go);
            if (entity != null) return entity.GetComponentInChildren(tp);
            else
            {
                var root = go.FindRoot();
                return root.GetComponentInChildren(tp);
            }
        }

        /// <summary>
        /// Finds a component starting at a gameobjects root and digging downward. First component found will be returned. 
        /// This method aught to be reserved for components that are unique to an Entity.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        public static Component FindComponent(this Component c, System.Type tp)
        {
            if (c == null) return null;
            return FindComponent(c.gameObject, tp);
        }

        public static bool FindComponent(this GameObject go, System.Type tp, out Component comp)
        {
            comp = FindComponent(go, tp);
            return comp != null;
        }

        public static bool FindComponent(this Component c, System.Type tp, out Component comp)
        {
            if (c == null)
            {
                comp = null;
                return false;
            }
            comp = FindComponent(c.gameObject, tp);
            return comp != null;
        }

        #endregion

        #region FindComponents

        public static T[] FindComponents<T>(this GameObject go, bool bIncludeInactive = false) where T : class
        {
            if (go == null) return ArrayUtil.Empty<T>();
            
            var root = go.FindRoot();
            return root.GetComponentsInChildren<T>(bIncludeInactive);
        }
        public static T[] FindComponents<T>(this Component c, bool bIncludeInactive = false) where T : class
        {
            if (c == null) return ArrayUtil.Empty<T>();
            return FindComponents<T>(c.gameObject, bIncludeInactive);
        }

        public static Component[] FindComponents(this GameObject go, System.Type tp, bool bIncludeInactive = false)
        {
            if (go == null) return ArrayUtil.Empty<Component>();
            return go.FindRoot().GetComponentsInChildren(tp, bIncludeInactive);
        }
        public static Component[] FindComponents(this Component c, System.Type tp, bool bIncludeInactive = false)
        {
            if (c == null) return ArrayUtil.Empty<Component>();
            return FindComponents(c.gameObject, tp, bIncludeInactive);
        }


        public static void FindComponents<T>(this GameObject go, ICollection<T> coll, bool bIncludeInactive = false) where T : class
        {
            if (go == null) return;
            GetChildComponents<T>(go.FindRoot(), coll, true, bIncludeInactive);
        }
        public static void FindComponents<T>(this Component c, ICollection<T> coll, bool bIncludeInactive = false) where T : class
        {
            if (c == null) return;
            GetChildComponents<T>(c.FindRoot(), coll, true, bIncludeInactive);
        }

        public static void FindComponents(this GameObject go, System.Type tp, ICollection<Component> coll, bool bIncludeInactive = false)
        {
            if (go == null) return;
            GetChildComponents(go.FindRoot(), tp, coll, true, bIncludeInactive);
        }
        public static void FindComponents(this Component c, System.Type tp, ICollection<Component> coll, bool bIncludeInactive = false)
        {
            if (c == null) return;
            GetChildComponents(c.FindRoot(), tp, coll, true, bIncludeInactive);
        }

        public static Component[] FindComponents(this GameObject go, System.Type[] types, bool bIncludeInactive = false)
        {
            if (go == null) return ArrayUtil.Empty<Component>();

            go = go.FindRoot();
            using (var lst = TempCollection.GetList<Component>())
            using (var set = ReduceLikeTypes(types))
            {
                var e = set.GetEnumerator();
                while (e.MoveNext())
                {
                    GetChildComponents(go, e.Current, lst, true, bIncludeInactive);
                }
                return lst.ToArray();
            }
        }

        public static Component[] FindComponents(this Component c, System.Type[] types, bool bIncludeInactive = false)
        {
            if (c == null) return ArrayUtil.Empty<Component>();

            return FindComponents(c.gameObject, types, bIncludeInactive);
        }

        public static void FindComponents(this GameObject go, System.Type[] types, ICollection<Component> coll, bool bIncludeInactive = false)
        {
            if (go == null) return;

            go = go.FindRoot();
            using (var set = ReduceLikeTypes(types))
            {
                var e = set.GetEnumerator();
                while (e.MoveNext())
                {
                    GetChildComponents(go, e.Current, coll, true, bIncludeInactive);
                }
            }
        }

        public static void FindComponents(this Component c, System.Type[] types, ICollection<Component> coll, bool bIncludeInactive = false)
        {
            if (c == null) return;

            FindComponents(c.gameObject, types, coll,bIncludeInactive);
        }

        #endregion






        #region Utils

        private static TempHashSet<System.Type> ReduceLikeTypes(System.Type[] arr)
        {
            var set = TempCollection.GetSet<System.Type>();
            foreach(var tp in arr)
            {
                if (set.Contains(tp)) continue;

                var e = set.GetEnumerator();
                bool donotadd = false;
                while(e.MoveNext())
                {
                    if (TypeUtil.IsType(tp, e.Current))
                    {
                        donotadd = true;
                        break;
                    }
                    if (TypeUtil.IsType(e.Current, tp))
                    {
                        set.Remove(e.Current);
                        break;
                    }
                }

                if (!donotadd)
                    set.Add(tp);
            }
            return set;
        }

        #endregion

    }
}
