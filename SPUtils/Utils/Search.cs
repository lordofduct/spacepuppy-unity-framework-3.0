using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Collections;

namespace com.spacepuppy.Utils
{

    public static class Search
    {
        
        #region Find By Path/Name

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
        public static GameObject Query(this GameObject go, string spath, bool bIgnoreCase = false)
        {
            var result = Query(go.transform, spath, bIgnoreCase);
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
        public static Transform Query(this Transform trans, string spath, bool bIgnoreCase = false)
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
                            trans = GameObjectUtil.FindByName(trans, sval, bIgnoreCase);
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

        #endregion
        
        #region Find By Layer

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

    }

}
