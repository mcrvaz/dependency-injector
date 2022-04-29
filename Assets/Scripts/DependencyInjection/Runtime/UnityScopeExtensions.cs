using UnityEngine;

namespace DependencyInjectionFramework
{
    public static class UnityScopeExtensions
    {
        public static void RegisterFromComponent<T> (
            this Scope scope,
            GameObject gameObject
        ) where T : UnityEngine.Component
        {
            scope.RegisterFromInstance(gameObject.GetComponent<T>());
        }

        public static void RegisterFromChildrenComponent<T> (
            this Scope scope,
            GameObject parent
        ) where T : UnityEngine.Component
        {
            scope.RegisterFromInstance(parent.GetComponentInChildren<T>(true));
        }
    }
}
