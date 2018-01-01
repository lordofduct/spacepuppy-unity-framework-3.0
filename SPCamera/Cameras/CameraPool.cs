using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras
{

    public static class CameraPool
    {

        #region Static Interface

        private static HashSet<ICamera> _cameras = new HashSet<ICamera>();

        #endregion

        #region Events

        public static event System.EventHandler<CameraRegistrationEvent> CameraRegistered;
        public static event System.EventHandler<CameraRegistrationEvent> CameraUnregistered;

        #endregion

        #region Methods

        /// <summary>
        /// Camera considered the main currently. This is the first camera found with 'MainCamera' tag, if not manually set.
        /// </summary>
        public static ICamera Main
        {
            get
            {
                var manager = Services.Get<ICameraManager>();
                if (manager != null)
                    return manager.Main;

                var cam = Camera.main;
                if (cam != null)
                    return cam.AddOrGetComponent<UnityCamera>();

                return null;
            }
            set
            {
                var manager = Services.Get<ICameraManager>();
                if (manager != null)
                    manager.Main = value;
            }
        }

        public static void Register(ICamera cam)
        {
            if (cam == null) throw new System.ArgumentNullException("cam");
            if (GameLoop.ApplicationClosing) return;
            if (_cameras.Add(cam))
            {
                var e = CameraRegistered;
                if (e != null)
                {
                    var ev = CameraRegistrationEvent.GetTemp(cam);
                    e(null, ev);
                }
            }
        }


        public static void Unregister(ICamera cam)
        {
            if (cam == null) throw new System.ArgumentNullException("cam");
            if (GameLoop.ApplicationClosing) return;
            if (_cameras.Remove(cam))
            {
                var e = CameraUnregistered;
                if (e != null)
                {
                    var ev = CameraRegistrationEvent.GetTemp(cam);
                    e(null, ev);
                }
            }
        }

        public static IEnumerable<ICamera> All
        {
            get
            {
                return _cameras;
            }
        }

        public static IEnumerable<ICamera> Group(string tag)
        {
            var e = _cameras.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.gameObject.HasTag(tag)) yield return e.Current;
            }
        }


        public static ICamera[] FindAllCameraControllers(System.Func<ICamera, bool> predicate = null)
        {
            using (var coll = com.spacepuppy.Collections.TempCollection.GetCollection<ICamera>())
            {
                FindAllCameraControllers(coll, predicate);
                return coll.ToArray();
            }
        }

        public static void FindAllCameraControllers(System.Collections.Generic.ICollection<ICamera> coll, System.Func<ICamera, bool> predicate = null)
        {
            if (coll == null) throw new System.ArgumentNullException("coll");

            if (predicate == null)
            {
                var e = _cameras.GetEnumerator();
                while (e.MoveNext())
                {
                    coll.Add(e.Current);
                }

                var ucams = Camera.allCameras;
                foreach (var c in ucams)
                {
                    coll.Add(c.AddOrGetComponent<UnityCamera>());
                }
            }
            else
            {
                var e = _cameras.GetEnumerator();
                while (e.MoveNext())
                {
                    if (predicate(e.Current)) coll.Add(e.Current);
                }

                var ucams = Camera.allCameras;
                foreach (var c in ucams)
                {
                    var uc = c.AddOrGetComponent<UnityCamera>();
                    if (predicate(uc)) coll.Add(uc);
                }
            }
        }

        public static ICamera FindCameraController(System.Func<ICamera, bool> predicate)
        {
            var e = _cameras.GetEnumerator();
            while (e.MoveNext())
            {
                if (predicate(e.Current)) return e.Current;
            }

            return null;
        }

        public static ICamera FindCameraController(Camera cam)
        {
            var e = _cameras.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.Contains(cam)) return e.Current;
            }

            return cam.AddOrGetComponent<UnityCamera>();
        }

        public static ICamera FindTaggedMainCamera()
        {
            var e = _cameras.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current.gameObject.HasTag(SPConstants.TAG_MAINCAMERA))
                {
                    return e.Current;
                }
            }

            var cams = Camera.allCameras;
            foreach (var cam in cams)
            {
                if (cam.HasTag(SPConstants.TAG_MAINCAMERA))
                {
                    return cam.AddOrGetComponent<UnityCamera>();
                }
            }

            return null;
        }


        public static Enumerator GetEnumerator()
        {
            return new Enumerator(_cameras);
        }

        #endregion

        #region Special Types

        public struct Enumerator : IEnumerator<ICamera>
        {

            #region Fields

            private HashSet<ICamera>.Enumerator _e;

            #endregion

            #region CONSTRUCTOR

            internal Enumerator(HashSet<ICamera> set)
            {
                _e = set.GetEnumerator();
            }

            #endregion

            #region IEnumerator Interface

            public ICamera Current
            {
                get
                {
                    return _e.Current;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return _e.Current;
                }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void System.Collections.IEnumerator.Reset()
            {
                (_e as System.Collections.IEnumerator).Reset();
            }

            public void Dispose()
            {
                _e.Dispose();
            }

            #endregion

        }

        #endregion

    }

}
