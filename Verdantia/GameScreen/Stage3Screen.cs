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
using static ScifiDruid.GameObjects.DoctorBoss;
using System.Diagnostics;

namespace ScifiDruid.GameScreen
{
    class Stage3Screen : PlayScreen
    {
        //create switch and wall
        private SwitchWall switch_wall1;
        private StageObject stage_wall1;

        private SwitchWall switch_wall2;
        private StageObject stage_wall2;

        private SwitchWall switch_wall3;
        private StageObject stage_wall3;

        private SwitchWall switch_wall4;
        private StageObject stage_wall4;

        private SwitchWall switch_wall5;
        private StageObject stage_wall5;

        private SwitchWall switch_wall6;
        private StageObject stage_wall6;

        //panel moving and falling
        private StageObject squashBlock;
        private List<StageObject> squashGroup;

        //create enemy
        private DoctorBoss boss;

        private RangeEnemy shieldDog;
        private List<RangeEnemy> shieldDogEnemies;
        private int shieldDogPositionList;
        private int shieldDogCount;

        private DroneEnemy eyeball;
        private List<DroneEnemy> eyeballEnemies;
        private int eyeballPositionList;
        private int eyeballCount;

        private MeleeEnemy meleeTentacle;
        private List<MeleeEnemy> meleeTentacleEnemies;
        private int meleeTentaclePositionList;
        private int meleeTentacleCount;

        //special occasion position
        //if boss event
        private Rectangle wallblock;
        private Rectangle boss_event;
        private Rectangle invWall;

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


        private Rectangle switch_button4;
        private Rectangle sign_wall4;
        private bool isOpenSwitch4 = false;

        private Rectangle switch_button5;
        private Rectangle sign_wall5;
        private bool isOpenSwitch5 = false;

        private Rectangle switch_button6;
        private Rectangle sign_wall6;
        private bool isOpenSwitch6 = false;

        //special panel
        private Rectangle squashBlockPosition;
        private List<Rectangle> squashBlocksPosition;

        //Map Theme
        private Song stage3Theme;
        private Song doctorTheme;

        //switch and wall size and panel
        private Vector2 switch_size = new Vector2(32, 32);
        private Vector2 switch_close_textureSize = new Vector2(64, 2);
        private Vector2 switch_open_textureSize = new Vector2(96, 2);

        private Vector2 wall_size = new Vector2(64, 192);
        private Vector2 wall_textureSize = new Vector2(0, 0);

        private Vector2 panel_size = new Vector2(64, 6);
        private Vector2 panel_textureSize = new Vector2(64, 66);

