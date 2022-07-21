#pragma warning disable IDE0150
using DependencyInjector.Exceptions;
using DependencyInjector.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DependencyInjector.Tests
{
    public class UnityRegistrationTests
    {
        class BaseUnityRegistrationTests
        {
            public Scope Scope { get; private set; }

            [SetUp]
            public void Setup ()
            {
                Scope = new Scope();
            }

            [TearDown]
            public void Cleanup () { }
        }

        class RegisterFromGameObjectComponent : BaseUnityRegistrationTests
        {
            [Test]
            public void Register_Basic_Component_By_Concrete_Type ()
            {
                GameObject gameObject = new GameObject("TestObject");
                BasicComponent component = gameObject.AddComponent<BasicComponent>();
                Scope.RegisterFromGameObjectComponent<BasicComponent>(gameObject);
                Assert.AreEqual(component, Scope.Resolve<BasicComponent>());
            }

            [Test]
            public void Register_Basic_Component_By_Interface ()
            {
                GameObject gameObject = new GameObject("TestObject");
                BasicComponent component = gameObject.AddComponent<BasicComponent>();
                Scope.RegisterFromGameObjectComponent<IBasicComponent>(gameObject);
                Assert.AreEqual(component, Scope.Resolve<IBasicComponent>());
            }
        }

        class RegisterFromGameObjectChildComponent : BaseUnityRegistrationTests
        {
            [Test]
            public void Register_Basic_Component_By_Concrete_Type ()
            {
                GameObject gameObject = new GameObject("TestObject");
                GameObject child = new GameObject("TestObject");
                child.transform.SetParent(gameObject.transform);
                BasicComponent component = child.AddComponent<BasicComponent>();
                Scope.RegisterFromGameObjectChildComponent<BasicComponent>(gameObject);
                Assert.AreEqual(component, Scope.Resolve<BasicComponent>());
            }

            [Test]
            public void Register_Basic_Component_By_Interface ()
            {
                GameObject gameObject = new GameObject("TestObject");
                GameObject child = new GameObject("TestObject");
                child.transform.SetParent(gameObject.transform);
                BasicComponent component = child.AddComponent<BasicComponent>();
                Scope.RegisterFromGameObjectChildComponent<IBasicComponent>(gameObject);
                Assert.AreEqual(component, Scope.Resolve<IBasicComponent>());
            }
        }

        class RegisterFromComponentInNewGameObject : BaseUnityRegistrationTests
        {
            [Test]
            public void Register_Basic_Component_By_Concrete_Type ()
            {
                Scope.RegisterFromComponentInNewGameObject<BasicComponent>(
                    Lifecycle.Transient,
                    "TestObject"
                );
                Assert.IsTrue(Scope.Resolve<BasicComponent>() is BasicComponent);
            }

            [Test]
            public void Register_Basic_Component_By_Interface ()
            {
                Scope.RegisterFromComponentInNewGameObject<IBasicComponent, BasicComponent>(
                    Lifecycle.Transient,
                    "TestObject"
                );
                Assert.IsTrue(Scope.Resolve<IBasicComponent>() is BasicComponent);
            }
        }

        class RegisterFromNewPrefabInstanceComponent : BaseUnityRegistrationTests
        {
            [Test]
            public void Register_Basic_Component_By_Concrete_Type ()
            {
                BasicComponent prefab = Resources.Load<BasicComponent>("TestObject");
                Scope.RegisterFromNewPrefabInstanceComponent<BasicComponent>(prefab);
                Assert.IsTrue(Scope.Resolve<BasicComponent>() is BasicComponent);
            }
        }

        class RegisterFromResourcesNewPrefabInstanceComponent : BaseUnityRegistrationTests
        {
            [Test]
            public void Register_Basic_Component_By_Concrete_Type ()
            {
                Scope.RegisterFromResourcesNewPrefabInstanceComponent<BasicComponent>("TestObject");
                Assert.IsTrue(Scope.Resolve<BasicComponent>() is BasicComponent);
            }

            [Test]
            public void Throws_With_Invalid_Path ()
            {
                Assert.Throws<RegistrationException>(
                    () => Scope.RegisterFromResourcesNewPrefabInstanceComponent<BasicComponent>(
                        "Invalid"
                    )
                );
            }
        }
    }
}