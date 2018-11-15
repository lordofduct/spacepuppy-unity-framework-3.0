using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.spacepuppy
{

    /// <summary>
    /// An implementation of mixin's for C# similar to python's mixins.
    /// 
    /// Since C# doesn't support multiple inheritance, we instead implement a mixin as an interface. 
    /// All mixin's should inherit from IMixin for initialization purpose. The interface is then attributed 
    /// with a MixinConstructorAttribute which acts as the signal for when a object with a mixin is 
    /// initialized/constructed. 
    /// 
    /// This constructor attribute can then do what it needs to to implement the the mixin. 
    /// 
    /// For mixin that just performs an action, it can register that action right then and there (such 
    /// as registering for events). 
    /// 
    /// For a mixin that needs state, you can create a state object there and store it somewhere (usually 
    /// as a component on the GameObject of the mixin). 
    /// 
    /// If you want the mixin to have methods, you should create a static class with extension methods 
    /// that accept the mixin interface as the first parameter.
    /// 
    /// IMixin initializing is handled during SPComponent.Awake. If you want a non-SPComponent to handle IMixin 
    /// you must call MixinUtil.Initialize during the constructor/awake of the class.
    /// </summary>
    public interface IMixin
    {
    }

    /// <summary>
    /// Base type for mixin constructor attributes.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public abstract class MixinConstructorAttribute : System.Attribute
    {
        public abstract void OnConstructed(IMixin obj);
    }

    /// <summary>
    /// Static class for initializing mixins.
    /// </summary>
    public static class MixinUtil
    {

        private static System.Type _mixinBaseType = typeof(IMixin);

        public static void Initialize(IMixin obj)
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
    /// <remarks>
    /// In earlier versions of Spacepuppy Framework this was implemented directly on SPComponent. I've since moved it to here to match our new IMixin interface, and so that only those components 
    /// that need OnStartOrEnable actually have it implemented. No need for empty method calls on ALL components.
    /// </remarks>
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
    /// Sometimes you want to run Start late, to allow Start to be called on all other scripts. Basically adding a final ordering for Start similar to LateUpdate.
    /// </summary>
    [MLateStartReceiverConstructor]
    public interface IMLateStartReceiver : IMixin, IEventfulComponent
    {
        void OnLateStart();
    }

    [System.AttributeUsage(System.AttributeTargets.Interface)]
    public class MLateStartReceiverConstructorAttribute : MixinConstructorAttribute
    {

        public override void OnConstructed(IMixin obj)
        {
            var c = obj as IMLateStartReceiver;
            if (c != null)
            {
                c.OnStarted += (s, e) =>
                {
                    GameLoop.LateUpdateHandle.BeginInvoke(() =>
                    {
                        c.OnLateStart();
                    });
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
                    GameLoop.LateUpdateHandle.BeginInvoke(() =>
                    {
                        c.OnLateStartOrEnable();
                    });
                };
            }
        }

    }

}
