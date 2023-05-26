using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DNet.Dynamics.Contacts;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using System.Linq;
using static ScifiDruid.Singleton;
using System.Diagnostics;

namespace ScifiDruid.GameObjects
{
    public class EnemyBullet : _GameObject
    {
        protected Texture2D texture; //enemy Texture (Animaiton)

        protected GameTime gameTime;

        //bullet state
        public BulletStatus enemyBulletStatus;

        //wallOrigin
        private Vector2 bulletOrigin;

        private bool animationDead = false;

        //bullet status
        private BulletStatus preStatus = BulletStatus.BULLETALIVE;

        public Body bulletBody;

        //animation
        public int bulletSizeX;
        public int bulletSizeY;

        private Vector2 bulletAliveSize;
        private Vector2 bulletDeadSize;

        private List<Vector2> bulletAliveRectVector;
        private List<Vector2> bulletDeadRectVector;

        private int bulletAliveCount;
        private int bulletDeadCount;
        //frames 
        private int frames = 0;
        private int allframes;

        public int bulletSpeed;
        public int bulletDistance;

        //all sprite position in spritesheet
        private Rectangle sourceRect;

        //srpite to run
        private Vector2 spriteSize;
        private List<Vector2> spriteVector;

        //time
        private float elapsed;
        private float delay;
        private bool isShootup;
        private Enemy self; //Use to not collide with enemy

        private List<Vector2> bulletSize;
        private List<List<Vector2>> bulletAliveSpriteList;

        public bool isdrone;


        public enum BulletStatus
        {
            BULLETALIVE,
            BULLETDEAD,
            BULLETEND
        }



        public EnemyBullet(Texture2D texture, Vector2 position, Enemy self, SpriteEffects charDirection) : base(texture)
        {
            this.texture = texture;
            this.charDirection = charDirection;
            this.position = position;
            this.self = self;
        }

        public void CreateBullet(int worldLevel, bool isdrone)
        {
            //build object
            bulletBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(bulletSizeX), ConvertUnits.ToSimUnits(bulletSizeY), 0, position, 0, BodyType.Dynamic, "EnemyBullet");
            bulletBody.IgnoreGravity = true;
            bulletBody.IgnoreCollisionWith(self.enemyHitBox);
            bulletBody.IsSensor = true;

            //animation
            enemyBulletStatus = BulletStatus.BULLETALIVE;
            preStatus = BulletStatus.BULLETALIVE;

            this.isdrone = isdrone;


            switch (worldLevel)
            {
                case 1:
                    //stage1
                    bulletSize = new List<Vector2>() { new Vector2(26, 30), new Vector2(26, 30) };
                    bulletAliveSpriteList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(265, 38) }, new List<Vector2>() { new Vector2(304, 38) } };
                    bulletAliveRectVector = bulletAliveSpriteList[0];
                    bulletDeadRectVector = bulletAliveSpriteList[1];
                    bulletAliveSize = bulletSize[0];
                    bulletDeadSize = bulletSize[1];
                    break;

                case 2:
                    //stage2

