#pragma warning disable IDE0150

using NUnit.Framework;
using static TestClasses;

public class DependencyInjectorTests
{
    class BaseDependencyInjectorTests
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

    class ResolveSingleton : BaseDependencyInjectorTests
    {
        [Test]
        public void Resolve_EmptyConstructor_Singleton ()
        {
            Scope.Install<EmptyConstructor>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<EmptyConstructor>() is EmptyConstructor);
        }

        [Test]
        public void Resolve_EmptyConstructor_Singleton_Twice_Returns_Same_Instance ()
        {
            Scope.Install<EmptyConstructor>(Lifecycle.Singleton);
            EmptyConstructor instance = Scope.Resolve<EmptyConstructor>();
            Assert.AreEqual(instance, Scope.Resolve<EmptyConstructor>());
        }
    }

    class ResolveTransient : BaseDependencyInjectorTests
    {
        [Test]
        public void Resolve_EmptyConstructor_Transient ()
        {
            Scope.Install<EmptyConstructor>(Lifecycle.Transient);
            Assert.IsTrue(Scope.Resolve<EmptyConstructor>() is EmptyConstructor);
        }

        [Test]
        public void Resolve_EmptyConstructor_Transient_Twice_Returns_Different_Instance ()
        {
            Scope.Install<EmptyConstructor>(Lifecycle.Transient);
            EmptyConstructor instance = Scope.Resolve<EmptyConstructor>();
            Assert.AreNotEqual(instance, Scope.Resolve<EmptyConstructor>());
        }
    }

    class Resolve_NoConstructor : BaseDependencyInjectorTests
    {
        [Test]
        public void Resolve ()
        {
            Scope.Install<NoConstructor>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<NoConstructor>() is NoConstructor);
        }
    }

    class Resolve_IntConstutor : BaseDependencyInjectorTests
    {
        [Test]
        public void Resolve_Instance ()
        {
            Scope.InstallFromInstance<int>(1);
            Scope.Install<IntConstructor>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<IntConstructor>() is IntConstructor);
        }

        [Test]
        public void Resolve_Instance_Value ()
        {
            int value = 1;
            Scope.InstallFromInstance(value);
            Scope.Install<IntConstructor>(Lifecycle.Singleton);
            IntConstructor instance = Scope.Resolve<IntConstructor>();
            Assert.AreEqual(value, instance.Value);
        }
    }

    class Resolve_NestedConstutor : BaseDependencyInjectorTests
    {
        [Test]
        public void Resolve_Instance ()
        {
            Scope.Install<EmptyConstructor>(Lifecycle.Singleton);
            Scope.Install<NestedEmptyConstructor>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<NestedEmptyConstructor>() is NestedEmptyConstructor);
        }

        [Test]
        public void Resolve_Instance_Value ()
        {
            Scope.Install<EmptyConstructor>(Lifecycle.Singleton);
            Scope.Install<NestedEmptyConstructor>(Lifecycle.Singleton);
            EmptyConstructor emptyConstructorInstance = Scope.Resolve<EmptyConstructor>();
            NestedEmptyConstructor instance = Scope.Resolve<NestedEmptyConstructor>();
            Assert.AreEqual(emptyConstructorInstance, instance.Value);
        }
    }
}
