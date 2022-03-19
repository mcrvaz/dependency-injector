#pragma warning disable IDE0150

using NUnit.Framework;
using static TestClasses;

public class ScopeTests
{
    class BaseScopeTests
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

    class ResolveInterface : BaseScopeTests
    {
        [Test]
        public void Resolve_IEmptyConstructor_To_EmptyConstructor_Singleton ()
        {
            Scope.Install<IEmptyConstructor, EmptyConstructor>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<IEmptyConstructor>() is EmptyConstructor);
        }

        [Test]
        public void Resolve_IIntConstructor_To_IntConstructor_Singleton ()
        {
            Scope.InstallFromInstance<int>(1);
            Scope.Install<IIntConstructor, IntConstructor>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<IIntConstructor>() is IntConstructor);
        }

        [Test]
        public void Resolve_INestedEmptyConstructor_To_NestedEmptyConstructor_Concrete_Dependency ()
        {
            Scope.Install<EmptyConstructor>(Lifecycle.Singleton);
            Scope.Install<INestedEmptyConstructor, NestedEmptyConstructor>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<INestedEmptyConstructor>() is NestedEmptyConstructor);
        }

        [Test]
        public void Resolve_INestedInterfaceEmptyConstructor_To_NestedEmptyConstructor_Interface_Dependency ()
        {
            Scope.Install<IEmptyConstructor, EmptyConstructor>(Lifecycle.Singleton);
            Scope.Install<INestedEmptyConstructor, NestedInterfaceEmptyConstructor>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<INestedEmptyConstructor>() is NestedInterfaceEmptyConstructor);
        }

        [Test]
        public void Resolve_IDoubleNestedInterfaceEmptyConstructor_To_DoubleNestedEmptyConstructor_Interface_Dependency ()
        {
            Scope.Install<IEmptyConstructor, EmptyConstructor>(Lifecycle.Singleton);
            Scope.Install<INestedEmptyConstructor, NestedInterfaceEmptyConstructor>(Lifecycle.Singleton);
            Scope.Install<IDoubleNestedInterfaceEmptyConstructor, DoubleNestedInterfaceEmptyConstructor>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<IDoubleNestedInterfaceEmptyConstructor>() is DoubleNestedInterfaceEmptyConstructor);
        }

        [Test]
        public void Resolve_INestedEmptyConstructorMultipleNestedParameters_To_NestedEmptyConstructorMultipleNestedParameters ()
        {
            Scope.Install<IEmptyConstructor, EmptyConstructor>(Lifecycle.Singleton);
            Scope.Install<INestedEmptyConstructor, NestedInterfaceEmptyConstructor>(Lifecycle.Singleton);
            Scope.Install<IDoubleNestedInterfaceEmptyConstructor, DoubleNestedInterfaceEmptyConstructor>(Lifecycle.Singleton);
            Scope.Install<INestedEmptyConstructorMultipleNestedParameters, NestedEmptyConstructorMultipleNestedParameters>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<INestedEmptyConstructorMultipleNestedParameters>() is NestedEmptyConstructorMultipleNestedParameters);
        }

        [Test]
        public void Resolve_INestedEmptyConstructorMultipleNestedParameters_To_NestedEmptyConstructorMultipleNestedParameters_Transient ()
        {
            Scope.Install<IEmptyConstructor, EmptyConstructor>(Lifecycle.Transient);
            Scope.Install<INestedEmptyConstructor, NestedInterfaceEmptyConstructor>(Lifecycle.Transient);
            Scope.Install<IDoubleNestedInterfaceEmptyConstructor, DoubleNestedInterfaceEmptyConstructor>(Lifecycle.Transient);
            Scope.Install<INestedEmptyConstructorMultipleNestedParameters, NestedEmptyConstructorMultipleNestedParameters>(Lifecycle.Transient);
            Assert.IsTrue(Scope.Resolve<INestedEmptyConstructorMultipleNestedParameters>() is NestedEmptyConstructorMultipleNestedParameters);
        }
    }

    class ResolveSingleton : BaseScopeTests
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

    class ResolveTransient : BaseScopeTests
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
            EmptyConstructor secondInstance = Scope.Resolve<EmptyConstructor>();
            Assert.IsNotNull(secondInstance);
            Assert.AreNotEqual(instance, secondInstance);
        }
    }

    class Resolve_NoConstructor : BaseScopeTests
    {
        [Test]
        public void Resolve ()
        {
            Scope.Install<NoConstructor>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<NoConstructor>() is NoConstructor);
        }
    }

    class Resolve_IntConstutor : BaseScopeTests
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

    class Resolve_NestedConstutor : BaseScopeTests
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

    class Resolve_NestedEmptyConstructorMultipleParameters : BaseScopeTests
    {
        [Test]
        public void Resolve_Instance ()
        {
            int intInstance = 1;
            Scope.InstallFromInstance(intInstance);
            Scope.Install<EmptyConstructor>(Lifecycle.Singleton);
            Scope.Install<NestedEmptyConstructorMultipleParameters>(Lifecycle.Singleton);
            Assert.IsTrue(Scope.Resolve<NestedEmptyConstructorMultipleParameters>() is NestedEmptyConstructorMultipleParameters);
        }

        [Test]
        public void Resolve_Instance_Value ()
        {
            int intInstance = 1;
            Scope.InstallFromInstance(intInstance);
            Scope.Install<EmptyConstructor>(Lifecycle.Singleton);
            Scope.Install<NestedEmptyConstructorMultipleParameters>(Lifecycle.Singleton);
            EmptyConstructor emptyConstructorInstance = Scope.Resolve<EmptyConstructor>();
            NestedEmptyConstructorMultipleParameters instance = Scope.Resolve<NestedEmptyConstructorMultipleParameters>();
            Assert.AreEqual(emptyConstructorInstance, instance.Value1);
            Assert.AreEqual(intInstance, instance.Value2);
        }
    }

    class ResolveFromParent : BaseScopeTests
    {
        [Test]
        public void Child_Override_Singleton ()
        {
            Scope parent = Scope;
            Scope child = parent.CreateChildScope();
            EmptyConstructor i1 = new EmptyConstructor();
            EmptyConstructor i2 = new EmptyConstructor();
            parent.InstallFromInstance(i1);
            child.InstallFromInstance(i2);
            Assert.AreEqual(i2, child.Resolve<EmptyConstructor>());
        }

        [Test]
        public void Parent_Singleton ()
        {
            Scope parent = Scope;
            Scope child = parent.CreateChildScope();
            EmptyConstructor i1 = new EmptyConstructor();
            parent.InstallFromInstance(i1);
            Assert.AreEqual(i1, child.Resolve<EmptyConstructor>());
        }

        [Test]
        public void Child_Override_Transient ()
        {
            Scope parent = Scope;
            Scope child = parent.CreateChildScope();
            parent.Install<EmptyConstructor>(Lifecycle.Transient);
            child.Install<EmptyConstructor>(Lifecycle.Transient);
            Assert.IsTrue(child.Resolve<EmptyConstructor>() is EmptyConstructor);
        }

        [Test]
        public void Parent_Transient ()
        {
            Scope parent = Scope;
            Scope child = parent.CreateChildScope();
            parent.Install<EmptyConstructor>(Lifecycle.Transient);
            Assert.IsTrue(child.Resolve<EmptyConstructor>() is EmptyConstructor);
        }

        [Test]
        public void Child_Override_Singleton_With_Transient ()
        {
            Scope parent = Scope;
            Scope child = parent.CreateChildScope();
            EmptyConstructor i1 = new EmptyConstructor();
            parent.InstallFromInstance(i1);
            child.Install<EmptyConstructor>(Lifecycle.Transient);
            EmptyConstructor instance = child.Resolve<EmptyConstructor>();
            Assert.IsNotNull(instance);
            Assert.AreNotEqual(i1, instance);
        }
    }
}
