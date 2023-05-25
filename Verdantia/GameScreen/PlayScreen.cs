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
using System.Threading;
using System.Diagnostics.Metrics;

namespace ScifiDruid.GameScreen
{
    class PlayScreen : _GameScreen
    {
        //create class player
        protected Player player;

        //all all texture
        protected Texture2D blackTex, whiteTex, greenTex, gameoverTex;
        protected Texture2D arrowLeftBGPic, arrowRightBGPic, arrowLeftSFXPic, arrowRightSFXPic;//sound setting pic
        protected Texture2D pausePopUpPic, continueButtonPic, restartButtonPic, exitButtonPic, muteTex, medTex, highTex;//for pause setting hovering
        protected Texture2D restartLPic, backLPic;//win or lose
        protected Texture2D confirmExitPopUpPic, noConfirmPic, yesConfirmPic;//for confirm

        protected Texture2D playerTex;

        //all stage texture
        protected Texture2D stage1BG, stage2BG, stage3BG;
        //all stage
        //GameObject
        protected Texture2D switch_wall_Tex;
        protected Texture2D birdTex, crocTex, lionTex;
        //HUD
        protected Texture2D healthBarTex, healthTex, manaBarTex, manaTex, skillBoxTex, healRTex, healNotRTex, dashRTex, dashNotRTex, crocRTex, crocNotRTex, lionRTex, lionNotRTex;
        //Portrait for dialog
        protected Texture2D dialogBoxTex, birdPortraitTex, soulBirdPortraitTex, crocPortraitTex, soulCrocPortraitTex, lionPortraitTex, soulLionPortraitTex;
        protected Texture2D bossPortraitTex;
        //stage 1
        protected Texture2D flameMechTex, chainsawMechTex, lucasBossTex;
        //stage 2
        protected Texture2D meleePoliceTex, gunPoliceTex, droneTex, janeBossTex, janeAmmoTex;
        //stage 3
        protected Texture2D tentacleTex, shieldDogTex, eyeBallTex, doctorBossTex, doctorAmmoTex;

        private FrameCounter _frameCounter = new FrameCounter();
        private float deltaTime;
        private string fps;
        protected Texture2D bullet;
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
        protected Button restartLButton, exitLButton;

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
        protected SpriteFont smallfonts, mediumfonts, bigfonts, kongfonts;//กำหนดชื่อ font

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

        //delay when press to change dialog GameState
        private float pressTime;
        private float pressTimeDelay = 1;

        protected enum GameState 
        { 
            START, OPENING, PLAY, INTROBOSS, BOSS, END, WIN, LOSE, PAUSE, EXIT
        }

