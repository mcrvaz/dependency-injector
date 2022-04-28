using DependencyInjectionFramework;

public class ApplicationInstaller : IInstaller
{
    public void Install (Scope scope)
    {
        scope.RegisterFromInstance(scope);
        scope.Register<IInstaller, GameContextInstaller>(Lifecycle.Singleton);
        scope.Register<GameContext>(Lifecycle.Singleton);
        scope.Register<GameModel>(Lifecycle.Singleton);
    }
}
