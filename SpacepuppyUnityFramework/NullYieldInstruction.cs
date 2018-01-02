

namespace com.spacepuppy
{

    /// <summary>
    /// An object representation of a null yield instruction. 
    /// A null yield instruction has object identity while still informing RadicalCoroutine to wait only a single tick (just like null).
    /// </summary>
    public class NullYieldInstruction : IProgressingYieldInstruction, IRadicalWaitHandle
    {

        #region COSNTRUCTOR

        private NullYieldInstruction()
        {

        }

        #endregion

        #region IRadicalYieldInstruction Interface

        bool IRadicalYieldInstruction.IsComplete
        {
            get { return true; }
        }

        bool IRadicalYieldInstruction.Tick(out object yieldObject)
        {
            yieldObject = null;
            return false;
        }

        float IProgressingYieldInstruction.Progress
        {
            get { return 1f; }
        }

        void IRadicalWaitHandle.OnComplete(System.Action<IRadicalWaitHandle> callback)
        {
            //do nothing
        }

        bool IRadicalWaitHandle.Cancelled
        {
            get { return false; }
        }

        #endregion

        #region Static Interface

        private static NullYieldInstruction _null = new NullYieldInstruction();
        public static NullYieldInstruction Null
        {
            get
            {
                return _null;
            }
        }

        #endregion

    }

}
