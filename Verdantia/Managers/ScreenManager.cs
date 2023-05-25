using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScifiDruid.GameScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static ScifiDruid.Singleton;
using Verdantia.GameScreen;

namespace ScifiDruid.Managers
{
    public class ScreenManager
    {
        public ContentManager Content { private set; get; }
        public enum GameScreenName
        {
            MenuScreen,
            PlayScreen,
            PerkScreen
        }
        //object gamescreen
        private _GameScreen currentScreen;

        //constructer ให้ current state == หน้าโหลด or หน้าปิด
        public ScreenManager()
        {
            currentScreen = new SplashScreen();
        }
        public void LoadScreen(GameScreenName screenName)
        {
            switch (screenName)
            {
                //ถ้าไปหน้าmenu
                case GameScreenName.MenuScreen:
                    currentScreen = new MenuScreen();
                    break;
                //ถ้าไปหน้าเล่นเกมด่านต่างๆ
                case GameScreenName.PlayScreen:
                    switch (Singleton.Instance.levelState)
                    {
                        case LevelState.FOREST:
                            currentScreen = new Stage1Screen();
                            break;
                        case LevelState.CITY:
                            currentScreen = new Stage2Screen();
                            break;
                        case LevelState.LAB:
                            currentScreen = new Stage3Screen();
                            break;
                    }
                    break;
                case GameScreenName.PerkScreen:
                    currentScreen = new PerkScreen();
                    break;
            }
            currentScreen.LoadContent();
        }
        public void LoadContent(ContentManager Content)
        {
            this.Content = new ContentManager(Content.ServiceProvider, "Content");
            currentScreen.LoadContent();
        }
        public void UnloadContent()
        {
            currentScreen.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
        }
        public void DrawFixScreen(SpriteBatch spriteBatch)
        {
            currentScreen.DrawFixScreen(spriteBatch);
        }

        public void DrawHUD(SpriteBatch spriteBatch)
        {
            currentScreen.DrawHUD(spriteBatch);
        }

        private static ScreenManager instance;
        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ScreenManager();
                return instance;
            }
        }
    }
}
