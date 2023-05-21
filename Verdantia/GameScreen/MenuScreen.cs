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

namespace ScifiDruid.GameScreen
{
    class MenuScreen : _GameScreen
    {
        //color for fade in
        private Color _Color = new Color(250, 250, 250, 0);

        //all picture
        private Texture2D bgTex, blackTex, settingbgTex;//background
        private Texture2D logoTex, newGameTex, continueTex, settingTex, exitTex;//mainmenu pic
        private Texture2D arrowLbgTex, arrowRbgTex, arrowLsfxTex, arrowRsfxTex, backSettingTex, selectStageTex;//setting pic
        private Texture2D confirmQuitPopUpTex, yesText1, noText1, yesText2, noText2;//exit confirmed pic

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
        private enum StateScreen
        {
            MAINSCREEN,
            SETTINGSCREEN,
            QUITSCREEN,
            SELECTSTAGESCREEN
        }

        //check if press arrow at howtoplay screen
        private int howToPlaySlide = 1;

        //timer and alpha for fade out screen
        private float _timer = 0.0f;
        private float timerPerUpdate = 0.03f;
        private int alpha = 255;

        //ตัวแปรของหน้า Option
        private float masterBGM = Singleton.Instance.bgMusicVolume;
        private float masterSFX = Singleton.Instance.soundMasterVolume;

