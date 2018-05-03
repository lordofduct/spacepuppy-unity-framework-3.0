using UnityEngine;
using UnityEngine.SceneManagement;
using com.spacepuppy.Scenes;

namespace com.spacepuppy
{

    public interface ISceneManager : IService
    {

        event System.EventHandler<LoadSceneWaitHandle> BeforeSceneLoaded;
        event System.EventHandler<SceneUnloadedEventArgs> BeforeSceneUnloaded;
        event System.EventHandler<SceneUnloadedEventArgs> SceneUnloaded;
        event System.EventHandler<LoadSceneWaitHandle> SceneLoaded;
        event System.EventHandler<ActiveSceneChangedEventArgs> ActiveSceneChanged;

        LoadSceneWaitHandle LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, LoadSceneBehaviour behaviour = LoadSceneBehaviour.Async);
        LoadSceneWaitHandle LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single, LoadSceneBehaviour behaviour = LoadSceneBehaviour.Async);

        AsyncOperation UnloadScene(Scene scene);
        Scene GetActiveScene();

        /// <summary>
        /// Test if a scene by the name exists.
        /// </summary>
        /// <param name="excludeInactive">False to test if the scene exists as a loadable scene, True if to test if the scene exists and is actively loaded.</param>
        /// <returns></returns>
        bool SceneExists(string sceneName, bool excludeInactive = false);

    }

}
