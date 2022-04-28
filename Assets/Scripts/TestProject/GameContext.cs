using System;
using DependencyInjectionFramework;

public class GameContext : IDisposable
{
    public Scope Scope { get; private set; }

    readonly GameModel model;

    public GameContext (Scope parentScope, GameModel model)
    {
        this.model = model;
        Scope = parentScope.CreateChildScope(new GameContextInstaller());
    }

    public void Initialize ()
    {
        UnityEngine.Debug.Log("init");
    }

    public void Dispose ()
    {
        UnityEngine.Debug.Log("dispose");
    }
}
