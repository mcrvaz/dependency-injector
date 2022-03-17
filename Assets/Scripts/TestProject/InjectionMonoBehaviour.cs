using UnityEngine;

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
        scope.Install<InjectionTest.InjectionTestEmpty>(Lifecycle.Transient);
        scope.InstallFromInstance<int>(1);
        scope.Install<InjectionTest.InjectionTestInt>(Lifecycle.Transient);
        scope.ResolveAll();
    }
}
