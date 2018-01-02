using UnityEngine;

namespace com.spacepuppy.Events
{

    public class i_QuitApplication : AutoTriggerable
    {
        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            GameLoop.QuitApplication();
            return GameLoop.QuitState > QuitState.None;
        }
    }

}
