using UnityEngine;
using System.Collections.Generic;

using com.spacepuppy.Collections;
using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    [System.Serializable()]
    public abstract class BaseSPEvent
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
        private List<EventTriggerTarget> _targets = new List<EventTriggerTarget>();
        [System.NonSerialized]
        private SignalingTargetList _signalList;

        [System.NonSerialized()]
        private string _id;
        [System.NonSerialized]
        private int _version;

        [System.NonSerialized]
        private HashSet<object> _hijackTokens;

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

        public IList<EventTriggerTarget> Targets
        {
            get { return _signalList ?? (_signalList = new SignalingTargetList(this)); }
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

        public bool CurrentlyHijacked
        {
            get { return _hijackTokens != null && _hijackTokens.Count > 0; }
        }

        #endregion

        #region Methods

        public EventTriggerTarget AddNew()
        {
            var targ = new EventTriggerTarget();
            _targets.Add(targ);
            _version++;
            return targ;
        }

        /// <summary>
        /// Begins a hijack, when a trigger is hijacked none of its targets are triggered, but its TriggerActivated event still fires. 
        /// If tokens are passed in it allows compounded hijacking so that just because one caller ends the hijack, another can still continue hijacking.
        /// </summary>
        /// <param name="token"></param>
        public void BeginHijack(object token = null)
        {
            if (token == null) token = "*DEFAULT*";

            if (_hijackTokens == null) _hijackTokens = new HashSet<object>();
            _hijackTokens.Add(token);
        }

        /// <summary>
        /// Attempts to stop a hijack, but if more than one token has been used to hijack the event it may continue hijacking.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Returns true if the trigger is no longer hijacked after calling this</returns>
        public bool EndHijack(object token = null)
        {
            if (_hijackTokens == null) return true;

            _hijackTokens.Remove(token ?? "*DEFAULT*");
            return _hijackTokens.Count == 0;
        }

        /// <summary>
        /// Forces the end of a hijack.
        /// </summary>
        public void ForceEndHijack()
        {
            if (_hijackTokens != null) _hijackTokens.Clear();
        }


        protected void ActivateTrigger(object sender, object arg)
        {
            if (_targets.Count > 0 && !this.CurrentlyHijacked)
            {
                var e = _targets.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current?.Trigger(sender, arg);
                }
            }

            this.OnTriggerActivated(sender, arg);
        }

        protected void ActivateTriggerAt(int index, object sender, object arg)
        {
            if (index >= 0 && index < _targets.Count && !this.CurrentlyHijacked)
            {
                _targets[index]?.Trigger(sender, arg);
            }

            this.OnTriggerActivated(sender, arg);
        }

        protected void ActivateRandomTrigger(object sender, object arg, bool considerWeights, bool selectOnlyIfActive)
        {
            if (_targets.Count > 0 && !this.CurrentlyHijacked)
            {
                EventTriggerTarget trig;
                if (selectOnlyIfActive)
                {
                    using (var lst = TempCollection.GetList<EventTriggerTarget>())
                    {
                        for (int i = 0; i < _targets.Count; i++)
                        {
                            var go = GameObjectUtil.GetGameObjectFromSource(_targets[i].CalculateTarget(arg));
                            if (object.ReferenceEquals(go, null) || go.IsAliveAndActive()) lst.Add(_targets[i]);
                        }
                        trig = (considerWeights) ? lst.PickRandom((t) => { return t.Weight; }) : lst.PickRandom();
                    }
                }
                else
                {
                    trig = (considerWeights) ? _targets.PickRandom((t) => { return t.Weight; }) : _targets.PickRandom();
                }
                if (trig != null) trig.Trigger(sender, arg);
            }

            this.OnTriggerActivated(sender, arg);
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

        private class SignalingTargetList : IList<EventTriggerTarget>
        {

            private BaseSPEvent _owner;

            public SignalingTargetList(BaseSPEvent owner)
            {
                _owner = owner;
            }

            public EventTriggerTarget this[int index] { get { return _owner._targets[index]; } set { throw new System.NotSupportedException(); } }

            public int Count { get { return _owner._targets.Count; } }

            public bool IsReadOnly { get { return false; } }

            public void Add(EventTriggerTarget item)
            {
                if (_owner._targets.Contains(item)) return;
                _owner._targets.Add(item);
                _owner._version++;
            }

            public void Clear()
            {
                _owner._targets.Clear();
                _owner._version++;
            }

            public bool Contains(EventTriggerTarget item)
            {
                return _owner._targets.Contains(item);
            }

            public void CopyTo(EventTriggerTarget[] array, int arrayIndex)
            {
                _owner._targets.CopyTo(array, arrayIndex);
            }

            public int IndexOf(EventTriggerTarget item)
            {
                return _owner._targets.IndexOf(item);
            }

            public void Insert(int index, EventTriggerTarget item)
            {
                int i = _owner._targets.IndexOf(item);
                if (i >= 0)
                {
                    if (i < index)
                    {
                        _owner._targets.RemoveAt(i);
                        index--; //slide back a slot to make up for the removal
                    }
                    else if (i == index)
                    {
                        //it already exists at index, do nothing
                        return;
                    }
                    else
                    {
                        _owner._targets.RemoveAt(index);
                    }
                }

                _owner._targets.Insert(index, item);
                _owner._version++;
            }

            public bool Remove(EventTriggerTarget item)
            {
                bool result = _owner._targets.Remove(item);
                _owner._version++;
                return result;
            }

            public void RemoveAt(int index)
            {
                _owner._targets.RemoveAt(index);
                _owner._version++;
            }

            public BaseSPEvent.Enumerator GetEnumerator()
            {
                return new BaseSPEvent.Enumerator(_owner);
            }

            IEnumerator<EventTriggerTarget> IEnumerable<EventTriggerTarget>.GetEnumerator()
            {
                return new BaseSPEvent.Enumerator(_owner);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new BaseSPEvent.Enumerator(_owner);
            }
        }

        #endregion

    }

    [System.Serializable()]
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

        public new void ActivateRandomTrigger(object sender, object arg, bool considerWeights, bool selectOnlyIfActive)
        {
            base.ActivateRandomTrigger(sender, arg, considerWeights, selectOnlyIfActive);
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
    public abstract class SPEvent<T> : BaseSPEvent where T : System.EventArgs
    {

        #region Events

        public new event System.EventHandler<T> TriggerActivated;
        protected virtual void OnTriggerActivated(object sender, T e)
        {
            this.TriggerActivated?.Invoke(sender, e);
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
                    return this.Targets.Count + 1;
                else
                    return this.Targets.Count;
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

        public void ActivateRandomTrigger(object sender, T arg, bool considerWeights, bool selectOnlyIfActive)
        {
            base.ActivateRandomTrigger(sender, arg, considerWeights, selectOnlyIfActive);
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
            if (_callback != null)
            {
                var c = _callback;
                c(arg);
            }

            if (_evCallback != null)
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
                    return this.Targets.Count + 1;
                else
                    return this.Targets.Count;
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

        public void ActivateRandomTrigger(object sender, T arg, bool considerWeights, bool selectOnlyIfActive)
        {
            base.ActivateRandomTrigger(sender, arg, considerWeights, selectOnlyIfActive);
            this.OnTriggerActivated(sender, arg);
        }

        #endregion

    }

}
