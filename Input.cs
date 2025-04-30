using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class Input : IUpdatable
{
    public static KeyboardState keyboard {get; private set;}
    public static MouseState mouse;
    public static Vector2 move;

    public void Update(float delta)
    {
        keyboard = Keyboard.GetState();
        mouse = Mouse.GetState();

        move.Y = Convert.ToInt32(keyboard.IsKeyDown(Keys.S)) - Convert.ToInt32(keyboard.IsKeyDown(Keys.W));
        move.X = Convert.ToInt32(keyboard.IsKeyDown(Keys.D)) - Convert.ToInt32(keyboard.IsKeyDown(Keys.A));

        //Debug.WriteLine(move);
    }
}