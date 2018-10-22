#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Linq;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    [Infobox("This acts like a switch/case for enums. Default is what is called if none of the cases match.\nNote that 'MatchFlagExactly' does not like 'Everything' options.")]
    public class i_TriggerOnEnumState : AutoTriggerable, IObservableTrigger
    {

        public enum EnumFlagTestModes
        {
            NoFlag,
            MatchAnyFlag,
            MatchFlagExactly
        }

        #region Fields

        [SerializeField]
        private VariantReference _target = new VariantReference(VariantReference.RefMode.Property);

        [SerializeField()]
        private ConditionBlock[] _conditions;
        [SerializeField]
        private SPEvent _defaultCondition;

        [SerializeField]
        private bool _passAlongTriggerArg;

        #endregion

        #region Properties

        public VariantReference Target
        {
            get { return _target; }
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (_conditions == null || _conditions.Length == 0) return false;

            int st = _target.IntValue;

            foreach (var c in _conditions)
            {
                if (c == null) continue;

                if (c.TestState(st))
                {
                    c.Trigger.ActivateTrigger(this, _passAlongTriggerArg ? arg : null);
                    return true;
                }
            }

            _defaultCondition.ActivateTrigger(this, _passAlongTriggerArg ? arg : null);
            return true;
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        public class ConditionBlock
        {

            [SerializeField]
            private int _value;
            [SerializeField]
            private EnumFlagTestModes _enumFlags;
            [SerializeField()]
            private SPEvent _trigger = new SPEvent();


            public int Value
            {
                get { return _value; }
                set { _value = value; }
            }

            public EnumFlagTestModes EnumFlags
            {
                get { return _enumFlags; }
                set { _enumFlags = value; }
            }

            public SPEvent Trigger
            {
                get { return _trigger; }
            }

            public bool TestState(int st)
            {
                switch (_enumFlags)
                {
                    case EnumFlagTestModes.NoFlag:
                        return st == _value;
                    case EnumFlagTestModes.MatchAnyFlag:
                        return (st & _value) != 0;
                    case EnumFlagTestModes.MatchFlagExactly:
                        return st == _value;
                }

                return false;
            }

        }

        #endregion

        #region IObservableTrigger Interface

        BaseSPEvent[] IObservableTrigger.GetEvents()
        {
            return (from c in _conditions select c.Trigger).Append(_defaultCondition).ToArray();
        }

        #endregion

    }

}
