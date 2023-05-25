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
using System.Diagnostics;

namespace ScifiDruid.GameScreen
{
    class Stage2Screen : PlayScreen
    {
        //create guardian tex
        private Guardian lion_guardian;

        //create switch and wall
        private SwitchWall switch_wall1;
        private StageObject stage_wall1;

        private SwitchWall switch_wall2;
        private StageObject stage_wall2;

        private SwitchWall switch_wall3;
        private StageObject stage_wall3;

        //panel moving and falling
        private StageObject panelLRMove;
        private List<StageObject> panelLRMoveBlocks;
        private int panelLRMovePositionList;
        private int panelLRMoveCount;

        private StageObject panelUDMove;
        private List<StageObject> panelUDMoveBlocks;
        private int panelUDMovePositionList;
        private int panelUDMoveCount;

        private StageObject panelFallMove;
        private List<StageObject> panelFallMoveBlocks;
        private int panelFallMovePositionList;
        private int panelFallMoveCount;

        //create enemy
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
        //guardian event
        private Rectangle lionRect;
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

        private Rectangle switch_button3;
        private Rectangle sign_wall3;
        private bool isOpenSwitch3 = false;

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

            openingDialogCount = 5;
            introDialogCount = 5;
            endDialogCount = 7;

            Player.level2Unlock = true;

            //map size
            startmaptileX = 10f;
            endmaptileX = 170f;

            Player.health = Player.maxHealth;
            Player.mana = Player.maxMana;
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

