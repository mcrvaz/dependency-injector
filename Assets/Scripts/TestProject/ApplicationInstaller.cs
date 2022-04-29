using DependencyInjectionFramework;

public class ApplicationInstaller : IInstaller
{
    public void Install (IScope scope) => Install((GameObjectScope)scope);
    public void Install (GameObjectScope scope)
    {
        scope.RegisterFromInstance<Scope>(scope);
        scope.Register<IInstaller, GameContextInstaller>(Lifecycle.Singleton);
        scope.Register<GameContext>(Lifecycle.Singleton);
        scope.Register<GameModel>(Lifecycle.Singleton);
        scope.Register<GameController>(Lifecycle.Singleton);
        scope.RegisterFromNewGameObject<GameView>("GameView", Lifecycle.Singleton);
    }
}
