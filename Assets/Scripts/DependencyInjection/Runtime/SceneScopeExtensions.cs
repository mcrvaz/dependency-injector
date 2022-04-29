using UnityEngine;
using UnityEngine.SceneManagement;

namespace DependencyInjectionFramework
{
    public static class SceneScopeExtensions
    {
        public static void RegisterFromResources<T> (
            this SceneScope scope,
            string path,
            Lifecycle lifecycle,
            Transform parent = null
        ) where T : UnityEngine.Component
        {
            scope.RegisterFromFactory<T>(() =>
            {
                T asset = Resources.Load<T>(path);
                if (asset == null)
                    throw new ResolutionException($"Could not find Resources asset at path {path}.");
                T go = GameObject.Instantiate(asset, parent);
                SceneManager.MoveGameObjectToScene(go.gameObject, scope.Scene);
                return go;
            }, lifecycle);
        }

        public static void RegisterFromNewGameObject<T> (
            this SceneScope scope,
            string gameObjectName,
            Lifecycle lifecycle,
            Transform parent = null
        ) where T : UnityEngine.Component
        {
            scope.RegisterFromFactory<T>(() =>
            {
                T go = new GameObject(gameObjectName).AddComponent<T>();
                go.transform.SetParent(parent);
                SceneManager.MoveGameObjectToScene(go.gameObject, scope.Scene);
                return go;
            }, lifecycle);
        }
    }
}
