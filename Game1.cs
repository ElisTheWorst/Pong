using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;
using FMOD.Studio;

namespace Pong;

public class Game1 : Game
{
    GameManager gameManager;
    Player player;

    public static List<Sprite> sprites = new List<Sprite>();
    public static GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    public static ImGuiRenderer GuiRenderer;
    private bool _toolActive;





    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            GraphicsProfile = GraphicsProfile.HiDef
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        gameManager = new();
        player = new();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        Window.Title = "I am about to bust";
        GuiRenderer = new ImGuiRenderer(this);
        _toolActive = true;

        Sound.Initialize();
        player.Initialize();
        
        Sprite grassMoment = new Sprite("Grass");
        grassMoment.pos = new(0,100);

        //I'm gay and also racist.

        base.Initialize();

        // ViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
        // camera = new(viewportAdapter);
    }

    protected override void LoadContent()
    {
        //GraphicsDevice.SamplerStates[0] = new SamplerState {Filter = TextureFilter.Point};
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        GuiRenderer.RebuildFontAtlas();

        Sound.Load();

        // TODO: use this.Content to load your game content here

        foreach(Sprite sprite in sprites) sprite.texture = Content.Load<Texture2D>(sprite.filePath);

        _background = Content.Load<Texture2D>("Grid");
        _infinite = Content.Load<Effect>("infinite");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        // TODO: Add your update logic here


        // The time since update was called last.
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Sound.Update();
        gameManager.Update(delta);
        Input.Update(delta);
        player.Update(delta);
        Camera.Update(delta);
        


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);


        Debug.WriteLine(GraphicsDevice.SamplerStates);

        int width = GraphicsDevice.Viewport.Width;
        int height = GraphicsDevice.Viewport.Height;

        Matrix transformMatrix = Camera.GetViewMatrix();

        // Render Grid Background.
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);
            Matrix uv_transform = GetUVTransform(_background, -Vector2.UnitY, 1, GraphicsDevice.Viewport);
            

            _infinite.Parameters["view_projection"].SetValue(Matrix.Identity * projection);
            _infinite.Parameters["uv_transform"].SetValue(Matrix.Invert(uv_transform));


            _spriteBatch.Begin(transformMatrix: transformMatrix, effect: _infinite, samplerState: SamplerState.PointWrap);
            _spriteBatch.Draw(_background, GraphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();
        }

        // Loops through the sprites list and renders each one.
        _spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        foreach(Sprite sprite in sprites) sprite.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);

        // ImGui Rendering
        GuiRenderer.BeginLayout(gameTime);
        if (_toolActive)
        {
            ImGui.Begin("Dev Menu: Fuck you", ref _toolActive, ImGuiWindowFlags.MenuBar);
            if (ImGui.BeginMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open..", "Ctrl+O")) { /* Do stuff */ }
                    if (ImGui.MenuItem("Save", "Ctrl+S")) { /* Do stuff */ }
                    if (ImGui.MenuItem("Close", "Ctrl+W")) { _toolActive = false; }
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }
            ImGui.SliderFloat("CameraZoom", ref Camera.zoom, .1f, 5f);
            ImGui.End();
        }
        GuiRenderer.EndLayout();
    }

    protected override void UnloadContent()
    {
        Sound.UnLoad();
        base.UnloadContent();
    }

    private Matrix GetUVTransform(Texture2D t, Vector2 offset, float scale, Viewport v) {
        return
            Matrix.CreateScale(t.Width, t.Height, 1f) *
            Matrix.CreateScale(scale, scale, 1f) *
            Matrix.CreateTranslation(offset.X, offset.Y, 0f) *
            Camera.GetViewMatrix() *
            Matrix.CreateScale(1f / v.Width, 1f / v.Height, 1f);
    }

    Texture2D _background;
    Effect _infinite;
}
