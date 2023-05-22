using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;
using ScifiDruid.GameObjects;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using ScifiDruid.Managers;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using static ScifiDruid.GameObjects.Player;
using Verdantia.GameObjects;
using static ScifiDruid.Singleton;
using static ScifiDruid.GameObjects.DoctorBoss;
using static ScifiDruid.GameObjects.JaneBoss;

namespace ScifiDruid.GameScreen
{
    class Stage2Screen : PlayScreen
    {
        //create guardian tex
        private Guardian guardian;

        //create switch and wall
        private SwitchWall switch_wall1;
        private StageObject stage_wall1;

        private SwitchWall switch_wall2;
        private StageObject stage_wall2;

        //panel moving and falling
        private StageObject panelMove;
        private List<StageObject> panelMoveBlocks;

        private StageObject panelFall;
        private List<StageObject> panelFallBlocks;

        //create enemy
        private List<Enemy> allEnemies;
        private JaneBoss boss;

        private RangeEnemy gunPolice;
        private List<RangeEnemy> gunPoliceEnemies;
        private int gunPolicePositionList;
        private int gunPoliceCount;

        private MeleeEnemy meleePolice;
        private List<MeleeEnemy> meleePoliceEnemies;
        private int meleePolicePositionList;
        private int meleePoliceCount;

        //special occasion position
        //if boss event
        private Rectangle wallblock;
        private Rectangle boss_event;

        //if open switch and wall gone
        private Rectangle switch_button1;
        private Rectangle sign_wall1;
        private bool isOpenSwitch1 = false;

        private Rectangle switch_button2;
        private Rectangle sign_wall2;
        private bool isOpenSwitch2 = false;

        //special panel
        private Rectangle movingLRBlock;
        private List<Rectangle> movingLRBlocks;
        private Rectangle movingUDBlock;
        private List<Rectangle> movingUDBlocks;

        private Rectangle fallingBlock;
        private List<Rectangle> fallingBlocks;

        //Map Theme
        private Song stage2Theme;
        private Song janeTheme;

        //switch and wall size and panel
        private Vector2 switch_size = new Vector2(32, 32);
        private Vector2 switch_close_textureSize = new Vector2(64, 2);
        private Vector2 switch_open_textureSize = new Vector2(96, 2);

        private Vector2 wall_size = new Vector2(64, 192);
        private Vector2 wall_textureSize = new Vector2(0, 10);

        private Vector2 panel_size = new Vector2(64, 6);
        private Vector2 panel_textureSize = new Vector2(64, 66);

        //check if boss dead
        private bool bossDead = false;
        public override void Initial()
        {
            base.Initial();
            Player.level2Unlock = true;

            //map size
            startmaptileX = 10f;
            endmaptileX = 170f;

            Player.health = 5;
            Player.mana = 100;
            Player.maxHealth = 5;
            Player.maxMana = 100;
            Player.level3Unlock = false;
            //create tileset for map1
            map = new TmxMap("Content/Stage2.tmx");
            tilesetStage2 = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage2Tileset/" + map.Tilesets[0].Name.ToString());

            tileWidth = map.Tilesets[0].TileWidth;
            tileHeight = map.Tilesets[0].TileHeight;
            tilesetTileWidth = tilesetStage2.Width / tileWidth;

            tilemapManager = new TileMapManager(map, tilesetStage2, tilesetTileWidth, tileWidth, tileHeight);

            //all object lists
            deadBlockRects = new List<Rectangle>();
            blockRects = new List<Rectangle>();
            playerRects = new List<Rectangle>();
            mechanicRects = new List<Rectangle>();
            ground1MonsterRects = new List<Rectangle>();
            ground2MonsterRects = new List<Rectangle>();
            flyMonsterRects = new List<Rectangle>();
            bossRect = new Rectangle();


            movingLRBlocks = new List<Rectangle>();
            movingUDBlocks = new List<Rectangle>();
            fallingBlocks = new List<Rectangle>();

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
                if (o.Name.Equals("wall1"))
                {
                    sign_wall1 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2) + 125, (int)o.Width, (int)o.Height);
                }
                if (o.Name.Equals("switch1"))
                {
                    switch_button1 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                }

