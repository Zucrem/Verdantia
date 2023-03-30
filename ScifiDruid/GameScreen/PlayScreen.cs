using ScifiDruid.GameObjects;
using ScifiDruid.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScifiDruid.Singleton;

namespace ScifiDruid.GameScreen
{
    class PlayScreen : _GameScreen
    {
        
        protected Player player;

        //all all texture
        protected Texture2D blackTex, whiteTex, greenTex;
        protected Texture2D arrowLeftBGPic, arrowRightBGPic, arrowLeftSFXPic, arrowRightSFXPic;//sound setting pic
        protected Texture2D pausePopUpPic, continueButtonPic, restartButtonPic, exitButtonPic, continueButtonPic2, restartButtonPic2, exitButtonPic2;//for pause setting hovering
        protected Texture2D restartWLPic, nextButtonPic, backWLPic, DefeatSignPic, VictorySignPic;//win or lose
        protected Texture2D confirmExitPopUpPic, noConfirmPic1, yesConfirmPic1, noConfirmPic2, yesConfirmPic2;//for confirm

        protected Texture2D testTexture;
        private FrameCounter _frameCounter = new FrameCounter();
        private float deltaTime;
        private string fps;

        //all button
        //button at pause screen
        protected Button continueButton, restartButton, exitButton;
        protected Button arrowLeftBGButton, arrowRightBGButton, arrowLeftSFXButton, arrowRightSFXButton;//setting button
        //button for confirm exit
        protected Button noConfirmButton, yesConfirmButton;
        //button for win lose
        protected Button nextButton, restartWLButton, exitWLButton;

        //color for fade in and out
        protected Color colorStart;
        protected Color colorEnd;
        //timer
        protected float _timer = 0f;
        protected float _scrollTime = 0f;
        protected float Timer = 0f;
        protected float timerPerUpdate = 0.05f;
        protected float tickPerUpdate = 30f;


        //Game state
        protected bool play = true;
        protected GameState gamestate;
        //change to go another screen
        protected bool nextScreen = false;

        //check if go next page or fade finish for change screen
        protected int alpha = 255;
        protected bool fadeFinish = false;
        protected bool changeScreen = false;
        //if confirm exit
        protected bool confirmExit = false;

        //fonts
        //private SpriteFont Alagan
        protected SpriteFont smallfonts, mediumfonts, bigfonts;//กำหนดชื่อ font
        protected Vector2 fontSize;//ขนาด font จาก SpriteFont

        //sound 
        protected float masterBGM = Singleton.Instance.bgMusicVolume;
        protected float masterSFX = Singleton.Instance.soundMasterVolume;



        protected enum GameState 
        { 
            START, PLAY, WIN, LOSE, PAUSE, EXIT
        }


