using ScifiDruid.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;
using System.Diagnostics;
using Box2DNet.Factories;
using Box2DNet;
using Box2DNet.Dynamics;
using ScifiDruid.Managers;
using Box2DNet.Content;
using Box2DNet.Common;
using System.Security.Cryptography;
using Microsoft.Xna.Framework.Input;
using Verdantia.GameObjects;
using static ScifiDruid.Singleton;
using static ScifiDruid.GameObjects.JaneBoss;
using static ScifiDruid.GameObjects.LucasBoss;
using static ScifiDruid.GameObjects.Player;

namespace ScifiDruid.GameScreen
{
    class Stage1Screen : PlayScreen
    {
        //guardian
        private Guardian bird_guardian;
        private Guardian croc_guardian;
        //create switch and wall
        private SwitchWall switch_wall;
        private StageObject stage_wall;

        //create enemy
        private LucasBoss boss;

        private RangeEnemy flameMech;
        private List<RangeEnemy> flameMechEnemies;
        private int flameMechPositionList;
        private int flameMechCount;

        private MeleeEnemy chainsawMech;
        private List<MeleeEnemy> chainsawMechEnemies;
        private int chainsawMechPositionList;
        private int chainsawMechCount;

        //special occasion position
        //guardian event
        private Rectangle birdRect;
        private Rectangle crocRect;

        private Rectangle sign1Rect;
        private Rectangle sign2Rect;
        private Rectangle sign3Rect;
        private Rectangle sign4Rect;
        private Rectangle sign5Rect;
        private Rectangle sign6Rect;

        private bool readSign1;
        private bool readSign2;
        private bool readSign3;
        private bool readSign4;
        private bool readSign5;
        private bool readSign6;
        //if boss event
        private Rectangle wallblock;
        private Rectangle boss_event;

        //if open switch and wall gone
        private Rectangle switch_button;
        private Rectangle rock_wall;
        private bool isOpenSwitch = false;

        //Map Theme
        private Song stage1Theme;
        private Song lucasTheme;

        //switch and wall size
        private Vector2 switch_size = new Vector2(32, 32);
        private Vector2 switch_close_textureSize = new Vector2(32, 0);
        private Vector2 switch_open_textureSize = new Vector2(64, 0);

        private Vector2 wall_size = new Vector2(32, 192);
        private Vector2 wall_textureSize = new Vector2(0, 0);

        //check if boss dead
        private bool bossDead = false;

        //time 
        private int time;


