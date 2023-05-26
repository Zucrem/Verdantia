using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScifiDruid.Managers;
using ScifiDruid.GameObjects;
using Microsoft.Xna.Framework.Media;
using static ScifiDruid.Singleton;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Verdantia;
using System.Threading;

namespace ScifiDruid.GameScreen
{
    class MenuScreen : _GameScreen
    {
        //color for fade in
        private Color _Color = new Color(250, 250, 250, 0);

        //all picture
        private Texture2D bgTex, blackTex, settingbgTex;//background
        private Texture2D newGameTex, continueTex, settingTex, exitTex;//mainmenu pic
        private Texture2D arrowLbgTex, arrowRbgTex, arrowLsfxTex, arrowRsfxTex, backSettingTex, selectStageTex, muteTex, medTex, highTex;//setting pic
        private Texture2D confirmQuitPopUpTex, yesText, noText;//exit confirmed pic
        private Texture2D comicBG1, comicBG2, p1Tex, p2Tex, p3Tex, p4Tex, p5Tex, p6_1Tex, p6_2Tex, p7Tex, p8Tex, p9Tex, p10Tex, p11Tex, p12Tex;//intro game

        //all button
        private Button newGameButton, continueButton, settingButton, exitButton;//mainmenu button
        private Button arrowLeftBGButton, arrowRightBGButton, arrowLeftSFXButton, arrowRightSFXButton, backSettingButton;//setting button
        private Button yesButton, noButton;//exit confirmed pic

        //private SpriteFont Alagan
        private SpriteFont smallfonts, mediumfonts, bigfonts;//กำหนดชื่อ font
        private Vector2 fontSize;//ขนาด font จาก SpriteFont

        //bg and sfx sound
        private Song MainmenuTheme;

        //check if go next screen or fade finish
        private bool fadeFinish = false;
        private StateScreen screen = StateScreen.MAINSCREEN;

        //comic
        private int comicPage = 1;
        private enum StateScreen
        {
            MAINSCREEN,
            SETTINGSCREEN,
            QUITSCREEN,
            INTROGAME
        }

        //timer and alpha for fade out screen
        private float _timer = 0.0f;
        private float timerPerUpdate = 0.03f;
        private int alpha = 255;

        //ตัวแปรของหน้า Option
        private float masterBGM = Singleton.Instance.bgMusicVolume;
        private float masterSFX = Singleton.Instance.soundMasterVolume;

        private MenuScreenBG mainBG;

