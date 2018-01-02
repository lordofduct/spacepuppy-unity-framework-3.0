using UnityEngine;

namespace com.spacepuppy
{

    /// <summary>
    /// Interface/contract for a type that is considered a GameObject source. Such as Components/IComponents, which are attached to GameObjects, so therefore are a source of a GameObject.
    /// </summary>
    public interface IGameObjectSource
    {

        GameObject gameObject { get; }
        Transform transform { get; }

    }

}
