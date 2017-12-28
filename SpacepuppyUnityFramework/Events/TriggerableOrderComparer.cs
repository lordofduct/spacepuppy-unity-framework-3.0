using System;

namespace com.spacepuppy.Events
{
    public class TriggerableOrderComparer : System.Collections.IComparer, System.Collections.Generic.IComparer<ITriggerable>
    {

        private static TriggerableOrderComparer _default;
        public static TriggerableOrderComparer Default
        {
            get
            {
                if (_default == null) _default = new TriggerableOrderComparer();
                return _default;
            }
        }

        int System.Collections.IComparer.Compare(object x, object y)
        {
            ITriggerable a = x as ITriggerable;
            ITriggerable b = y as ITriggerable;
            if (a == null) return b == null ? 0 : -1;
            if (b == null) return -1;
            return a.Order.CompareTo(b.Order);
        }

        public int Compare(ITriggerable x, ITriggerable y)
        {
            if (x == null) return y == null ? 0 : -1;
            if (y == null) return -1;
            return x.Order.CompareTo(y.Order);
        }
    }
}