        public virtual void Initial()
        {
            player = new Player(playerTex, bullet, whiteTex)
            {
                name = "Player Character",
                size = new Vector2(46, 94),
                speed = 13,
                //speed = 50,
                jumpHigh = 10.5f,
            };

            gamestate = GameState.START;

            //create button on pause
            continueButton = new Button(continueButtonPic, new Vector2(507, 238), new Vector2(203, 32));
            restartButton = new Button(restartButtonPic, new Vector2(487, 318), new Vector2(239, 32));
            exitButton = new Button(exitButtonPic, new Vector2(539, 562), new Vector2(131, 32));

            //create button win or lose screen
            restartLButton = new Button(restartLPic, new Vector2(1055, 326), new Vector2(190, 68));//create Button after win
            exitLButton = new Button(backLPic, new Vector2(57, 356), new Vector2(190, 68));//create Button after win

            //setting button
            arrowLeftBGButton = new Button(arrowLeftBGPic, new Vector2(584, 389), new Vector2(32, 56));
            arrowRightBGButton = new Button(arrowRightBGPic, new Vector2(842, 389), new Vector2(32, 56));
            arrowLeftSFXButton = new Button(arrowLeftSFXPic, new Vector2(584, 469), new Vector2(32, 56));
            arrowRightSFXButton = new Button(arrowRightSFXPic, new Vector2(842, 469), new Vector2(32, 56));

            //confirm exit button
            yesConfirmButton = new Button(yesConfirmPic, new Vector2(401, 452), new Vector2(190, 68));
            noConfirmButton = new Button(noConfirmPic, new Vector2(670, 452), new Vector2(190, 68));

            //camera
            camera = new Camera();

            if (Singleton.Instance.levelState == LevelState.FOREST)
            {
                Singleton.Instance.stageunlock = 1;
            }
            else if (Singleton.Instance.levelState == LevelState.CITY)
            {
                Singleton.Instance.stageunlock = 2;
            }
            else if (Singleton.Instance.levelState == LevelState.LAB)
            {
                Singleton.Instance.stageunlock = 3;
            }
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
            birdPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/birdPortrait");
            soulBirdPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/soulBirdPortrait");
            crocPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/crocPortrait");
            soulCrocPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/soulCrocPortrait");
            lionPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/lionPortrait");
            soulLionPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/soulLionPortrait");

            //guardian texture
            birdTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/birdTex");
            crocTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/crocTex");
            lionTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/lionTex");

            //pause screen
            pausePopUpPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/PAUSEPOPUP");
            //all button on pausescreen
            continueButtonPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/RESUME");
            exitButtonPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/EXIT");
            restartButtonPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/RESTART");

            arrowLeftBGPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/LeftArrow");
            arrowRightBGPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/RightArrow");
            arrowLeftSFXPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/LeftArrow");
            arrowRightSFXPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/RightArrow");

            muteTex = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/MUTE");
            medTex = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/MED");
            highTex = content.Load<Texture2D>("Pictures/Play/PlayScreen/Pause/FULL");

            //add button when win or lose
            restartLPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/WinLose/Continue");
            backLPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/WinLose/Exit");
            gameoverTex = content.Load<Texture2D>("Pictures/Play/PlayScreen/WinLose/Gameover");

            //confirmExit pic
            confirmExitPopUpPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/ConfirmExit/ConfirmExitPopUp");
            yesConfirmPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/ConfirmExit/Yes");
            noConfirmPic = content.Load<Texture2D>("Pictures/Play/PlayScreen/ConfirmExit/No");

            //Hud pic
            healthBarTex = content.Load<Texture2D>("Pictures/Play/Hud/healthBar");
            healthTex = content.Load<Texture2D>("Pictures/Play/Hud/health");

            manaBarTex = content.Load<Texture2D>("Pictures/Play/Hud/manaBar");
            manaTex = content.Load<Texture2D>("Pictures/Play/Hud/mana");

            skillBoxTex = content.Load<Texture2D>("Pictures/Play/Hud/skillSlot");

            healRTex = content.Load<Texture2D>("Pictures/Play/Hud/healR");
            healNotRTex = content.Load<Texture2D>("Pictures/Play/Hud/healNotR");

            dashRTex = content.Load<Texture2D>("Pictures/Play/Hud/dashR");
            dashNotRTex = content.Load<Texture2D>("Pictures/Play/Hud/dashNotR");

            crocRTex = content.Load<Texture2D>("Pictures/Play/Hud/crocR");
            crocNotRTex = content.Load<Texture2D>("Pictures/Play/Hud/crocNotR");

            lionRTex = content.Load<Texture2D>("Pictures/Play/Hud/lionR");
            lionNotRTex = content.Load<Texture2D>("Pictures/Play/Hud/lionNotR");

            //fonts
            smallfonts = content.Load<SpriteFont>("Fonts/font20");
            bigfonts = content.Load<SpriteFont>("Fonts/font60");
            mediumfonts = content.Load<SpriteFont>("Fonts/font30");
            kongfonts = content.Load<SpriteFont>("Fonts/KongFont");

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
            //delay dialog time
            pressTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

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
                        //change dialog
                        if (Keyboard.GetState().IsKeyDown(Keys.Space) && pressTime > pressTimeDelay)
                        {
                            openingDialog++;
                            pressTime = 0;
                        }
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
                        //change dialog
                        if (Keyboard.GetState().IsKeyDown(Keys.Space) && pressTime > pressTimeDelay)
                        {
                            introBossDialog++;
                            pressTime = 0;
                        }
                        break;
                    case GameState.BOSS:
                        break;
                    case GameState.END:
                        //change dialog
                        if (Keyboard.GetState().IsKeyDown(Keys.Space) && pressTime > pressTimeDelay)
                        {
                            endDialog++;
                            pressTime = 0;
                        }
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
                                ResetWorld();
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
                            ResetWorld();
                            changeScreen = true;

                            //Next Screen
                            if (nextScreen)
                            {
                                ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PerkScreen);
                            }
                            break;
                        case GameState.WIN:
                            MediaPlayer.Stop();


