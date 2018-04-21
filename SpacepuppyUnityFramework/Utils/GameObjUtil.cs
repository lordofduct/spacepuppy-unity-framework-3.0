using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;
using com.spacepuppy.Geom;

namespace com.spacepuppy.Utils
{
    public static class GameObjectUtil
    {
        
        public static GameObject CreateRoot(string name)
        {
            var go = new GameObject(name);
            go.AddTag(SPConstants.TAG_ROOT);
            return go;
        }

        public static GameObject CreateRoot(string name, params System.Type[] components)
        {
            var go = new GameObject(name);
            go.AddTag(SPConstants.TAG_ROOT);
            foreach (var tp in components)
            {
                go.AddComponent(tp);
            }
            return go;
        }
        
        #region Get*FromSource

        public static bool IsGameObjectSource(object obj)
        {
            return (obj is GameObject || obj is Component || obj is IGameObjectSource);
        }

        public static bool IsGameObjectSource(object obj, bool respectProxy)
        {
            if (respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return false;
            }

            return (obj is GameObject || obj is Component || obj is IGameObjectSource);
        }

        /*
        public static GameObject GetGameObjectFromSource(object obj)
        {
            if (obj == null) return null;
            if (obj is GameObject)
                return obj as GameObject;
            if (obj is Component)
                return (obj as Component).gameObject;
            if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).gameObject;

            return null;
        }
        */

        public static GameObject GetGameObjectFromSource(object obj, bool respectProxy = false)
        {
            if (obj == null) return null;

            if(respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return null;
            }

            if (obj is GameObject)
                return obj as GameObject;
            if (obj is Component)
                return (obj as Component).gameObject;
            if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).gameObject;

            return null;
        }

        /*
        public static Transform GetTransformFromSource(object obj)
        {
            if (obj == null) return null;
            if (obj is Transform)
                return obj as Transform;
            if (obj is GameObject)
                return (obj as GameObject).transform;
            if (obj is Component)
                return (obj as Component).transform;
            if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).transform;

            return null;
        }
        */

        public static Transform GetTransformFromSource(object obj, bool respectProxy = false)
        {
            if (obj == null) return null;

            if (respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return null;
            }

            if (obj is Transform)
                return obj as Transform;
            if (obj is GameObject)
                return (obj as GameObject).transform;
            if (obj is Component)
                return (obj as Component).transform;
            if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).transform;

