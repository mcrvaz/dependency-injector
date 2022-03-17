#pragma warning disable IDE0150

using NUnit.Framework;

public class DependencyInjectorTests
{
    class BaseDependencyInjectorTests
    {
        public class NoConstructor { }

        public class EmptyConstructor
        {
            public EmptyConstructor () { }
        }

        public class ConstructorInt
        {
            public ConstructorInt (int i) { }
        }

        public Scope Scope { get; private set; }

        [SetUp]
        public void Setup ()
        {
            Scope = new Scope();
        }

        [TearDown]
        public void Cleanup () { }
    }

    class Resolve : BaseDependencyInjectorTests
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
}