                            ResetWorld();
                            Singleton.Instance.levelState = LevelState.NULL;
                            changeScreen = true;
                            if (nextScreen)
                            {
                                ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                            }
                            break;
                        case GameState.LOSE:
                            MediaPlayer.Stop();
                            //Restart
                            if (restartLButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                if (!worldReset)
                                {
                                    ResetWorld();
                                    worldReset = true;
                                }
                                changeScreen = true;
                            }
                            //Exit
                            if (exitLButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                            {
                                ResetWorld();
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
                                    ResetWorld();
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
                        ResetWorld();
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

        public void ResetWorld()
        {
            Singleton.Instance.world.ClearForces();
            Singleton.Instance.world.Clear();
            Singleton.Instance.enemiesInWorld.Clear();
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
                        spriteBatch.Draw(blackTex, Vector2.Zero, Color.White);
                    }
                    //game state
                    switch (gamestate)
                    {
                        case GameState.WIN:
                            break;
                        case GameState.LOSE:
                            spriteBatch.Draw(gameoverTex, Vector2.Zero, Color.White);
                            restartLButton.Draw(spriteBatch);
                            exitLButton.Draw(spriteBatch);
                            break;
                        case GameState.PAUSE:
                            spriteBatch.Draw(pausePopUpPic, new Vector2(300,60), new Color(255, 255, 255, 255));

                            //BGM
                            spriteBatch.DrawString(mediumfonts, "Musics", new Vector2(400, 400), Color.White);

                            //SFX
                            spriteBatch.DrawString(mediumfonts, "Sounds", new Vector2(400, 480), Color.White);

                            arrowLeftBGButton.Draw(spriteBatch);
                            arrowRightBGButton.Draw(spriteBatch);

                            arrowLeftSFXButton.Draw(spriteBatch);
                            arrowRightSFXButton.Draw(spriteBatch);

                            continueButton.Draw(spriteBatch, continueButtonPic);
                            restartButton.Draw(spriteBatch, restartButtonPic);
                            exitButton.Draw(spriteBatch, exitButtonPic);
                            switch (Singleton.Instance.bgmState)
                            {
                                case AudioState.MUTE:
                                    spriteBatch.Draw(muteTex, new Vector2(634, 389), Color.White);
                                    break;
                                case AudioState.MEDIUM:
                                    spriteBatch.Draw(medTex, new Vector2(634, 389), Color.White);
                                    break;
                                case AudioState.FULL:
                                    spriteBatch.Draw(highTex, new Vector2(634, 389), Color.White);
                                    break;
                            }
                            // Click Arrow SFX button
                            switch (Singleton.Instance.sfxState)
                            {
                                case AudioState.MUTE:
                                    spriteBatch.Draw(muteTex, new Vector2(634, 469), Color.White);
                                    break;
                                case AudioState.MEDIUM:
                                    spriteBatch.Draw(medTex, new Vector2(634, 469), Color.White);
                                    break;
                                case AudioState.FULL:
                                    spriteBatch.Draw(highTex, new Vector2(634, 469), Color.White);
                                    break;
                            }
                            break;
                    }
                }
                else if (confirmExit)
                {
                    spriteBatch.Draw(blackTex, Vector2.Zero, Color.White);
                    spriteBatch.Draw(confirmExitPopUpPic, new Vector2(439, 224), Color.White);
                    noConfirmButton.Draw(spriteBatch, noConfirmPic);
                    yesConfirmButton.Draw(spriteBatch, yesConfirmPic);
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
            }
        }
        