        public virtual void Initial()
        {
            gamestate = GameState.START;

            //create button on pause
            continueButton = new Button(continueButtonPic, new Vector2((Singleton.Instance.Dimensions.X / 2) - 107, 370), new Vector2(215, 50));
            restartButton = new Button(restartButtonPic, new Vector2((Singleton.Instance.Dimensions.X / 2) - 82, 455), new Vector2(165, 50));
            exitButton = new Button(exitButtonPic, new Vector2((Singleton.Instance.Dimensions.X / 2) - 50, 540), new Vector2(100, 50));

            //create button win or lose screen
            nextButton = new Button(nextButtonPic, new Vector2(540, 430), new Vector2(200, 60));//create Button after win
            restartWLButton = new Button(restartWLPic, new Vector2(540, 510), new Vector2(200, 60));//create Button after win
            exitWLButton = new Button(backWLPic, new Vector2(540, 591), new Vector2(200, 60));//create Button after win

            //setting button
            arrowLeftBGButton = new Button(arrowLeftBGPic, new Vector2(570, 200), new Vector2(40, 40));
            arrowRightBGButton = new Button(arrowRightBGPic, new Vector2(675, 200), new Vector2(40, 40));
            arrowLeftSFXButton = new Button(arrowLeftSFXPic, new Vector2(570, 305), new Vector2(40, 40));
            arrowRightSFXButton = new Button(arrowRightSFXPic, new Vector2(675, 305), new Vector2(40, 40));


            //confirm exit button
            yesConfirmButton = new Button(yesConfirmPic1, new Vector2(495, 390), new Vector2(120, 60));
            noConfirmButton = new Button(noConfirmPic1, new Vector2(710, 390), new Vector2(70, 60));

            player = new Player(testTexture, 99, 164)
            {
                name = "Player Character",
                position = new Vector2(300, 300),
                size = new Vector2(50, 100),
                speed = 10f,
            };

            player.Initial();

        }
        public override void LoadContent()
        {
            base.LoadContent();

            //stageBGPic picture add
            //fade in / out
            blackTex = content.Load<Texture2D>("Pictures/Play/PlayScreen/Black");

            //background
            whiteTex = content.Load<Texture2D>("Pictures/Play/PlayScreen/White");
            greenTex = content.Load<Texture2D>("Pictures/Play/PlayScreen/Green");

            //pause screen
            pausePopUpPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/pausePopUpBG");
            //all button on pausescreen
            continueButtonPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/Continue");
            exitButtonPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/Exit");
            restartButtonPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/Restart");
            continueButtonPic2 = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/ContinueGlow");
            exitButtonPic2 = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/ExitGlow");
            restartButtonPic2 = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/RestartGlow");

            arrowLeftBGPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Sound/ArrowButton");
            arrowRightBGPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Sound/ArrowRButton");
            arrowLeftSFXPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Sound/ArrowButton");
            arrowRightSFXPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Sound/ArrowRButton");

            //add button when win or lose
            nextButtonPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/WinLose/Next");
            restartWLPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/WinLose/Restart");
            backWLPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/WinLose/Mainmenu");
            DefeatSignPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/WinLose/DefeatSign");
            VictorySignPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/WinLose/VictorySign");

            //confirmExit pic
            confirmExitPopUpPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/ConfirmExit/ConfirmExitPopUp");
            yesConfirmPic1 = content.Load<Texture2D>("Pictures/Play/PlayScreen/ConfirmExit/Yes");
            noConfirmPic1 = content.Load<Texture2D>("Pictures/Play/PlayScreen/ConfirmExit/No");
            yesConfirmPic2 = content.Load<Texture2D>("Pictures/Play/PlayScreen/ConfirmExit/YesGlow");
            noConfirmPic2 = content.Load<Texture2D>("Pictures/Play/PlayScreen/ConfirmExit/NoGlow");

            //fonts
            smallfonts = content.Load<SpriteFont>("Fonts/font20");
            bigfonts = content.Load<SpriteFont>("Fonts/font60");
            mediumfonts = content.Load<SpriteFont>("Fonts/font30");

            testTexture = content.Load<Texture2D>("Pictures/Hermes anim");

            Initial();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Singleton.Instance.MousePrevious = Singleton.Instance.MouseCurrent;
            Singleton.Instance.MouseCurrent = Mouse.GetState();

            if (play)
            {
                switch (gamestate)
                {
                    case GameState.START:
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
                        else
                        {
                            gamestate = GameState.PLAY;
                        }
                        break;
                    case GameState.PLAY:
                        //if want to pause
                        player.Walking();

                        //if want to pause
                        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        {
                            play = false;
                            gamestate = GameState.PAUSE;
                            //MediaPlayer.Pause();
                        }
                        break;
                }
            }
            else
            {
                //if not press Exit
                if (!confirmExit)
                {
                    switch (gamestate)
                    {
                        case GameState.WIN:
                            //Next
                            if (nextButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                MediaPlayer.Stop();
                                //ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.MenuScreen);
                            }
                            //Restart
                            if (restartButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                changeScreen = true;
                            }
                            //Exit
                            if (exitWLButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                confirmExit = true;
                            }


                            //Restart Screen
                            if (nextScreen)
                            {
                                switch (Singleton.Instance.levelState)
                                {
                                    case LevelState.LAB:
                                        ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                                        break;
                                }
                            }
                            break;
                        case GameState.LOSE:
                            //Restart
                            if (restartButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                changeScreen = true;
                            }
                            //Exit
                            if (exitWLButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                confirmExit = true;
                            }

                            //Restart Screen
                            if (nextScreen)
                            {
                                switch (Singleton.Instance.levelState)
                                {
                                    case LevelState.FOREST:
                                        ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                                        break;
                                    case LevelState.CITY:
                                        ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                                        break;
                                    case LevelState.LAB:
                                        ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                                        break;
                                }
                            }
                            break;
                        case GameState.PAUSE:
                            //Resume
                            if (continueButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                play = true;
                                gamestate = GameState.PLAY; 
                                //MediaPlayer.Resume();
                            }
                            //Restart
                            if (restartButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                changeScreen = true;
                            }
                            //Exit
                            if (exitButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                confirmExit = true;
                            }

                            //sound setting
                            switch (Singleton.Instance.bgmState)
                            {
                                case AudioState.MUTE:
                                    if (arrowLeftBGButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                                    {
                                        Singleton.Instance.bgmState = AudioState.FULL;
                                    }
                                    if (arrowRightBGButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                                    {
                                        Singleton.Instance.bgmState = AudioState.MEDIUM;
                                    }
                                    break;
                                case AudioState.MEDIUM:
                                    if (arrowLeftBGButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                                    {
                                        Singleton.Instance.bgmState = AudioState.MUTE;
                                    }
                                    if (arrowRightBGButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
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

                            //Restart Screen
                            if (nextScreen)
                            {
                                switch (Singleton.Instance.levelState)
                                {
                                    case LevelState.FOREST:
                                        ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                                        break;
                                    case LevelState.CITY:
                                        ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                                        break;
                                    case LevelState.LAB:
                                        ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                                        break;
                                }
                            }
                            break;
                    }
                }
                //if press Exit
                else if (confirmExit)
                {
                    if (yesConfirmButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                    {
                        changeScreen = true;
                    }
                    if (noConfirmButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                    {
                        confirmExit = false;
                    }

                    //change Screen To MenuScreen
                    if (nextScreen)
                    {
                        changeScreen = true;
                        ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.MenuScreen);
                    }
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

            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _frameCounter.Update(deltaTime);

            fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //background
            spriteBatch.Draw(whiteTex, Vector2.Zero, Color.White);
            if (play)
            {
                //game state
                switch (gamestate)
                {
                    case GameState.START:
                        if (!fadeFinish)
                        {
                            spriteBatch.Draw(blackTex, Vector2.Zero, colorStart);
                        }
                        break;
                    case GameState.PLAY:
                        spriteBatch.DrawString(mediumfonts, fps, new Vector2(1, 1), Color.Black);

                        player.Draw(spriteBatch);
                        break;
                }
            }
            else
            {
                //not play background
                spriteBatch.Draw(greenTex, Vector2.Zero, Color.White);
                if (!confirmExit)
                {
                    //game state
                    switch (gamestate)
                    {
                        case GameState.WIN:
                            spriteBatch.Draw(VictorySignPic, new Vector2(190, 82), new Color(255, 255, 255, 255));
                            //draw only for Stage1 and Stage2
                            if (Singleton.Instance.levelState == LevelState.LAB)
                            {
                                nextButton.Draw(spriteBatch);
                            }
                            restartWLButton.SetPosition(new Vector2(540, 510));
                            exitWLButton.SetPosition(new Vector2(540, 591));
                            restartWLButton.Draw(spriteBatch);
                            exitWLButton.Draw(spriteBatch);
                            break;
                        case GameState.LOSE:
                            restartWLButton.SetPosition(new Vector2(540, 430));
                            exitWLButton.SetPosition(new Vector2(540, 510));
                            spriteBatch.Draw(DefeatSignPic, new Vector2(190, 82), new Color(255, 255, 255, 255));
                            restartWLButton.Draw(spriteBatch);
                            exitWLButton.Draw(spriteBatch);
                            break;
                        case GameState.PAUSE:
                            spriteBatch.Draw(pausePopUpPic, new Vector2((Singleton.Instance.Dimensions.X - pausePopUpPic.Width) / 2, (Singleton.Instance.Dimensions.Y - pausePopUpPic.Height) / 2), new Color(255, 255, 255, 255));
                            fontSize = mediumfonts.MeasureString("Pause");
                            spriteBatch.DrawString(mediumfonts, "Pause", new Vector2(((Singleton.Instance.Dimensions.X - fontSize.X) / 2), 70), new Color(33, 35, 60, 255));
                            //BGM
                            fontSize = mediumfonts.MeasureString("Musics");
                            spriteBatch.DrawString(mediumfonts, "Musics", new Vector2(((Singleton.Instance.Dimensions.X - fontSize.X) / 2), 155), Color.Gray);

                            arrowLeftBGButton.Draw(spriteBatch);
                            arrowRightBGButton.Draw(spriteBatch);

                            //SFX
                            fontSize = mediumfonts.MeasureString("Sounds");
                            spriteBatch.DrawString(mediumfonts, "Sounds", new Vector2(((Singleton.Instance.Dimensions.X - fontSize.X) / 2), 255), Color.Gray);

                            arrowLeftSFXButton.Draw(spriteBatch);
                            arrowRightSFXButton.Draw(spriteBatch);

                            continueButton.Draw(spriteBatch, continueButtonPic2);// Click Arrow BGM button
                            restartButton.Draw(spriteBatch, restartButtonPic2);
                            exitButton.Draw(spriteBatch, exitButtonPic2);
                            switch (Singleton.Instance.bgmState)
                            {
                                case AudioState.MUTE:
                                    fontSize = smallfonts.MeasureString("MUTE");
                                    spriteBatch.DrawString(smallfonts, "MUTE", new Vector2(((Singleton.Instance.Dimensions.X - fontSize.X) / 2) + 5, 208), Color.Gray);
                                    break;
                                case AudioState.MEDIUM:
                                    fontSize = smallfonts.MeasureString("MED");
                                    spriteBatch.DrawString(smallfonts, "MED", new Vector2(((Singleton.Instance.Dimensions.X - fontSize.X) / 2) + 5, 208), Color.Gray);
                                    break;
                                case AudioState.FULL:
                                    fontSize = smallfonts.MeasureString("FULL");
                                    spriteBatch.DrawString(smallfonts, "FULL", new Vector2(((Singleton.Instance.Dimensions.X - fontSize.X) / 2) + 5, 208), Color.Gray);
                                    break;
                            }
                            // Click Arrow SFX button
                            switch (Singleton.Instance.sfxState)
                            {
                                case AudioState.MUTE:
                                    fontSize = smallfonts.MeasureString("MUTE");
                                    spriteBatch.DrawString(smallfonts, "MUTE", new Vector2(((Singleton.Instance.Dimensions.X - fontSize.X) / 2) + 5, 313), Color.Gray);
                                    break;
                                case AudioState.MEDIUM:
                                    fontSize = smallfonts.MeasureString("MED");
                                    spriteBatch.DrawString(smallfonts, "MED", new Vector2(((Singleton.Instance.Dimensions.X - fontSize.X) / 2) + 5, 313), Color.Gray);
                                    break;
                                case AudioState.FULL:
                                    fontSize = smallfonts.MeasureString("FULL");
                                    spriteBatch.DrawString(smallfonts, "FULL", new Vector2(((Singleton.Instance.Dimensions.X - fontSize.X) / 2) + 5, 313), Color.Gray);
                                    break;
                            }
                            break;
                    }
                }
                else if (confirmExit)
                {
                    spriteBatch.Draw(confirmExitPopUpPic, new Vector2((Singleton.Instance.Dimensions.X - confirmExitPopUpPic.Width) / 2, (Singleton.Instance.Dimensions.Y - confirmExitPopUpPic.Height) / 2), new Color(255, 255, 255, 255));
                    fontSize = mediumfonts.MeasureString("Exit");
                    spriteBatch.DrawString(mediumfonts, "Exit", new Vector2(((Singleton.Instance.Dimensions.X - fontSize.X) / 2), 193), new Color(33, 35, 60, 255));
                    fontSize = mediumfonts.MeasureString("Are you sure");
                    spriteBatch.DrawString(mediumfonts, "Are you sure", new Vector2((Singleton.Instance.Dimensions.X - fontSize.X) / 2, 270), Color.DarkGray);
                    fontSize = mediumfonts.MeasureString("you want to exit?");
                    spriteBatch.DrawString(mediumfonts, "you want to exit?", new Vector2((Singleton.Instance.Dimensions.X - fontSize.X) / 2, 325), Color.DarkGray);

                    noConfirmButton.Draw(spriteBatch, noConfirmPic2);
                    yesConfirmButton.Draw(spriteBatch, yesConfirmPic2);
                }

                if (changeScreen)
                {
                    if (fadeFinish)
                    {
                        spriteBatch.Draw(blackTex, Vector2.Zero, colorEnd);
                    }
                }
                if (nextScreen)
                {
                    spriteBatch.Draw(blackTex, Vector2.Zero, Color.Black);
                }
            }
        }
    }
}
