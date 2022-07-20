using UnityEngine;
using UnityEngine.SceneManagement;
using DependencyInjector.Exceptions;

namespace DependencyInjector.Unity
{
    public static class UnityRegistrationExtensions
    {
        public static void RegisterFromSceneComponent<T1, T2> (
            this Scope scope,
            Scene scene,
            bool includeInactive = true
        )
        {
            foreach (GameObject gameObject in scene.GetRootGameObjects())
            {
                T1 instance = gameObject.GetComponentInChildren<T1>(includeInactive);
                if (instance != null)
                {
                    scope.RegisterFromInstance(instance);
                    return;
                }
            }
            throw new RegistrationException($"Component of type {typeof(T1)} not found in scene {scene.name}.");
        }

        public static void RegisterFromGameObjectComponent<T1, T2> (
            this Scope scope,
            GameObject gameObject
        )
        {
            if (gameObject.TryGetComponent<T1>(out var instance))
                scope.RegisterFromInstance(instance);
            else
                throw new RegistrationException($"Component of type {typeof(T1)} not found in gameObject {gameObject.name}.");
        }

        public static void RegisterFromGameObjectChildComponent<T1, T2> (
            this Scope scope,
            GameObject gameObject,
            bool includeInactive = true
        )
        {
            T1 instance = gameObject.GetComponentInChildren<T1>(includeInactive);
            if (instance != null)
                scope.RegisterFromInstance(instance);
            else
                throw new RegistrationException($"Component of type {typeof(T1)} not found in gameObject {gameObject.name} children.");
        }

        public static void RegisterFromComponentInNewGameObject<T1, T2> (
            this Scope scope,
            Lifecycle lifecycle,
            string name = null
        ) where T2 : Component
        {
            RegistrationOptions options = new RegistrationOptions(
                typeof(T1),
                typeof(T2),
                lifecycle,
                () =>
                {
                    GameObject gameObject = new GameObject(name);
                    return gameObject.AddComponent<T2>();
                }
            );
            scope.Register(options);
        }

        public static void RegisterFromNewPrefabInstanceComponent<T1, T2> (
            this Scope scope,
            T2 prefab
        ) where T2 : UnityEngine.Object
        {
            InstantiateAndRegister(scope, prefab);
        }

        public static void RegisterFromNewPrefabInstanceComponent<T1, T2> (
            this Scope scope,
            T2 prefab,
            Vector3 position,
            Quaternion rotation
        ) where T2 : UnityEngine.Object
        {
            InstantiateAndRegister(scope, prefab, position, rotation);
        }

        public static void RegisterFromNewPrefabInstanceComponent<T1, T2> (
            this Scope scope,
            T2 prefab,
            Vector3 position,
            Quaternion rotation,
            Transform parent
        ) where T2 : UnityEngine.Object
        {
            InstantiateAndRegister(scope, prefab, position, rotation, parent);
        }

        public static void RegisterFromNewPrefabInstanceComponent<T1, T2> (
            this Scope scope,
            T2 prefab,
            Transform parent
        ) where T2 : UnityEngine.Object
        {
            InstantiateAndRegister(scope, prefab, parent);
        }

        public static void RegisterFromNewPrefabInstanceComponent<T1, T2> (
            this Scope scope,
            T2 prefab,
            Transform parent,
            bool worldPositionStays
        ) where T2 : UnityEngine.Object
        {
            InstantiateAndRegister(scope, prefab, parent, worldPositionStays);
        }

        public static void RegisterFromResourcesNewPrefabInstanceComponent<T1, T2> (
            this Scope scope,
            string path
        ) where T2 : UnityEngine.Object
        {
            InstantiateAndRegister(scope, LoadPrefabFromResources<T2>(path));
        }

        public static void RegisterFromResourcesNewPrefabInstanceComponent<T1, T2> (
            this Scope scope,
            string path,
            Vector3 position,
            Quaternion rotation
        ) where T2 : UnityEngine.Object
        {
            InstantiateAndRegister(scope, LoadPrefabFromResources<T2>(path), position, rotation);
        }

        public static void RegisterFromResourcesNewPrefabInstanceComponent<T1, T2> (
            this Scope scope,
            string path,
            Vector3 position,
            Quaternion rotation,
            Transform parent
        ) where T2 : UnityEngine.Object
        {
            InstantiateAndRegister(
                scope,
                LoadPrefabFromResources<T2>(path),
                position,
                rotation,
                parent
            );
        }

        public static void RegisterFromResourcesNewPrefabInstanceComponent<T1, T2> (
            this Scope scope,
            string path,
            Transform parent
        ) where T2 : UnityEngine.Object
        {
            InstantiateAndRegister(scope, LoadPrefabFromResources<T2>(path), parent);
        }

        public static void RegisterFromResourcesNewPrefabInstanceComponent<T1, T2> (
            this Scope scope,
            string path,
            Transform parent,
            bool worldPositionStays
        ) where T2 : UnityEngine.Object
        {
            InstantiateAndRegister(
                scope,
                LoadPrefabFromResources<T2>(path),
                parent,
                worldPositionStays
            );
        }

        static T LoadPrefabFromResources<T> (string path) where T : UnityEngine.Object
        {
            T resource = Resources.Load<T>(path);
            if (resource == null)
                throw new RegistrationException($"Resource of type {typeof(T)} not found in {path}.");
            return resource;
        }

        static void InstantiateAndRegister<T> (
            this Scope scope,
            T prefab
        ) where T : UnityEngine.Object
        {
            T instance = GameObject.Instantiate(prefab);
            scope.RegisterFromInstance(instance);
        }

        static void InstantiateAndRegister<T> (
            this Scope scope,
            T prefab,
            Vector3 position,
            Quaternion rotation
        ) where T : UnityEngine.Object
        {
            T instance = GameObject.Instantiate(prefab, position, rotation);
            scope.RegisterFromInstance(instance);
        }

        static void InstantiateAndRegister<T> (
            this Scope scope,
            T prefab,
            Vector3 position,
            Quaternion rotation,
            Transform parent
        ) where T : UnityEngine.Object
        {
            T instance = GameObject.Instantiate(prefab, position, rotation, parent);
            scope.RegisterFromInstance(instance);
        }

        static void InstantiateAndRegister<T> (
            this Scope scope,
            T prefab,
            Transform parent
        ) where T : UnityEngine.Object
        {
            T instance = GameObject.Instantiate(prefab, parent);
            scope.RegisterFromInstance(instance);
        }
        static void InstantiateAndRegister<T> (
            this Scope scope,
            T prefab,
            Transform parent,
            bool worldPositionStays
        ) where T : UnityEngine.Object
        {
            T instance = GameObject.Instantiate(prefab, parent, worldPositionStays);
            scope.RegisterFromInstance(instance);
        }
    }
}