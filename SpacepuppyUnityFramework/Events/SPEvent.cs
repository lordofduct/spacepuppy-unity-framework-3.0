using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;
using System;

namespace com.spacepuppy.Events
{

    [System.Serializable()]
    public class BaseSPEvent : ICollection<EventTriggerTarget>
    {

        public const string ID_DEFAULT = "Trigger";

        #region Events

        protected System.EventHandler<TempEventArgs> _triggerActivated;
        public event System.EventHandler<TempEventArgs> TriggerActivated
        {
            add
            {
                _triggerActivated += value;
            }
            remove
            {
                _triggerActivated -= value;
            }
        }
        protected virtual void OnTriggerActivated(object sender, object arg)
        {
            if (_triggerActivated != null)
            {
                var e = TempEventArgs.Create(arg);
                var d = _triggerActivated;
                d(sender, e);
                TempEventArgs.Release(e);
            }
        }

        #endregion

        #region Fields

        [SerializeField()]
        protected readonly List<EventTriggerTarget> _targets = new List<EventTriggerTarget>();

        [System.NonSerialized()]
        private string _id;

        #endregion

        #region CONSTRUCTOR

        public BaseSPEvent()
        {
            _id = ID_DEFAULT;
        }

        public BaseSPEvent(string id)
        {
            _id = id;
        }
        
        #endregion

        #region Properties
        
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
        public virtual int Count
        {
            get
            {
                if (_triggerActivated != null)
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

        protected void ActivateTrigger(object sender, object arg)
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

        protected void ActivateTriggerAt(int index, object sender, object arg)
        {
            if (index >= 0 && index < _targets.Count)
            {
                EventTriggerTarget trig = _targets[index];
                if (trig != null) trig.Trigger(sender, arg);
            }

            this.OnTriggerActivated(sender, arg);
        }

        protected void ActivateRandomTrigger(object sender, object arg, bool considerWeights)
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

        public struct Enumerator : IEnumerator<EventTriggerTarget>
        {

            private List<EventTriggerTarget>.Enumerator _e;

            public Enumerator(BaseSPEvent t)
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

    public class SPEvent : BaseSPEvent
    {
        
        #region CONSTRUCTOR

        public SPEvent()
        {
        }

        public SPEvent(string id) : base(id)
        {
        }

        #endregion

        #region Methods
        
        public new void ActivateTrigger(object sender, object arg)
        {
            base.ActivateTrigger(sender, arg);
        }

        public new void ActivateTriggerAt(int index, object sender, object arg)
        {
            base.ActivateTriggerAt(index, sender, arg);
        }

        public new void ActivateRandomTrigger(object sender, object arg, bool considerWeights)
        {
            base.ActivateRandomTrigger(sender, arg, considerWeights);
        }

        #endregion

        #region Special Types
        
        /*
         * This may be defined here, it is still usable on all types inheriting from BaseSPEvent. It's only here for namespace purposes to be consistent across the framework.
         */
        public class ConfigAttribute : System.Attribute
        {
            public bool Weighted;
            public bool AlwaysExpanded;

            public ConfigAttribute()
            {

            }

        }

        #endregion

    }

    [System.Serializable()]
    public class SPEvent<T> : BaseSPEvent where T : System.EventArgs
    {

        #region Events

        public new event System.EventHandler<T> TriggerActivated;
        protected virtual void OnTriggerActivated(object sender, T e)
        {
            if (TriggerActivated != null)
            {
                var d = this.TriggerActivated;
                d(sender, e);
            }
        }

        #endregion

        #region CONSTRUCTOR

        public SPEvent()
        {
        }

        public SPEvent(string id) : base(id)
        {
        }

        #endregion

        #region Methods

        public override int Count
        {
            get
            {
                if (this.TriggerActivated != null || _triggerActivated != null)
                    return _targets.Count + 1;
                else
                    return _targets.Count;
            }
        }
        
        public void ActivateTrigger(object sender, T arg)
        {
            base.ActivateTrigger(sender, arg);
            this.OnTriggerActivated(sender, arg);
        }

        public void ActivateTriggerAt(int index, object sender, T arg)
        {
            base.ActivateTriggerAt(index, sender, arg);
            this.OnTriggerActivated(sender, arg);
        }

        public void ActivateRandomTrigger(object sender, T arg, bool considerWeights)
        {
            base.ActivateRandomTrigger(sender, arg, considerWeights);
            this.OnTriggerActivated(sender, arg);
        }

        #endregion
        
    }

    [System.Serializable()]
    public class SPActionEvent<T> : BaseSPEvent
    {

        #region Events

        private System.Action<T> _callback;
        private System.Action<object, T> _evCallback;
        protected virtual void OnTriggerActivated(object sender, T arg)
        {
            if(_callback != null)
            {
                var c = _callback;
                c(arg);
            }

            if(_evCallback != null)
            {
                var c = _evCallback;
                c(sender, arg);
            }
        }

        #endregion

        #region CONSTRUCTOR

        public SPActionEvent()
        {
        }

        public SPActionEvent(string id) : base(id)
        {
        }

        #endregion

        #region Methods

        public override int Count
        {
            get
            {
                if (_callback != null || _evCallback != null || _triggerActivated != null)
                    return _targets.Count + 1;
                else
                    return _targets.Count;
            }
        }

        public void AddListener(System.Action<T> callback)
        {
            _callback += callback;
        }

        public void AddListener(System.Action<object, T> callback)
        {
            _evCallback += callback;
        }

        public void RemoveListener(System.Action<T> callback)
        {
            _callback -= callback;
        }

        public void RemoveListener(System.Action<object, T> callback)
        {
            _evCallback -= callback;
        }

        public void ActivateTrigger(object sender, T arg)
        {
            base.ActivateTrigger(sender, arg);
            this.OnTriggerActivated(sender, arg);
        }

        public void ActivateTriggerAt(int index, object sender, T arg)
        {
            base.ActivateTriggerAt(index, sender, arg);
            this.OnTriggerActivated(sender, arg);
        }

        public void ActivateRandomTrigger(object sender, T arg, bool considerWeights)
        {
            base.ActivateRandomTrigger(sender, arg, considerWeights);
            this.OnTriggerActivated(sender, arg);
        }

        #endregion

    }

}
