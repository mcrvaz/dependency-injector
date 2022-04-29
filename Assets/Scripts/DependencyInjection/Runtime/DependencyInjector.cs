using System;

namespace DependencyInjectionFramework
{
    public class DependencyInjector : IDisposable
    {
        public Scope RootScope { get; } = new Scope();

        public DependencyInjector () { }

        public void Dispose ()
        {
            RootScope.Dispose();
        }
    }
}
