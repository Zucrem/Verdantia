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
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using TiledSharp;
using System.ComponentModel.DataAnnotations;
using Box2DNet.Content;
using Microsoft.Xna.Framework.Audio;
using Verdantia.GameObjects;

namespace ScifiDruid.GameScreen
{
    class PlayScreen : _GameScreen
    {
        //create class player
        protected Player player;

        //all all texture
        protected Texture2D blackTex, whiteTex, greenTex;
        protected Texture2D arrowLeftBGPic, arrowRightBGPic, arrowLeftSFXPic, arrowRightSFXPic;//sound setting pic
        protected Texture2D pausePopUpPic, continueButtonPic, restartButtonPic, exitButtonPic, continueButtonPic2, restartButtonPic2, exitButtonPic2;//for pause setting hovering
        protected Texture2D restartWLPic, nextButtonPic, backWLPic, DefeatSignPic, VictorySignPic;//win or lose
        protected Texture2D confirmExitPopUpPic, noConfirmPic1, yesConfirmPic1, noConfirmPic2, yesConfirmPic2;//for confirm

        protected Texture2D playerTex;

        //all stage texture
        //all stage
        //GameObject
        protected Texture2D switch_wall_Tex;
        protected Texture2D birdTex, crocTex, lionTex;
        //Portrait for dialog
        protected Texture2D dialogBoxTex, birdPortraitTex, soulBirdPortraitTex, crocPortraitTex, soulCrocPortraitTex, lionPortraitTex, soulLionPortraitTex;
        protected Texture2D bossPortraitTex;
        //stage 1
        protected Texture2D flameMechTex, chainsawMechTex, lucasBossTex;
        //dialog 


        //stage 2
        protected Texture2D meleePoliceTex, gunPoliceTex, droneTex, janeBossTex, janeAmmoTex;
        //dialog 


        //stage 3
        protected Texture2D tentacleTex, shieldDogTex, eyeBallTex, doctorBossTex, doctorAmmoTex;
        //dialog 


        private FrameCounter _frameCounter = new FrameCounter();
        private float deltaTime;
        private string fps;
        private Texture2D bullet;
        //private Body _groundBody;

        //player animation
        protected PlayerAnimation playerAnimation;

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
        protected GameState prestate;
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

        //map
        protected TmxMap map;
        protected TileMapManager tilemapManager;
        //player spawn and next map position
        protected Rectangle startRect;
        protected Rectangle endRect;
        //boss state position
        protected Rectangle bossState;

        //check if player is in boss state or not
        protected bool boss_area = false;
        protected bool created_boss = false;
        protected bool getPlayerPosition = true;
        protected Vector2 cameraNow;

        protected Texture2D tilesetStage1, tilesetStage2, tilesetStage3;

        protected int tileWidth;
        protected int tileHeight;
        protected int tilesetTileWidth;
        protected List<Rectangle> collisionRects, deadBlockRects, blockRects, playerRects, mechanicRects, ground1MonsterRects, ground2MonsterRects, flyMonsterRects;
        protected Rectangle bossRect;
        protected Dictionary<Polygon, Vector2> polygon;

        protected float startmaptileX;
        protected float endmaptileX;

        //camera
        protected Camera camera;
        protected Matrix scaleMatrix;

        private bool worldReset = false;

        //stage dialog
        protected int openingDialog = 1;
        protected int introBossDialog = 1;
        protected int endDialog = 1;

        protected int openingDialogCount;
        protected int introDialogCount;
        protected int endDialogCount;
        protected enum GameState 
        { 
            START, OPENING, PLAY, INTROBOSS, BOSS, END, WIN, LOSE, PAUSE, EXIT
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

            player = new Player(playerTex, bullet,whiteTex)
            {
                name = "Player Character",
                size = new Vector2(46, 94),
                //speed = 13,
                speed = 50,
                //speed = 40,
                jumpHigh = 10.5f,
            };

