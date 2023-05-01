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

namespace ScifiDruid.GameScreen
{
    class Stage1Screen : PlayScreen
    {

        public override void Initial()
        {
            base.Initial();
            startmaptileX = 10f;
            endmaptileX = 170f;

            Player.health = 5;
            Player.mana = 100;

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
            groundMonsterRects = new List<Rectangle>();
            flyMonsterRects = new List<Rectangle>();
            bossRects = new List<Rectangle>();

            polygon = new Dictionary<Polygon, Vector2>();
            //add list rectangle
            foreach (var o in map.ObjectGroups["Blocks"].Objects)
            {
                if (o.Name == "")
                {
                    blockRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                }
                if (o.Name == "triangle")
                {
                    Vertices vertices = new Vertices();
                    foreach (var item in o.Points)
                    {
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2((float)item.X, (float)item.Y)));
                    }

                    Vector2 position = new Vector2((float)o.X, (float)o.Y);
                    Polygon poly = new Polygon(vertices, true);
                    polygon.Add(poly, position);
                }
            }
            foreach (var o in map.ObjectGroups["Player"].Objects)
            {
                if (o.Name == "startRect")
                {
                    startRect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                }
                if (o.Name == "end")
                {
                    endRect = new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height);
                    //playerRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                }
            }
            foreach (var o in map.ObjectGroups["SpecialBlocks"].Objects)
            {
                if (o.Name == "water")
                {
                    deadBlockRects.Add(new Rectangle((int)o.X + ((int)o.Width / 2), (int)o.Y + ((int)o.Height / 2), (int)o.Width, (int)o.Height));
                }
            }
            foreach (var o in map.ObjectGroups["SpecialProps"].Objects)
            {
            }
            foreach (var o in map.ObjectGroups["GroundMonster"].Objects)
            {
            }
            foreach (var o in map.ObjectGroups["FlyingMonster"].Objects)
            {
            }
            foreach (var o in map.ObjectGroups["Boss"].Objects)
            {
            }

            //create collision for block in the world
            foreach (Rectangle rect in blockRects)
            {
                Vector2 collisionPosition = ConvertUnits.ToSimUnits(new Vector2(rect.X, rect.Y));
                //Singleton.Instance.world.Step(0.001f);

                Body body1 = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(rect.Width), ConvertUnits.ToSimUnits(rect.Height), 1f, collisionPosition);
                body1.UserData = "ground";
                body1.Restitution = 0.0f;
                body1.Friction = 0.3f;
            }

            foreach (var poly in polygon)
            {
                Vector2 collisionPosition = ConvertUnits.ToSimUnits(poly.Value);
                //Singleton.Instance.world.Step(0.001f);

                Body body1 = BodyFactory.CreatePolygon(Singleton.Instance.world, poly.Key.Vertices, 1f, collisionPosition);
                body1.UserData = "ground";
                body1.Restitution = 0.0f;
                body1.Friction = 0.0f;
            }
            //create dead block for block in the world
            foreach (Rectangle rect in deadBlockRects)
            {
                Vector2 deadBlockPosition = ConvertUnits.ToSimUnits(new Vector2(rect.X, rect.Y));
                //Singleton.Instance.world.Step(0.001f);

                Body body2 = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(rect.Width), ConvertUnits.ToSimUnits(rect.Height), 1f, deadBlockPosition);
                body2.UserData = "dead";
                body2.Restitution = 0.0f;
                body2.Friction = 0.3f;
            }

            player.Initial(startRect);
            enemy.Initial(startRect);
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

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //draw tileset for map1
            if (play)
            {
                if (gamestate == GameState.START || gamestate == GameState.PLAY)
                {
                    tilemapManager.Draw(spriteBatch);
                    //foreach (var poly in polygon)
                    //{
                    //    spriteBatch.Draw(blackTex, ConvertUnits.ToDisplayUnits(poly.Key), Color.White);
                    //}

                    if (!fadeFinish)
                    {
                        spriteBatch.Draw(blackTex, Vector2.Zero, colorStart);
                    }
                }
            }
        }
    }
}
