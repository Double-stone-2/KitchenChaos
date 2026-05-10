using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader 
{

    public enum SceneName
    {
        MainMenuScene,
        GameScene,
        LoadingScene
    }
   private static SceneName targetScene;

    public static void Load(SceneName targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(SceneName.LoadingScene.ToString());
        

    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