            //camera
            camera = new Camera();
        }
        public override void LoadContent()
        {
            base.LoadContent();
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            //stageBGPic picture add
            //fade in / out
            blackTex = content.Load<Texture2D>("Pictures/Play/PlayScreen/Black");

            //background
            whiteTex = content.Load<Texture2D>("Pictures/Play/PlayScreen/White");
            greenTex = content.Load<Texture2D>("Pictures/Play/PlayScreen/Green");

            //dialog
            dialogBoxTex = content.Load<Texture2D>("Pictures/Play/Dialog/dialogBox");
            //guardian portrait
            birdPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/birdPortriat");
            soulBirdPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/soulBirdPortrait");
            crocPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/crocPortriat");
            soulCrocPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/soulCrocPortrait");
            lionPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/lionPortrait");
            soulLionPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/soulLionPortrait");

            //guardian texture
            birdTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/birdTex");
            crocTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/crocTex");
            lionTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/lionTex");

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

            //player asset and bullet
            playerTex = content.Load<Texture2D>("Pictures/Play/Characters/Player/keeperSheet");
            bullet = content.Load<Texture2D>("Pictures/Play/Skills/PlayerSkills/PlayerSkills");

            //stage1 enemy and all shoot
            flameMechTex = content.Load<Texture2D>("Pictures/Play/Characters/Enemy/flameMech");
            chainsawMechTex = content.Load<Texture2D>("Pictures/Play/Characters/Enemy/chainsawMech");

            lucasBossTex = content.Load<Texture2D>("Pictures/Play/Characters/Boss/LucasSheet");

            //stage2 enemy and all shoot
            meleePoliceTex = content.Load<Texture2D>("Pictures/Play/Characters/Enemy/MeleePolice");
            gunPoliceTex = content.Load<Texture2D>("Pictures/Play/Characters/Enemy/RangePolice");
            droneTex = content.Load<Texture2D>("Pictures/Play/Characters/Enemy/FlyingDrone");

            janeBossTex = content.Load<Texture2D>("Pictures/Play/Characters/Boss/JaneSheet");
            janeAmmoTex = content.Load<Texture2D>("Pictures/Play/Skills/BossSkills/JaneAmmoSheet");

            //stage3 enemy and all shoot
            tentacleTex = content.Load<Texture2D>("Pictures/Play/Characters/Enemy/Tentacle");
            shieldDogTex = content.Load<Texture2D>("Pictures/Play/Characters/Enemy/ShieldDog");
            eyeBallTex = content.Load<Texture2D>("Pictures/Play/Characters/Enemy/Eyeball");

            doctorBossTex = content.Load<Texture2D>("Pictures/Play/Characters/Boss/DrSheet");
            doctorAmmoTex = content.Load<Texture2D>("Pictures/Play/Skills/BossSkills/DrAmmoSheet");

            //song and sfx
            MediaPlayer.IsRepeating = true;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            //sound
            MediaPlayer.Volume = Singleton.Instance.bgMusicVolume;

            //mouse state
            Singleton.Instance.MousePrevious = Singleton.Instance.MouseCurrent;
            Singleton.Instance.MouseCurrent = Mouse.GetState();

            if (play)
            {
                //resume music
                MediaPlayer.Resume();

                //important to make everything move in the world
                Singleton.Instance.world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

                //update player
                player.Update(gameTime);
                if (gamestate == GameState.PLAY || gamestate == GameState.BOSS)
                {
                    player.Action();
                }

                //if want to pause
                if (Keyboard.GetState().IsKeyDown(Keys.Escape) && gamestate != GameState.START)
                {
                    play = false;
                    prestate = gamestate;
                    gamestate = GameState.PAUSE;
                }
                //game state
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
                            gamestate = GameState.OPENING;
                        }
                        break;
                    case GameState.OPENING:
                        //if skip the story dialog
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter) || openingDialog == openingDialogCount)
                        {
                            gamestate = GameState.PLAY;
                        }
                        break;
                    case GameState.PLAY:

                        //camera update for scroll
                        Matrix lastScreen = camera.Follow(player.position, endmaptileX, endmaptileX);

