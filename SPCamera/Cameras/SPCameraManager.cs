using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Cameras
{

    public class SPCameraManager : ServiceComponent<ICameraManager>, ICameraManager
    {

        #region Fields

        private ICamera _main;
        private bool _overrideAsNull;

        #endregion

        #region CONSTRUCTOR

        protected override void OnValidAwake()
        {
            base.OnValidAwake();

            if (this.MainNeedsSyncing())
            {
                this.ForceSyncTaggedMainCamera();
            }

            CameraPool.CameraRegistered += OnRegistered;
            CameraPool.CameraUnregistered += OnUnregistered;
            SceneManager.sceneLoaded += this.OnSceneWasLoaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            CameraPool.CameraRegistered -= OnRegistered;
            CameraPool.CameraUnregistered -= OnUnregistered;
            SceneManager.sceneLoaded -= this.OnSceneWasLoaded;
        }

        private void OnSceneWasLoaded(Scene sc, LoadSceneMode mode)
        {
            if (this.MainNeedsSyncing())
            {
                this.ForceSyncTaggedMainCamera();
            }
        }

        #endregion

        #region Methods

        public void ForceSyncTaggedMainCamera()
        {
            _main = CameraPool.FindTaggedMainCamera();
            _overrideAsNull = false;
        }

        private bool MainNeedsSyncing()
        {
            if (_main == null)
                return !_overrideAsNull;
            else
                return !_main.IsAlive;
        }

        private bool AnyNonUnityCamerasContains(Camera c)
        {
            var e = CameraPool.GetEnumerator();
            while (e.MoveNext())
            {
                if (!(e.Current is UnityCamera) && e.Current.Contains(c)) return true;
            }
            return false;
        }

        #endregion

        #region Event Handlers

        private void OnRegistered(object sender, CameraRegistrationEvent e)
        {
            if (this.started && this.MainNeedsSyncing())
            {
                this.ForceSyncTaggedMainCamera();
            }
        }

        private void OnUnregistered(object sender, CameraRegistrationEvent e)
        {
            var cam = e.Camera;
            if (_main == cam)
            {
                _main = null;
                if (this.started && !GameLoop.ApplicationClosing)
                {
                    this.ForceSyncTaggedMainCamera();
                }
            }
        }

        #endregion

        #region ICameraManager Interface

        public ICamera Main
        {
            get
            {
                if (this.MainNeedsSyncing()) this.ForceSyncTaggedMainCamera();
                return _main;
            }
            set
            {
                _main = value;
                _overrideAsNull = (value == null);
            }
        }

        #endregion

    }

}
