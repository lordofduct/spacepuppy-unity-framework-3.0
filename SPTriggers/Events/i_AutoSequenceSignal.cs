using UnityEngine;

namespace com.spacepuppy.Events
{

    public class i_AutoSequenceSignal : AutoTriggerable, IAutoSequenceSignal
    {

        private RadicalWaitHandle _handle;


        public IRadicalWaitHandle Wait()
        {
            if (_handle == null) _handle = RadicalWaitHandle.Create();
            return _handle;
        }


        public override bool Trigger(object sender, object arg)
        {
            if (!this.CanTrigger) return false;

            if (_handle != null)
            {
                _handle.SignalComplete();
                _handle = null;
            }

            return true;
        }

    }

}