        public override void DrawHUD(SpriteBatch spriteBatch)
        {
            if (play)
            {
                //draw dialog box, spacebar text and skip text
                if (gamestate == GameState.OPENING || gamestate == GameState.INTROBOSS || gamestate == GameState.END)
                {
                    spriteBatch.Draw(dialogBoxTex, new Rectangle(94, 508, 1092, 192), new Rectangle(0, 0, 1092, 192), Color.White);

                    spriteBatch.DrawString(kongfonts, "Press Spacebar", new Vector2(988, 667), Color.White);
                }

                if (gamestate == GameState.PLAY || gamestate == GameState.BOSS)
                {
                    int mana = (int)Player.mana;
                    int health = (int)Player.health;

                    //HUD
                    //Health Bar
                    spriteBatch.Draw(healthBarTex, new Rectangle(1, 1, 338, 86), Color.White);
                    if (health != 0)
                    {
                        for (int i = 0; i < health; i++)
                        {
                            spriteBatch.Draw(healthTex, new Rectangle((i * 36) + 73, 43, 32, 28), Color.White);
                        }
                    }
                    //mana Bar
                    double manaRatio = ((double)mana / 100) * 200;//mana percent per 100 * manaTex.length
                    spriteBatch.Draw(manaBarTex, new Rectangle(1, 95, 216, 56), Color.White);
                    spriteBatch.Draw(manaTex, new Rectangle(9, 103, (int)manaRatio, 40), Color.White);

                    //all skill
                    //lion skill
                    spriteBatch.Draw(skillBoxTex, new Rectangle(1, 160, 62, 56), Color.White);
                    if (Singleton.Instance.stageunlock < 3)
                    {
                        spriteBatch.Draw(lionNotRTex, new Rectangle(14, 171, 36, 34), Color.White);
                    }
                    else
                    {
                        if (player.skill3Cooldown <= 0)
                        {
                            spriteBatch.Draw(lionRTex, new Rectangle(14, 171, 36, 34), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(lionNotRTex, new Rectangle(14, 171, 36, 34), Color.White);
                        }
                    }

                    //croc skill
                    spriteBatch.Draw(skillBoxTex, new Rectangle(68, 160, 62, 56), Color.White);
                    if (Singleton.Instance.stageunlock < 2)
                    {
                        spriteBatch.Draw(crocNotRTex, new Rectangle(78, 178, 42, 20), Color.White);
                    }
                    else
                    {
                        if (player.skill2Cooldown <= 0)
                        {
                            spriteBatch.Draw(crocRTex, new Rectangle(78, 178, 42, 20), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(crocNotRTex, new Rectangle(78, 178, 42, 20), Color.White);
                        }
                    }

                    //dash skill
                    spriteBatch.Draw(skillBoxTex, new Rectangle(135, 160, 62, 56), Color.White);
                    if (player.dashCooldown <= 0)
                    {
                        spriteBatch.Draw(dashRTex, new Rectangle(152, 182, 30, 14), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(dashNotRTex, new Rectangle(152, 182, 30, 14), Color.White);
                    }

                    //heal skill
                    spriteBatch.Draw(skillBoxTex, new Rectangle(202, 160, 62, 56), Color.White);
                    if (player.skill1Cooldown <= 0)
                    {
                        spriteBatch.Draw(healRTex, new Rectangle(210, 175, 44, 26), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(healNotRTex, new Rectangle(210, 175, 44, 26), Color.White);
                    }
                }
            }
        }
    }
}
