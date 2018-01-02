#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras
{

    public class SPPostProcessingManager : ServiceComponent<IPostProcessingManager>, IPostProcessingManager
    {

        #region Fields

        [SerializeField]
        [TypeRestriction(typeof(IPostProcessingEffect))]
        private List<UnityEngine.Object> _globalEffects;

        [System.NonSerialized]
        private GlobalEffectsList _globalEffectsBindingList;
        [System.NonSerialized]
        private GlobalEffectCamera _globalEffectsCamera;

        #endregion

        #region CONSTRUCTOR

        protected override void OnValidAwake()
        {
            base.OnValidAwake();

            if (_globalEffects == null)
            {
                _globalEffects = new List<UnityEngine.Object>();
            }
            else
            {
                for (int i = 0; i < _globalEffects.Count; i++)
                {
                    if (_globalEffects[i] == null)
                    {
                        _globalEffects.RemoveAt(i);
                        i--;
                    }
                }
            }

            _globalEffectsBindingList = new GlobalEffectsList(this);
            if (_globalEffects.Count > 0) this.EffectsListChanged();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_globalEffectsCamera != null)
            {
                ObjUtil.SmartDestroy(_globalEffectsCamera.gameObject);
                ObjUtil.SmartDestroy(_globalEffectsCamera);
            }
        }

        #endregion

        #region Methods

        private void EffectsListChanged()
        {
            if (_globalEffects.Count == 0)
            {
                if (_globalEffectsCamera != null && _globalEffectsCamera.gameObject.activeSelf)
                {
                    _globalEffectsCamera.gameObject.SetActive(false);
                }
            }
            else
            {
                if (_globalEffectsCamera == null)
                {
                    var go = new GameObject("Spacepuppy.GlobalPostProcessingEffects");
                    UnityEngine.Object.DontDestroyOnLoad(go);

                    _globalEffectsCamera = go.AddComponent<GlobalEffectCamera>();
                    _globalEffectsCamera.Init(this);
                    go.SetActive(true);
                }
                else
                {
                    _globalEffectsCamera.enabled = true;
                    _globalEffectsCamera.gameObject.SetActive(true);
                }
            }
        }

        #endregion

        #region IPostProcessingManager Interface

        public IList<IPostProcessingEffect> GlobalEffects { get { return _globalEffectsBindingList; } }

        public bool ApplyGlobalPostProcessing(ICamera camera, RenderTexture source, RenderTexture destination)
        {
            if (_globalEffects.Count == 0) return false;

            if (_globalEffects.Count == 1)
            {
                var effect = _globalEffects[0] as IPostProcessingEffect;
                if (_globalEffects[0] == null || effect == null)
                {
                    _globalEffects.RemoveAt(0);
                    this.EffectsListChanged();
                    Graphics.Blit(source, destination);
                }
                else if (effect.enabled)
                {
                    effect.RenderImage(camera, source, destination);
                }
                else
                {
                    Graphics.Blit(source, destination);
                }
            }
            else
            {
                using (var set = com.spacepuppy.Collections.TempCollection.GetSet<RenderTexture>())
                {
                    var src = source;

                    for (int i = 0; i < _globalEffects.Count; i++)
                    {
                        var effect = _globalEffects[i] as IPostProcessingEffect;
                        if (_globalEffects[i] == null || effect == null)
                        {
                            _globalEffects.RemoveAt(i);
                            this.EffectsListChanged();
                            i--;
                        }
                        else if (effect.enabled)
                        {
                            var dest = RenderTexture.GetTemporary(src.width, src.height);
                            set.Add(dest);

                            effect.RenderImage(camera, src, dest);

                            src = dest;
                        }
                    }

                    Graphics.Blit(src, destination);

                    var e = set.GetEnumerator();
                    while (e.MoveNext())
                    {
                        RenderTexture.ReleaseTemporary(e.Current);
                    }
                }
            }

            return true;
        }

        #endregion

        #region Special Types

        private class GlobalEffectsList : IList<IPostProcessingEffect>
        {

            #region Fields

            private SPPostProcessingManager _owner;

            #endregion

            #region CONSTRUCTOR

            public GlobalEffectsList(SPPostProcessingManager owner)
            {
                _owner = owner;
            }

            #endregion

            #region Properties

            public int Count
            {
                get { return _owner._globalEffects.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public IPostProcessingEffect this[int index]
            {
                get { return _owner._globalEffects[index] as IPostProcessingEffect; }
                set
                {
                    if (value == null) throw new System.ArgumentNullException("value");
                    var obj = ObjUtil.GetAsFromSource<UnityEngine.Object>(value);
                    if (obj == null) throw new System.ArgumentException("value must be a UnityEngine.Object", "value");
                    _owner._globalEffects[index] = obj;
                }
            }

            #endregion

            #region Methods

            public void Add(IPostProcessingEffect item)
            {
                if (item == null) throw new System.ArgumentNullException("item");
                var obj = ObjUtil.GetAsFromSource<UnityEngine.Object>(item);
                if (obj == null) throw new System.ArgumentException("item must be a UnityEngine.Object", "value");

                _owner._globalEffects.Add(obj);
                _owner.EffectsListChanged();
            }

            public void Insert(int index, IPostProcessingEffect item)
            {
                if (item == null) throw new System.ArgumentNullException("item");
                var obj = ObjUtil.GetAsFromSource<UnityEngine.Object>(item);
                if (obj == null) throw new System.ArgumentException("item must be a UnityEngine.Object", "value");

                _owner._globalEffects.Insert(index, obj);
                _owner.EffectsListChanged();
            }

            public bool Remove(IPostProcessingEffect item)
            {
                var obj = ObjUtil.GetAsFromSource<UnityEngine.Object>(item);
                if (obj == null) return false;

                if (_owner._globalEffects.Remove(obj))
                {
                    if (_owner._globalEffects.Count == 0)
                    {
                        _owner.EffectsListChanged();
                    }
                    return true;
                }

                return false;
            }

            public void RemoveAt(int index)
            {
                _owner._globalEffects.RemoveAt(index);
                if (_owner._globalEffects.Count == 0)
                {
                    _owner.EffectsListChanged();
                }
            }

            public void Clear()
            {
                if (_owner._globalEffects.Count == 0) return;

                _owner._globalEffects.Clear();
                _owner.EffectsListChanged();
            }

            public bool Contains(IPostProcessingEffect item)
            {
                return _owner._globalEffects.Contains(item);
            }

            public void CopyTo(IPostProcessingEffect[] array, int arrayIndex)
            {
                if (array == null) throw new System.ArgumentNullException("array");

                for (int i = 0; i < _owner._globalEffects.Count; i++)
                {
                    int j = i + arrayIndex;
                    if (j >= array.Length) return;

                    array[j] = _owner._globalEffects[i] as IPostProcessingEffect;
                }
            }

            public int IndexOf(IPostProcessingEffect item)
            {
                var obj = ObjUtil.GetAsFromSource<UnityEngine.Object>(item);
                if (obj == null) return -1;

                return _owner._globalEffects.IndexOf(obj);
            }

            public IEnumerator<IPostProcessingEffect> GetEnumerator()
            {
                for (int i = 0; i < _owner._globalEffects.Count; i++)
                {
                    yield return _owner._globalEffects[i] as IPostProcessingEffect;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _owner._globalEffects.GetEnumerator();
            }

            #endregion

        }

        private class GlobalEffectCamera : MonoBehaviour, ICamera
        {

            #region Fields

            private SPPostProcessingManager _manager;
            private Camera _camera;

            #endregion

            #region CONSTRUCTOR

            private void Awake()
            {
                _camera = this.AddOrGetComponent<Camera>();
                _camera.clearFlags = CameraClearFlags.Nothing;
                _camera.cullingMask = 0;
                _camera.depth = float.MaxValue;
            }

            public void Init(SPPostProcessingManager manager)
            {
                _manager = manager;
            }

            private void OnEnable()
            {
                _camera.enabled = true;
            }

            #endregion

            #region Methods

            private void OnRenderImage(RenderTexture source, RenderTexture destination)
            {
                if (_manager == null) return;

                if (_manager._globalEffects.Count == 0)
                {
                    _manager.EffectsListChanged();
                    return;
                }

                _manager.ApplyGlobalPostProcessing(this, source, destination);
            }

            #endregion

            #region ICamera Interface

            CameraCategory ICamera.Category
            {
                get { return CameraCategory.Other; }
                set { }
            }

            bool ICamera.IsAlive
            {
                get { return this.gameObject != null && this.gameObject.activeInHierarchy; }
            }

            Camera ICamera.camera
            {
                get { return _camera; }
            }

            bool ICamera.Contains(Camera cam)
            {
                return !object.ReferenceEquals(cam, null) && object.ReferenceEquals(cam, _camera);
            }

            #endregion

        }

        #endregion

    }

}