                    if (!isdrone)
                    {
                        //range
                        bulletSize = new List<Vector2>() { new Vector2(9, 3), new Vector2(8, 13) };
                        bulletAliveSpriteList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(245, 264) }, new List<Vector2>() { new Vector2(276, 260), new Vector2(305, 260) } };
                        bulletAliveRectVector = bulletAliveSpriteList[0];
                        bulletDeadRectVector = bulletAliveSpriteList[1];
                        bulletAliveSize = bulletSize[0];
                        bulletDeadSize = bulletSize[1];
                    }
                    else
                    {
                        //drone
                        bulletSize = new List<Vector2>() { new Vector2(3, 14), new Vector2(27, 14) };
                        bulletAliveSpriteList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(244, 92) }, new List<Vector2>() { new Vector2(306, 92), new Vector2(382, 92), new Vector2(450, 92) } };
                        bulletAliveRectVector = bulletAliveSpriteList[0];
                        bulletDeadRectVector = bulletAliveSpriteList[1];
                        bulletAliveSize = bulletSize[0];
                        bulletDeadSize = bulletSize[1];
                    }
                    break;

                case 3:
                    //stage3
                    if (!isdrone)
                    {
                        //range
                        bulletSize = new List<Vector2>() { new Vector2(14, 6), new Vector2(14, 24) };
                        bulletAliveSpriteList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(1138, 12) }, new List<Vector2>() { new Vector2(1194, 184), new Vector2(1254, 4) } };
                        bulletAliveRectVector = bulletAliveSpriteList[0];
                        bulletDeadRectVector = bulletAliveSpriteList[1];
                        bulletAliveSize = bulletSize[0];
                        bulletDeadSize = bulletSize[1];
                    }
                    else
                    {
                        //drone
                        bulletSize = new List<Vector2>() { new Vector2(16, 8), new Vector2(12, 14) };
                        bulletAliveSpriteList = new List<List<Vector2>>() { new List<Vector2>() { new Vector2(385, 34) }, new List<Vector2>() { new Vector2(415, 31), new Vector2(439, 31) } };
                        bulletAliveRectVector = bulletAliveSpriteList[0];
                        bulletDeadRectVector = bulletAliveSpriteList[1];
                        bulletAliveSize = bulletSize[0];
                        bulletDeadSize = bulletSize[1];
                    }
                    break;

            }
            if (Singleton.Instance.levelState == LevelState.FOREST)
            {
                switch (charDirection)
                {
                    case SpriteEffects.None:
                        bulletBody.Position += new Vector2(-0.5f, 0);
                        break;
                    case SpriteEffects.FlipHorizontally:
                        bulletBody.Position += new Vector2(1f, 0);
                        break;
                }
            }
            else
            {
                switch (charDirection)
                {
                    case SpriteEffects.None:
                        bulletBody.Position += new Vector2(0.3f, 0);
                        break;
                    case SpriteEffects.FlipHorizontally:
                        bulletBody.Position += new Vector2(-0.3f, 0);
                        break;
                }
            }


            bulletAliveSize = new Vector2(bulletSizeX, bulletSizeY);
            bulletAliveCount = bulletAliveRectVector.Count();
            bulletDeadCount = bulletDeadRectVector.Count();

            bulletOrigin = new Vector2(bulletSizeX / 2, bulletSizeY / 2);
        }

        public override void Update(GameTime gameTime)
        {
            ChangeBulletAnimationStatus();
            
            IsContact();
            IsOutRange();
            
            position = bulletBody.Position;
            //if dead animation animationEnd
            if (animationDead)
            {
                enemyBulletStatus = BulletStatus.BULLETEND;
            }

            if (preStatus != enemyBulletStatus)
            {
                frames = 0;
            }

            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsed >= delay)
            {
                if (enemyBulletStatus == BulletStatus.BULLETALIVE)
                {
                    if (frames < allframes - 1)
                    {
                        frames++;
                    }
                    else if (frames >= allframes - 1)
                    {
                        frames = 0;
                    }
                }
                else if (enemyBulletStatus == BulletStatus.BULLETDEAD)
                {
                    if (frames < allframes - 1)
                    {
                        frames++;
                    }
                    else if (frames >= allframes - 1)
                    {
                        animationDead = true;
                        return;
                    }
                }
                elapsed = 0;
            }

            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            preStatus = enemyBulletStatus;
        }

        public void Shoot()
        {
            if (isdrone)
            {
                bulletBody.ApplyForce(new Vector2(0, bulletSpeed));
                rotation = MathHelper.ToRadians(90);
                charDirection = SpriteEffects.FlipVertically;
            }
            if (Singleton.Instance.levelState == LevelState.FOREST)
            {
                switch (charDirection)
                {
                    case SpriteEffects.None:
                        bulletBody.ApplyForce(new Vector2(-bulletSpeed, 0));
                        enemyBulletStatus = BulletStatus.BULLETALIVE;
                        break;
                    case SpriteEffects.FlipHorizontally:
                        bulletBody.ApplyForce(new Vector2(bulletSpeed, 0));
                        enemyBulletStatus = BulletStatus.BULLETALIVE;
                        break;
                }
            }
            else
            {
                switch (charDirection)
                {
                    case SpriteEffects.FlipHorizontally:
                        bulletBody.ApplyForce(new Vector2(-bulletSpeed, 0));
                        enemyBulletStatus = BulletStatus.BULLETALIVE;
                        break;
                    case SpriteEffects.None:
                        bulletBody.ApplyForce(new Vector2(bulletSpeed, 0));
                        enemyBulletStatus = BulletStatus.BULLETALIVE;
                        break;
                }

            }

            enemyBulletStatus = BulletStatus.BULLETALIVE;
        }

        public void IsContact()
        {
            ContactEdge contactEdge = bulletBody.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                Body fixtureA_Body = contactEdge.Contact.FixtureA.Body;
                Body fixtureB_Body = contactEdge.Contact.FixtureB.Body;

                bool contactA = (fixtureA_Body.UserData != null && fixtureA_Body.UserData.Equals("Ground"));
                bool contactB = (fixtureB_Body.UserData != null && fixtureB_Body.UserData.Equals("Ground"));
                bool contactGround = contactA || contactB;

                contactA = (fixtureA_Body.UserData != null && fixtureA_Body.UserData.Equals("Player"));
                contactB = (fixtureB_Body.UserData != null && fixtureB_Body.UserData.Equals("Player"));
                bool contactEnemy = contactA || contactB;

                if (contactFixture.IsTouching && (contactGround || contactEnemy))
                {
                    bulletBody.Dispose();
                    return ;
                }

                // Check if the contact fixture is the ground
                contactEdge = contactEdge.Next;
            }
        }

        public void IsOutRange()
        {
            if (position.X - bulletBody.Position.X < -bulletDistance || position.X - bulletBody.Position.X > bulletDistance)
            {
                // The Bullet was Out of range
                bulletBody.Dispose();
                enemyBulletStatus = BulletStatus.BULLETEND;
                return;
            }
        }

        public void ChangeBulletAnimationStatus()
        {
            switch (enemyBulletStatus)
            {
                case BulletStatus.BULLETALIVE:
                    delay = 150f;
                    spriteVector = bulletAliveRectVector;
                    spriteSize = bulletAliveSize;
                    allframes = bulletAliveCount;
                    break;
                case BulletStatus.BULLETDEAD:
                    delay = 200f;
                    spriteVector = bulletDeadRectVector;
                    spriteSize = bulletDeadSize;
                    allframes = bulletDeadCount;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!bulletBody.IsDisposed)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, bulletOrigin, 1f, charDirection, 0f);
            }
        }
    }
}

