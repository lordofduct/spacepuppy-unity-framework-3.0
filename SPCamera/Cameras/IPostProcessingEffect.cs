using UnityEngine;

namespace com.spacepuppy.Cameras
{

    public interface IPostProcessingEffect
    {

        bool enabled { get; set; }

        void RenderImage(ICamera camera, RenderTexture source, RenderTexture destination);

    }

}
