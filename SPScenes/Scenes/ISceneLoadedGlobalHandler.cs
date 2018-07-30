using System;

namespace com.spacepuppy.Scenes
{

    /// <summary>
    /// A found message receiver to handle OnSceneLoaded event.
    /// </summary>
    public interface ISceneLoadedGlobalHandler
    {

        void OnSceneLoaded(LoadSceneWaitHandle handle);

    }

}
