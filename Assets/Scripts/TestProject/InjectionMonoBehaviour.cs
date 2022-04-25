using UnityEngine;

namespace DependencyInjector.Tests
{
    public class InjectionMonoBehaviour : MonoBehaviour
    {
        public class InjectionTest
        {
            public class InjectionTestEmpty
            {
                public InjectionTestEmpty ()
                {
                    Debug.Log("InjectionTestEmpty");
                }
            }

            public class InjectionTestInt
            {
                public InjectionTestInt (int i)
                {
                    Debug.Log($"InjectionTestInt {i}");
                }
            }
        }

        void Awake ()
        {
            Scope scope = new Scope();
            scope.Register<InjectionTest.InjectionTestEmpty>(Lifecycle.Transient);
            scope.RegisterFromInstance<int>(1);
            scope.Register<InjectionTest.InjectionTestInt>(Lifecycle.Transient);

            scope.ResolveAll();
        }
    }
}