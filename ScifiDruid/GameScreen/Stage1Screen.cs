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

namespace ScifiDruid.GameScreen
{
    class Stage1Screen : PlayScreen
    {
        //create enemy
        protected List<Enemy> allEnemies;
        protected LucasBoss boss;

        protected RangeEnemy flameMech;
        protected List<RangeEnemy> flameMechEnemies;
        protected int flameMechPositionList;
        protected int flameMechCount;

        protected MeleeEnemy chainsawMech;
        protected List<MeleeEnemy> chainsawMechEnemies;
        protected int chainsawMechPositionList;
        protected int chainsawMechCount;

        //special occasion position
        protected Rectangle wallblock;
        protected Rectangle boss_left_side;
        protected Rectangle boss_right_side;
        protected Rectangle boss_event;

        public override void Initial()
        {
            base.Initial();

            startmaptileX = 10f;
            endmaptileX = 170f;

            Player.health = 5;
            Player.mana = 100;
            Player.maxHealth = 5;
            Player.maxMana = 100;
            Player.level2Unlock = false;
            Player.level3Unlock = false;

            //create tileset for map1
            //map = new TmxMap("Content/stage1test.tmx");
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
            }
            foreach (var o in map.ObjectGroups["SpecialOccasions"].Objects)
            {
                if (o.Name.Equals("wallblock"))
                {
                    wallblock = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                }
                if (o.Name.Equals("left_side"))
                {
                    boss_left_side = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height); 
                    Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(boss_left_side.Width), ConvertUnits.ToSimUnits(boss_left_side.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(boss_left_side.X, boss_left_side.Y)));
                    body.UserData = "Boss_left_side";
                    body.IsSensor = true;
                }
                if (o.Name.Equals("right_side"))
                {
                    boss_right_side = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(boss_right_side.Width), ConvertUnits.ToSimUnits(boss_right_side.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(boss_right_side.X, boss_right_side.Y)));
                    body.UserData = "Boss_right_side";
                    body.IsSensor = true;
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

            player.Initial(startRect);
            //player.Initial(boss_event);

            //create enemy on position
            allEnemies = new List<Enemy>();

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
            List<Vector2> chainsawMechSizeList = new List<Vector2>() { new Vector2(118, 100), new Vector2(136, 100), new Vector2(118, 100) };
            List<List<Vector2>> chainsawMechAnimateList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(0, 0), new Vector2(144, 0) }, new List<Vector2>() { new Vector2(0, 136), new Vector2(136, 136) }, new List<Vector2>() { new Vector2(0, 254), new Vector2(142, 254) } };
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
            boss = new LucasBoss(lucasBossTex,whiteTex)
            {
                size = new Vector2(196, 186),
                health = 6,
                speed = 1.2f,
            };
            //spawn boss
            boss.Initial(bossRect, player);

            //add to all enemy for
            allEnemies.AddRange(flameMechEnemies);
            allEnemies.AddRange(chainsawMechEnemies);
            allEnemies.Add(boss);


            //add all enemy for player to know em all
            player.enemies = allEnemies;
        }
        public override void LoadContent()
        {
            base.LoadContent();

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
                        //boss
                        boss.Update(gameTime);

                        //if player get into boss state
                        if (player.IsContact("Boss_event", "A") && !created_boss)
                        {
                            boss_area = true;
                        }
                        //if player is in boss area just spawn
                        if (boss_area && !created_boss)
                        {
                            boss.isAlive = true;

                            //create block to block player
                            Body body = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(wallblock.Width), ConvertUnits.ToSimUnits(wallblock.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(wallblock.X, wallblock.Y)));
                            body.UserData = "ground";
                            created_boss = true;
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
                    foreach (RangeEnemy flameBot in flameMechEnemies)
                    {
                        flameBot.Draw(spriteBatch);
                    }
                    foreach (MeleeEnemy chainsawBot in chainsawMechEnemies)
                    {
                        chainsawBot.Draw(spriteBatch);
                    }

                    player.Draw(spriteBatch);

                    //draw boss animation
                    boss.Draw(spriteBatch);

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
