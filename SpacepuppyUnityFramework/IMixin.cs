using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{

    public interface IMixin
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public abstract class MixinConstructorAttribute : System.Attribute
    {
        public abstract void OnConstructed(IMixin obj);
    }

    public static class MixinUtil
    {

        private static System.Type _mixinBaseType = typeof(IMixin);

        public static void Register(IMixin obj)
        {
            if (obj == null) return;

            var mixinTypes = obj.GetType().FindInterfaces((tp, c) =>
            {
                return tp != _mixinBaseType && tp.IsInterface && _mixinBaseType.IsAssignableFrom(tp);
            }, null);

            foreach (var mixinType in mixinTypes)
            {
                var constructedAttribs = mixinType.GetCustomAttributes(typeof(MixinConstructorAttribute), false);
                if (constructedAttribs != null && constructedAttribs.Length > 0)
                {
                    for (int i = 0; i < constructedAttribs.Length; i++)
                    {
                        (constructedAttribs[i] as MixinConstructorAttribute).OnConstructed(obj);
                    }
                }
            }

        }

    }

    /// <summary>
    /// On start or on enable if and only if start already occurred. This adjusts the order of 'OnEnable' so that it can be used in conjunction with 'OnDisable' to wire up handlers cleanly. 
    /// OnEnable occurs BEFORE Start sometimes, and other components aren't ready yet. This remedies that.
    /// </summary>
    [MStartOrEnableReceiverConstructor]
    public interface IMStartOrEnableReceiver : IMixin, IEventfulComponent
    {

        void OnStartOrEnable();

    }

    [System.AttributeUsage(System.AttributeTargets.Interface)]
    public class MStartOrEnableReceiverConstructorAttribute : MixinConstructorAttribute
    {

        public override void OnConstructed(IMixin obj)
        {
            var c = obj as IMStartOrEnableReceiver;
            if (c != null)
            {
                c.OnStarted += (s, e) =>
                {
                    c.OnStartOrEnable();
                };
                c.OnEnabled += (s, e) =>
                {
                    if (c.started)
                    {
                        c.OnStartOrEnable();
                    }
                };
            }
        }

    }

    /// <summary>
    /// Sometimes you want to run StartOrEnable late, to allow Start to be called on all other scripts. Basically adding a final ordering point for Start similar to LateUpdate.
    /// </summary>
    [MLateStartOrEnableReceiverConstructor]
    public interface IMLateStartOrEnableReceiver : IMixin, IEventfulComponent
    {

        void OnLateStartOrEnable();

    }

    [System.AttributeUsage(System.AttributeTargets.Interface)]
    public class MLateStartOrEnableReceiverConstructorAttribute : MixinConstructorAttribute
    {

        public override void OnConstructed(IMixin obj)
        {
            var c = obj as IMLateStartOrEnableReceiver;
            if (c != null)
            {
                c.OnEnabled += (s, e) =>
                {
                    GameLoop.UpdateHandle.BeginInvoke(() =>
                    {
                        c.OnLateStartOrEnable();
                    });
                };
            }
        }

    }

}
