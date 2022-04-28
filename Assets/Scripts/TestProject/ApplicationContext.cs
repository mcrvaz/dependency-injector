using System;
using DependencyInjectionFramework;

public class ApplicationContext : IDisposable
{
    public Scope Scope { get; private set; }
    readonly DependencyInjector injector = new DependencyInjector();

    public ApplicationContext ()
    {
        Scope = injector.RootScope.CreateChildScope(new ApplicationInstaller());
    }

    public void Initialize ()
    {
        GameContext gameContext = Scope.Resolve<GameContext>();
        gameContext.Initialize();
    }

    public void Dispose ()
    {
        injector.Dispose();
    }
}
