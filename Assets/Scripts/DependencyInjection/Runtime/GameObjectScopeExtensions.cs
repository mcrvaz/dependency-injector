using UnityEngine;

namespace DependencyInjectionFramework
{
    public static class GameObjectScopeExtensions
    {
        public static void RegisterFromResources<T> (
            this GameObjectScope scope,
            string path,
            Lifecycle lifecycle,
            Transform parent = null
        ) where T : UnityEngine.Object
        {
            scope.RegisterFromFactory<T>(() =>
            {
                Transform actualParent = parent != null ? parent : scope.Root.transform;
                T go = GameObject.Instantiate(Resources.Load<T>(path), actualParent);
                return go;
            }, lifecycle);
        }

        public static void RegisterFromNewGameObject<T> (
            this GameObjectScope scope,
            string gameObjectName,
            Lifecycle lifecycle,
            Transform parent = null
        ) where T : UnityEngine.Component
        {
            scope.RegisterFromFactory<T>(() =>
            {
                Transform actualParent = parent != null ? parent : scope.Root.transform;
                T go = new GameObject(gameObjectName).AddComponent<T>();
                go.transform.SetParent(actualParent);
                return go;
            }, lifecycle);
        }
    }
}