                if (o.Name.Equals("wall3"))
                {
                    sign_wall3 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2) + 27, (int)o.Width, (int)o.Height);
                }
                if (o.Name.Equals("switch3"))
                {
                    switch_button3 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                }

                if (o.Name.Equals("guardian_croc"))
                {
                    lionRect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
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

            //player.Initial(startRect);
            player.Initial(bossState);
            //croc
            Vector2 guardianSize = new Vector2(86, 76);
            List<Vector2> guardianAnimateList = new List<Vector2>() { new Vector2(0, 0), new Vector2(0, 77), new Vector2(0, 153) };
            lion_guardian = new Guardian(lionTex, guardianSize, guardianAnimateList);
            lion_guardian.Initial(lionRect);

            
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
            boss = new JaneBoss(janeBossTex, janeAmmoTex)
            {
                size = new Vector2(74, 112),
                health = 1,
                //health = 6,
                speed = 1.2f,
            };
            //spawn boss
            boss.Initial(bossRect, player,boss_event);

            //add to all enemy for
            Singleton.Instance.enemiesInWorld.AddRange(gunPoliceEnemies);
            Singleton.Instance.enemiesInWorld.AddRange(meleePoliceEnemies);
            Singleton.Instance.enemiesInWorld.Add(boss);

            //switch event
            //create switch button on position
            switch_wall1 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) {size = new Vector2(32, 32)};
            switch_wall1.Initial(switch_button1);

            switch_wall2 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) {size = new Vector2(32, 32)};
            switch_wall2.Initial(switch_button2);

            switch_wall3 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) { size = new Vector2(32, 32) };
            switch_wall3.Initial(switch_button3);

            //create wall button on position
            stage_wall1 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) {size = new Vector2(64, 182)};
            stage_wall1.Initial(sign_wall1);

            stage_wall2 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) { size = new Vector2(64, 182) };
            stage_wall2.Initial(sign_wall2);

            stage_wall3 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) { size = new Vector2(64, 182) };
            stage_wall3.Initial(sign_wall3);

            //panel

            //create up down block in map 2
            /*foreach (Rectangle rect in movingUDBlocks)
            {
                Vector2 udPosition = ConvertUnits.ToSimUnits(new Vector2(rect.X, rect.Y));
                //Singleton.Instance.world.Step(0.001f);

                Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(64), ConvertUnits.ToSimUnits(6), 1f, udPosition);
                body.UserData = "UpDown";
                body.Restitution = 0.0f;
                body.Friction = 0.3f;
            }

            //create left right block in map 2
            foreach (Rectangle rect in movingLRBlocks)
            {
                Vector2 lrPosition = ConvertUnits.ToSimUnits(new Vector2(rect.X, rect.Y));
                //Singleton.Instance.world.Step(0.001f);

                Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(64), ConvertUnits.ToSimUnits(6), 1f, lrPosition);
                body.UserData = "LeftRight";
                body.Restitution = 0.0f;
                body.Friction = 0.3f;
            }*/

            //create falling block in map 2
            foreach (Rectangle rect in fallingBlocks)
            {
                Vector2 fallingPosition = ConvertUnits.ToSimUnits(new Vector2(rect.X, rect.Y));
                //Singleton.Instance.world.Step(0.001f);

                Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(64), ConvertUnits.ToSimUnits(6), 1f, fallingPosition, 0, BodyType.Dynamic);
                body.UserData = "StepFall";
                body.Restitution = 0.0f;
                body.Friction = 0.3f;
                body.IgnoreGravity = true;
            }

            //Left Right
            /*panelLRMoveBlocks = new List<StageObject>();
            panelLRMovePositionList = movingLRBlocks.Count();
            for (int i = 0; i < panelLRMovePositionList; i++)
            {
                panelLRMove = new StageObject(switch_wall_Tex, panel_size, panel_textureSize) { 
                    size = new Vector2(64, 6) 
                };
                panelLRMoveBlocks.Add(panelLRMove);
            }
            //create LR position
            panelLRMoveCount = 0;
            foreach (StageObject lrMove in panelLRMoveBlocks)
            {
                lrMove.Initial(movingLRBlocks[panelLRMoveCount]);
                panelLRMoveCount++;
            }*/

            //Up Down
            /*panelUDMoveBlocks = new List<StageObject>();
            panelUDMovePositionList = movingUDBlocks.Count();
            for (int i = 0; i < panelUDMovePositionList; i++)
            {
                panelUDMove = new StageObject(switch_wall_Tex, panel_size, panel_textureSize)
                {
                    size = new Vector2(64, 6)
                };
                panelUDMoveBlocks.Add(panelUDMove);
            }
            //create UD position
            panelUDMoveCount = 0;
            foreach (StageObject udMove in panelUDMoveBlocks)
            {
                udMove.Initial(movingUDBlocks[panelUDMoveCount]);
                panelUDMoveCount++;
            }*/
            //falling
            panelFallMoveBlocks = new List<StageObject>();
            panelFallMovePositionList = fallingBlocks.Count();
            for (int i = 0; i < panelFallMovePositionList; i++)
            {
                panelFallMove = new StageObject(switch_wall_Tex, panel_size, panel_textureSize)
                {
                    size = new Vector2(64, 6)
                };
                panelFallMoveBlocks.Add(panelFallMove);
            }
            //create Fall position
            panelFallMoveCount = 0;
            foreach (StageObject fallMove in panelFallMoveBlocks)
            {
                fallMove.Initial(fallingBlocks[panelFallMoveCount]);
                panelFallMoveCount++;
            }
        }
        public override void LoadContent()
        {
            base.LoadContent();
            //bg
            //stage2BG = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage2Tileset/Stage2BG");

            //button and rock wall
            switch_wall_Tex = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage2Tileset/specialProps2");

            //boss dialog
            bossPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/janePortrait");

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
            base.Update(gameTime);
            if (play)
            {
                //stage 2 dialog
                if (gamestate == GameState.OPENING)
                {
                }
                if (gamestate == GameState.INTROBOSS)
                {
                }

                if (gamestate == GameState.END)
                {
                    lion_guardian.Update(gameTime);
                }

                if (gamestate == GameState.PLAY)
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

                    //switch button
                    switch_wall1.Update(gameTime);
                    switch_wall2.Update(gameTime);
                    switch_wall3.Update(gameTime);
                    //stage wall
                    stage_wall1.Update(gameTime);
                    stage_wall2.Update(gameTime);
                    stage_wall3.Update(gameTime);

                    //update panel
                    /*foreach (StageObject lrMove in panelLRMoveBlocks)
                    {
                        lrMove.Update(gameTime);
                    }
                    foreach (StageObject udMove in panelUDMoveBlocks)
                    {
                        udMove.Update(gameTime);
                    }*/
                    foreach (StageObject fallMove in panelFallMoveBlocks)
                    {
                        fallMove.Update(gameTime);
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

                    if (!isOpenSwitch3 && switch_wall3.pressSwitch)
                    {
                        isOpenSwitch3 = true;
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

                    if (isOpenSwitch3)
                    {
                        stage_wall3.wallHitBox.Dispose();
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
                }

                if (gamestate == GameState.PLAY || gamestate == GameState.INTROBOSS || gamestate == GameState.BOSS)
                {
                    //boss
                    boss.Update(gameTime);
                }

                if (gamestate == GameState.BOSS)
                {
                    if (!created_boss && boss_area)
                    {
                        //player active after this
                        player.isAlive = true;
                        boss.isAlive = true;
                        boss.skillTime = 5;

                        created_boss = true;

                        //player Song
                        MediaPlayer.Play(janeTheme);
                    }

                    //check if boss death then change to END state
                    if (boss.IsBossDead() && !bossDead)
                    {
                        bossDead = true;
                        MediaPlayer.Stop();
                        MediaPlayer.Play(stage2Theme);
                    }

                    if (boss.IsBossEnd() && bossDead)
                    {
                        //set player to inactive
                        gamestate = GameState.END;
                    }
                }
                if (gamestate == GameState.END)
                {
                }
            }
        }
        public override void DrawFixScreen(SpriteBatch spriteBatch)
        {
            base.DrawFixScreen(spriteBatch);
            //bg
            spriteBatch.Draw(whiteTex, Vector2.Zero, Color.White);
        }

        public override void DrawHUD(SpriteBatch spriteBatch)
        {
            //draw tileset for map 2
            if (play)
            {
                base.DrawHUD(spriteBatch);
                //Dialog OPENING
                if (gamestate == GameState.OPENING)
                {
                    spriteBatch.DrawString(kongfonts, "Crush the Lake Guardian", new Vector2(132, 525), Color.White);
                    spriteBatch.Draw(soulCrocPortraitTex, new Vector2(886, 306), Color.White);
                    switch (openingDialog)
                    {
                        case 1:
                            spriteBatch.DrawString(kongfonts, "Now you got my power", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(kongfonts, "An ability to shoot water power straight to enemy", new Vector2(132, 610), Color.White);
                            break;
                        case 2:
                            spriteBatch.DrawString(kongfonts, "press Z and down arrow to shoot it", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(kongfonts, "Its powerful but hard to use it", new Vector2(132, 610), Color.White);
                            break;
                        case 3:
                            spriteBatch.DrawString(kongfonts, "Only use this skill when necessary", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(kongfonts, "You can check this skill on top of the bar", new Vector2(132, 610), Color.White);
                            break;
                        case 4:
                            spriteBatch.DrawString(kongfonts, "This city is full of security guard", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(kongfonts, "Beware every steps you walk and the path you choose", new Vector2(132, 610), Color.White);
                            break;
                    }
                }
                //Dialog INTROBOSS
                if (gamestate == GameState.INTROBOSS)
                {
                    if (introBossDialog == 1 || introBossDialog == 4)
                    {
                        spriteBatch.Draw(bossPortraitTex, new Vector2(948, 312), Color.White);
                        spriteBatch.DrawString(kongfonts, "Jane the Security", new Vector2(132, 525), Color.White);
                    }

                    if (introBossDialog == 2) 
                    {
                        spriteBatch.DrawString(kongfonts, "Gale the Sky Guardian", new Vector2(132, 525), Color.White);
                        spriteBatch.Draw(soulBirdPortraitTex, new Vector2(780, 156), Color.White);
                    }
                    if (introBossDialog == 3)
                    {
                        spriteBatch.DrawString(kongfonts, "Crush the Lake Guardian", new Vector2(132, 525), Color.White);
                        spriteBatch.Draw(soulCrocPortraitTex, new Vector2(886, 306), Color.White);
                    }
                    switch (introBossDialog)
                    {
                        case 1:
                            spriteBatch.DrawString(kongfonts, "Welcome to the Neon City! I didn't expect you to come this far", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(kongfonts, "But this will be the end of you, so give up now", new Vector2(132, 610), Color.White);
                            break;
                        case 2:
                            spriteBatch.DrawString(kongfonts, "In your dream! Who said we will give up?", new Vector2(132, 578), Color.White);
                            break;
                        case 3:
                            spriteBatch.DrawString(kongfonts, "This is not our end. But it's yours!", new Vector2(132, 578), Color.White);
                            break;
                        case 4:
                            spriteBatch.DrawString(kongfonts, "Ooh! What a bad mouth! Let me tell you something, I don't care who you are", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(kongfonts, "And there will be no mercy for all of you", new Vector2(132, 610), Color.White);
                            break;
                    }
                }
                //Dialog END
                if (gamestate == GameState.END)
                {
                    if (endDialog == 1)
                    {
                        spriteBatch.Draw(bossPortraitTex, new Vector2(948, 312), Color.White);
                        spriteBatch.DrawString(kongfonts, "Jane the Security", new Vector2(132, 525), Color.White);
                    }

                    if (endDialog == 3)
                    {
                        spriteBatch.DrawString(kongfonts, "Gale the Sky Guardian", new Vector2(132, 525), Color.White);
                        spriteBatch.Draw(soulBirdPortraitTex, new Vector2(780, 156), Color.White);
                    }

                    if (endDialog == 5)
                    {
                        spriteBatch.DrawString(kongfonts, "Crush the Lake Guardian", new Vector2(132, 525), Color.White);
                        spriteBatch.Draw(soulCrocPortraitTex, new Vector2(886, 306), Color.White);
                    }

                    if (endDialog == 2 || endDialog == 4 || endDialog == 6)
                    {
                        spriteBatch.DrawString(kongfonts, "Roark the Lake Guardian", new Vector2(132, 525), Color.White);
                        spriteBatch.Draw(lionPortraitTex, new Vector2(937, 255), Color.White);
                    }
                    switch (endDialog)
                    {
                        case 1:
                            spriteBatch.DrawString(kongfonts, "Hhmph! You lucky this time. But when you get inside the lab", new Vector2(132, 578), Color.White);
                            break;
                        case 2:
                            spriteBatch.DrawString(kongfonts, "Thank you for saving me!", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(kongfonts, "I thought I will die in this city", new Vector2(132, 610), Color.White);
                            break;
                        case 3:
                            spriteBatch.DrawString(kongfonts, "Are you hurt? We are worried about you", new Vector2(132, 578), Color.White);
                            break;
                        case 4:
                            spriteBatch.DrawString(kongfonts, "Im still ok", new Vector2(132, 578), Color.White);
                            break;
                        case 5:
                            spriteBatch.DrawString(kongfonts, "Good then. By the way", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(kongfonts, "Do you know, Where did they bring the tree to?", new Vector2(132, 610), Color.White);
                            break;
                        case 6:
                            spriteBatch.DrawString(kongfonts, "The tree is in the laboratory in front of this building", new Vector2(132, 578), Color.White);
                            spriteBatch.DrawString(kongfonts, "But it's dangerous inside, so let me help you, or else you won't be able to survive", new Vector2(132, 610), Color.White);
                            break;
                    }
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw tileset for map 2
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

                if (gamestate == GameState.END)
                {
                    if (endDialog > 1)
                    {
                        lion_guardian.Draw(spriteBatch);
                    }
                }

                if (gamestate == GameState.PLAY)
                {
                    //draw enemy animation
                    foreach (RangeEnemy gun in gunPoliceEnemies)
                    {
                        gun.Draw(spriteBatch);
                    }
                    foreach (MeleeEnemy melee in meleePoliceEnemies)
                    {
                        melee.Draw(spriteBatch);
                    }

                    //draw switch animation
                    switch_wall1.Draw(spriteBatch);
                    switch_wall2.Draw(spriteBatch);
                    switch_wall3.Draw(spriteBatch);
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
                    //draw wall3
                    if (!isOpenSwitch3)
                    {
                        stage_wall3.Draw(spriteBatch);
                    }

                    //all panel
                    /*foreach (StageObject lrMove in panelLRMoveBlocks)
                    {
                        lrMove.Draw(spriteBatch);
                    }
                    foreach (StageObject udMove in panelUDMoveBlocks)
                    {
                        udMove.Draw(spriteBatch);
                    }*/
                    foreach (StageObject fallMove in panelFallMoveBlocks)
                    {
                        fallMove.Draw(spriteBatch);
                    }
                }

                if (gamestate == GameState.PLAY || gamestate == GameState.INTROBOSS || gamestate == GameState.BOSS)
                {
                    //draw boss animation
                    boss.Draw(spriteBatch);
                }
            }
            base.Draw(spriteBatch);

        }
    }
}
