﻿using Microsoft.Xna.Framework.Graphics;
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

namespace ScifiDruid.GameScreen
{
    class Stage2Screen : PlayScreen
    {
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

        /*private RangeEnemy flameMech;
        private List<RangeEnemy> flameMechEnemies;
        private int flameMechPositionList;
        private int flameMechCount;

        private MeleeEnemy chainsawMech;
        private List<MeleeEnemy> chainsawMechEnemies;
        private int chainsawMechPositionList;
        private int chainsawMechCount;*/

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
            Player.level2Unlock = false;
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
                //chainsaw machine position
                if (o.Name.Equals("fly_mon"))
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

            //player.Initial(startRect);
            player.Initial(bossState);

            //create enemy on position
            allEnemies = new List<Enemy>();

            /*
            //range enemy
            flameMechEnemies = new List<RangeEnemy>();
            flameMechPositionList = ground1MonsterRects.Count();
            List<Vector2> flameMechSizeList = new List<Vector2>() { new Vector2(112, 86), new Vector2(112, 86) };
            List<List<Vector2>> flameMechAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 0), new Vector2(112, 0) }, new List<Vector2>() { new Vector2(0, 108), new Vector2(112, 108) } };
            for (int i = 0; i < flameMechPositionList; i++)
            {
                flameMech = new RangeEnemy(flameMechTex, flameMechSizeList, flameMechAnimateList)
                {
                    size = new Vector2(112, 86),
                    health = 3,
                    speed = 0.22f,
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
            List<Vector2> chainsawMechSizeList = new List<Vector2>() { new Vector2(118, 100), new Vector2(136, 100), new Vector2(136, 100), new Vector2(118, 100) };
            List<List<Vector2>> chainsawMechAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 0), new Vector2(144, 0) }, new List<Vector2>() { new Vector2(0, 136), new Vector2(136, 136) }, new List<Vector2>() { new Vector2(0, 136), new Vector2(136, 136) }, new List<Vector2>() { new Vector2(0, 254), new Vector2(142, 254) } };
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
            }*/

            //create boss on position
            boss = new JaneBoss(janeBossTex,whiteTex)
            {
                size = new Vector2(74, 112),
                health = 6,
                speed = 1.2f,
            };
            //spawn boss
            boss.Initial(bossRect, player, boss_event);

            //add to all enemy for
            //allEnemies.AddRange(flameMechEnemies);
            //allEnemies.AddRange(chainsawMechEnemies);
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
                if (gamestate == GameState.PLAY)
                {
                    if (!Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        //all enemy
                        /*foreach (RangeEnemy flamewBot in flameMechEnemies)
                        {
                            flamewBot.Update(gameTime);
                            flamewBot.Action();
                        }
                        foreach (MeleeEnemy chainsawBot in chainsawMechEnemies)
                        {
                            chainsawBot.Update(gameTime);
                            chainsawBot.Action();
                        }*/
                        //boss
                        boss.Update(gameTime);

                        //check if boss death then
                        if (!boss.isAlive && created_boss)
                        {
                            MediaPlayer.Stop();
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
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw tileset for map1
            if (play)
            {
                if (gamestate == GameState.START || gamestate == GameState.PLAY)
                {
                    tilemapManager.Draw(spriteBatch);

                    //draw enemy animation
                    /*foreach (RangeEnemy flameBot in flameMechEnemies)
                    {
                        flameBot.Draw(spriteBatch);
                    }
                    foreach (MeleeEnemy chainsawBot in chainsawMechEnemies)
                    {
                        chainsawBot.Draw(spriteBatch);
                    }*/

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
