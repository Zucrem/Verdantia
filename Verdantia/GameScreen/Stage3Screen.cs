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
        //create guardian tex
        private Guardian guardian;

        //create switch and wall
        private SwitchWall switch_wall1;
        private StageObject stage_wall1;

        private SwitchWall switch_wall2;
        private StageObject stage_wall2;

        //panel moving and falling
        private StageObject squashBlock;
        private List<StageObject> squashGroup;

        //create enemy
        private List<Enemy> allEnemies;
        private DoctorBoss boss;

        private RangeEnemy shieldDog;
        private List<RangeEnemy> shieldDogEnemies;
        private int shieldDogPositionList;
        private int shieldDogCount;

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
        private Vector2 wall_textureSize = new Vector2(0, 10);

        private Vector2 panel_size = new Vector2(64, 6);
        private Vector2 panel_textureSize = new Vector2(64, 66);

        //check boss state
        private bool bossDead = false;

        //state 3 dialog
        private int openingDialog = 1;
        private int introBossDialog = 1;
        private int endDialog = 1;

        public override void Initial()
        {
            base.Initial();
            Player.level3Unlock = true;

            //map size
            startmaptileX = 10f;
            endmaptileX = 170f;

            Player.health = 5;
            Player.mana = 100;
            Player.maxHealth = 5;
            Player.maxMana = 100;
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

            //create player on position
            player.Initial(startRect);
            //player.Initial(bossState);

            Vector2 guardianSize = new Vector2(49, 55);
            List<Vector2> guardianAnimateList = new List<Vector2>() { new Vector2(10, 2), new Vector2(67, 2), new Vector2(4, 59), new Vector2(61, 59) };
            //guardian = new Guardian(guardianTex, guardianSize, guardianAnimateList);


            //create enemy on position
            allEnemies = new List<Enemy>();


            //range enemy
            shieldDogEnemies = new List<RangeEnemy>();
            shieldDogPositionList = ground1MonsterRects.Count();
            List<Vector2> shieldDogSizeList = new List<Vector2>() { new Vector2(64, 47), new Vector2(64, 47), new Vector2(64, 47), new Vector2(69, 47) };
            List<List<Vector2>> shieldDogAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 0) }, new List<Vector2>() { new Vector2(0, 0), new Vector2(147, 0), new Vector2(297, 0) }, new List<Vector2>() { new Vector2(0, 0), new Vector2(457, 0) }, new List<Vector2>() { new Vector2(0, 85), new Vector2(141, 85), new Vector2(291, 85) } };
            for (int i = 0; i < shieldDogPositionList; i++)
            {
                shieldDog = new RangeEnemy(shieldDogTex, shieldDogSizeList, shieldDogAnimateList)
                {
                    size = new Vector2(64, 47),
                    health = 3,
                    speed = 0.1f,
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

            //melee enemy
            meleeTentacleEnemies = new List<MeleeEnemy>();
            meleeTentaclePositionList = ground2MonsterRects.Count();
            List<Vector2> meleePoliceSizeList = new List<Vector2>() { new Vector2(49, 83), new Vector2(49, 83), new Vector2(49, 83), new Vector2(123, 111), new Vector2(66, 83) };
            List<List<Vector2>> meleePoliceAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(1, 28) }, new List<Vector2>() { new Vector2(1, 28), new Vector2(148, 28), new Vector2(298, 28) }, new List<Vector2>() { new Vector2(1, 28), new Vector2(148, 28), new Vector2(298, 28) }, new List<Vector2>() { new Vector2(415, 0), new Vector2(549, 0) }, new List<Vector2>() { new Vector2(0, 140), new Vector2(142, 140), new Vector2(290, 139), new Vector2(446, 139) } };
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
            boss = new DoctorBoss(doctorBossTex)
            {
                size = new Vector2(38, 88),
                health = 6,
                speed = 1.2f,
            };
            //spawn boss
            boss.Initial(bossRect, player);

            //add to all enemy for
            allEnemies.AddRange(shieldDogEnemies);
            allEnemies.AddRange(meleeTentacleEnemies);
            allEnemies.Add(boss);

            //switch event
            //create switch button on position
            /*switch_wall1 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) { size = new Vector2(32, 32) };
            switch_wall1.Initial(switch_button1);

            switch_wall2 = new SwitchWall(switch_wall_Tex, switch_size, switch_close_textureSize, switch_open_textureSize) { size = new Vector2(32, 32) };
            switch_wall2.Initial(switch_button2);

            //create wall button on position
            stage_wall1 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) { size = new Vector2(64, 182) };
            stage_wall1.Initial(sign_wall1);

            stage_wall2 = new StageObject(switch_wall_Tex, wall_size, wall_textureSize) { size = new Vector2(64, 182) };
            stage_wall2.Initial(sign_wall2);*/

            //add all enemy for player to know em all
            player.enemies = allEnemies;
        }
        public override void LoadContent()
        {
            base.LoadContent();

            //button and rock wall
            switch_wall_Tex = content.Load<Texture2D>("Pictures/Play/StageScreen/Stage2Tileset/specialProps2");
            //guardianTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/birdTex");
            //guardianTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/crocTex");
            //guardianTex = content.Load<Texture2D>("Pictures/Play/Characters/Guardian/lionTex");

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
                //stage 3 dialog
                if (gamestate == GameState.OPENING)
                {
                    //change dialog
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        openingDialog++;
                    }
                }
                if (gamestate == GameState.INTROBOSS)
                {
                    //change dialog
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        introBossDialog++;
                    }
                }
                if (gamestate == GameState.END)
                {
                    //change dialog
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        endDialog++;
                    }
                }

                if (gamestate == GameState.OPENING || gamestate == GameState.END)
                {
                    //guardian.Update(gameTime);
                }
                if (gamestate == GameState.PLAY)
                {
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

                    //switch button
                    /*switch_wall1.Update(gameTime);
                    switch_wall2.Update(gameTime);
                    //stage wall
                    stage_wall1.Update(gameTime);
                    stage_wall2.Update(gameTime);*/

                    //switch event
                    //press switch button
                    /*if (!isOpenSwitch1 && switch_wall1.pressSwitch)
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
                    }*/

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
                }
                if (gamestate == GameState.END)
                {
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw tileset for map 3
            if (play)
            {
                //draw dialog box, spacebar text and skip text
                if (gamestate == GameState.OPENING || gamestate == GameState.INTROBOSS || gamestate == GameState.END)
                {
                    //draw
                }
                //Dialog OPENING
                if (gamestate == GameState.OPENING)
                {
                    switch (openingDialog)
                    {
                        case 1:
                            break;
                    }
                }
                //Dialog INTROBOSS
                if (gamestate == GameState.INTROBOSS)
                {
                    switch (introBossDialog)
                    {
                        case 1:
                            break;
                    }
                }
                //Dialog END
                if (gamestate == GameState.END)
                {
                    switch (endDialog)
                    {
                        case 1:
                            break;
                    }
                }

                if (gamestate == GameState.OPENING || gamestate == GameState.END)
                {
                    //guardian.Draw(spriteBatch);
                }

                if (gamestate == GameState.START || gamestate == GameState.OPENING || gamestate == GameState.PLAY || gamestate == GameState.INTROBOSS || gamestate == GameState.BOSS || gamestate == GameState.END)
                {
                    //draw player animation
                    player.Draw(spriteBatch);

                    tilemapManager.Draw(spriteBatch);
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
                    }
                    foreach (MeleeEnemy tentacle in meleeTentacleEnemies)
                    {
                        tentacle.Draw(spriteBatch);
                    }

                    ////draw switch animation
                    /*switch_wall1.Draw(spriteBatch);
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
                    }*/
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
