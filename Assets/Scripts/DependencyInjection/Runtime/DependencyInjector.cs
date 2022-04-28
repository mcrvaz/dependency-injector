using System;

namespace DependencyInjectionFramework
{
    public class DependencyInjector : IDisposable
    {
        public Scope RootScope { get; } = new Scope();

        public void Dispose ()
        {
            RootScope.Dispose();
        }
    }
}
