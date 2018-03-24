using UnityEngine;

namespace com.spacepuppy.Anim
{

    public interface ISPAnimator
    {
        
        void Play(string id, QueueMode queuMode = QueueMode.PlayNow, PlayMode playMode = PlayMode.StopSameLayer);

    }

    public class SPAnimatorMethodAttribute : System.Attribute
    {

    }

}
