using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace com.spacepuppy.Project
{

    /// <summary>
    /// An interface that represents a bundle of resources that can be loaded on demand. This facilitates wrappers 
    /// around the global 'Resources' (see: ResourceAssetBundle), portions of 'Resources' (see: ResourcePackage), 
    /// 'AssetBundle' (see: AssetBundlePackage), as well as groups of bundles (see: AssetBundleGroup).
    /// </summary>
    public interface IAssetBundle : System.IDisposable
    {

        IEnumerable<string> GetAllAssetNames();

        bool Contains(string name);

        bool Contains(UnityEngine.Object asset);

        UnityEngine.Object LoadAsset(string name);
        UnityEngine.Object LoadAsset(string name, System.Type tp);

        T LoadAsset<T>(string name) where T : UnityEngine.Object;

        void UnloadAsset(UnityEngine.Object asset);
        void UnloadAllAssets();

    }

    /// <summary>
    /// A wrapper around the global 'Resources' class so it can be used as an IAssetBundle.
    /// </summary>
    public sealed class ResourcesAssetBundle : IAssetBundle
    {

        #region Singleton Interface

        private static ResourcesAssetBundle _instance;
        public static ResourcesAssetBundle Instance
        {
            get
            {
                if (_instance == null) _instance = new ResourcesAssetBundle();
                return _instance;
            }
        }

        #endregion

        #region Fields

        #endregion

        #region CONSTRUCTOR

        private ResourcesAssetBundle()
        {
            //enforce as singleton
        }

        #endregion

        #region Methods

        IEnumerable<string> IAssetBundle.GetAllAssetNames()
        {
            return Enumerable.Empty<string>();
        }

        public bool Contains(string path)
        {
            //there's no way to test it, so we assume true
            return true;
        }

        public bool Contains(UnityEngine.Object asset)
        {
            //there's no way to test it, so we assume true
            return true;
        }

        public UnityEngine.Object LoadAsset(string path)
        {
            return Resources.Load(path);
        }

        public UnityEngine.Object LoadAsset(string path, System.Type tp)
        {
            return Resources.Load(path, tp);
        }

        public T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load(path, typeof(T)) as T;
        }

        public void UnloadAsset(UnityEngine.Object asset)
        {
            if (asset is GameObject || asset is Component)
            {
                //UnityEngine.Object.Destroy(asset);
                return;
            }
            Resources.UnloadAsset(asset);
        }

        public void UnloadAllAssets()
        {
            //technically this doesn't act the same as LoadedAssetBundle, it only unloads ununsed assets
            Resources.UnloadUnusedAssets();
        }

        #endregion

        #region IDisposable Interface

        public void Dispose()
        {
            this.UnloadAllAssets();
        }

        #endregion


        #region Equality Overrides

        public override int GetHashCode()
        {
            return 1;
        }

        #endregion

    }

}
