using UnityEngine;
using UnityEngine.SceneManagement;

namespace DependencyInjectionFramework
{
    public class GameObjectScope : Scope
    {
        public GameObject Root { get; }

        GameObjectScope (GameObject gameObject, Scope parent, IInstaller installer)
            : base(parent, installer)
        {
            Root = gameObject;
        }

        public static GameObjectScope FromGameObject (
            Scope parent,
            IInstaller installer,
            GameObject gameObject
        )
        {
            return new GameObjectScope(gameObject, parent, installer);
        }

        public static GameObjectScope FromNewGameObject (
            Scope parent,
            IInstaller installer,
            string gameObjectName
        )
        {
            GameObject go = new GameObject(gameObjectName);
            return FromGameObject(parent, installer, go);
        }

        public static GameObjectScope FromNewGameObject (
            GameObjectScope parent,
            IInstaller installer,
            string gameObjectName
        )
        {
            GameObject go = new GameObject(gameObjectName);
            go.transform.SetParent(parent.Root.transform);
            return FromGameObject(parent, installer, go);
        }

        public static GameObjectScope FromNewGameObject (
            SceneScope parent,
            IInstaller installer,
            string gameObjectName
        )
        {
            GameObject go = new GameObject(gameObjectName);
            SceneManager.MoveGameObjectToScene(go, parent.Scene);
            return FromGameObject(parent, installer, go);
        }
    }
}
