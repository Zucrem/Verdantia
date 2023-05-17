using Box2DNet.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScifiDruid.Managers;
using System;
using System.Diagnostics;

namespace ScifiDruid
{
    public class Verdantia : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private bool b = true;

        public Verdantia()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int)Singleton.Instance.Dimensions.X,
                PreferredBackBufferHeight = (int)Singleton.Instance.Dimensions.Y
            };
            Content.RootDirectory = "Content";

            //Singleton.Instance.CenterScreen = new Vector2(_graphics.PreferredBackBufferWidth / 2f, _graphics.PreferredBackBufferHeight / 2f);

            //_graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;
            IsMouseVisible = true;
            _graphics.IsFullScreen = false;
            //Window.AllowUserResizing = true;
            //Window.IsBorderless = false;// make window borderless

            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //IsFixedTimeStep = true;
            //double temp = (1000d / (double)144) * 10000d;
            //TargetElapsedTime = new TimeSpan((long)temp);
            Singleton.Instance.CenterScreen = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);

            Singleton.Instance.tfMatrix = Matrix.CreateTranslation(Vector3.Zero);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ScreenManager.Instance.LoadContent(Content);
        }
        protected override void UnloadContent()
        {
            ScreenManager.Instance.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            ScreenManager.Instance.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.I))
            {
                _graphics.PreferredBackBufferHeight = 1080;
                _graphics.PreferredBackBufferWidth = 1920;
                _graphics.ApplyChanges();
            }
            if (Singleton.Instance.cmdExit)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            ScreenManager.Instance.DrawFixScreen(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin(transformMatrix : Singleton.Instance.tfMatrix);
            ScreenManager.Instance.Draw(_spriteBatch);
            _spriteBatch.End();

            

            base.Draw(gameTime);
        }
    }
}