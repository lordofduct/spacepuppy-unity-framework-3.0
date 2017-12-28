using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    [System.Serializable()]
    public class SPEvent : ICollection<EventTriggerTarget>
    {

        public const string ID_DEFAULT = "Trigger";

        #region Events

        public event System.EventHandler<TempEventArgs> TriggerActivated;
        protected virtual void OnTriggerActivated(object sender, object arg)
        {
            if (TriggerActivated != null)
            {
                var e = TempEventArgs.Create(arg);
                TriggerActivated(sender, e);
                TempEventArgs.Release(e);
            }

            //if(_owner != null)
            //{
            //    _owner.PostNotification<TriggerActivatedNotification>(new TriggerActivatedNotification(_owner, _id), false);
            //}
        }

        #endregion

        #region Fields

        [SerializeField()]
        private bool _yield;

        [SerializeField()]
        private List<EventTriggerTarget> _targets = new List<EventTriggerTarget>();

        [System.NonSerialized()]
        private string _id;

        #endregion

        #region CONSTRUCTOR

        public SPEvent()
        {
            _id = ID_DEFAULT;
        }

        public SPEvent(string id)
        {
            _id = id;
        }

        public SPEvent(bool yielding)
        {
            _id = ID_DEFAULT;
            _yield = yielding;
        }

        public SPEvent(string id, bool yielding)
        {
            _id = id;
            _yield = yielding;
        }

        #endregion

        #region Properties

        public bool Yielding
        {
            get { return _yield; }
            set { _yield = value; }
        }

        public string ObservableTriggerId
        {
            get { return _id; }
            set { _id = value; }
        }

        public List<EventTriggerTarget> Targets
        {
            get { return _targets; }
        }

        /// <summary>
        /// Count is total count of targets including the TriggerActivated event. 
        /// Check the count of 'Targets' for the direct targets only.
        /// </summary>
        public int Count
        {
            get
            {
                if (this.TriggerActivated != null)
                    return _targets.Count + 1;
                else
                    return _targets.Count;
            }
        }

        #endregion

        #region Methods

        public EventTriggerTarget AddNew()
        {
            var targ = new EventTriggerTarget();
            _targets.Add(targ);
            return targ;
        }

        public void ActivateTrigger(object sender, object arg)
        {
            if (_targets.Count > 0)
            {
                var e = _targets.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current != null) e.Current.Trigger(sender, arg);
                }
            }

            this.OnTriggerActivated(sender, arg);

        }

        public void ActivateTriggerAt(int index, object sender, object arg)
        {
            if (index >= 0 && index < _targets.Count)
            {
                EventTriggerTarget trig = _targets[index];
                if (trig != null) trig.Trigger(sender, arg);
            }

            this.OnTriggerActivated(sender, arg);
        }

        public void ActivateRandomTrigger(object sender, object arg, bool considerWeights)
        {
            if (_targets.Count > 0)
            {
                EventTriggerTarget trig = (considerWeights) ? _targets.PickRandom((t) => { return t.Weight; }) : _targets.PickRandom();
                if (trig != null) trig.Trigger(sender, arg);
            }

            this.OnTriggerActivated(sender, arg);
        }
        
        #endregion

        #region ICollection Interface

        public void Add(EventTriggerTarget item)
        {
            _targets.Add(item);
        }

        public void Clear()
        {
            _targets.Clear();
        }

        public bool Contains(EventTriggerTarget item)
        {
            return _targets.Contains(item);
        }

        public void CopyTo(EventTriggerTarget[] array, int arrayIndex)
        {
            _targets.CopyTo(array, arrayIndex);
        }

        int ICollection<EventTriggerTarget>.Count
        {
            get
            {
                return _targets.Count;
            }
        }

        bool ICollection<EventTriggerTarget>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(EventTriggerTarget item)
        {
            return _targets.Remove(item);
        }

        public Enumerator GetEnumerator()
        {
            //return _targets.GetEnumerator();
            return new Enumerator(this);
        }

        IEnumerator<EventTriggerTarget> IEnumerable<EventTriggerTarget>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion


        #region Special Types

        public class ConfigAttribute : System.Attribute
        {
            public bool Weighted;
            public bool AlwaysExpanded;

            public ConfigAttribute()
            {

            }

        }

        public struct Enumerator : IEnumerator<EventTriggerTarget>
        {

            private List<EventTriggerTarget>.Enumerator _e;

            public Enumerator(SPEvent t)
            {
                _e = t._targets.GetEnumerator();
            }

            public EventTriggerTarget Current
            {
                get { return _e.Current; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return _e.Current; }
            }

            public bool MoveNext()
            {
                return _e.MoveNext();
            }

            void System.Collections.IEnumerator.Reset()
            {
                (_e as IEnumerator<EventTriggerTarget>).Reset();
            }

            public void Dispose()
            {
                _e.Dispose();
            }

        }

        #endregion

    }

}