                if (o.Name.Equals("wall2"))
                {
                    sign_wall2 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2) + 27, (int)o.Width, (int)o.Height);
                }
                if (o.Name.Equals("switch2"))
                {
                    switch_button2 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                }

            }
            foreach (var o in map.ObjectGroups["FallingBlocks"].Objects)
            {
                if (o.Name.Equals("Fall"))
                {
                    fallingBlocks.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                }
            }
            foreach (var o in map.ObjectGroups["MovingBlocksArea"].Objects)
            {
                if (o.Name.Equals("LR"))
                {
                    movingLRBlocks.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                }
                if (o.Name.Equals("UD"))
                {
                    movingUDBlocks.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                }
            }
            //boss event
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
            }
            foreach (var o in map.ObjectGroups["GroundMonster"].Objects)
            {
                //gun police machine position
                if (o.Name.Equals("ground_mon_1"))
                {
                    ground1MonsterRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                }
                //melee police position
                if (o.Name.Equals("ground_mon_2"))
                {
                    ground2MonsterRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                }
            }
            foreach (var o in map.ObjectGroups["FlyingMonster"].Objects)
            {
                //drone position
                if (o.Name.Equals("fly_mon"))
                {
                    flyMonsterRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                }
            }
            foreach (var o in map.ObjectGroups["Boss"].Objects)
            {
                if (o.Name.Equals("Boss"))
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

            player.Initial(startRect);
            //player.Initial(bossState);

            //bird
            Vector2 guardianSize = new Vector2(49, 55);
            List<Vector2> guardianAnimateList = new List<Vector2>() { new Vector2(10, 2), new Vector2(67, 2), new Vector2(4, 59), new Vector2(61, 59) };
            //croc
            //Vector2 guardianSize = new Vector2(193, 37);
            //List<Vector2> guardianAnimateList = new List<Vector2>() { new Vector2(4, 0) };
            //lion
            //Vector2 guardianSize = new Vector2(86, 76);
            //List<Vector2> guardianAnimateList = new List<Vector2>() { new Vector2(0, 0), new Vector2(0, 77), new Vector2(0, 153) };
            guardian = new Guardian(guardianTex, guardianSize, guardianAnimateList);
            guardian.FlyInitial(bossState);


            //create enemy on position
            allEnemies = new List<Enemy>();

            
            //range enemy
            gunPoliceEnemies = new List<RangeEnemy>();
            gunPolicePositionList = ground1MonsterRects.Count();
            List<Vector2> gunPoliceSizeList = new List<Vector2>() { new Vector2(46, 92), new Vector2(46, 92), new Vector2(46, 92), new Vector2(99, 92)};
            List<List<Vector2>> gunPoliceAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 0), new Vector2(119, 0), new Vector2(228, 1) }, new List<Vector2>() { new Vector2(337, 0), new Vector2(0, 111), new Vector2(119, 111), new Vector2(227, 111) } , new List<Vector2>() { new Vector2(0, 226), new Vector2(116, 226) } , new List<Vector2>() { new Vector2(0, 355), new Vector2(116, 355), new Vector2(227, 353), new Vector2(326, 352), new Vector2(435, 357) } };
            for (int i = 0; i < gunPolicePositionList; i++)
            {
                gunPolice = new RangeEnemy(gunPoliceTex, gunPoliceSizeList, gunPoliceAnimateList)
                {
                    size = new Vector2(46, 92),
                    health = 3,
                    speed = 0.1f,
                };
                gunPoliceEnemies.Add(gunPolice);
            }

            //create enemy position
            gunPoliceCount = 0;
            foreach (RangeEnemy gun in gunPoliceEnemies)
            {
                gun.Initial(ground1MonsterRects[gunPoliceCount], player);
                gunPoliceCount++;
            }

            //melee enemy
            meleePoliceEnemies = new List<MeleeEnemy>();
            meleePolicePositionList = ground2MonsterRects.Count();
            List<Vector2> meleePoliceSizeList = new List<Vector2>() { new Vector2(55, 94), new Vector2(44, 94), new Vector2(44, 94), new Vector2(74, 109), new Vector2(99, 94) };
            List<List<Vector2>> meleePoliceAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 0), new Vector2(107, 0), new Vector2(214, 0), new Vector2(321, 1) }, new List<Vector2>() { new Vector2(428, 2), new Vector2(0, 135), new Vector2(108, 135), new Vector2(219, 135), new Vector2(328, 135), new Vector2(423, 135) }, new List<Vector2>() { new Vector2(428, 2), new Vector2(0, 135), new Vector2(108, 135), new Vector2(219, 135), new Vector2(328, 135), new Vector2(423, 135) }, new List<Vector2>() { new Vector2(0, 270), new Vector2(97, 270), new Vector2(214, 270), new Vector2(318, 270) }, new List<Vector2>() { new Vector2(0, 406), new Vector2(100, 406), new Vector2(249, 406), new Vector2(375, 406), new Vector2(501, 406) } };
            for (int i = 0; i < meleePolicePositionList; i++)
            {
                meleePolice = new MeleeEnemy(meleePoliceTex, meleePoliceSizeList, meleePoliceAnimateList)
                {
                    size = new Vector2(44, 94),
                    health = 4,
                    speed = 0.1f,
                };
                meleePoliceEnemies.Add(meleePolice);
            }

            //create enemy position
            meleePoliceCount = 0;
            foreach (MeleeEnemy melee in meleePoliceEnemies)
            {
                melee.Initial(ground2MonsterRects[meleePoliceCount], player);
                meleePoliceCount++;
            }

            //create boss on position
            boss = new JaneBoss(janeBossTex)
            {
                size = new Vector2(74, 112),
                health = 6,
                speed = 1.2f,
            };
            //spawn boss
            boss.Initial(bossRect, player);

            //add to all enemy for
            allEnemies.AddRange(gunPoliceEnemies);
            allEnemies.AddRange(meleePoliceEnemies);
            allEnemies.Add(boss);

            //switch event
            //create switch button on position
            switch_wall1 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) {size = new Vector2(32, 32)};
            switch_wall1.Initial(switch_button1);

            switch_wall2 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) {size = new Vector2(32, 32)};
            switch_wall2.Initial(switch_button2);

            //create wall button on position
            stage_wall1 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) {size = new Vector2(64, 182)};
            stage_wall1.Initial(sign_wall1);

            stage_wall2 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) { size = new Vector2(64, 182) };
            stage_wall2.Initial(sign_wall2);

            //add all enemy for player to know em all
            player.enemies = allEnemies;
        }
        public override void LoadContent()
        {
            base.LoadContent();

            //button and rock wall
            switch_wall_Tex = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage2Tileset/specialProps2");
            //guardian
            guardianTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/birdTex");
            //guardianTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/crocTex");
            //guardianTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/lionTex");

            //bg music and sfx
            stage2Theme = content.Load<Song>("Songs/Stage2Screen/Stage2Theme");
            janeTheme = content.Load<Song>("Songs/Stage2Screen/BossStage2Theme");
            MediaPlayer.Play(stage2Theme);

            Initial();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (play)
            {
                if (gamestate == GameState.OPENING || gamestate == GameState.END)
                {
                    guardian.Update(gameTime);
                }
                if (gamestate == GameState.PLAY || gamestate == GameState.END)
                {
                    if (!Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        //all enemy
                        foreach (RangeEnemy gun in gunPoliceEnemies)
                        {
                            gun.Update(gameTime);
                            gun.Action();
                        }
                        foreach (MeleeEnemy melee in meleePoliceEnemies)
                        {
                            melee.Update(gameTime);
                            melee.Action();
                        }
                        //boss
                        boss.Update(gameTime);

                        //check if boss death then change to END state
                        if (boss.IsBossDead() && !bossDead)
                        {
                            bossDead = true;
                            MediaPlayer.Stop();
                            MediaPlayer.Play(stage2Theme);
                        }
                        if (boss.isBossEnd() && bossDead)
                        {
                            //set player to inactive
                            player.playerStatus = PlayerStatus.IDLE;
                            player.isAlive = false;
                            gamestate = GameState.END;
                        }

                        //switch button
                        switch_wall1.Update(gameTime);
                        switch_wall2.Update(gameTime);
                        //stage wall
                        stage_wall1.Update(gameTime);
                        stage_wall2.Update(gameTime);

                        //if player get into boss state
                        if (!created_boss && player.IsContact(player.hitBox, "Boss_event"))
                        {
                            boss_area = true;
                            MediaPlayer.Stop();

                            //set player to inactive before boss
                            player.playerStatus = PlayerStatus.IDLE;
                            player.isAlive = false;
                        }

                        //if player is in boss area just spawn
                        Matrix lastScreen = camera.Follow(player.position, endmaptileX, endmaptileX);
                        if (!created_boss && boss_area && Singleton.Instance.tfMatrix.M41 == lastScreen.M41)
                        {
                            //player active after this
                            player.isAlive = true;
                            boss.isAlive = true;
                            boss.skillTime = 5;
                            //create block to block player
                            Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(wallblock.Width), ConvertUnits.ToSimUnits(wallblock.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(wallblock.X, wallblock.Y)));
                            body.UserData = "Ground";

                            //endRect at boss state
                            Body endRectBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(endRect.Width), ConvertUnits.ToSimUnits(endRect.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(endRect.X, endRect.Y)));
                            endRectBody.UserData = "Ground";

                            created_boss = true;

                            //player Song
                            MediaPlayer.Play(janeTheme);
                        }

                        //switch event
                        //press switch button
                        if (!isOpenSwitch1 && switch_wall1.pressSwitch)
                        {
                            isOpenSwitch1 = true;
                        }

                        if (!isOpenSwitch2 && switch_wall2.pressSwitch)
                        {
                            isOpenSwitch2 = true;
                        }

                        //after open switch = clear wall
                        if (isOpenSwitch1)
                        {
                            stage_wall1.wallHitBox.Dispose();
                        }

                        if (isOpenSwitch2)
                        {
                            stage_wall2.wallHitBox.Dispose();
                        }
                    }
                }
                if (gamestate == GameState.END)
                {
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw tileset for map1
            if (play)
            {
                if (gamestate == GameState.OPENING || gamestate == GameState.END)
                {
                    guardian.Draw(spriteBatch);
                }
                if (gamestate == GameState.START || gamestate == GameState.PLAY || gamestate == GameState.OPENING || gamestate == GameState.END)
                {
                    tilemapManager.Draw(spriteBatch);

                    //draw enemy animation
                    foreach (RangeEnemy gun in gunPoliceEnemies)
                    {
                        gun.Draw(spriteBatch);
                    }
                    foreach (MeleeEnemy melee in meleePoliceEnemies)
                    {
                        melee.Draw(spriteBatch);
                    }

                    //draw boss animation
                    boss.Draw(spriteBatch);

                    //draw player animation
                    player.Draw(spriteBatch);

                    ////draw switch animation
                    switch_wall1.Draw(spriteBatch);
                    switch_wall2.Draw(spriteBatch);
                    //draw wall1
                    if (!isOpenSwitch1)
                    {
                        stage_wall1.Draw(spriteBatch);
                    }
                    //draw wall2
                    if (!isOpenSwitch2)
                    {
                        stage_wall2.Draw(spriteBatch);
                    }

                    if (!fadeFinish)
                    {
                        spriteBatch.Draw(blackTex, Vector2.Zero, colorStart);
                    }
                }
            }

            base.Draw(spriteBatch);
        }
    }
}
