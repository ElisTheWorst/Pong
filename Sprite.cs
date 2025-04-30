using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pong;

public class Sprite {
    public Texture2D texture;
    public string filePath;
    public Vector2 pos = Vector2.Zero, scale = Vector2.One;
    public float rot;
    public bool flipX;
    public int layerDepth = 0;

    public Sprite(string filePath)
    {
        this.filePath = filePath;
        Game1.sprites.Add(this);
    }

    public virtual void Update(float delta)
    {

    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            texture,
            pos,
            null,
            Color.White,
            0f,
            new Vector2(texture.Width / 2, texture.Height),
            scale,
            flipX? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            rot
        );
    }
}