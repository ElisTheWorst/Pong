using Microsoft.Xna.Framework;
using Pong;

public class Camera
{
    public static float zoom = 1.5f;
    public static float rot;
    static float offset = -5;
    public static Sprite target;
    public static Vector2 pos {get; private set;}

    // public Camera(ViewportAdapter viewportAdapter)
    // {
    //     camera = new(viewportAdapter);
    // }


    // Update is called once per frame
    public static void Update(float delta)
    {
        Vector2 tarPos = target.pos - new Vector2((float)Game1._graphics.GraphicsDevice.Viewport.Width / 2f, (float)Game1._graphics.GraphicsDevice.Viewport.Height / 2f); //position - new Vector2((float)_viewportAdapter.VirtualWidth / 2f, (float)_viewportAdapter.VirtualHeight / 2f)

        tarPos.Y += offset;

        pos = Vector2.Lerp(pos, tarPos, delta * 15);
    }

    //public Matrix GetViewMatrix() => camera.GetViewMatrix();

    public static Matrix GetViewMatrix() 
    {
        int width = Game1._graphics.GraphicsDevice.Viewport.Width;
        int height = Game1._graphics.GraphicsDevice.Viewport.Height;
        Vector2 origin = new(width / 2f, height / 2f);

        return
            Matrix.CreateTranslation(-origin.X, -origin.Y, 0f) *
            Matrix.CreateTranslation(new(-pos.X, -pos.Y, 0)) *
            Matrix.CreateRotationZ(rot) *
            Matrix.CreateScale(zoom * Vector3.One) *
            Matrix.CreateTranslation(origin.X, origin.Y, 0f);
    }

    public static void SetTarget(ref Sprite _target) { target = _target;}
}