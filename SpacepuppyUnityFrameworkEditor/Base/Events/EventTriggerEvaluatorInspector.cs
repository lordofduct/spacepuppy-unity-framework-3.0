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
        
        private static void SignalTriggered(GameObject go)
        {
            if (go == null) return;

            int id = go.GetInstanceID();
            _triggered[id] = COOLDOWN_TRIGGERED;
        }

        private static void SignalTriggered(ITriggerable mech)
        {
            if (mech == null) return;
            SignalTriggered(GameObjectUtil.GetGameObjectFromSource(mech));
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
                    SignalTriggered(GameObjectUtil.GetGameObjectFromSource(target));
                    EventTriggerEvaluator.Default.TriggerAllOnTarget(target, sender, arg);
                    return;
                }

                using (var lst = com.spacepuppy.Collections.TempCollection.GetList<ITriggerable>())
                {
                    /*
                     * OLD
                     * 
                    var go = GameObjectUtil.GetGameObjectFromSource(target);
                    if (go != null)
                    {
                        go.GetComponents<ITriggerableMechanism>(lst);
                        lst.Sort(TriggerableMechanismOrderComparer.Default);
                    }
                    else if (target is ITriggerableMechanism)
                        lst.Add(target as ITriggerableMechanism);
                      */
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
                SignalTriggered(target as ITriggerable);
                EventTriggerEvaluator.Default.TriggerSelectedTarget(target, sender, arg);
            }

            void EventTriggerEvaluator.IEvaluator.CallMethodOnSelectedTarget(object target, string methodName, VariantReference[] methodArgs)
            {
                SignalTriggered(GameObjectUtil.GetGameObjectFromSource(target));
                EventTriggerEvaluator.Default.CallMethodOnSelectedTarget(target, methodName, methodArgs);
            }

            void EventTriggerEvaluator.IEvaluator.SendMessageToTarget(object target, string message, object arg)
            {
                SignalTriggered(GameObjectUtil.GetGameObjectFromSource(target));
                EventTriggerEvaluator.Default.SendMessageToTarget(target, message, arg);
            }

            void EventTriggerEvaluator.IEvaluator.EnableTarget(object target, EnableMode mode)
            {
                SignalTriggered(GameObjectUtil.GetGameObjectFromSource(target));
                EventTriggerEvaluator.Default.EnableTarget(target, mode);
            }

            void EventTriggerEvaluator.IEvaluator.DestroyTarget(object target)
            {
                SignalTriggered(GameObjectUtil.GetGameObjectFromSource(target));
                EventTriggerEvaluator.Default.DestroyTarget(target);
            }

        }

        #endregion

    }

}
