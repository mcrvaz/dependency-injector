using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace DependencyInjectionFramework
{
    public class SceneScope : Scope
    {
        public Scene Scene { get; }

        SceneScope (Scene scene, Scope parent, IInstaller installer) : base(parent, installer)
        {
            Scene = scene;
        }

        public static SceneScope FromNewScene (
            Scope parent,
            IInstaller installer,
            string sceneName,
            LoadSceneMode loadSceneMode
        )
        {
            Scene scene = SceneLoader.LoadScene(sceneName, loadSceneMode);
            return new SceneScope(scene, parent, installer);
        }

        public static async UniTask<SceneScope> FromNewSceneAsync (
            Scope parent,
            IInstaller installer,
            string sceneName,
            LoadSceneMode loadSceneMode
        )
        {
            Scene scene = await SceneLoader.LoadSceneAsync(sceneName, loadSceneMode);
            return new SceneScope(scene, parent, installer);
        }

        public override void Dispose ()
        {
            base.Dispose();
            SceneManager.UnloadSceneAsync(Scene);
        }
    }
}
