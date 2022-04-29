using System;
using Cysharp.Threading.Tasks;
using DependencyInjectionFramework;

public class ApplicationContext : IDisposable
{
    public Scope Scope { get; private set; }

    readonly DependencyInjector injector = new DependencyInjector();

    public ApplicationContext ()
    {
        Scope = GameObjectScope.FromNewGameObject(
            injector.RootScope,
            new ApplicationInstaller(),
            "ApplicationContext"
        );
    }

    public async UniTask Initialize ()
    {
        GameContext gameContext = Scope.Resolve<GameContext>();
        await gameContext.Initialize();
    }

    public void Dispose ()
    {
        injector.Dispose();
    }
}
