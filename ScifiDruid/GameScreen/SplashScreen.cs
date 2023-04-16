using ScifiDruid.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ScifiDruid.GameScreen
{
    class SplashScreen : _GameScreen
    {
        private Vector2 fontSize;//ขนาด font ที่เอามา
        private Color color; //เพื่อupdate ค่าความโปร่งสี
        private SpriteFont smallfonts, mediumfonts, bigfonts;//กำหนดชื่อ font
        private Texture2D logoTex, blackTex;//กำหนด ภาพของหน้า splashscreen
        private int alpha; // ค่าความโปร่งสี
        private int displayIndex; // order of index to display splash screen
        private float timer; // Elapsed time in game 
        private float timePerUpdate; // Will do update function when _timer > _timePerUpdate
        private bool Show; // true will fade in and false will fade out

        //bg and sfx sound
        private Song openningTheme;

        public SplashScreen()
        {
            Show = true;
            timePerUpdate = 0.05f;
            displayIndex = 0;
            alpha = 0;
            color = new Color(255, 255, 255, alpha);
        }
        public override void LoadContent()
        {
            base.LoadContent();
            smallfonts = content.Load<SpriteFont>("Fonts/font20");
            bigfonts = content.Load<SpriteFont>("Fonts/font60");
            mediumfonts = content.Load<SpriteFont>("Fonts/font30");
            logoTex = content.Load<Texture2D>("Pictures/Splash/SplashScreen/GameLogo");
            blackTex = content.Load<Texture2D>("Pictures/Splash/SplashScreen/Black");

            //song and sfx
            /*openningTheme = content.Load<Song>("Sounds/OpenningTheme");
            MediaPlayer.Play(openningTheme);*/
        }
        public override void UnloadContent() { base.UnloadContent(); }
        public override void Update(GameTime gameTime)
        {
            Singleton.Instance.MouseCurrent = Mouse.GetState();
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || Singleton.Instance.MouseCurrent.LeftButton == ButtonState.Pressed)
            {
                ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.MenuScreen);
                Singleton.Instance.lastClickTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
            }
            // Add elapsed time to _timer
            timer += (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
            if (timer >= timePerUpdate)
            {
                if (Show)
                {
                    //fade in
                    alpha += 5;
                    // when fade in finish
                    if (alpha >= 250)
                    {
                        Show = false;

                        // transition screen
                        if (displayIndex == 4)
                        {
                            ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.MenuScreen);
                        }
                    }
                }
                else
                {
                    // fade out
                    alpha -= 20;
                    // whene fade out finish
                    if (alpha <= 0)
                    {
                        Show = true;
                        // Change display index and set next display
                        displayIndex++;
                        //change 
                        switch (displayIndex)
                        {
                            case 0:
                                color = Color.Wheat;
                                timePerUpdate -= 0.015f;
                                break;
                            case 1:
                                timePerUpdate += 0.03f;
                                color = Color.SaddleBrown;
                                break;
                            case 2:
                                color = Color.Wheat;
                                timePerUpdate -= 0.015f;
                                break;
                            case 3:
                                timePerUpdate += 0.03f;
                                color = Color.SaddleBrown;
                                break;
                            case 4:
                                timePerUpdate -= 0.035f;
                                break;
                        }
                    }
                }
                timer -= timePerUpdate;
                color.A = (byte)alpha;
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (displayIndex)
            {
                case 0:
                    fontSize = smallfonts.MeasureString("Press spacebar to skip");
                    spriteBatch.DrawString(smallfonts, "Press spacebar to skip", new Vector2((Singleton.Instance.Dimensions.X - fontSize.X), (Singleton.Instance.Dimensions.Y - fontSize.Y)), color);
                    break;
                case 1:
                    fontSize = smallfonts.MeasureString("Teletubbie's Group Present");
                    spriteBatch.DrawString(smallfonts, "Teletubbie's Group Present", new Vector2((Singleton.Instance.Dimensions.X - fontSize.X) / 2, 450), color);
                    break;
                case 2:
                    spriteBatch.Draw(logoTex, new Vector2((Singleton.Instance.Dimensions.X - logoTex.Width) / 2, 200), color);
                    break;
                case 3:
                    fontSize = mediumfonts.MeasureString("The Best 2D Platform Game Ever");
                    spriteBatch.DrawString(mediumfonts, "The Best 2D Platform Game Ever", new Vector2((Singleton.Instance.Dimensions.X - fontSize.X) / 2, 530), color);
                    break;
                case 4:
                    spriteBatch.Draw(blackTex, Vector2.Zero, color);
                    break;
            }
        }
    }
}
