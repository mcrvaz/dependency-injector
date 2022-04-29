public class GameController
{
    readonly GameModel model;
    readonly GameView view;

    public GameController (GameModel model, GameView view)
    {
        this.model = model;
        this.view = view;
    }
}
