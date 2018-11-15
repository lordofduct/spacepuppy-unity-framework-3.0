using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.Anim.Events
{

    [System.Serializable]
    public class PlayAnimInfo
    {

        #region Fields

        [SerializeField]
        public float Weight;
        [SerializeField]
        private i_PlayAnimation.PlayByMode _mode;
        [SerializeField]
        private string _id;
        [SerializeField]
        private UnityEngine.Object _clip;
        [SerializeField]
        public AnimSettingsMask SettingsMask;
        [SerializeField]
        public AnimSettings Settings = AnimSettings.Default;
        [SerializeField]
        public QueueMode QueueMode = QueueMode.PlayNow;
        [SerializeField]
        public PlayMode PlayMode = PlayMode.StopSameLayer;
        [SerializeField]
        public float CrossFadeDur = 0f;

        #endregion

        #region Properties

        public i_PlayAnimation.PlayByMode Mode
        {
            get { return _mode; }
        }

        public string Id
        {
            get { return _id; }
        }

        public UnityEngine.Object Clip
        {
            get { return _clip; }
        }

        #endregion

        #region Methods

        public void Configure(AnimationClip clip)
        {
            _mode = i_PlayAnimation.PlayByMode.PlayAnim;
            _id = null;
            _clip = clip;
        }

        public void Configure(IScriptableAnimationClip clip)
        {
            _mode = i_PlayAnimation.PlayByMode.PlayAnim;
            _id = null;
            _clip = clip as UnityEngine.Object;
        }

        public void Configure(string animId)
        {
            _mode = i_PlayAnimation.PlayByMode.PlayAnimByID;
            _id = animId;
            _clip = null;
        }

        public void ConfigureAsResource(string resourceId)
        {
            _mode = i_PlayAnimation.PlayByMode.PlayAnimFromResource;
            _id = resourceId;
            _clip = null;
        }

        #endregion

    }

}