        public void Initial()
        {
            //main menu button
            newGameButton = new Button(newGameTex, new Vector2(273, 293), new Vector2(160, 80));
            continueButton = new Button(continueTex, new Vector2(273, 622), new Vector2(160, 80));
            settingButton = new Button(settingTex, new Vector2(273, 410), new Vector2(160, 60));
            exitButton = new Button(exitTex, new Vector2(273, 505), new Vector2(100, 45));

            //setting button
            arrowLeftBGButton = new Button(arrowLbgTex, new Vector2(525, 320), new Vector2(40, 40));
            arrowRightBGButton = new Button(arrowRbgTex, new Vector2(705, 320), new Vector2(40, 40));
            arrowLeftSFXButton = new Button(arrowLsfxTex, new Vector2(525, 440), new Vector2(40, 40));
            arrowRightSFXButton = new Button(arrowRsfxTex, new Vector2(705, 440), new Vector2(40, 40));

            backSettingButton = new Button(backSettingTex, new Vector2(980, 570), new Vector2(150, 60));

            //confirm Exit button
            yesButton = new Button(yesText1, new Vector2(495, 390), new Vector2(120, 60));
            noButton = new Button(noText1, new Vector2(710, 390), new Vector2(70, 60));
        }
        public override void LoadContent()
        {
            base.LoadContent();
            // Texture2D รูปต่างๆ
            //allbackground
            logoTex = content.Load<Texture2D>("Pictures/Main/MainMenu/GameLogo");
            bgTex = content.Load<Texture2D>("Pictures/Main/MainMenu/MainMenuBG");
            blackTex = content.Load<Texture2D>("Pictures/Main/MainMenu/Black");
            settingbgTex = content.Load<Texture2D>("Pictures/Main/Setting/SettingBG");

            //all pic for button
            //mainscreen pic
            newGameTex = content.Load<Texture2D>("Pictures/Main/MainMenu/PlayButton");
            continueTex = content.Load<Texture2D>("Pictures/Main/MainMenu/PlayButton");
            settingTex = content.Load<Texture2D>("Pictures/Main/MainMenu/SettingButton");
            exitTex = content.Load<Texture2D>("Pictures/Main/MainMenu/ExitButton");
            //setting pic
            backSettingTex = content.Load<Texture2D>("Pictures/Main/Setting/DoneButton");
            arrowLbgTex = content.Load<Texture2D>("Pictures/Main/Setting/ArrowButton");
            arrowRbgTex = content.Load<Texture2D>("Pictures/Main/Setting/ArrowRButton");
            arrowLsfxTex = content.Load<Texture2D>("Pictures/Main/Setting/ArrowButton");
            arrowRsfxTex = content.Load<Texture2D>("Pictures/Main/Setting/ArrowRButton");

            //confirmQuit pic
            confirmQuitPopUpTex = content.Load<Texture2D>("Pictures/Main/ConfirmExit/ConfirmQuitPopUp");
            yesText1 = content.Load<Texture2D>("Pictures/Main/ConfirmExit/Yes");
            noText1 = content.Load<Texture2D>("Pictures/Main/ConfirmExit/No");
            yesText2 = content.Load<Texture2D>("Pictures/Main/ConfirmExit/YesGlow");
            noText2 = content.Load<Texture2D>("Pictures/Main/ConfirmExit/NoGlow");

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

            Singleton.Instance.MousePrevious = Singleton.Instance.MouseCurrent;
            Singleton.Instance.MouseCurrent = Mouse.GetState();
            switch (screen)
            {
                case StateScreen.MAINSCREEN:
                    // Click start new game
                    if (newGameButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                    {
                        Singleton.Instance.levelState = LevelState.FOREST;
                        //Singleton.Instance.levelState = LevelState.CITY;
                        //Singleton.Instance.levelState = LevelState.LAB;
                        ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);

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
                case StateScreen.MAINSCREEN:
                    spriteBatch.Draw(bgTex, Vector2.Zero, Color.White);//bg
                    spriteBatch.Draw(logoTex, new Vector2(865, 130), new Color(255, 255, 255, 255));
                    newGameButton.Draw(spriteBatch);
                    continueButton.Draw(spriteBatch);
                    settingButton.Draw(spriteBatch);
                    exitButton.Draw(spriteBatch);
                    break;
                case StateScreen.SETTINGSCREEN:
                    spriteBatch.Draw(settingbgTex, Vector2.Zero, Color.White);
                    backSettingButton.Draw(spriteBatch);
                    spriteBatch.DrawString(bigfonts, "Setting", new Vector2(550, 165), Color.Gray);

                    //BGM
                    spriteBatch.DrawString(mediumfonts, "Musics", new Vector2(300, 324), Color.Gray);

                    arrowLeftBGButton.Draw(spriteBatch);
                    arrowRightBGButton.Draw(spriteBatch);

                    //SFX
                    spriteBatch.DrawString(mediumfonts, "Sounds", new Vector2(300, 444), Color.Gray);

                    arrowLeftSFXButton.Draw(spriteBatch);
                    arrowRightSFXButton.Draw(spriteBatch);

                    // Click Arrow BGM button
                    switch (Singleton.Instance.bgmState)
                    {
                        case AudioState.MUTE:
                            spriteBatch.DrawString(mediumfonts, "MUTE", new Vector2(585, 324), Color.Gray);
                            break;
                        case AudioState.MEDIUM:
                            spriteBatch.DrawString(mediumfonts, "MED", new Vector2(585, 324), Color.Gray);
                            break;
                        case AudioState.FULL:
                            spriteBatch.DrawString(mediumfonts, "FULL", new Vector2(585, 324), Color.Gray);
                            break;
                    }
                    // Click Arrow SFX button
                    switch (Singleton.Instance.sfxState)
                    {
                        case AudioState.MUTE:
                            spriteBatch.DrawString(mediumfonts, "MUTE", new Vector2(580, 444), Color.Gray);
                            break;
                        case AudioState.MEDIUM:
                            spriteBatch.DrawString(mediumfonts, "MED", new Vector2(580, 444), Color.Gray);
                            break;
                        case AudioState.FULL:
                            spriteBatch.DrawString(mediumfonts, "FULL", new Vector2(580, 445), Color.Gray);
                            break;
                    }
                    break;
                case StateScreen.QUITSCREEN:
                    spriteBatch.Draw(blackTex, Vector2.Zero, new Color(255, 255, 255, 210));
                    spriteBatch.Draw(confirmQuitPopUpTex, new Vector2((Singleton.Instance.Dimensions.X - confirmQuitPopUpTex.Width) / 2, (Singleton.Instance.Dimensions.Y - confirmQuitPopUpTex.Height) / 2), new Color(255, 255, 255, 255));
                    fontSize = mediumfonts.MeasureString("Are you sure");
                    spriteBatch.DrawString(mediumfonts, "Are you sure", new Vector2((Singleton.Instance.Dimensions.X - fontSize.X) / 2, 255), Color.DarkGray);
                    fontSize = mediumfonts.MeasureString("you want to quit?");
                    spriteBatch.DrawString(mediumfonts, "you want to quit?", new Vector2((Singleton.Instance.Dimensions.X - fontSize.X) / 2, 310), Color.DarkGray);

                    noButton.Draw(spriteBatch, noText2);
                    yesButton.Draw(spriteBatch, yesText2);
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
