using System;

namespace DependencyInjectionFramework
{
    public interface IScope : IDisposable
    {
        Scope CreateChildScope (IInstaller installer = null);
        void Register<AbstractT, ConcreteT> (Lifecycle lifecycle);
        void Register<T> (Lifecycle lifecycle);
        void RegisterFromFactory<T> (Func<object> factory, Lifecycle lifecycle);
        void RegisterFromFactory<AbstractT, ConcreteT> (Func<object> factory, Lifecycle lifecycle);
        void RegisterFromInstance<T> (T instance);
        T Resolve<T> ();
    }
}
