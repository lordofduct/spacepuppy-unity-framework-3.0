#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public class i_PlaySoundEffect : AutoTriggerable
    {

        #region Fields

        [SerializeField()]
        [TriggerableTargetObject.Config(typeof(AudioSource))]
        private TriggerableTargetObject _targetAudioSource = new TriggerableTargetObject();

        [SerializeField()]
        [WeightedValueCollection("Weight", "Clip", ElementLabelFormatString = "Clip {0:00}")]
        [Tooltip("One or Many, if many they will be randomly selected by the weights supplied.")]
        private AudioClipEntry[] _clips;

        [SerializeField()]
        private AudioInterruptMode _interrupt = AudioInterruptMode.StopIfPlaying;

        [SerializeField()]
        [TimeUnitsSelector()]
        private float _delay;

        [Tooltip("Trigger something at the end of the sound effect. This is NOT perfectly accurate and really just starts a timer for the duration of the sound being played.")]
        [SerializeField()]
        private SPEvent _onAudioComplete;

        [System.NonSerialized()]
        private RadicalCoroutine _completeRoutine;

        #endregion

        #region CONSTRUCTOR

        #endregion

        #region Properties

        public TriggerableTargetObject TargetAudioSource
        {
            get { return _targetAudioSource; }
        }

        public AudioClipEntry[] Clips
        {
            get { return _clips; }
            set { _clips = value ?? ArrayUtil.Empty<AudioClipEntry>(); }
        }

        public AudioInterruptMode Interrupt
        {
            get { return _interrupt; }
            set { _interrupt = value; }
        }

        public float Delay
        {
            get { return _delay; }
        }

        public SPEvent OnAudioComplete
        {
            get { return _onAudioComplete; }
        }

        #endregion

        #region Methods

        private void OnAudioCompleteHandler()
        {
            _completeRoutine = null;
            _onAudioComplete.ActivateTrigger(this, null);
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool CanTrigger
        {
            get
            {
                return base.CanTrigger && _clips != null && _clips.Length > 0;
            }
        }

        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            var src = _targetAudioSource.GetTarget<AudioSource>(arg);
            if (src == null)
            {
                Debug.LogWarning("Failed to play audio due to a lack of AudioSource on the target.", this);
                return false;
            }
            if (src.isPlaying)
            {
                switch (this.Interrupt)
                {
                    case AudioInterruptMode.StopIfPlaying:
                        if (_completeRoutine != null) _completeRoutine.Cancel();
                        _completeRoutine = null;
                        src.Stop();
                        break;
                    case AudioInterruptMode.DoNotPlayIfPlaying:
                        return false;
                    case AudioInterruptMode.PlayOverExisting:
                        //play one shot over existing audio
                        break;
                }
            }
            
            AudioClip clip;
            if (_clips.Length == 0)
                return false;
            else if (_clips.Length == 1)
                clip = _clips[0].Clip;
            else
            {
                clip = _clips.PickRandom((e) => e.Weight).Clip;
            }


            if (clip != null)
            {
                if (this._delay > 0)
                {
                    this.Invoke(() =>
                    {
                        if (src != null)
                        {
                            _completeRoutine = this.InvokeRadical(this.OnAudioCompleteHandler, clip.length);
                            //src.Play();
                            src.PlayOneShot(clip);
                        }
                    }, this._delay);
                }
                else
                {
                    _completeRoutine = this.InvokeRadical(this.OnAudioCompleteHandler, clip.length);
                    //src.Play();
                    src.PlayOneShot(clip);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Special Types

        [System.Serializable]
        public struct AudioClipEntry
        {
            public float Weight;
            public AudioClip Clip;
        }

        #endregion

    }

}