                        if (!boss_area)
                        {
                            Singleton.Instance.tfMatrix = camera.Follow(player.position, startmaptileX, endmaptileX);
                        }
                        else if (Singleton.Instance.tfMatrix.M41 != lastScreen.M41)
                        {
                            if (getPlayerPosition)
                            {
                                getPlayerPosition = false;
                                cameraNow = player.position;
                            }
                            cameraNow += new Vector2((float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                            Singleton.Instance.tfMatrix = camera.Follow(cameraNow, startmaptileX, endmaptileX);
                        }
                        else
                        {
                            Singleton.Instance.tfMatrix = lastScreen;
                            gamestate = GameState.INTROBOSS;
                        }
                        break;
                    case GameState.INTROBOSS:
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter) || introBossDialog == introDialogCount)
                        {
                            gamestate = GameState.BOSS;
                        }
                        break;
                    case GameState.BOSS:
                        break;
                    case GameState.END:
                        //if skip the story dialog
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter) || endDialog == endDialogCount)
                        {
                            if (Singleton.Instance.levelState == LevelState.LAB)
                            {
                                play = false;
                                gamestate = GameState.WIN;
                            }
                            else
                            {
                                resetWorld();
                                if (Singleton.Instance.levelState == LevelState.FOREST)
                                {
                                    Singleton.Instance.levelState = LevelState.CITY;
                                    Singleton.Instance.stageunlock = 2;
                                }
                                else if (Singleton.Instance.levelState == LevelState.CITY)
                                {
                                    Singleton.Instance.levelState = LevelState.LAB;
                                    Singleton.Instance.stageunlock = 3;
                                }
                                play = false;
                            }
                        }
                        break;
                }
                if (player.playerStatus == Player.PlayerStatus.END)
                {
                    play = false;
                    gamestate = GameState.LOSE;
                }
            }
            else
            {
                //pause music
                MediaPlayer.Pause();

                //change camera position
                Singleton.Instance.tfMatrix = Matrix.CreateTranslation(Vector3.Zero);
                //if not press Exit
                if (!confirmExit)
                {
                    switch (gamestate)
                    {
                        case GameState.END:
                            changeScreen = true;

                            //Next Screen
                            if (nextScreen)
                            {
                                ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                            }
                            break;
                        case GameState.WIN:
                            MediaPlayer.Stop();
                            //Next
                            if (nextButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                //unlock another stage
                                //Singleton.Instance.stageunlock++;
                                if (Singleton.Instance.levelState == LevelState.FOREST)
                                {
                                    Singleton.Instance.levelState = LevelState.CITY;
                                }
                                else if (Singleton.Instance.levelState == LevelState.CITY)
                                {
                                    Singleton.Instance.levelState = LevelState.LAB;
                                }
                                resetWorld();
                                changeScreen = true;
                            }
                            //Restart
                            if (restartButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                resetWorld();
                                changeScreen = true;
                            }
                            //Exit
                            if (exitWLButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                confirmExit = true;
                                Singleton.Instance.levelState = LevelState.NULL;
                            }


                            //Restart Screen
                            if (nextScreen)
                            {
                                ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                            }
                            break;
                        case GameState.LOSE:
                            MediaPlayer.Stop();
                            //Restart
                            if (restartButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                resetWorld();
                                changeScreen = true;
                            }
                            //Exit
                            if (exitWLButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                resetWorld();
                                confirmExit = true;
                                Singleton.Instance.levelState = LevelState.NULL;
                            }

                            //Restart Screen
                            if (nextScreen)
                            {
                                ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                            }
                            break;
                        case GameState.PAUSE:
                            //Resume
                            if (continueButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                play = true;
                                gamestate = prestate;
                            }
                            //Restart
                            if (restartButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                if (!worldReset)
                                {
                                    resetWorld();
                                    worldReset = true;
                                }
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
                                ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                            }
                            break;
                    }
                }
                //if press Exit
                else if (confirmExit)
                {
                    if (yesConfirmButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                    {
                        resetWorld();
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
                    masterBGM = 0.4f;
                    Singleton.Instance.bgMusicVolume = masterBGM;
                    break;
                case AudioState.FULL:
                    masterBGM = 0.8f;
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
            //fps show
            fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

            _frameCounter.Update(deltaTime);

            
            base.Update(gameTime);
        }

        public void resetWorld()
        {
            Singleton.Instance.world.ClearForces();
            Singleton.Instance.world.Clear();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (play)
            {
                //in play and boss state only
                if (gamestate == GameState.PLAY || gamestate == GameState.BOSS)
                {
                    if (Player.isAttack)
                    {
                        foreach (PlayerBullet bullet in player.bulletList)
                        {
                            bullet.Draw(spriteBatch);
                        }
                    }
                }
            }
            else
            {
                if (!confirmExit)
                {
                    //not play background
                    if (gamestate == GameState.END)
                    {
                        spriteBatch.Draw(whiteTex, Vector2.Zero, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(greenTex, Vector2.Zero, Color.White);
                    }
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

        public override void DrawFixScreen(SpriteBatch spriteBatch)
        {
            if (play)
            {
                //background
                //spriteBatch.Draw(blackTex, Vector2.Zero, Color.White);
                spriteBatch.Draw(whiteTex, Vector2.Zero, Color.White);
            }
        }
        public override void DrawHUD(SpriteBatch spriteBatch)
        {
            if (play)
            {
                if (play)
                {
                    //draw dialog box, spacebar text and skip text
                    if (gamestate == GameState.OPENING || gamestate == GameState.INTROBOSS || gamestate == GameState.END)
                    {
                        spriteBatch.Draw(dialogBoxTex, new Rectangle(0, 528, 1092, 192), new Rectangle(0, 0, 1092, 192), Color.White);

                        fontSize = smallfonts.MeasureString("Press Spacebar to next");
                        spriteBatch.DrawString(smallfonts, "Press Spacebar to next", new Vector2(797, 690), Color.White);
                    }
                }
                int mana = (int)Player.mana;
                int health = (int)Player.health;

                spriteBatch.DrawString(mediumfonts, health.ToString(), new Vector2(1, 1), Color.Black);
                spriteBatch.DrawString(mediumfonts, mana.ToString(), new Vector2(1, 65), Color.Black);
            }
        }
    }
}