            return null;
        }

        public static GameObject GetRootFromSource(object obj, bool respectProxy = false)
        {
            if (obj.IsNullOrDestroyed()) return null;

            if (respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return null;
            }

            if (obj is IComponent) obj = (obj as IComponent).component;

            if (obj is Transform)
                return (obj as Transform).FindRoot();
            else if (obj is GameObject)
                return (obj as GameObject).FindRoot();
            else if (obj is SPComponent)
            {
                return (obj as SPComponent).entityRoot;
            }
            else if (obj is Component)
                return (obj as Component).FindRoot();
            else if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).gameObject.FindRoot();

            return null;
        }

        public static GameObject GetTrueRootFromSource(object obj, bool respectProxy = false)
        {
            if (obj.IsNullOrDestroyed()) return null;

            if (respectProxy && obj is IProxy)
            {
                obj = (obj as IProxy).GetTarget();
                if (obj == null) return null;
            }

            if (obj is IComponent) obj = (obj as IComponent).component;

            if (obj is Transform)
                return (obj as Transform).FindTrueRoot();
            else if (obj is GameObject)
                return (obj as GameObject).FindTrueRoot();
            else if (obj is SPComponent)
            {
                var r = (obj as SPComponent).entityRoot;
                if (r.HasTag(SPConstants.TAG_ROOT))
                    return r;
                else
                    return null;
            }
            else if (obj is Component)
                return (obj as Component).FindTrueRoot();
            else if (obj is IGameObjectSource)
                return (obj as IGameObjectSource).gameObject.FindTrueRoot();

            return null;
        }

        #endregion
        
        #region Kill Extension Methods

        /// <summary>
        /// Object is not null, dead/killed, and is active in hierarchy.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsAliveAndActive(this GameObject obj)
        {
            return obj != null && obj.activeInHierarchy && !obj.IsKilled();
        }

        /// <summary>
        /// Object is not null, dead/killed, and is active in hierarchy as well as enabled.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsAliveAndActive(this Component obj)
        {
            return obj != null && obj.IsActiveAndEnabled() && !obj.IsKilled();
        }

        /// <summary>
        /// Object is not null, dead/killed, and is active in hierarchy as well as enabled.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsAliveAndActive(this Behaviour obj)
        {
            return obj != null && obj.isActiveAndEnabled && !obj.IsKilled();
        }
        
        /// <summary>
        /// Tests if the object is either destroyed or killed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsKilled(this GameObject obj)
        {
            if (obj == null) return true;
            
            using (var lst = TempCollection.GetList<IKillableEntity>())
            {
                obj.GetComponents<IKillableEntity>(lst);
                if (lst.Count > 0)
                {
                    var e = lst.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current.IsDead) return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tests if the object is either destroyed or killed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsKilled(this Component obj)
        {
            if (obj == null) return true;
            
            using (var lst = TempCollection.GetList<IKillableEntity>())
            {
                obj.GetComponents<IKillableEntity>(lst);
                if (lst.Count > 0)
                {
                    var e = lst.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current.IsDead) return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Destroys the GameObject and its children, if the GameObject contains a KillableEntity component that will handle the death first and foremost.
        /// </summary>
        /// <param name="obj"></param>
        public static void Kill(this GameObject obj)
        {
            if (obj.IsNullOrDestroyed()) return;

            if(Application.isPlaying)
            {
                using (var lst = TempCollection.GetList<IKillableEntity>())
                {
                    //this returns in the order from top down, we will loop backwards to kill bottom up
                    obj.GetComponentsInChildren<IKillableEntity>(true, lst);
                    if (lst.Count > 0)
                    {
                        for (int i = lst.Count - 1; i > -1; i--)
                        {
                            lst[i].Kill();
                        }

                        if (lst[0].gameObject != obj)
                        {
                            UnityEngine.Object.Destroy(obj);
                        }
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(obj);
                    }
                }
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }

        }

        /// <summary>
        /// Destroys the entire entity, if the entity contains a KillableEntity component that will handle death first and foremost.
        /// </summary>
        /// <param name="obj"></param>
        public static void KillEntity(this GameObject obj)
        {
            Kill(obj.FindRoot());
        }

        #endregion

        #region CompareName

        public static bool CompareName(this GameObject go, string name)
        {
            if (go == null) return false;

            return go.name == name;
        }

        public static bool CompareName(this Component c, string name)
        {
            if (c == null) return false;

            if (c is SPEntity)
                return (c as SPEntity).CompareName(name);
            else
                return c.name == name;
        }
        
        #endregion

        #region Find Root

        /**
         * HasTrueRoot
         */

        /// <summary>
        /// Returns true if 
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool HasTrueRoot(this GameObject go)
        {
            if (go == null) return false;

            var t = go.transform;
            while (t != null)
            {
                if (MultiTagHelper.HasTag(t.gameObject, SPConstants.TAG_ROOT)) return true;
            }

            return false;
        }

        public static bool HasTrueRoot(this Component c)
        {
            if (c == null) return false;
            return HasTrueRoot(c.gameObject);
        }

        public static bool HasTrueRoot(this GameObject go, out GameObject root)
        {
            root = null;
            if (go == null) return false;

            var t = go.transform;
            while (t != null)
            {
                if (MultiTagHelper.HasTag(t.gameObject, SPConstants.TAG_ROOT))
                {
                    root = t.gameObject;
                    return true;
                }
            }

            return false;
        }

        public static bool HasTrueRoot(this Component c, out GameObject root)
        {
            if (c == null)
            {
                root = null;
                return false;
            }
            return HasTrueRoot(c.gameObject, out root);
        }

        /**
         * FindTrueRoot
         */

        /// <summary>
        /// Attempts to find a parent with a tag of 'Root', if none is found, null is returned.
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static GameObject FindTrueRoot(this GameObject go)
        {
            if (go == null) return null;

            var t = go.transform;
            while (t != null)
            {
                if (MultiTagHelper.HasTag(t, SPConstants.TAG_ROOT)) return t.gameObject;
                t = t.parent;
            }
            return null;
        }

        public static GameObject FindTrueRoot(this Component c)
        {
            if (c == null) return null;

            var t = c.transform;
            while (t != null)
            {
                if (MultiTagHelper.HasTag(t, SPConstants.TAG_ROOT)) return t.gameObject;
                t = t.parent;
            }
            return null;
        }

        /**
         * FindRoot
         */

        /// <summary>
        /// Attempts to find a parent with a tag of 'Root', if none is found, self is returned.
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static GameObject FindRoot(this GameObject go)
        {
            if (go == null) return null;

            var root = FindTrueRoot(go);
            return (root != null) ? root : go; //we return self if no root was found...
        }

        public static GameObject FindRoot(this Component c)
        {
            if (c == null) return null;

            //return FindRoot(c.gameObject);
            var root = FindTrueRoot(c);
            return (root != null) ? root : c.gameObject;
        }


        /**
         * ReduceToRoot IEnum
         */

        public static IEnumerable<GameObject> ReduceToRoot(this IEnumerable<GameObject> e, bool bdistinct = true)
        {
            if (bdistinct)
            {
                foreach (var obj in ReduceToRoot(e, false).Distinct()) yield return obj;
            }
            else
            {
                foreach (var obj in e)
                {
                    yield return FindRoot(obj);
                }
            }
        }

        public static IEnumerable<GameObject> ReduceToRoot(this IEnumerable<Component> e, bool bdistinct = true)
        {
            if (bdistinct)
            {
                foreach (var obj in ReduceToRoot(e, false).Distinct()) yield return obj;
            }
            else
            {
                foreach (var obj in e)
                {
                    yield return FindRoot(obj);
                }
            }
        }

        #endregion

        #region Find By Name

        /// <summary>
        /// Recurses through all children until a child of some name is found.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="sname"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static GameObject FindByName(this GameObject go, string sname, bool bIgnoreCase = false)
        {
            if (go == null) return null;
            var result = FindByName(go.transform, sname, bIgnoreCase);
            if (result != null)
                return result.gameObject;
            else
                return null;
        }

        /// <summary>
        /// Recurses through all children until a child of some name is found.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="sname"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static Transform FindByName(this Transform trans, string sname, bool bIgnoreCase = false)
        {
            if (trans == null) return null;
            foreach (var child in trans.IterateAllChildren())
            {
                if (StringUtil.Equals(child.name, sname, bIgnoreCase)) return child;
            }
            return null;
        }

        public static IEnumerable<GameObject> FindAllByName(string sname, bool bIgnoreCase = false)
        {
            foreach (var go in Object.FindObjectsOfType<GameObject>())
            {
                if (StringUtil.Equals(go.name, sname, bIgnoreCase)) yield return go;
            }
        }

        public static void FindAllByName(string sname, ICollection<GameObject> results, bool bIgnoreCase = false)
        {
            if (results == null) throw new System.ArgumentNullException("results");

            foreach (var go in Object.FindObjectsOfType<GameObject>())
            {
                if (StringUtil.Equals(go.name, sname, bIgnoreCase)) results.Add(go);
            }
        }

        public static Transform[] FindAllByName(this Transform trans, string sname, bool bIgnoreCase = false)
        {
            if (trans == null) return ArrayUtil.Empty<Transform>();

            using (var results = TempCollection.GetList<Transform>())
            {
                FindAllByName(trans, sname, results, bIgnoreCase);
                return results.ToArray();
            }
        }

        public static void FindAllByName(this Transform trans, string sname, ICollection<Transform> results, bool bIgnoreCase = false)
        {
            if (results == null) throw new System.ArgumentNullException("results");
            if (trans == null) return;

            using (var lst = TempCollection.GetList<Transform>())
            {
                trans.GetAllChildrenAndSelf(lst);
                var e = lst.GetEnumerator();
                while (e.MoveNext())
                {
                    if (StringUtil.Equals(e.Current.name, sname, bIgnoreCase))
                    {
                        results.Add(e.Current);
                    }
                }
            }
        }

        public static GameObject FindParentWithName(this GameObject go, string sname, bool bIgnoreCase = false)
        {
            if (go == null) return null;
            var result = FindParentWithName(go.transform, sname, bIgnoreCase);
            if (result != null)
                return result.gameObject;
            else
                return null;
        }

        public static Transform FindParentWithName(this Transform trans, string sname, bool bIgnoreCase = false)
        {
            var p = trans.parent;
            while (p != null)
            {
                if (StringUtil.Equals(p.name, sname, bIgnoreCase)) return p;
                p = p.parent;
            }
            return null;
        }

        #endregion

        #region Find By Tags

        /**
         * Find
         */

        public static GameObject[] FindGameObjectsWithMultiTag(string tag)
        {
            if (tag == SPConstants.TAG_MULTITAG)
            {
                return GameObject.FindGameObjectsWithTag(SPConstants.TAG_MULTITAG);
            }
            else
            {
                using (var tmp = TempList<GameObject>.GetList())
                {
                    foreach (var go in GameObject.FindGameObjectsWithTag(tag)) tmp.Add(go);

                    MultiTag.FindAll(tag, tmp);

                    return tmp.ToArray();
                }
            }
        }

        public static int FindGameObjectsWithMultiTag(string tag, ICollection<UnityEngine.GameObject> coll)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");

            int cnt = coll.Count;
            if (tag == SPConstants.TAG_MULTITAG)
            {
                coll.AddRange(GameObject.FindGameObjectsWithTag(SPConstants.TAG_MULTITAG));
            }
            else
            {
                foreach (var go in GameObject.FindGameObjectsWithTag(tag)) coll.Add(go);

                MultiTag.FindAll(tag, coll);
            }
            return coll.Count - cnt;
        }

        public static GameObject FindWithMultiTag(string tag)
        {
            if (tag == SPConstants.TAG_MULTITAG)
            {
                return GameObject.FindWithTag(SPConstants.TAG_MULTITAG);
            }
            else
            {
                var directHit = GameObject.FindWithTag(tag);
                if (directHit != null) return directHit;

                var comp = MultiTag.Find(tag);
                return (comp != null) ? comp.gameObject : null;
            }
        }

        public static GameObject FindWithMultiTag(this GameObject go, string tag)
        {
            if (MultiTagHelper.HasTag(go, tag)) return go;

            foreach (var child in go.transform.IterateAllChildren())
            {
                if (MultiTagHelper.HasTag(child.gameObject, tag)) return child.gameObject;
            }

            return null;
        }

        public static IEnumerable<GameObject> FindAllWithMultiTag(this GameObject go, string tag)
        {
            if (MultiTagHelper.HasTag(go)) yield return go;

            foreach (var child in go.transform.IterateAllChildren())
            {
                if (MultiTagHelper.HasTag(child.gameObject, tag)) yield return child.gameObject;
            }
        }

        /**
         * FindParentWithTag
         */

        public static GameObject FindParentWithTag(this GameObject go, string stag)
        {
            if (go == null) return null;
            return FindParentWithTag(go.transform, stag);
        }

        public static GameObject FindParentWithTag(this Component c, string stag)
        {
            if (c == null) return null;
            return FindParentWithTag(c.transform, stag);
        }

        public static GameObject FindParentWithTag(this Transform t, string stag)
        {
            while (t != null)
            {
                if (MultiTagHelper.HasTag(t, stag)) return t.gameObject;
                t = t.parent;
            }

            return null;
        }

        #endregion

        #region Parenting

        public static IEnumerable<Transform> IterateAllChildren(this Transform trans)
        {
            for(int i = 0; i < trans.childCount; i++)
            {
                yield return trans.GetChild(i);
            }
            
            for(int i = 0; i < trans.childCount; i++)
            {
                foreach (var c in IterateAllChildren(trans.GetChild(i)))
                    yield return c;
            }
        }

        public static Transform[] GetAllChildren(this GameObject go)
        {
            if (go == null) return null;
            return GetAllChildren(go.transform);
        }

        public static Transform[] GetAllChildren(this Component c)
        {
            if (c == null) return null;
            return GetAllChildren(c.transform);
        }

        public static Transform[] GetAllChildren(this Transform t)
        {
            using (var lst = TempCollection.GetList<Transform>())
            {
                GetAllChildren(t, lst);

                return lst.ToArray();
            }
        }

        public static void GetAllChildren(this Transform t, ICollection<Transform> coll)
        {
            if(coll is IList<Transform>)
            {
                GetAllChildren(t, coll as IList<Transform>);
            }
            else
            {
                using (var lst = TempCollection.GetList<Transform>())
                {
                    GetAllChildren(t, lst);
                    var e = lst.GetEnumerator();
                    while (e.MoveNext()) coll.Add(e.Current);
                }
            }
        }

        public static void GetAllChildren(this Transform t, IList<Transform> lst)
        {
            int i = lst.Count;
            
            for(int j = 0; j < t.childCount; j++)
            {
                lst.Add(t.GetChild(j));
            }

            while (i < lst.Count)
            {
                t = lst[i];
                for (int j = 0; j < t.childCount; j++)
                {
                    lst.Add(t.GetChild(j));
                }
                i++;
            }
        }

        public static Transform[] GetAllChildrenAndSelf(this GameObject go)
        {
            if (go == null) return null;
            return GetAllChildrenAndSelf(go.transform);
        }

        public static Transform[] GetAllChildrenAndSelf(this Component c)
        {
            if (c == null) return null;
            return GetAllChildrenAndSelf(c.transform);
        }

        public static Transform[] GetAllChildrenAndSelf(this Transform t)
        {
            using (var lst = TempCollection.GetList<Transform>())
            {
                GetAllChildrenAndSelf(t, lst);
                return lst.ToArray();
            }
        }

        public static void GetAllChildrenAndSelf(this Transform t, ICollection<Transform> coll)
        {
            if (coll is IList<Transform>)
            {
                GetAllChildrenAndSelf(t, coll as IList<Transform>);
            }
            else
            {
                using (var lst = TempCollection.GetList<Transform>())
                {
                    GetAllChildrenAndSelf(t, lst);
                    var e = lst.GetEnumerator();
                    while (e.MoveNext()) coll.Add(e.Current);
                }
            }
        }

        public static void GetAllChildrenAndSelf(this Transform t, IList<Transform> lst)
        {
            int i = lst.Count;
            lst.Add(t);

            while (i < lst.Count)
            {
                t = lst[i];
                for(int j = 0; j < t.childCount; j++)
                {
                    lst.Add(t.GetChild(j));
                }
                i++;
            }
        }

        public static IEnumerable<Transform> GetParents(this GameObject go, bool stopAtRoot = false)
        {
            if (go == null) return null;
            return GetParents(go.transform, stopAtRoot);
        }

        public static IEnumerable<Transform> GetParents(this Component c, bool stopAtRoot = false)
        {
            if (c == null) return null;
            return GetParents(c.transform, stopAtRoot);
        }

        public static IEnumerable<Transform> GetParents(this Transform t, bool stopAtRoot = false)
        {
            if (stopAtRoot)
            {
                if (t.HasTag(SPConstants.TAG_ROOT)) yield break;

                t = t.parent;
                while (t != null)
                {
                    yield return t;
                    if (t.HasTag(SPConstants.TAG_ROOT)) yield break;
                    t = t.parent;
                }
            }
            else
            {
                t = t.parent;
                while (t != null)
                {
                    yield return t;
                    t = t.parent;
                }
            }
        }

        // ##############
        // Is Parent
        // ##########

        public static bool IsParentOf(this GameObject parent, GameObject possibleChild)
        {
            //if (parent == null || possibleChild == null) return false;
            return IsParentOf(parent.transform, possibleChild.transform);
        }

        public static bool IsParentOf(this Transform parent, GameObject possibleChild)
        {
            //if (parent == null || possibleChild == null) return false;
            return IsParentOf(parent, possibleChild.transform);
        }

        public static bool IsParentOf(this GameObject parent, Transform possibleChild)
        {
            //if (parent == null || possibleChild == null) return false;
            return IsParentOf(parent.transform, possibleChild);
        }

        public static bool IsParentOf(this Transform parent, Transform possibleChild)
        {
            /*
             * Since implementation of this, Unity has since added 'IsChildOf' that is far superior in efficiency
             * 
            if (parent == null || possibleChild == null) return false;
            while (possibleChild != null)
            {
                if (parent == possibleChild.parent) return true;
                possibleChild = possibleChild.parent;
            }
            return false;
            */

            return possibleChild.IsChildOf(parent);
        }

        // ##############
        // Add Child
        // ##########

        /// <summary>
        /// Set the parent of some GameObject to this GameObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="child"></param>
        /// <param name="suppressChangeHierarchyMessage"></param>
        public static void AddChild(this GameObject obj, GameObject child)
        {
            var p = (obj != null) ? obj.transform : null;
            var t = (child != null) ? child.transform : null;
            AddChild(p, t);
        }
        
        /// <summary>
        /// Set the parent of some GameObject to this GameObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="child"></param>
        /// <param name="suppressChangeHierarchyMessage">Don't send the OnTransformHierarchyChanged message.</param>
        public static void AddChild(this GameObject obj, Transform child)
        {
            var p = (obj != null) ? obj.transform : null;
            AddChild(p, child);
        }

        /// <summary>
        /// Set the parent of some GameObject to this GameObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="child"></param>
        /// <param name="suppressChangeHierarchyMessage">Don't send the OnTransformHierarchyChanged message.</param>
        public static void AddChild(this Transform obj, GameObject child)
        {
            var t = (child != null) ? child.transform : null;
            AddChild(obj, t);
        }

        /// <summary>
        /// Set the parent of some GameObject to this GameObject.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="child"></param>
        public static void AddChild(this Transform obj, Transform child)
        {
            if (child == null) throw new System.ArgumentNullException("child");

            if (child.parent == obj) return;
            child.parent = obj;
        }

        /// <summary>
        /// Sets the parent property of this GameObject to null.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="suppressChangeHierarchyMessage">Don't send the OnTransformHierarchyChanged message.</param>
        public static void RemoveFromParent(this GameObject obj)
        {
            if (obj == null) throw new System.ArgumentNullException("obj");

            var t = obj.transform;
            if (t.parent == null) return;
            t.parent = null;
        }

        /// <summary>
        /// Sets the parent property of this GameObject to null.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="suppressChangeHierarchyMessage">Don't send the OnTransformHierarchyChanged message.</param>
        public static void RemoveFromParent(this Transform obj)
        {
            if (obj == null) throw new System.ArgumentNullException("t");

            if (obj.parent == null) return;
            obj.parent = null;
        }

#endregion
        
    }
}