        //delay when press to change dialog GameState
        private float pressTime;
        private float pressTimeDelay = 0.2f;
        public void Initial()
        {
            //bg
            mainBG = new MenuScreenBG(bgTex);
            //main menu button
            newGameButton = new Button(newGameTex, new Vector2(56, 392), new Vector2(144, 32));
            continueButton = new Button(continueTex, new Vector2(56, 454), new Vector2(276, 32));
            settingButton = new Button(settingTex, new Vector2(56, 520), new Vector2(264, 32));
            exitButton = new Button(exitTex, new Vector2(56, 628), new Vector2(140, 32));

            //setting button
            arrowLeftBGButton = new Button(arrowLbgTex, new Vector2(664, 244), new Vector2(32, 56));
            arrowRightBGButton = new Button(arrowRbgTex, new Vector2(968, 244), new Vector2(32, 56));
            arrowLeftSFXButton = new Button(arrowLsfxTex, new Vector2(664, 334), new Vector2(32, 56));
            arrowRightSFXButton = new Button(arrowRsfxTex, new Vector2(968, 334), new Vector2(32, 56));

            backSettingButton = new Button(backSettingTex, new Vector2(1156, 60), new Vector2(60, 60));

            //confirm Exit button
            yesButton = new Button(yesText, new Vector2(401, 452), new Vector2(190, 68));
            noButton = new Button(noText, new Vector2(670, 452), new Vector2(190, 68));
        }
        public override void LoadContent()
        {
            base.LoadContent();
            // Texture2D รูปต่างๆ
            //allbackground
            bgTex = content.Load<Texture2D>("Pictures/Main/MainMenu/MainMenuBG");
            blackTex = content.Load<Texture2D>("Pictures/Main/MainMenu/Black");
            settingbgTex = content.Load<Texture2D>("Pictures/Main/Setting/MainSettingScreen");
            //comic Page at INTROGAME
            comicBG1 = content.Load<Texture2D>("Pictures/Main/INTROGAME/comicBG1");
            comicBG2 = content.Load<Texture2D>("Pictures/Main/INTROGAME/comicBG2");
            p1Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p1");
            p2Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p2");
            p3Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p3");
            p4Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p4");
            p5Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p5");
            p6_1Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p6.1");
            p6_2Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p6.2");
            p7Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p7");
            p8Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p8");
            p9Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p9");
            p10Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p10");
            p11Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p11");
            p12Tex = content.Load<Texture2D>("Pictures/Main/INTROGAME/p12");

            //all pic for button
            //mainscreen pic
            newGameTex = content.Load<Texture2D>("Pictures/Main/MainMenu/playHold");
            continueTex = content.Load<Texture2D>("Pictures/Main/MainMenu/continueHold");
            settingTex = content.Load<Texture2D>("Pictures/Main/MainMenu/settingHold");
            exitTex = content.Load<Texture2D>("Pictures/Main/MainMenu/exitHold");
            //setting pic
            backSettingTex = content.Load<Texture2D>("Pictures/Main/Setting/exitSetting");
            arrowLbgTex = content.Load<Texture2D>("Pictures/Main/Setting/leftArrow");
            arrowRbgTex = content.Load<Texture2D>("Pictures/Main/Setting/rightArrow");
            arrowLsfxTex = content.Load<Texture2D>("Pictures/Main/Setting/leftArrow");
            arrowRsfxTex = content.Load<Texture2D>("Pictures/Main/Setting/rightArrow");
            muteTex = content.Load<Texture2D>("Pictures/Main/Setting/MUTE");
            medTex = content.Load<Texture2D>("Pictures/Main/Setting/MED");
            highTex = content.Load<Texture2D>("Pictures/Main/Setting/FULL");

            //confirmQuit pic
            confirmQuitPopUpTex = content.Load<Texture2D>("Pictures/Main/ConfirmExit/ConfirmQuitPopUp");
            yesText = content.Load<Texture2D>("Pictures/Main/ConfirmExit/Yes");
            noText = content.Load<Texture2D>("Pictures/Main/ConfirmExit/No");

            // Fonts
            smallfonts = content.Load<SpriteFont>("Fonts/font20");
            mediumfonts = content.Load<SpriteFont>("Fonts/font30");
            bigfonts = content.Load<SpriteFont>("Fonts/font60");

            //song and sfx
            /*MainmenuTheme = content.Load<Song>("Sounds/MainmenuTheme");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(MainmenuTheme);*/

            //reset Stage and Char
            Singleton.Instance.levelState = LevelState.NULL;

            // Call Init
            Initial();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            //click sound
            //MediaPlayer.Volume = Singleton.Instance.bgMusicVolume;

            //delay dialog time
            pressTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            Singleton.Instance.MousePrevious = Singleton.Instance.MouseCurrent;
            Singleton.Instance.MouseCurrent = Mouse.GetState();
            switch (screen)
            {
                case StateScreen.INTROGAME:
                    if ((Keyboard.GetState().IsKeyDown(Keys.Space) && pressTime > pressTimeDelay))
                    {
                        comicPage++;
                        pressTime = 0;
                    }
                    if (comicPage >= 16 || Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        Singleton.Instance.levelState = LevelState.FOREST;
                        //Singleton.Instance.levelState = LevelState.CITY;
                        //Singleton.Instance.levelState = LevelState.LAB;
                        ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                    }
                    break;
                case StateScreen.MAINSCREEN:
                    mainBG.Update(gameTime);
                    // Click start new game
                    if (newGameButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                    {
                        Singleton.Instance.stageunlock = 1;

                        screen = StateScreen.INTROGAME;
                    }
                    // Click start continue game
                    if (continueButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                    {
                        if (Singleton.Instance.stageunlock != 1)
                        {
                            if (Singleton.Instance.stageunlock == 2)
                            {
                                Singleton.Instance.levelState = LevelState.CITY;
                            }
                            else if (Singleton.Instance.stageunlock == 3)
                            {
                                Singleton.Instance.levelState = LevelState.LAB;
                            }
                            ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                        }
                    }
                    // Click setting
                    if (settingButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                    {
                        screen = StateScreen.SETTINGSCREEN;
                    }
                    // Click Exit
                    if (exitButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                    {
                        screen = StateScreen.QUITSCREEN;
                    }
                    break;
                case StateScreen.SETTINGSCREEN:
                    // Click Arrow BGM button
                    switch (Singleton.Instance.bgmState)
                    {
                        case AudioState.MUTE:
                            if (arrowLeftBGButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.bgmState = AudioState.FULL;
                            }
                            else if (arrowRightBGButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.bgmState = AudioState.MEDIUM;
                            }
                            break;
                        case AudioState.MEDIUM:
                            if (arrowLeftBGButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.bgmState = AudioState.MUTE;
                            }
                            else if (arrowRightBGButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.bgmState = AudioState.FULL;
                            }
                            break;
                        case AudioState.FULL:
                            if (arrowLeftBGButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.bgmState = AudioState.MEDIUM;
                            }
                            else if (arrowRightBGButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.bgmState = AudioState.MUTE;
                            }
                            break;
                    }
                    // Click Arrow SFX button
                    switch (Singleton.Instance.sfxState)
                    {
                        case AudioState.MUTE:
                            if (arrowLeftSFXButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.sfxState = AudioState.FULL;
                            }
                            else if (arrowRightSFXButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.sfxState = AudioState.MEDIUM;
                            }
                            break;
                        case AudioState.MEDIUM:
                            if (arrowLeftSFXButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.sfxState = AudioState.MUTE;
                            }
                            else if (arrowRightSFXButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.sfxState = AudioState.FULL;
                            }
                            break;
                        case AudioState.FULL:
                            if (arrowLeftSFXButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.sfxState = AudioState.MEDIUM;
                            }
                            else if (arrowRightSFXButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                Singleton.Instance.sfxState = AudioState.MUTE;
                            }
                            break;
                    }

                    // Click back
                    if (backSettingButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                    {
                        screen = StateScreen.MAINSCREEN;
                    }
                    break;
                case StateScreen.QUITSCREEN:
                    if (noButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                    {
                        screen = StateScreen.MAINSCREEN;
                    }
                    else if (yesButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                    {
                        Singleton.Instance.cmdExit = true;
                    }
                    break;
            }

            //fade out
            if (!fadeFinish)
            {
                _timer += (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                if (_timer >= timerPerUpdate)
                {
                    alpha -= 5;
                    _timer -= timerPerUpdate;
                    if (alpha <= 5)
                    {
                        fadeFinish = true;
                    }
                    _Color.A = (byte)alpha;
                }
            }

            //set sound status everytime
            switch (Singleton.Instance.bgmState)
            {
                case AudioState.MUTE:
                    masterBGM = 0f;
                    Singleton.Instance.bgMusicVolume = masterBGM;
                    break;
                case AudioState.MEDIUM:
                    masterBGM = 0.5f;
                    Singleton.Instance.bgMusicVolume = masterBGM;
                    break;
                case AudioState.FULL:
                    masterBGM = 1f;
                    Singleton.Instance.bgMusicVolume = masterBGM;
                    break;
            }

            switch (Singleton.Instance.sfxState)
            {
                case AudioState.MUTE:
                    masterSFX = 0f;
                    Singleton.Instance.soundMasterVolume = masterSFX;
                    break;
                case AudioState.MEDIUM:
                    masterSFX = 0.3f;
                    Singleton.Instance.soundMasterVolume = masterSFX;
                    break;
                case AudioState.FULL:
                    masterSFX = 0.6f;
                    Singleton.Instance.soundMasterVolume = masterSFX;
                    break;
            }

            //button state
            if (Singleton.Instance.stageunlock == 1)
            {
                continueButton.SetCantHover(true);
            }
            else
            {
                continueButton.SetCantHover(false);
            }

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw in each screen
            switch (screen)
            {
                case StateScreen.INTROGAME:
                    //draw
                    if (comicPage >= 0 && comicPage < 9)
                    {
                        spriteBatch.Draw(comicBG1, Vector2.Zero, Color.White);
                        spriteBatch.DrawString(mediumfonts, "Press Spacebar", new Vector2(870, 677), Color.White);

                        if (comicPage > 1)
                        {
                            spriteBatch.Draw(p1Tex, new Vector2(72, 60), Color.White);
                        }
                        if (comicPage > 2)
                        {
                            spriteBatch.Draw(p2Tex, new Vector2(500, 60), Color.White);
                        }
                        if (comicPage > 3)
                        {
                            spriteBatch.Draw(p3Tex, new Vector2(804, 60), Color.White);
                        }
                        if (comicPage > 4)
                        {
                            spriteBatch.Draw(p4Tex, new Vector2(148, 268), Color.White);
                        }
                        if (comicPage > 5)
                        {
                            spriteBatch.Draw(p5Tex, new Vector2(128, 260), Color.White);
                        }
                        if (comicPage == 7)
                        {
                            spriteBatch.Draw(p6_1Tex, new Vector2(696, 404), Color.White);
                        }
                        if (comicPage > 7)
                        {
                            spriteBatch.Draw(p6_2Tex, new Vector2(668, 300), Color.White);
                        }
                    }
                    else if (comicPage > 8 && comicPage <= 15)
                    {
                        spriteBatch.Draw(comicBG2, Vector2.Zero, Color.White);
                        spriteBatch.DrawString(mediumfonts, "Press Spacebar", new Vector2(38, 547), Color.White);

                        if (comicPage > 9)
                        {
                            spriteBatch.Draw(p7Tex, new Vector2(72, 68), Color.White);
                        }
                        if (comicPage > 10)
                        {
                            spriteBatch.Draw(p8Tex, new Vector2(112, 76), Color.White);
                        }
                        if (comicPage > 11)
                        {
                            spriteBatch.Draw(p9Tex, new Vector2(576, 76), Color.White);
                        }
                        if (comicPage > 12)
                        {
                            spriteBatch.Draw(p10Tex, new Vector2(1064, 72), Color.White);
                        }
                        if (comicPage > 13)
                        {
                            spriteBatch.Draw(p11Tex, new Vector2(444, 440), Color.White);
                        }
                        if (comicPage > 14)
                        {
                            spriteBatch.Draw(p12Tex, new Vector2(552, 444), Color.White);
                        }
                    }

                    break;
                case StateScreen.MAINSCREEN:
                    //bg
                    mainBG.Draw(spriteBatch);
                    newGameButton.Draw(spriteBatch);
                    continueButton.Draw(spriteBatch);
                    settingButton.Draw(spriteBatch);
                    exitButton.Draw(spriteBatch);
                    break;
                case StateScreen.SETTINGSCREEN:
                    spriteBatch.Draw(blackTex, Vector2.Zero, Color.White);
                    spriteBatch.Draw(settingbgTex, new Vector2(112, 60), Color.White);
                    backSettingButton.Draw(spriteBatch);

                    arrowLeftBGButton.Draw(spriteBatch);
                    arrowRightBGButton.Draw(spriteBatch);

                    arrowLeftSFXButton.Draw(spriteBatch);
                    arrowRightSFXButton.Draw(spriteBatch);

                    // Click Arrow BGM button
                    switch (Singleton.Instance.bgmState)
                    {
                        case AudioState.MUTE:
                            spriteBatch.Draw(muteTex, new Vector2(736, 244), Color.White);
                            break;
                        case AudioState.MEDIUM:
                            spriteBatch.Draw(medTex, new Vector2(736, 244), Color.White);
                            break;
                        case AudioState.FULL:
                            spriteBatch.Draw(highTex, new Vector2(736, 244), Color.White);
                            break;
                    }
                    // Click Arrow SFX button
                    switch (Singleton.Instance.sfxState)
                    {
                        case AudioState.MUTE:
                            spriteBatch.Draw(muteTex, new Vector2(736, 334), Color.White);
                            break;
                        case AudioState.MEDIUM:
                            spriteBatch.Draw(medTex, new Vector2(736, 334), Color.White);
                            break;
                        case AudioState.FULL:
                            spriteBatch.Draw(highTex, new Vector2(736, 334), Color.White);
                            break;
                    }
                    break;
                case StateScreen.QUITSCREEN:
                    spriteBatch.Draw(blackTex, Vector2.Zero, new Color(255, 255, 255, 210));
                    spriteBatch.Draw(confirmQuitPopUpTex, new Vector2(436, 224), new Color(255, 255, 255, 255));

                    noButton.Draw(spriteBatch, noText);
                    yesButton.Draw(spriteBatch, yesText);
                    break;
            }

            // Draw fade out
            if (!fadeFinish)
            {
                spriteBatch.Draw(blackTex, Vector2.Zero, _Color);
            }
        }
    }
}
