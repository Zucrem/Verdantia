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
    public class DoctorLaser : _GameObject
    {
        protected Texture2D texture; //enemy Texture (Animaiton)

        protected GameTime gameTime;

        //bullet state
        public LaserStatus bossLaserStatus;

        //wallOrigin
        private Vector2 laserOrigin;
        public int textureWidth;
        public int textureHeight;

        private bool animationDead = false;

        public Body laserBody;

        //animation
        private Vector2 laserSize;
        //sprite to run
        private Vector2 spriteSize;
        private List<Vector2> spriteVector;
        //frames 
        public int frames = 0;
        private int allframes;

        //all sprite position in spritesheet
        protected Rectangle sourceRect;

        //ball speed
        private int laserSpeed;

        //time
        private float elapsed;
        private float delay = 200f;

        private Vector2 laserAliveSize = new Vector2(28, 324);
        private List<Vector2> laserAliveAnimateList = new List<Vector2>() { new Vector2(16, 4), new Vector2(62, 4)};

        public enum LaserStatus
        {
            LASERALIVE,
            LASEREND
        }

        public DoctorLaser(Texture2D texture, Vector2 position, Enemy enemy) : base(texture)
        {
            this.texture = texture;
            this.position = position;

            laserSpeed = 400;

            //create wall hitbox
            laserBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(laserSize.X), ConvertUnits.ToSimUnits(laserSize.Y), 0, position, 0, BodyType.Dynamic, "EnemyBullet");
            laserBody.IgnoreGravity = true;
            laserBody.IgnoreCollisionWith(enemy.enemyHitBox);

            //animation
            laserSize = laserAliveSize;
            spriteVector = laserAliveAnimateList;

            spriteSize = laserSize;

            switch (charDirection)
            {
                case SpriteEffects.None:
                    laserBody.Position += new Vector2(-1f, 0);
                    break;
                case SpriteEffects.FlipHorizontally:
                    laserBody.Position += new Vector2(1f, 0);
                    break;
            }

            laserOrigin = new Vector2(laserSize.X / 2, laserSize.Y / 2);
        }

        public void Shoot()
        {
            bossLaserStatus = LaserStatus.LASERALIVE;
        }

        public override void Update(GameTime gameTime)
        {
            //if dead animation animationEnd
            if (animationDead)
            {
                bossLaserStatus = LaserStatus.LASEREND;
            }

            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsed >= delay)
            {
                if (bossLaserStatus == LaserStatus.LASERALIVE)
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
                elapsed = 0;
            }

            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, laserOrigin, 1f, charDirection, 0f);
        }

        public bool IsContact()
        {
            ContactEdge contactEdge = laserBody.ContactList;
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
                    laserBody.Dispose();
                    return true;
                }

                // Check if the contact fixture is the ground
                contactEdge = contactEdge.Next;
            }
            return false;
        }
    }
}

