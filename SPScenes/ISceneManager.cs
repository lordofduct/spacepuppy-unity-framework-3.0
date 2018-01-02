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
        event System.EventHandler<SceneLoadedEventArgs> SceneLoaded;
        event System.EventHandler<ActiveSceneChangedEventArgs> ActiveSceneChanged;

        LoadSceneWaitHandle LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, LoadSceneBehaviour behaviour = LoadSceneBehaviour.Async);
        LoadSceneWaitHandle LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single, LoadSceneBehaviour behaviour = LoadSceneBehaviour.Async);

        AsyncOperation UnloadScene(Scene scene);
        Scene GetActiveScene();
    }

}