        //check boss state
        private bool bossDead = false;
        public override void Initial()
        {
            if (!initialized)
            {
                base.Initial();

                openingDialogCount = 7;
                introDialogCount = 9;
                endDialogCount = 5;

                //map size
                startmaptileX = 10f;
                endmaptileX = 170f;

                player.health = maxHealth;
                player.mana = maxMana;

                //create tileset for map1
                map = new TmxMap("Content/Stage3.tmx");
                tilesetStage2 = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage3Tileset/" + map.Tilesets[0].Name.ToString());

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

                squashBlocksPosition = new List<Rectangle>();

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
                        sign_wall1 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2) + 27, (int)o.Width, (int)o.Height);
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
                    if (o.Name.Equals("wall4"))
                    {
                        sign_wall4 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2) + 27, (int)o.Width, (int)o.Height);
                    }
                    if (o.Name.Equals("switch4"))
                    {
                        switch_button4 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    }
                    if (o.Name.Equals("wall5"))
                    {
                        sign_wall5 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2) + 27, (int)o.Width, (int)o.Height);
                    }
                    if (o.Name.Equals("switch5"))
                    {
                        switch_button5 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    }
                    if (o.Name.Equals("wall6"))
                    {
                        sign_wall6 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2) + 27, (int)o.Width, (int)o.Height);
                    }
                    if (o.Name.Equals("switch6"))
                    {
                        switch_button6 = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    }
                }
                foreach (var o in map.ObjectGroups["SquashBlocks"].Objects)
                {
                    if (o.Name.Equals("Fall"))
                    {
                        squashBlocksPosition.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
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
                    if (o.Name.Equals("inv_wall"))
                    {
                        invWall = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);

                        Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(invWall.Width), ConvertUnits.ToSimUnits(invWall.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(invWall.X, invWall.Y)));
                        body.UserData = "Wall";
                    }
                }
                foreach (var o in map.ObjectGroups["FlyingMonster"].Objects)
                {
                    //fy monster position
                    if (o.Name.Equals("fly_mon"))
                    {
                        flyMonsterRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                    }
                }
                foreach (var o in map.ObjectGroups["GroundMonster"].Objects)
                {
                    //shield dog machine position
                    if (o.Name.Equals("ground_mon_1"))
                    {
                        ground1MonsterRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                    }
                    //tentacle machine position
                    if (o.Name.Equals("ground_mon_2"))
                    {
                        ground2MonsterRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
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


                Vector2 guardianSize = new Vector2(49, 55);
                List<Vector2> guardianAnimateList = new List<Vector2>() { new Vector2(10, 2), new Vector2(67, 2), new Vector2(4, 59), new Vector2(61, 59) };
                //guardian = new Guardian(guardianTex, guardianSize, guardianAnimateList);

                //range enemy
                shieldDogEnemies = new List<RangeEnemy>();
                shieldDogPositionList = ground1MonsterRects.Count();
                List<Vector2> shieldDogSizeList = new List<Vector2>() { new Vector2(128, 94), new Vector2(128, 94), new Vector2(128, 94), new Vector2(138, 94) };
                List<List<Vector2>> shieldDogAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 0) }, new List<Vector2>() { new Vector2(0, 0), new Vector2(294, 0), new Vector2(594, 0) }, new List<Vector2>() { new Vector2(0, 0), new Vector2(914, 0) }, new List<Vector2>() { new Vector2(0, 170), new Vector2(282, 170), new Vector2(582, 170) } };
                for (int i = 0; i < shieldDogPositionList; i++)
                {
                    shieldDog = new RangeEnemy(shieldDogTex, shieldDogSizeList, shieldDogAnimateList)
                    {
                        size = new Vector2(128, 94),
                        health = 3,
                        speed = 0.22f,
                        bulletSpeed = 400,
                        bulletSizeX = 4,
                        bulletSizeY = 6,
                        bulletDistance = 10,
                        isdrone = false,
                    };
                    shieldDogEnemies.Add(shieldDog);
                }

                //create enemy position
                shieldDogCount = 0;
                foreach (RangeEnemy gun in shieldDogEnemies)
                {
                    gun.Initial(ground1MonsterRects[shieldDogCount], player);
                    shieldDogCount++;
                }

                //eye ball drone enemy
                eyeballEnemies = new List<DroneEnemy>();
                eyeballPositionList = flyMonsterRects.Count();
                List<Vector2> eyeBallSizeList = new List<Vector2>() { new Vector2(52, 55), new Vector2(52, 55), new Vector2(61, 68), new Vector2(61, 62) };
                List<List<Vector2>> eyeBallAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(2, 13) }, new List<Vector2>() { new Vector2(2, 13), new Vector2(69, 13) }, new List<Vector2>() { new Vector2(142, 0), new Vector2(221, 6), new Vector2(295, 4) }, new List<Vector2>() { new Vector2(0, 87), new Vector2(93, 90), new Vector2(169, 85), new Vector2(243, 101), new Vector2(315, 111) } };
                for(int i = 0;i < eyeballPositionList; i++)
                {
                    eyeball = new DroneEnemy(eyeBallTex, eyeBallSizeList, eyeBallAnimateList)
                    {
                        size = new Vector2(52, 55),
                        health = 4,
                        speed = 0.1f,
                        bulletSpeed = 400,
                        bulletSizeX = 16,
                        bulletSizeY = 8,
                        bulletDistance = 10,
                    };
                    eyeballEnemies.Add(eyeball);
                }
                //create eyeball position
                eyeballCount= 0;
                foreach (DroneEnemy drone in eyeballEnemies)
                {
                    drone.Initial(flyMonsterRects[eyeballCount], player);
                    eyeballCount++;
                }

                //melee enemy
                meleeTentacleEnemies = new List<MeleeEnemy>();
                meleeTentaclePositionList = ground2MonsterRects.Count();
                List<Vector2> meleePoliceSizeList = new List<Vector2>() { new Vector2(49, 83), new Vector2(49, 83), new Vector2(49, 83), new Vector2(49, 83), new Vector2(66, 83) };
                List<List<Vector2>> meleePoliceAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 0) }, new List<Vector2>() { new Vector2(0, 0), new Vector2(147, 0), new Vector2(297, 0) }, new List<Vector2>() { new Vector2(0, 0), new Vector2(147, 0), new Vector2(297, 0) }, new List<Vector2>() { new Vector2(446, 0), new Vector2(607, 0) }, new List<Vector2>() { new Vector2(0, 112), new Vector2(142, 112), new Vector2(286, 111), new Vector2(446, 111) } };
                for (int i = 0; i < meleeTentaclePositionList; i++)
                {
                    meleeTentacle = new MeleeEnemy(tentacleTex, meleePoliceSizeList, meleePoliceAnimateList)
                    {
                        size = new Vector2(49, 83),
                        health = 4,
                        speed = 0.1f,
                    };
                    meleeTentacleEnemies.Add(meleeTentacle);
                }

                //create enemy position
                meleeTentacleCount = 0;
                foreach (MeleeEnemy melee in meleeTentacleEnemies)
                {
                    melee.Initial(ground2MonsterRects[meleeTentacleCount], player);
                    meleeTentacleCount++;
                }

                //create boss on position
                boss = new DoctorBoss(doctorBossTex, doctorAmmoTex)
                {
                    size = new Vector2(38, 88),
                    health = 30,
                    speed = 1.2f,
                };
                //spawn boss
                boss.Initial(bossRect, player, boss_event);

                //add to all enemy for
                Singleton.Instance.enemiesInWorld.AddRange(shieldDogEnemies);
                Singleton.Instance.enemiesInWorld.AddRange(meleeTentacleEnemies);
                Singleton.Instance.enemiesInWorld.Add(boss);

                //switch event
                //create switch button on position
                switch_wall1 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) { size = new Vector2(32, 32) };
                switch_wall1.Initial(switch_button1);

                switch_wall2 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) { size = new Vector2(32, 32) };
                switch_wall2.Initial(switch_button2);

                switch_wall3 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) { size = new Vector2(32, 32) };
                switch_wall3.Initial(switch_button3);

                switch_wall4 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) { size = new Vector2(32, 32) };
                switch_wall4.Initial(switch_button4);

                switch_wall5 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) { size = new Vector2(32, 32) };
                switch_wall5.Initial(switch_button5);

                switch_wall6 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) { size = new Vector2(32, 32) };
                switch_wall6.Initial(switch_button6);

                //create wall button on position
                stage_wall1 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) { size = new Vector2(64, 192) };
                stage_wall1.Initial(sign_wall1);

                stage_wall2 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) { size = new Vector2(64, 192) };
                stage_wall2.Initial(sign_wall2);

                stage_wall3 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) { size = new Vector2(64, 192) };
                stage_wall3.Initial(sign_wall3);

                stage_wall4 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) { size = new Vector2(64, 192) };
                stage_wall4.Initial(sign_wall4);

                stage_wall5 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) { size = new Vector2(64, 192) };
                stage_wall5.Initial(sign_wall5);

                stage_wall6 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) { size = new Vector2(64, 192) };
                stage_wall6.Initial(sign_wall6);

                //create player on position
                player.SetSpawn(startRect);
                //player.Initial(bossState);
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            //bg
            stage3BG = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage3Tileset/Stage3BG");
            labBG = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage3Tileset/LabBG");

            //button and rock wall
            switch_wall_Tex = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage3Tileset/specialProps3");

            //boss dialog
            bossPortraitTex = content.Load<Texture2D>("Pictures/Play/Dialog/doctorPortrait");

            //bg music and sfx
            stage3Theme = content.Load<Song>("Songs/Stage3Screen/Stage3Theme");
            doctorTheme = content.Load<Song>("Songs/Stage3Screen/BossStage3Theme");
            MediaPlayer.Play(stage3Theme);

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
                switch (gamestate)
                {
                    case GameState.PLAY:
                        //all enemy
                        foreach (RangeEnemy dog in shieldDogEnemies)
                        {
                            dog.Update(gameTime);
                            dog.Action();
                        }
                        foreach (MeleeEnemy tentacle in meleeTentacleEnemies)
                        {
                            tentacle.Update(gameTime);
                            tentacle.Action();
                        }
                        foreach (DroneEnemy drone in eyeballEnemies)
                        {
                            drone.Update(gameTime);
                            drone.Action();
                        }

                        //switch button
                        switch_wall1.Update(gameTime);
                        switch_wall2.Update(gameTime);
                        switch_wall3.Update(gameTime);
                        switch_wall4.Update(gameTime);
                        switch_wall5.Update(gameTime);
                        switch_wall6.Update(gameTime);
                        //stage wall
                        stage_wall1.Update(gameTime);
                        stage_wall2.Update(gameTime);
                        stage_wall3.Update(gameTime);
                        stage_wall4.Update(gameTime);
                        stage_wall5.Update(gameTime);
                        stage_wall6.Update(gameTime);

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
                        if (!isOpenSwitch4 && switch_wall4.pressSwitch)
                        {
                            isOpenSwitch4 = true;
                        }
                        if (!isOpenSwitch5 && switch_wall5.pressSwitch)
                        {
                            isOpenSwitch5 = true;
                        }
                        if (!isOpenSwitch6 && switch_wall6.pressSwitch)
                        {
                            isOpenSwitch6 = true;
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
                        if (isOpenSwitch4)
                        {
                            stage_wall4.wallHitBox.Dispose();
                        }
                        if (isOpenSwitch5)
                        {
                            stage_wall5.wallHitBox.Dispose();
                        }
                        if (isOpenSwitch6)
                        {
                            stage_wall6.wallHitBox.Dispose();
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
                            MediaPlayer.Play(doctorTheme);
                        }

                        //check if boss death then change to END state
                        if (boss.IsBossDead() && !bossDead)
                        {
                            bossDead = true;
                            MediaPlayer.Stop();
                            MediaPlayer.Play(stage3Theme);
                        }

                        if (boss.IsBossEnd() && bossDead)
                        {
                            //set player to inactive
                            gamestate = GameState.END;
                        }
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
            spriteBatch.Draw(stage3BG, Vector2.Zero, Color.White);
            if (gamestate == GameState.INTROBOSS || gamestate == GameState.BOSS || gamestate == GameState.INTROBOSS)
            {
                spriteBatch.Draw(labBG, Vector2.Zero, Color.White);
            }
            else
            {
                spriteBatch.Draw(stage3BG, Vector2.Zero, Color.White);
            }
        }

        public override void DrawHUD(SpriteBatch spriteBatch)
        {
            base.DrawHUD(spriteBatch);
            //draw tileset for map 3
            if (play)
            {
                switch (gamestate)
                {
                    case GameState.OPENING:
                        if (openingDialog < 5)
                        {
                            spriteBatch.DrawString(alagardFont, "Roark the Lake Guardian", new Vector2(142, 521), Color.White);
                            spriteBatch.Draw(soulLionPortraitTex, new Vector2(937, 255), Color.White);
                        }
                        if (openingDialog == 5)
                        {
                            spriteBatch.DrawString(alagardFont, "Crush the Lake Guardian", new Vector2(147, 521), Color.White);
                            spriteBatch.Draw(soulCrocPortraitTex, new Vector2(886, 306), Color.White);
                        }
                        if (openingDialog == 6)
                        {
                            spriteBatch.DrawString(alagardFont, "Gale the Sky Guardian", new Vector2(162, 521), Color.White);
                            spriteBatch.Draw(soulBirdPortraitTex, new Vector2(780, 156), Color.White);
                        }
                        switch (openingDialog)
                        {
                            case 1:
                                spriteBatch.DrawString(alagardFont, "I'm your spirit now", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "With my power, You can use the power of the jungle's king's roar", new Vector2(132, 610), Color.White);
                                break;
                            case 2:
                                spriteBatch.DrawString(alagardFont, "When you use it, it can eliminate the robots around us in a short", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, " range within a certain period of time, Press Z and Up arrow to use this skill", new Vector2(132, 610), Color.White);
                                break;
                            case 3:
                                spriteBatch.DrawString(alagardFont, "But this ability is not always active, so use it crefully", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "Check the skill bar to see when it's active", new Vector2(132, 610), Color.White);
                                break;
                            case 4:
                                spriteBatch.DrawString(alagardFont, "And be careful, this place is dangerous", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "I heard the robots everywhere", new Vector2(132, 610), Color.White);
                                break;
                            case 5:
                                spriteBatch.DrawString(alagardFont, "This is terrifying! Look at those things they made.", new Vector2(132, 578), Color.White);
                                break;
                            case 6:
                                spriteBatch.DrawString(alagardFont, "The tree must be at the end of the laboratory. We must hurry!", new Vector2(132, 578), Color.White);
                                break;
                        }
                        break;
                    case GameState.INTROBOSS:
                        if (introBossDialog == 1 || introBossDialog == 3 || introBossDialog == 4 || introBossDialog == 6 || introBossDialog == 7)
                        {
                            spriteBatch.DrawString(alagardFont, "Viroj the Mad Scientist", new Vector2(152, 521), Color.White);
                            spriteBatch.Draw(bossPortraitTex, new Vector2(820, 306), Color.White);
                        }
                        if (introBossDialog == 2)
                        {
                            spriteBatch.DrawString(alagardFont, "Roark the Lake Guardian", new Vector2(142, 521), Color.White);
                            spriteBatch.Draw(soulLionPortraitTex, new Vector2(937, 255), Color.White);
                        }
                        if (introBossDialog == 5)
                        {
                            spriteBatch.DrawString(alagardFont, "Crush the Lake Guardian", new Vector2(147, 521), Color.White);
                            spriteBatch.Draw(soulCrocPortraitTex, new Vector2(886, 306), Color.White);
                        }
                        if (introBossDialog == 8)
                        {
                            spriteBatch.DrawString(alagardFont, "Gale the Sky Guardian", new Vector2(162, 521), Color.White);
                            spriteBatch.Draw(soulBirdPortraitTex, new Vector2(780, 156), Color.White);
                        }
                        switch (introBossDialog)
                        {
                            case 1:
                                spriteBatch.DrawString(alagardFont, "Oh! The precious creatures walked into my lab?! How wonderful!", new Vector2(132, 578), Color.White);
                                break;
                            case 2:
                                spriteBatch.DrawString(alagardFont, "Return the tree to us before we take it by ourself", new Vector2(132, 578), Color.White);
                                break;
                            case 3:
                                spriteBatch.DrawString(alagardFont, "Don't be hasty. Aren't you going to see what this tree can do? It's amazing", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "The energy from the tree is a source of unlimited power", new Vector2(132, 610), Color.White);
                                break;
                            case 4:
                                spriteBatch.DrawString(alagardFont, "These energies have helped me invent many great things", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "Why should I giving it back to you", new Vector2(132, 610), Color.White);
                                break;
                            case 5:
                                spriteBatch.DrawString(alagardFont, "Because it's belong to the forest, you psycho", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "You have no right to steal it from our forest", new Vector2(132, 610), Color.White);
                                break;
                            case 6:
                                spriteBatch.DrawString(alagardFont, "Damn creatures talked to much", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "Fine, if you want it, come and get it from me", new Vector2(132, 610), Color.White);
                                break;
                            case 7:
                                spriteBatch.DrawString(alagardFont, "But if you cant, I will catch you for my experiment", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "Muahahaha, be prepared!", new Vector2(132, 610), Color.White);
                                break;
                            case 8:
                                spriteBatch.DrawString(alagardFont, "I feel strange about this man, it seems like there's a tree power inside him", new Vector2(132, 578), Color.White);
                                spriteBatch.DrawString(alagardFont, "Focus, this guy is tough. If we lose, the tree earth will be gone forever", new Vector2(132, 610), Color.White);
                                break;
                        }
                        break;
                    case GameState.END:
                        if (endDialog == 1)
                        {
                            spriteBatch.DrawString(alagardFont, "Viroj the Mad Scientist", new Vector2(152, 525), Color.White);
                            spriteBatch.Draw(bossPortraitTex, new Vector2(820, 306), Color.White);
                        }
                        if (endDialog == 2)
                        {
                            spriteBatch.DrawString(alagardFont, "Roark the Lake Guardian", new Vector2(142, 521), Color.White);
                            spriteBatch.Draw(soulLionPortraitTex, new Vector2(937, 255), Color.White);
                        }
                        if (endDialog == 3)
                        {
                            spriteBatch.DrawString(alagardFont, "Crush the Lake Guardian", new Vector2(147, 521), Color.White);
                            spriteBatch.Draw(soulCrocPortraitTex, new Vector2(886, 306), Color.White);
                        }
                        if (endDialog == 4)
                        {
                            spriteBatch.DrawString(alagardFont, "Gale the Sky Guardian", new Vector2(162, 521), Color.White);
                            spriteBatch.Draw(soulBirdPortraitTex, new Vector2(780, 156), Color.White);
                        }
                        switch (endDialog)
                        {
                            case 1:
                                spriteBatch.DrawString(alagardFont, "Noooooooooo, I lost to these animals.", new Vector2(132, 578), Color.White);
                                break;
                            case 2:
                                spriteBatch.DrawString(alagardFont, "We actually did it!", new Vector2(132, 578), Color.White);
                                break;
                            case 3:
                                spriteBatch.DrawString(alagardFont, "Without this man, Earth will be more peaceful", new Vector2(132, 578), Color.White);
                                break;
                            case 4:
                                spriteBatch.DrawString(alagardFont, "Indeed. Now its time to do the thing we need to do", new Vector2(132, 578), Color.White);
                                break;
                        }
                        break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw tileset for map 3
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

                if (gamestate == GameState.PLAY)
                {
                    //draw enemy animation
                    foreach (RangeEnemy dog in shieldDogEnemies)
                    {
                        dog.Draw(spriteBatch);
                        foreach (EnemyBullet dogbullet in dog.bulletList)
                        {
                            dogbullet.Draw(spriteBatch);
                        }
                    }
                    foreach (MeleeEnemy tentacle in meleeTentacleEnemies)
                    {
                        tentacle.Draw(spriteBatch);
                    }
                    foreach (DroneEnemy drone in eyeballEnemies)
                    {
                        drone.Draw(spriteBatch);
                        foreach (EnemyBullet dronebullet in drone.bulletList)
                        {

                            dronebullet.Draw(spriteBatch);
                        }
                    }

                    ////draw switch animation
                    switch_wall1.Draw(spriteBatch);
                    switch_wall2.Draw(spriteBatch);
                    switch_wall3.Draw(spriteBatch);
                    switch_wall4.Draw(spriteBatch);
                    switch_wall5.Draw(spriteBatch);
                    switch_wall6.Draw(spriteBatch);
                    //draw wall
                    if (!isOpenSwitch1)
                    {
                        stage_wall1.Draw(spriteBatch);
                    }
                    //draw wall
                    if (!isOpenSwitch2)
                    {
                        stage_wall2.Draw(spriteBatch);
                    }
                    if (!isOpenSwitch3)
                    {
                        stage_wall3.Draw(spriteBatch);
                    }
                    if (!isOpenSwitch4)
                    {
                        stage_wall4.Draw(spriteBatch);
                    }
                    if (!isOpenSwitch5)
                    {
                        stage_wall5.Draw(spriteBatch);
                    }
                    if (!isOpenSwitch6)
                    {
                        stage_wall6.Draw(spriteBatch);
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
