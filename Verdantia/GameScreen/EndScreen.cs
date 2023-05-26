using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;
using ScifiDruid;
using ScifiDruid.GameObjects;
using ScifiDruid.GameScreen;
using ScifiDruid.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Verdantia.GameScreen
{
    class EndScreen : _GameScreen
    {
        //bg
        private Texture2D endBG, blackBG;

        //fonts
        private SpriteFont smallfonts, mediumfonts, bigfonts, kongfonts;//กำหนดชื่อ font
        private Vector2 fontSize;//ขนาด font ที่เอามา

        //fade
        private int alpha = 255;
        private bool fadeFinish = false;
        private bool changeScreen = false;
        //timer
        private float _timer = 0f;
        private float _scrollTime = 0f;
        private float Timer = 0f;
        private float timerPerUpdate = 0.05f;
        protected float tickPerUpdate = 30f;

        //color for fade in and out
        private Color colorStart;
        private Color colorEnd;

        //change to go another screen
        protected bool nextScreen = false;


        public void Initial()
        {
        }
        public override void LoadContent()
        {
            base.LoadContent();
            endBG = content.Load<Texture2D>("Pictures/Play/EndScreen/EndCredit");
            blackBG = content.Load<Texture2D>("Pictures/Main/MainMenu/Black");

            //fonts
            smallfonts = content.Load<SpriteFont>("Fonts/font20");
            bigfonts = content.Load<SpriteFont>("Fonts/font60");
            mediumfonts = content.Load<SpriteFont>("Fonts/font30");

            Initial();
        }
        public override void Update(GameTime gameTime)
        {
            //fade in
            if (!fadeFinish)
            {
                _timer += (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                // fade out when start
                if (_timer >= timerPerUpdate)
                {
                    alpha -= 10;
                    _timer -= timerPerUpdate;
                    if (alpha <= 10)
                    {
                        fadeFinish = true;
                    }
                    colorStart.A = (byte)alpha;
                }
            }

            //next map
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                changeScreen = true;
            }

            //fade in
            if (changeScreen)
            {
                if (fadeFinish)
                {
                    _timer += (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                    // fade out when start
                    if (_timer >= timerPerUpdate)
                    {
                        alpha += 10;
                        _timer -= timerPerUpdate;
                        if (alpha > 245)
                        {
                            fadeFinish = false;
                            nextScreen = true;
                        }
                        colorEnd.A = (byte)alpha;
                    }
                }
            }
            if (nextScreen)
            {
                ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.MenuScreen);
            }

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //bg
            spriteBatch.Draw(endBG, Vector2.Zero, Color.White);

            fontSize = mediumfonts.MeasureString("Press Spacebar to Mainmenu");
            spriteBatch.DrawString(mediumfonts, "Press Spacebar to Mainmenu", new Vector2((Singleton.Instance.Dimensions.X - fontSize.X) / 2, 680), Color.White);

            //fade
            if (fadeFinish)
            {
                spriteBatch.Draw(blackBG, Vector2.Zero, colorEnd);
            }
            if (!fadeFinish)
            {
                spriteBatch.Draw(blackBG, Vector2.Zero, colorStart);
            }
        }
    }
}