        public override void Initial()
        {
            if (!initialized)
            {
                base.Initial();

                openingDialogCount = 12;
                introDialogCount = 4;
                endDialogCount = 5;

                //map size
                startmaptileX = 10f;
                endmaptileX = 170f;

                player.health = maxHealth;
                player.mana = maxMana;

                //create tileset for map1
                map = new TmxMap("Content/Stage1.tmx");
                tilesetStage1 = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage1Tileset/" + map.Tilesets[0].Name.ToString());

                tileWidth = map.Tilesets[0].TileWidth;
                tileHeight = map.Tilesets[0].TileHeight;
                tilesetTileWidth = tilesetStage1.Width / tileWidth;

                tilemapManager = new TileMapManager(map, tilesetStage1, tilesetTileWidth, tileWidth, tileHeight);

                //all object lists
                deadBlockRects = new List<Rectangle>();
                blockRects = new List<Rectangle>();
                playerRects = new List<Rectangle>();
                mechanicRects = new List<Rectangle>();
                ground1MonsterRects = new List<Rectangle>();
                ground2MonsterRects = new List<Rectangle>();
                flyMonsterRects = new List<Rectangle>();
                bossRect = new Rectangle();

                //add list rectangle
                foreach (var o in map.ObjectGroups["Blocks"].Objects)
                {
                    if (o.Name.Equals(""))
                    {
                        blockRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                    }
                }
                foreach (var o in map.ObjectGroups["Player"].Objects)
                {
                    if (o.Name.Equals("startRect"))
                    {
                        startRect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    }
                    if (o.Name.Equals("endRect"))
                    {
                        endRect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    }
                    if (o.Name.Equals("bossState"))
                    {
                        bossState = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    }
                }
                foreach (var o in map.ObjectGroups["SpecialBlocks"].Objects)
                {
                    if (o.Name.Equals("spike"))
                    {
                        deadBlockRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                    }
                }
                foreach (var o in map.ObjectGroups["SpecialProps"].Objects)
                {
                    if (o.Name.Equals("wall"))
                    {
                        rock_wall = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2) + 50, (int)o.Width, (int)o.Height);
                    }
                    if (o.Name.Equals("switch"))
                    {
                        switch_button = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    }

                    if (o.Name.Equals("sign1"))
                    {
                        sign1Rect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);

                        Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(sign1Rect.Width), ConvertUnits.ToSimUnits(sign1Rect.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(sign1Rect.X, sign1Rect.Y)));
                        body.UserData = "Sign1";
                        body.IsSensor = true;
                    }
                    if (o.Name.Equals("sign2"))
                    {
                        sign2Rect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);

                        Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(sign2Rect.Width), ConvertUnits.ToSimUnits(sign2Rect.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(sign2Rect.X, sign2Rect.Y)));
                        body.UserData = "Sign2";
                        body.IsSensor = true;
                    }
                    if (o.Name.Equals("sign3"))
                    {
                        sign3Rect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);

                        Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(sign3Rect.Width), ConvertUnits.ToSimUnits(sign3Rect.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(sign3Rect.X, sign3Rect.Y)));
                        body.UserData = "Sign3";
                        body.IsSensor = true;
                    }
                    if (o.Name.Equals("sign4"))
                    {
                        sign4Rect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);

                        Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(sign4Rect.Width), ConvertUnits.ToSimUnits(sign4Rect.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(sign4Rect.X, sign4Rect.Y)));
                        body.UserData = "Sign4";
                        body.IsSensor = true;
                    }
                    if (o.Name.Equals("sign5"))
                    {
                        sign5Rect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);

                        Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(sign5Rect.Width), ConvertUnits.ToSimUnits(sign5Rect.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(sign5Rect.X, sign5Rect.Y)));
                        body.UserData = "Sign5";
                        body.IsSensor = true;
                    }
                    if (o.Name.Equals("sign6"))
                    {
                        sign6Rect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);

                        Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(sign6Rect.Width), ConvertUnits.ToSimUnits(sign6Rect.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(sign6Rect.X, sign6Rect.Y)));
                        body.UserData = "Sign6";
                        body.IsSensor = true;
                    }
                }
                foreach (var o in map.ObjectGroups["SpecialOccasions"].Objects)
                {
                    if (o.Name.Equals("wallblock"))
                    {
                        wallblock = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    }
                    if (o.Name.Equals("boss_event"))
                    {
                        boss_event = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);

                        Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(boss_event.Width), ConvertUnits.ToSimUnits(boss_event.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(boss_event.X, boss_event.Y)));
                        body.UserData = "Boss_event";
                        body.IsSensor = true;
                    }
                    if (o.Name.Equals("guardian_bird"))
                    {
                        birdRect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    }
                    if (o.Name.Equals("guardian_lion"))
                    {
                        crocRect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    }
                }
                foreach (var o in map.ObjectGroups["GroundMonster"].Objects)
                {
                    //flamethrower machine position
                    if (o.Name.Equals("ground_mon_1"))
                    {
                        ground1MonsterRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                    }
                    //chainsaw machine position
                    if (o.Name.Equals("ground_mon_2"))
                    {
                        ground2MonsterRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                    }
                }
                foreach (var o in map.ObjectGroups["Boss"].Objects)
                {
                    if (o.Name.Equals("boss"))
                    {
                        bossRect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    }
                }

                //create collision for block in the world
                foreach (Rectangle rect in blockRects)
                {
                    Vector2 collisionPosition = ConvertUnits.ToSimUnits(new Vector2(rect.X, rect.Y));
                    //Singleton.Instance.world.Step(0.001f);

                    Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(rect.Width), ConvertUnits.ToSimUnits(rect.Height), 1f, collisionPosition);
                    body.UserData = "Ground";
                    body.Restitution = 0.0f;
                    body.Friction = 0.3f;
                }

                //create dead block for block in the world
                foreach (Rectangle rect in deadBlockRects)
                {
                    Vector2 deadBlockPosition = ConvertUnits.ToSimUnits(new Vector2(rect.X, rect.Y));
                    //Singleton.Instance.world.Step(0.001f);

                    Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(rect.Width), ConvertUnits.ToSimUnits(rect.Height), 1f, deadBlockPosition);
                    body.UserData = "Dead";
                    body.Restitution = 0.0f;
                    body.Friction = 0.3f;
                }

                //create player on position

                //bird
                Vector2 birdSize = new Vector2(49, 55);
                List<Vector2> birdAnimateList = new List<Vector2>() { new Vector2(10, 2), new Vector2(67, 2), new Vector2(4, 59), new Vector2(61, 59) };
                bird_guardian = new Guardian(birdTex, birdSize, birdAnimateList);
                bird_guardian.Initial(birdRect);

                Vector2 crocSize = new Vector2(193, 37);
                List<Vector2> crocAnimateList = new List<Vector2>() { new Vector2(4, 0) };
                croc_guardian = new Guardian(crocTex, crocSize, crocAnimateList);
                croc_guardian.Initial(crocRect);

                //range enemy
                flameMechEnemies = new List<RangeEnemy>();
                flameMechPositionList = ground1MonsterRects.Count();
                List<Vector2> flameMechSizeList = new List<Vector2>() { new Vector2(112, 86), new Vector2(112, 86), new Vector2(112, 86), new Vector2(112, 86) };
                List<List<Vector2>> flameMechAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 0) }, new List<Vector2>() { new Vector2(0, 0), new Vector2(112, 0) }, new List<Vector2>() { new Vector2(0, 0) }, new List<Vector2>() { new Vector2(0, 108), new Vector2(112, 108) } };
                for (int i = 0; i < flameMechPositionList; i++)
                {
                    flameMech = new RangeEnemy(flameMechTex, flameMechSizeList, flameMechAnimateList)
                    {
                        size = new Vector2(112, 86),
                        health = 3,
                        speed = 0.22f,
                        bulletSpeed = 400,
                        bulletSizeX = 26,
                        bulletSizeY = 30,
                        bulletDistance = 10,
                        isdrone = false,
                    };
                    flameMechEnemies.Add(flameMech);
                }

                //create enemy position
                flameMechCount = 0;
                foreach (RangeEnemy chainsawBot in flameMechEnemies)
                {
                    chainsawBot.Initial(ground1MonsterRects[flameMechCount], player);
                    flameMechCount++;
                }

                //melee enemy
                chainsawMechEnemies = new List<MeleeEnemy>();
                chainsawMechPositionList = ground2MonsterRects.Count();
                List<Vector2> chainsawMechSizeList = new List<Vector2>() { new Vector2(118, 100), new Vector2(118, 100), new Vector2(136, 100), new Vector2(136, 100), new Vector2(118, 100) };
                List<List<Vector2>> chainsawMechAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 0) }, new List<Vector2>() { new Vector2(0, 0), new Vector2(144, 0) }, new List<Vector2>() { new Vector2(0, 136), new Vector2(136, 136) }, new List<Vector2>() { new Vector2(0, 136), new Vector2(136, 136) }, new List<Vector2>() { new Vector2(0, 254), new Vector2(142, 254) } };
                for (int i = 0; i < chainsawMechPositionList; i++)
                {
                    chainsawMech = new MeleeEnemy(chainsawMechTex, chainsawMechSizeList, chainsawMechAnimateList)
                    {
                        size = new Vector2(118, 100),
                        health = 4,
                        speed = 0.22f,
                    };
                    chainsawMechEnemies.Add(chainsawMech);
                }

                //create enemy position
                chainsawMechCount = 0;
                foreach (MeleeEnemy chainsawBot in chainsawMechEnemies)
                {
                    chainsawBot.Initial(ground2MonsterRects[chainsawMechCount], player);
                    chainsawMechCount++;
                }

                //create boss on position
                boss = new LucasBoss(lucasBossTex, whiteTex)
                {
                    size = new Vector2(196, 186),
                    health = 15,
                    speed = 1.2f,
                };
                //spawn boss
                boss.Initial(bossRect, player, boss_event);

                //add All enemy to locate enemy
                Singleton.Instance.enemiesInWorld.AddRange(flameMechEnemies);
                Singleton.Instance.enemiesInWorld.AddRange(chainsawMechEnemies);
                Singleton.Instance.enemiesInWorld.Add(boss);

                //switch event
                //create switch button on position
                switch_wall = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize)
                {
                    size = new Vector2(32, 32),
                };
                switch_wall.Initial(switch_button);

                //create wall button on position
                stage_wall = new StageObject(switch_wall_Tex, wall_size, wall_textureSize)
                {
                    size = new Vector2(32, 350),
                };
                stage_wall.Initial(rock_wall);

                player.SetSpawn(startRect);
                //player.SetSpawn(bossState);
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            //bg
            stage1BG = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage1Tileset/Stage1BG");

            //button and rock wall
            switch_wall_Tex = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage1Tileset/specialProps1");

            //boss dialog
            bossPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/lucasPortrait");

            //bg music and sfx
            stage1Theme = content.Load<Song>("Songs/Stage1Screen/Stage1Theme");
            lucasTheme = content.Load<Song>("Songs/Stage1Screen/BossStage1Theme");
            MediaPlayer.Play(stage1Theme);

            //sfx
            //switch
            switchSound = content.Load<SoundEffect>("Sounds/Stage1/switch1Sound");

            Initial();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (play)
            {
                //stage 1 dialog
                switch (gamestate)
                {
                    case GameState.OPENING:
                        bird_guardian.Update(gameTime);
                        break;
                    case GameState.PLAY://all enemy
                        foreach (RangeEnemy flamewBot in flameMechEnemies)
                        {
                            flamewBot.Update(gameTime);
                            flamewBot.Action();
                        }
                        foreach (MeleeEnemy chainsawBot in chainsawMechEnemies)
                        {
                            chainsawBot.Update(gameTime);
                            chainsawBot.Action();
                        }

                        //switch button
                        switch_wall.Update(gameTime);
                        //stage wall
                        stage_wall.Update(gameTime);

                        //switch event
                        //press switch button
                        if (!isOpenSwitch && switch_wall.pressSwitch)
                        {
                            switchSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
                            isOpenSwitch = true;
                        }
                        //after open switch = clear wall
                        if (isOpenSwitch)
                        {
                            stage_wall.wallHitBox.Dispose();
                        }


                        //if player get into boss state
                        if (!created_boss && player.IsContact(player.hitBox, "Boss_event"))
                        {
                            //create block to block player
                            Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(wallblock.Width), ConvertUnits.ToSimUnits(wallblock.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(wallblock.X, wallblock.Y)));
                            body.UserData = "Ground";

                            //endRect at boss state
                            Body endRectBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(endRect.Width), ConvertUnits.ToSimUnits(endRect.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(endRect.X, endRect.Y)));
                            endRectBody.UserData = "Ground";

                            boss_area = true;
                            MediaPlayer.Stop();

                            //set player to inactive before boss
                            player.playerStatus = PlayerStatus.IDLE;
                            player.isAlive = false;
                        }

                        //read sign event
                        if (player.IsContact(player.hitBox, "Sign1"))
                        {
                            readSign1 = true;
                        }
                        else
                        {
                            readSign1 = false;
                        }
                        if (player.IsContact(player.hitBox, "Sign2"))
                        {
                            readSign2 = true;
                        }
                        else
                        {
                            readSign2 = false;
                        }
                        if (player.IsContact(player.hitBox, "Sign3"))
                        {
                            readSign3 = true;
                        }
                        else
                        {
                            readSign3 = false;
                        }
                        if (player.IsContact(player.hitBox, "Sign4"))
                        {
                            readSign4 = true;
                        }
                        else
                        {
                            readSign4 = false;
                        }
                        if (player.IsContact(player.hitBox, "Sign5"))
                        {
                            readSign5 = true;
                        }
                        else
                        {
                            readSign5 = false;
                        }
                        if (player.IsContact(player.hitBox, "Sign6"))
                        {
                            readSign6 = true;
                        }
                        else
                        {
                            readSign6 = false;
                        }
                        break;
                    case GameState.BOSS:
                        if (!created_boss && boss_area)
                        {
                            //player active after this
                            player.isAlive = true;
                            boss.isAlive = true;
                            boss.skillTime = 5;

                            created_boss = true;

                            //player Song
                            MediaPlayer.Play(lucasTheme);
                        }

                        //check if boss death then change to END state
                        if (boss.IsBossDead() && !bossDead)
                        {
                            bossDead = true;
                            MediaPlayer.Stop();
                            MediaPlayer.Play(stage1Theme);
                        }

                        if (boss.IsBossEnd() && bossDead)
                        {
                            //set player to inactive
                            gamestate = GameState.END;
                        }
                        break;
                    case GameState.END:
                        croc_guardian.Update(gameTime);
                        break;
                }
                if (gamestate == GameState.PLAY || gamestate == GameState.INTROBOSS || gamestate == GameState.BOSS)
                {
                    //boss
                    boss.Update(gameTime);
                }
            }
        }

        public override void DrawFixScreen(SpriteBatch spriteBatch)
        {
            base.DrawFixScreen(spriteBatch);
            //bg
            spriteBatch.Draw(stage1BG, Vector2.Zero, Color.White);
        }
        public override void DrawHUD(SpriteBatch spriteBatch)
        {
            //flash when guardian merge with player
            if (openingDialog == 5)
            {
                spriteBatch.Draw(whiteTex, Vector2.Zero, Color.White);
            }
            base.DrawHUD(spriteBatch);
            if (play)
            {
                switch (gamestate)
                {
                    case GameState.OPENING:
                        //draw bird guardian portrait and name
                        if (openingDialog < 5)
                        {
                            spriteBatch.Draw(birdPortraitTex, new Vector2(780, 156), Color.White);
                            spriteBatch.DrawString(alagardFont, "Gale the Sky Guardian", new Vector2(162, 521), Color.White);
                        }
                        else if (openingDialog > 5 && openingDialog < openingDialogCount)
                        {
                            spriteBatch.Draw(soulBirdPortraitTex, new Vector2(780, 156), Color.White);
                            spriteBatch.DrawString(alagardFont, "Gale the Sky Guardian", new Vector2(162, 521), Color.White);
                        }

                        switch (openingDialog)
                        {
                            case 1:
                                spriteBatch.DrawString(alagardFont, "Help me!", new Vector2(132, 578), Color.White);
                                break;
                            case 2:
                                spriteBatch.DrawString(alagardFont, "A sacred tree was taken by the humans and our friends were also kidnapped", new Vector2(132, 578), Color.White);
                                break;
                            case 3:
                                spriteBatch.DrawString(alagardFont, "Lets go help them and bring back the tree", new Vector2(132, 578), Color.White);
                                break;
                            case 4:
                                spriteBatch.DrawString(alagardFont, "But wait!, Before you go to help them, take my power", new Vector2(132, 578), Color.White);
                                break;
                            case 5:
                                spriteBatch.DrawString(alagardFont, "....................................................", new Vector2(132, 578), Color.White);
                                break;
                            case 6:
                                spriteBatch.DrawString(alagardFont, "Now you got an ability to fly", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "You can press Spacebar in the air to double jump", new Vector2(132, 610), Color.White);
                                break;
                            case 7:
                                spriteBatch.DrawString(alagardFont, "And you got a power of wind", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "You can press C to dash or dodge something except blocks", new Vector2(132, 610), Color.White);
                                break;
                            case 8:
                                spriteBatch.DrawString(alagardFont, "Skill status is shown on the bar", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "if its white thats mean you can use it.", new Vector2(132, 610), Color.White);
                                break;
                            case 9:
                                spriteBatch.DrawString(alagardFont, "But if not, thats mean it's inactive or still on cooldown time", new Vector2(132, 578), Color.White);
                                break;
                            case 10:
                                spriteBatch.DrawString(alagardFont, "The blue bar is your mana. If its gone, it's mean you are tired", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "Just take some rest to regenerate it", new Vector2(132, 610), Color.White);
                                break;
                            case 11:
                                spriteBatch.DrawString(alagardFont, "And be careful. Humans still left there robot around here", new Vector2(132, 578), Color.White);
                                break;
                        }
                        break;
                    case GameState.PLAY:
                        //sign dialog
                        if (readSign1 || readSign2 || readSign3 || readSign4 || readSign5 || (readSign6 && !isOpenSwitch))
                        {
                            spriteBatch.Draw(dialogBoxTex, new Rectangle(94, 508, 1092, 192), new Rectangle(0, 0, 1092, 192), Color.White);
                            spriteBatch.DrawString(alagardFont, "Gale the Sky Guardian", new Vector2(162, 521), Color.White);
                            spriteBatch.Draw(soulBirdPortraitTex, new Vector2(780, 156), Color.White);
                        }
                        if (readSign1)
                        {
                            spriteBatch.DrawString(alagardFont, "Go ahead", new Vector2(132, 578), Color.White);
                        }
                        if (readSign2)
                        {
                            spriteBatch.DrawString(alagardFont, "Jump over the mound, Press Spacebar to jump", new Vector2(132, 578), Color.White);
                        }
                        if (readSign3)
                        {
                            spriteBatch.DrawString(alagardFont, "I forgot to say. You can shoot magical bullet to enemy", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(alagardFont, "Press X to shoot. But it's waste of mana, use it carefully!", new Vector2(132, 610), Color.White);
                        }
                        if (readSign4)
                        {
                            spriteBatch.DrawString(alagardFont, "Enemy ahead! shoot it before you jump over the pit", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(alagardFont, "Don't fall into the pit of thorns, it's dangerous", new Vector2(132, 610), Color.White);
                        }
                        if (readSign5)
                        {
                            spriteBatch.DrawString(alagardFont, "By now, you should know the basics of the game", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(alagardFont, "So hurry up and go help right away", new Vector2(132, 610), Color.White);
                        }
                        if (readSign6 && !isOpenSwitch)
                        {
                            spriteBatch.DrawString(alagardFont, "Damn, the wall is blocking us, We need to find the switch and open it", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(alagardFont, "The switch button must be around her, Let's find it", new Vector2(132, 610), Color.White);
                        }
                        break;
                    case GameState.INTROBOSS:
                        if (introBossDialog == 1 || introBossDialog == 3)
                        {
                            spriteBatch.Draw(bossPortraitTex, new Vector2(874, 290), Color.White);
                            spriteBatch.DrawString(alagardFont, "Lucas the Lumbersaw", new Vector2(167, 521), Color.White);
                        }

                        if (introBossDialog == 2)
                        {
                            spriteBatch.DrawString(alagardFont, "Gale the Sky Guardian", new Vector2(162, 521), Color.White);
                            spriteBatch.Draw(soulBirdPortraitTex, new Vector2(780, 156), Color.White);
                        }
                        switch (introBossDialog)
                        {
                            case 1:
                                spriteBatch.DrawString(alagardFont, "Well well well! , Look who's here", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "A loser bird. You bring me a new friend?", new Vector2(132, 610), Color.White);
                                break;
                            case 2:
                                spriteBatch.DrawString(alagardFont, "Shut up! This is a man who captured other guardians and the tree", new Vector2(132, 578), Color.White);
                                break;
                            case 3:
                                spriteBatch.DrawString(alagardFont, "Hahaha, I see. You come to help your loser guardians", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "If you wanna help them, Come challege me. I will make it in short time", new Vector2(132, 610), Color.White);
                                break;
                        }
                        break;
                    case GameState.END:
                        if (endDialog == 1)
                        {
                            spriteBatch.Draw(bossPortraitTex, new Vector2(874, 290), Color.White);
                            spriteBatch.DrawString(alagardFont, "Lucas the Lumbersaw", new Vector2(167, 521), Color.White);
                        }

                        if (endDialog == 3)
                        {
                            spriteBatch.DrawString(alagardFont, "Gale the Sky Guardian", new Vector2(162, 521), Color.White);
                            spriteBatch.Draw(soulBirdPortraitTex, new Vector2(780, 156), Color.White);
                        }

                        if (endDialog == 2 || endDialog >= 4)
                        {
                            spriteBatch.DrawString(alagardFont, "Crush the Lake Guardian", new Vector2(147, 521), Color.White);
                            spriteBatch.Draw(crocPortraitTex, new Vector2(886, 306), Color.White);
                        }

                        switch (endDialog)
                        {
                            case 1:
                                spriteBatch.DrawString(alagardFont, "How is this possible! My belove machine is destroyed. Damn you loser bird", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "And you! I will tell my boss to capture you! Bye bye sucker", new Vector2(132, 610), Color.White);
                                break;
                            case 2:
                                spriteBatch.DrawString(alagardFont, "You save me! Thank you very much!", new Vector2(132, 578), Color.White);
                                break;
                            case 3:
                                spriteBatch.DrawString(alagardFont, "It's good to see you're still safe.", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "Where is Roark", new Vector2(132, 610), Color.White);
                                break;
                            case 4:
                                spriteBatch.DrawString(alagardFont, "He has been sent to the city now", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "We must help him now, Lets go", new Vector2(132, 610), Color.White);
                                break;
                        }
                        break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw tileset for map1
            if (play)
            {
                if (gamestate == GameState.START || gamestate == GameState.OPENING || gamestate == GameState.PLAY || gamestate == GameState.INTROBOSS || gamestate == GameState.BOSS || gamestate == GameState.END)
                {
                    tilemapManager.Draw(spriteBatch);

                    //draw player animation
                    player.Draw(spriteBatch);

                    if (!fadeFinish)
                    {
                        spriteBatch.Draw(blackTex, Vector2.Zero, colorStart);
                    }
                }
                switch (gamestate)
                {
                    case GameState.OPENING:
                        if (openingDialog < 6)
                        {
                            bird_guardian.Draw(spriteBatch);
                        }
                        break;
                    case GameState.PLAY:
                        //draw enemy animation
                        foreach (RangeEnemy flameBot in flameMechEnemies)
                        {
                            flameBot.Draw(spriteBatch);
                            foreach (EnemyBullet enemybullet in flameBot.bulletList)
                            {
                                enemybullet.Draw(spriteBatch);
                            }
                        }
                        foreach (MeleeEnemy chainsawBot in chainsawMechEnemies)
                        {
                            chainsawBot.Draw(spriteBatch);
                        }


                        //draw switch animation
                        switch_wall.Draw(spriteBatch);

                        //draw wall
                        if (!isOpenSwitch)
                        {
                            stage_wall.Draw(spriteBatch);
                        }
                        break;
                    case GameState.END:
                        if (endDialog >= 2 && endDialog < 5)
                        {
                            croc_guardian.Draw(spriteBatch);
                        }
                        break;
                }

                if (gamestate == GameState.PLAY || gamestate == GameState.INTROBOSS || gamestate == GameState.BOSS || gamestate == GameState.END)
                {
                    //draw boss animation
                    boss.Draw(spriteBatch);
                }
            }
            base.Draw(spriteBatch);
        }
    }
}
