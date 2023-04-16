using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScifiDruid.Managers;
using System;

namespace ScifiDruid
{
    public class ScifiDruid : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public ScifiDruid()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            
            _graphics.PreferredBackBufferWidth = (int)Singleton.Instance.Dimensions.X;
            _graphics.PreferredBackBufferHeight = (int)Singleton.Instance.Dimensions.Y;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            Singleton.Instance.CenterScreen = new Vector2(_graphics.PreferredBackBufferWidth / 2f, _graphics.PreferredBackBufferHeight / 2f);
            //IsFixedTimeStep = false;
            IsMouseVisible = true;
            _graphics.IsFullScreen = false;
            //Window.AllowUserResizing = true;
            //Window.IsBorderless = false;// make window borderless

            //Singleton.Instance.tfMatrix = Matrix.CreateTranslation(new Vector3(Singleton.Instance.CenterScreen, 0f)) * Matrix.CreateTranslation(new Vector3(Singleton.Instance.CenterScreen, 0f));
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
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin(transformMatrix : Singleton.Instance.tfMatrix);
            //_spriteBatch.Begin();
            ScreenManager.Instance.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}