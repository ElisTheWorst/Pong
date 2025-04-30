
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

public class GameManager : IUpdatable
{
    public static GameState gameState;

    public GameManager()
    {

    }

    public void Restart()
    {

    }

    public void Update(float delta)
    {

    }

    public void Draw()
    {
        
    }
}

public enum GameState
{
    MainMenu,
    Paused,
    Inventory,
    Crafting,
    Playing
}