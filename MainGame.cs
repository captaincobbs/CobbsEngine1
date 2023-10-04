using Cobbs_Engine.Components;
using Cobbs_Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;

namespace Cobbs_Engine
{
    public class MainGame : Game
    {
        // Graphics
        public static (uint Width, uint Height) INTENDED_RESOLUTION { get; } = (1280, 720);
        public static float INTENDED_ASPECT_RATIO { get { return INTENDED_RESOLUTION.Width / (float)INTENDED_RESOLUTION.Height; } }
        public static GameWindow GameWindow { get { return Instance.Window; } }
        public static MainGame Instance {
            get { return instance; }
            set { instance ??= value;}
        }

        private static MainGame instance;

        private RenderTarget2D renderTarget { get; set; }
        private Rectangle renderScaleRectangle { get; set; }
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // ECS
        public InputManager Input;

        private Scene currentScene;

        // Configuration
        public static Settings Settings { get; set; }
        private bool DebugOverlay = false;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            Input = new();

            Settings = IO.LoadSettings();
            ApplySettings(Settings);
            Input.AddKeybindings(IO.LoadKeybindings());

            Input.InputActionTriggered += (sender, action) =>
            {
                switch (action)
                {
                    case InputAction.Exit:
                        Exit();
                        break;
                    case InputAction.DebugOverlay:
                        ToggleDebugOverlay();
                        break;
                    case InputAction.Fullscreen:
                        ToggleFullscreen();
                        break;
                    case InputAction.Screenshot:
                        TakeScreenshot();
                        break;
                }
            };

            GameWindow.AllowUserResizing = true;
            GameWindow.IsBorderless = true;
            GameWindow.ClientSizeChanged += MainGame_OnWindowResized;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.HardwareModeSwitch = false;
            graphics.PreferredBackBufferHeight = (int)INTENDED_RESOLUTION.Height;
            graphics.PreferredBackBufferWidth = (int)INTENDED_RESOLUTION.Width;
            graphics.ApplyChanges();

            // Aspect ratio scaling
            renderTarget = new RenderTarget2D(
                graphics.GraphicsDevice,            // Graphics Device
                (int)INTENDED_RESOLUTION.Width,     // Width
                (int)INTENDED_RESOLUTION.Height,    // Height
                false,                              // MipMap
                SurfaceFormat.Color,                // Surface Format
                DepthFormat.None,                   // Depth Format
                0,                                  // PreferredMultiSampleCount
                RenderTargetUsage.DiscardContents   // RenderTargetUsage
            );

            renderScaleRectangle = GetScaleRectangle();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //SwitchScurrentScene();
        }

        protected override void UnloadContent()
        {
            currentScene?.UnloadContent();
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update(gameTime);

            currentScene?.HandleInput(gameTime);
            currentScene?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.White);

            spriteBatch?.Begin();
            currentScene?.Draw(spriteBatch);
            spriteBatch?.End();

            // Scale rendered content
            graphics.GraphicsDevice.SetRenderTarget(null);
            graphics.GraphicsDevice.Clear(ClearOptions.Target, new Color(24, 24, 24), 1.0f, 0);
            spriteBatch?.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch?.Draw(renderTarget, renderScaleRectangle, Color.Black);
            spriteBatch?.End();

            // Continue draw loop
            base.Draw(gameTime);
        }

        private void SwitchScurrentScene(Scene scene)
        {
            if (currentScene != null)
            {
                currentScene.OnSceneSwitched -= CurrentScene_OnSceneSwitched;
                currentScene.OnEventNotification -= CurrentScene_OnEventNotification;
                currentScene.UnloadContent();
            }

            currentScene = scene;
            currentScene.Initialize(Content);
            currentScene.LoadContent();
            currentScene.OnSceneSwitched += CurrentScene_OnSceneSwitched;
            currentScene.OnEventNotification += CurrentScene_OnEventNotification;
        }

        public void ToggleDebugOverlay()
        {
            DebugOverlay = !DebugOverlay;
            Diagnostics.LogDebug($"Debug overlay {(DebugOverlay ? "enabled" : "disabled")}");
        }

        public void ToggleFullscreen()
        {
            graphics.ToggleFullScreen();
            Settings.IsFullscreen = !Settings.IsFullscreen;
            RecalculatePositions();
        }

        public void TakeScreenshot()
        {
            Color[] colors = new Color[GraphicsDevice.Viewport.Width * GraphicsDevice.Viewport.Width];

            GraphicsDevice.GetBackBufferData(colors);

            string filename = $"Screenshot - {DateTime.Now:yyyy-MM-dd @HH-mm-ss}.png";
            using (Texture2D tex2D = new(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height))
            {
                tex2D.SetData(colors);
                using FileStream stream = File.Create(Path.Combine(IO.Paths[PathType.Media], filename));
                tex2D.SaveAsPng(stream, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            }

            Diagnostics.LogDebug($"Screenshot saved to \"{filename}\"");
        }

        private void RecalculatePositions()
        {
            renderScaleRectangle = GetScaleRectangle();
        }

        private Rectangle GetScaleRectangle()
        {
            const float variance = 0.5f;
            float currentAspectRatio = Window.ClientBounds.Width / (float)Window.ClientBounds.Height;

            if (currentAspectRatio <= INTENDED_ASPECT_RATIO)
            {
                int presentHeight = (int)((Window.ClientBounds.Width / INTENDED_ASPECT_RATIO) + variance);
                int barHeight = (Window.ClientBounds.Height - presentHeight) / 2;

                return new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
            }
            else
            {
                int presentWidth = (int)((Window.ClientBounds.Height * INTENDED_ASPECT_RATIO) + variance);
                int barWidth = (Window.ClientBounds.Width - presentWidth) / 2;

                return new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
            }
        }

        private void ApplySettings(Settings settings)
        {
            // Apply resolution & vsync
            graphics.PreferredBackBufferWidth = Settings.Width;
            graphics.PreferredBackBufferHeight = Settings.Height;
            graphics.SynchronizeWithVerticalRetrace = Settings.IsVsync;
            graphics.IsFullScreen = Settings.IsFullscreen;
            graphics.HardwareModeSwitch = !Settings.IsBorderless;
            graphics.ApplyChanges();

            Diagnostics.LogMessage("Settings Applied");
        }

        private void MainGame_OnWindowResized(object sender, EventArgs e)
        {
            RecalculatePositions();
        }

        private void CurrentScene_OnSceneSwitched(object sender, Scene e)
        {

        }

        private void CurrentScene_OnEventNotification(object sender, ISceneEvent e)
        {

        }
    }
}