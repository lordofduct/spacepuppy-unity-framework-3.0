#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;
using System.Collections.Generic;
using com.spacepuppy.Cameras;

namespace com.spacepuppy
{

    public interface IPostProcessingManager : IService
    {

        IList<IPostProcessingEffect> GlobalEffects { get; }

        /// <summary>
        /// Applies all global post processing effects.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="callback"></param>
        /// <returns>Returns true if effects were processed.</returns>
        bool ApplyGlobalPostProcessing(ICamera camera, RenderTexture source, RenderTexture destination);

    }

}
