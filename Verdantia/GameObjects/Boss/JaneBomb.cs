using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DNet.Dynamics.Contacts;
using Box2DNet.Dynamics;
using Microsoft.Xna.Framework.Input;
using Box2DNet.Factories;
using Box2DNet;
using System.Diagnostics;
using static ScifiDruid.GameObjects.PlayerSkillAnimation;
using System.Linq;
using static ScifiDruid.GameObjects.PlayerBullet;

namespace ScifiDruid.GameObjects
{
    public class JaneBomb : _GameObject
    {
        protected Texture2D texture; //enemy Texture (Animaiton)
        private Enemy enemy;
        protected GameTime gameTime;

        //bullet state
        public BombStatus bossBombStatus;

        //wallOrigin
        private Vector2 bombOrigin;
        public int textureWidth;
        public int textureHeight;

        private bool animationDead = false;

        public Body bombBody;
        public Body explosiveBody;

        //animation
        public Vector2 bombSize;
        //sprite to run
        private Vector2 spriteSize;
        private List<Vector2> spriteVector;
        //frames 
        public int frames = 0;
        private int allframes;

        //bomb status
        private BombStatus preStatus = BombStatus.BOMBALIVE;

        private int bombSpeed;
        private int bombDistance;

        //all sprite position in spritesheet
        protected Rectangle sourceRect;


        //time
        private float elapsed;
        private float delay = 150f;

        private Vector2 bombAliveSize = new Vector2(54, 56);
        private List<Vector2> bombAliveAnimateList = new List<Vector2>() { new Vector2(20, 830) };

        private Vector2 bombDeadSize = new Vector2(114, 108);
        private List<Vector2> bombDeadAnimateList = new List<Vector2>() { new Vector2(100, 774), new Vector2(224, 774), new Vector2(367, 774), new Vector2(507, 778) };

        public enum BombStatus
        {
            BOMBALIVE,
            BOMBDEAD,
            BOMBEND
        }

        public JaneBomb(Texture2D texture, Vector2 position, Enemy enemy) : base(texture)
        {
            this.texture = texture;
            this.enemy = enemy;
            this.position = position;

            bossBombStatus = BombStatus.BOMBALIVE;

            //animation
            bombSize = bombAliveSize;
            spriteVector = bombAliveAnimateList;

            spriteSize = bombSize;

            //create wall hitbox
            bombBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(bombSize.X), ConvertUnits.ToSimUnits(bombSize.Y), 0, position, 0, BodyType.Dynamic, "SkillBoss");
            bombBody.IgnoreCollisionWith(enemy.enemyHitBox);
            bombBody.IsSensor = true;

            bombOrigin = new Vector2(bombSize.X / 2, bombSize.Y / 2);
        }

        public void CreateExplosive(Vector2 ExplosivePosition)
        {
            //animation
            bombSize = bombDeadSize;
            spriteVector = bombDeadAnimateList;

            //create wall hitbox
            explosiveBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(bombSize.X), ConvertUnits.ToSimUnits(bombSize.Y), 0, ExplosivePosition, 0, BodyType.Static, "SkillBoss");
            explosiveBody.IgnoreCollisionWith(enemy.enemyHitBox);
            explosiveBody.IsSensor = true;
        }

        public override void Update(GameTime gameTime)
        {
            ChangeBombStatus();
            if (explosiveBody != null)
            {
                position = explosiveBody.Position;
            }
            else
            {
                position = bombBody.Position;
            }
            //if dead animation animationEnd
            if (animationDead)
            {
                bossBombStatus = BombStatus.BOMBEND;
            }

            if (preStatus != bossBombStatus)
            {
                frames = 0;
            }
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsed >= delay)
            {
                if (bossBombStatus == BombStatus.BOMBALIVE)
                {
                    if (frames < allframes - 1)
                    {
                        frames++;
                    }
                }
                else if (bossBombStatus == BombStatus.BOMBDEAD)
                {
                    if (frames < allframes - 1)
                    {
                        frames++;
                    }
                    else
                    {
                        animationDead = true;
                        return;
                    }
                }
                elapsed = 0;
            }

            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            preStatus = bossBombStatus;
        }

        public void ChangeBombStatus()
        {
            switch (bossBombStatus)
            {
                case BombStatus.BOMBALIVE:
                    delay = 200f;
                    spriteVector = bombAliveAnimateList;
                    spriteSize = new Vector2(bombAliveSize.X, bombAliveSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case BombStatus.BOMBDEAD:
                    delay = 200f;
                    spriteVector = bombDeadAnimateList;
                    spriteSize = new Vector2(bombDeadSize.X, bombDeadSize.Y);
                    allframes = spriteVector.Count();
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (bossBombStatus != BombStatus.BOMBEND)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, bombOrigin, 1f, charDirection, 0f);
            }
        }

    }
}

