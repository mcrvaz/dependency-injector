using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

internal static class SceneLoader
{
    public static Scene LoadScene (string sceneName, LoadSceneMode loadSceneMode)
    {
        SceneManager.LoadScene(sceneName, loadSceneMode);
        return SceneManager.GetSceneByName(sceneName);
    }

    public static async UniTask<Scene> LoadSceneAsync (
        string sceneName,
        LoadSceneMode loadSceneMode
    )
    {
        await SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        Scene scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
        return scene;
    }
}
