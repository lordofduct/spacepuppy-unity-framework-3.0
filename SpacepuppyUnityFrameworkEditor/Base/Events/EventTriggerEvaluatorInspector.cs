using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

using com.spacepuppy;
using com.spacepuppy.Events;
using com.spacepuppy.Utils;

namespace com.spacepuppyeditor.Base.Events
{

    [InitializeOnLoad()]
    internal static class EventTriggerEvaluatorInspector
    {

        #region Fields

        public const long COOLDOWN_TRIGGERED = System.TimeSpan.TicksPerSecond * 1;

        private static long _lastT;
        private static Dictionary<int, long> _triggered = new Dictionary<int, long>();
        private static HashSet<int> _cache = new HashSet<int>();

        #endregion

        #region Static Constructor

        static EventTriggerEvaluatorInspector()
        {
            EventTriggerEvaluator.SetCurrentEvaluator(new SpecialEventTriggerEvaluator());
            _lastT = System.DateTime.Now.Ticks;
            EditorApplication.update += Update;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
        }

        #endregion

        #region Methods

        private static void SignalTriggered(object obj)
        {
            if (obj is IProxy)
            {
                var proxy = obj as IProxy;
                obj = proxy.GetTarget();

                // // SHOULD WE DRAW THE PROXY TOO???
                //if(GameObjectUtil.IsGameObjectSource(proxy))
                //{
                //    var go = GameObjectUtil.GetGameObjectFromSource(proxy);
                //    if(go != null)
                //    {
                //        int id = go.GetInstanceID();
                //        _triggered[id] = COOLDOWN_TRIGGERED;
                //    }
                //}
            }

            if (GameObjectUtil.IsGameObjectSource(obj))
            {
                var go = GameObjectUtil.GetGameObjectFromSource(obj);
                if (go != null)
                {
                    int id = go.GetInstanceID();
                    _triggered[id] = COOLDOWN_TRIGGERED;
                }
            }
        }





        private static void Update()
        {
            var t = System.DateTime.Now.Ticks;
            var dt = t - _lastT;
            _lastT = t;

            if (_triggered.Count > 0)
            {
                _cache.AddRange(_triggered.Keys);

                foreach (var id in _cache)
                {
                    long value = _triggered[id] - dt;
                    if (value <= 0)
                        _triggered.Remove(id);
                    else
                        _triggered[id] = value;
                }

                _cache.Clear();
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
        {
            long ticks;
            if (!_triggered.TryGetValue(instanceID, out ticks)) return;

            float t = (float)((double)ticks / (double)COOLDOWN_TRIGGERED);
            //t = com.spacepuppy.Tween.EaseMethods.ExpoEaseOut(1f - t, 1f, -1f, 1f);
            t = Mathf.Clamp01(t * t * t);
            var c = Color.blue;
            c.a = t * 0.2f;
            EditorGUI.DrawRect(selectionRect, c);
        }

        #endregion

        #region Special Types
        
        private class SpecialEventTriggerEvaluator : EventTriggerEvaluator.IEvaluator
        {

            public void GetAllTriggersOnTarget(object target, List<ITriggerable> outputColl)
            {
                if (Application.isPlaying)
                {
                    EventTriggerEvaluator.Default.GetAllTriggersOnTarget(target, outputColl);
                }
                else
                {
                    if (target is IProxy) target = (target as IProxy).GetTarget();
                    var go = GameObjectUtil.GetGameObjectFromSource(target);
                    if (go != null)
                    {
                        go.GetComponents<ITriggerable>(outputColl);
                        outputColl.Sort(TriggerableOrderComparer.Default);
                    }
                    else if (target is ITriggerable)
                        outputColl.Add(target as ITriggerable);
                }
            }

            void EventTriggerEvaluator.IEvaluator.TriggerAllOnTarget(object target, object sender, object arg)
            {
                if (Application.isPlaying)
                {
                    SignalTriggered(target);
                    EventTriggerEvaluator.Default.TriggerAllOnTarget(target, sender, arg);
                    return;
                }

                using (var lst = com.spacepuppy.Collections.TempCollection.GetList<ITriggerable>())
                {
                    this.GetAllTriggersOnTarget(target, lst);

                    var e = lst.GetEnumerator();
                    while (e.MoveNext())
                    {
                        var t = e.Current;
                        if (t.CanTrigger)
                        {
                            SignalTriggered(t);
                            t.Trigger(sender, arg);
                        }
                    }
                }
            }

            void EventTriggerEvaluator.IEvaluator.TriggerSelectedTarget(object target, object sender, object arg)
            {
                SignalTriggered(target);
                EventTriggerEvaluator.Default.TriggerSelectedTarget(target, sender, arg);
            }

            void EventTriggerEvaluator.IEvaluator.CallMethodOnSelectedTarget(object target, string methodName, VariantReference[] methodArgs)
            {
                SignalTriggered(target);
                EventTriggerEvaluator.Default.CallMethodOnSelectedTarget(target, methodName, methodArgs);
            }

            void EventTriggerEvaluator.IEvaluator.SendMessageToTarget(object target, string message, object arg)
            {
                SignalTriggered(target);
                EventTriggerEvaluator.Default.SendMessageToTarget(target, message, arg);
            }

            void EventTriggerEvaluator.IEvaluator.EnableTarget(object target, EnableMode mode)
            {
                SignalTriggered(target);
                EventTriggerEvaluator.Default.EnableTarget(target, mode);
            }

            void EventTriggerEvaluator.IEvaluator.DestroyTarget(object target)
            {
                SignalTriggered(target);
                EventTriggerEvaluator.Default.DestroyTarget(target);
            }

        }

        #endregion

    }

}
