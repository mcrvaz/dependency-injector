using System;
using Cysharp.Threading.Tasks;
using DependencyInjectionFramework;
using UnityEngine.SceneManagement;

public class GameContext : IDisposable
{
    public Scope Scope { get; private set; }

    readonly GameModel model;
    readonly GameController controller;
    readonly GameView view;
    readonly Scope parentScope;

    public GameContext (
        Scope parentScope,
        GameModel model,
        GameController controller,
        GameView view
    )
    {
        this.model = model;
        this.controller = controller;
        this.view = view;
        this.parentScope = parentScope;
    }

    public async UniTask Initialize ()
    {
        Scope = await SceneScope.FromNewSceneAsync(
            parentScope,
            new GameContextInstaller(),
            "GameScene",
            LoadSceneMode.Additive
        );
    }

    public void Dispose () { }
}
