using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{

    public static class SearchUtil
    {

        #region Layer Methods

        public static IEnumerable<GameObject> FindGameObjectOnLayer(int mask)
        {
            var arr = GameObject.FindObjectsOfType(typeof(GameObject));

            foreach (GameObject go in arr)
            {
                if (((1 << go.layer) & mask) != 0) yield return go;
            }
        }

        public static IEnumerable<GameObject> FindGameObjectOnLayer(this GameObject go, int mask)
        {
            if (go == null) yield break;
            if (((1 << go.layer) & mask) != 0) yield return go;

            foreach (Transform child in go.transform.IterateAllChildren())
            {
                if (((1 << child.gameObject.layer) & mask) != 0) yield return child.gameObject;
            }
        }

        #endregion

        #region Search/Find

        public static GameObject Find(this GameObject go, string spath)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            var child = go.transform.Find(spath);
            return (child != null) ? child.gameObject : null;
        }

        /// <summary>
        /// Finds a gameobject based on some path. This works just like Transform.Find, but added a case sensitivity option.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="spath"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static GameObject Find(this GameObject go, string spath, bool bIgnoreCase)
        {
            var result = Find(go.transform, spath, bIgnoreCase);
            if (result != null)
                return result.gameObject;
            else
                return null;
        }

        /// <summary>
        /// Finds a gameobject based on some path. This works just like Transform.Find, but added a case sensitivity option.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="spath"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static Transform Find(this Transform trans, string spath, bool bIgnoreCase)
        {
            if (bIgnoreCase)
            {
                var arr = spath.Split('/');
                if (arr == null || arr.Length == 0) return null;

                foreach (string sname in arr)
                {
                    bool foundNext = false;
                    foreach (Transform child in trans)
                    {
                        if (StringUtil.Equals(sname, child.name, true))
                        {
                            foundNext = true;
                            trans = child;
                            break;
                        }
                    }
                    if (!foundNext) return null;
                }

                return trans;
            }
            else
            {
                return trans.Find(spath);
            }
        }

        /// <summary>
        /// Attempts to find a gameobject based on some name, if it isn't found a gameobject is created with the name and added as a child. 
        /// Note, this is unlike Transform.Find in that it only searches for direct children, and does not traverse the hierarchy.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static GameObject FindOrAddChild(this GameObject go, string name, bool bIgnoreCase)
        {
            if (go == null) throw new System.ArgumentNullException("go");

            GameObject child = (from Transform c in go.transform where StringUtil.Equals(c.name, name, bIgnoreCase) select c.gameObject).FirstOrDefault();

            if (child == null)
            {
                child = new GameObject(name);
                child.transform.parent = go.transform;
                child.transform.ZeroOut(false);
            }

            return child;
        }

        /// <summary>
        /// Attempts to find a gameobject based on some name, if it isn't found a gameobject is created with the name and added as a child. 
        /// Note, this is unlike Transform.Find in that it only searches for direct children, and does not traverse the hierarchy.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static Transform FindOrAddChild(this Transform trans, string name, bool bIgnoreCase)
        {
            if (trans == null) throw new System.ArgumentNullException("trans");

            Transform child = (from Transform c in trans where StringUtil.Equals(c.name, name, bIgnoreCase) select c).FirstOrDefault();

            if (child == null)
            {
                var childGo = new GameObject(name);
                child = childGo.transform;
                child.parent = trans;
                child.ZeroOut(false);
            }

            return child;
        }

        /// <summary>
        /// This is similar to Find, but allows for arbitrary path definitions using the '*'.
        /// </summary>
        /// <param name="go"></param>
        /// <param name="spath"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static GameObject Search(this GameObject go, string spath, bool bIgnoreCase = false)
        {
            var result = Search(go.transform, spath, bIgnoreCase);
            if (result != null)
                return result.gameObject;
            else
                return null;
        }

        /// <summary>
        /// This is similar to Find, but allows for arbitrary path definitions using the '*'.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="spath"></param>
        /// <param name="bIgnoreCase"></param>
        /// <returns></returns>
        public static Transform Search(this Transform trans, string spath, bool bIgnoreCase = false)
        {
            if (!spath.Contains("*"))
            {
                return trans.Find(spath, bIgnoreCase);
            }
            else
            {
                var arr = spath.Split('/');
                string sval;
                spath = "";

                for (int i = 0; i < arr.Length; i++)
                {
                    sval = arr[i];

                    if (sval == "*")
                    {
                        if (spath != "")
                        {
                            trans = trans.Find(spath, bIgnoreCase);
                            if (trans == null) return null;
                            spath = "";
                        }

                        i++;
                        if (i >= arr.Length)
                            return (trans.childCount > 0) ? trans.GetChild(0) : null;
                        else
                        {
                            sval = arr[i];
                            //now we're going to do our recursing search
                            trans = FindByName(trans, sval, bIgnoreCase);
                            if (trans == null) return null;
                        }
                    }
                    else
                    {
                        if (spath != "") spath += "/";
                        spath += sval;
                    }

                }

                return trans;
            }
        }

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

        public static string GetPathNameRelativeTo(this GameObject go, Transform parent)
        {
            if (go == null) return null;
            return GetPathNameRelativeTo(go.transform, parent);
        }

        public static string GetPathNameRelativeTo(this Transform t, Transform parent)
        {
            if (t == null) return null;
            if (t == parent) return null;

            var bldr = StringUtil.GetTempStringBuilder();
            bldr.Append(t.name);
            t = t.parent;
            while (t != null && t != parent)
            {
                bldr.Insert(0, '/');
                bldr.Insert(0, t.name);
            }
            return StringUtil.Release(bldr);
        }

        public static string GetFullPathName(this Transform t)
        {
            var builder = StringUtil.GetTempStringBuilder();
            while (t.parent != null)
            {
                t = t.parent;
                builder.Insert(0, @"\");
                builder.Insert(0, t.name);
            }
            return StringUtil.Release(builder);
        }

        #endregion

        #region Tags

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

                //MultiTag comp;
                //foreach (var go in GameObject.FindGameObjectsWithTag(SPConstants.TAG_MULTITAG))
                //{
                //    if (go.GetComponent<MultiTag>(out comp))
                //    {
                //        if (comp.ContainsTag(tag)) return go;
                //    }
                //}

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

    }

}
