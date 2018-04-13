using System;
using System.Collections.Generic;

namespace com.spacepuppy
{

    /// <summary>
    /// Interface/contract for an object that has an 'Update' method.
    /// 
    /// Can be added to an UpdatePump collection to receive an Update message on some interval.
    /// 
    /// See GameLoop and its various Update pump's for hooking into Update/FixedUpdate/LateUpdate.
    /// </summary>
    public interface IUpdateable
    {

        void Update();

    }

    /// <summary>
    /// A collection for IUpdateable objects. Call the Update method on some interval to signal IUpdateable.Update to every object in the collection.
    /// </summary>
    public class UpdatePump
    {

        #region Fields

        private HashSet<IUpdateable> _set = new HashSet<IUpdateable>();
        private HashSet<IUpdateable> _toAdd = new HashSet<IUpdateable>();
        private HashSet<IUpdateable> _toRemove = new HashSet<IUpdateable>();
        private bool _inUpdate;

        #endregion

        #region Methods

        public bool Contains(IUpdateable obj)
        {
            return _set.Contains(obj) && !_toRemove.Contains(obj);
        }

        public void Add(IUpdateable obj)
        {
            if (_inUpdate)
            {
                if (_set.Contains(obj) && !_toRemove.Contains(obj)) return;
                _toAdd.Add(obj);
            }
            else
            {
                _set.Add(obj);
            }
        }

        /// <summary>
        /// Adds the IUpdateable after the next update cycle.
        /// </summary>
        /// <param name="obj"></param>
        public void DelayedAdd(IUpdateable obj)
        {
            _toAdd.Add(obj);
        }

        public void Remove(IUpdateable obj)
        {
            if (_inUpdate)
            {
                if (_set.Contains(obj))
                    _toRemove.Add(obj);
            }
            else
            {
                _set.Remove(obj);
            }
        }

        public void Update()
        {
            HashSet<IUpdateable>.Enumerator e;

            if(_set.Count > 0)
            {
                _inUpdate = true;
                e = _set.GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Update();
                }
                _inUpdate = false;
            }

            if (_toAdd.Count > 0)
            {
                e = _toAdd.GetEnumerator();
                while (e.MoveNext())
                {
                    _set.Add(e.Current);
                }
            }

            if (_toRemove.Count > 0)
            {
                e = _toRemove.GetEnumerator();
                while (e.MoveNext())
                {
                    _set.Remove(e.Current);
                }
                _toRemove.Clear();
            }
        }

        #endregion

    }

}
