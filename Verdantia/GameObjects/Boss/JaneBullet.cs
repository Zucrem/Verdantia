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
    public class JaneBullet : _GameObject
    {
        protected Texture2D texture; //enemy Texture (Animaiton)

        protected GameTime gameTime;

        //bullet state
        private BulletStatus bossBulletStatus;

        //wallOrigin
        private Vector2 bulletOrigin;
        public int textureWidth;
        public int textureHeight;

        private bool animationDead = false;

        public Body bulletBody;

        //animation
        private int bulletSizeX;
        private int bulletSizeY;
        private Vector2 bulletSize;
        //frames 
        public int frames = 0;

        //bullet status
        private BulletStatus preStatus = BulletStatus.BULLETALIVE;

        private int bulletSpeed;
        private int bulletDistance;

        //all sprite position in spritesheet
        protected Rectangle sourceRect;

        //srpite to run
        private Vector2 spriteSize;
        private Vector2 spriteVector;

        private enum BulletStatus
        {
            BULLETALIVE,
            BULLETEND
        }

        public JaneBullet(Texture2D texture, Vector2 position, Enemy self, SpriteEffects charDirection) : base(texture)
        {
            this.texture = texture;
            this.charDirection = charDirection;
            this.position = position;

            bulletSpeed = 400;
            bulletSizeX = 36;
            bulletSizeY = 10;
            bulletDistance = 10;

            //create wall hitbox
            bulletBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(bulletSizeX), ConvertUnits.ToSimUnits(bulletSizeY), 0, position, 0, BodyType.Dynamic, "EnemyBullet");
            bulletBody.IgnoreGravity = true;
            bulletBody.IsSensor = true;
            bulletBody.IgnoreCollisionWith(self.enemyHitBox);

            //animation
            bulletSize = new Vector2(bulletSizeX, bulletSizeY);
            spriteVector = new Vector2(28, 718);

            spriteSize = bulletSize;

            switch (charDirection)
            {
                case SpriteEffects.None:
                    bulletBody.Position += new Vector2(0.5f, 0);
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletBody.Position += new Vector2(-0.5f, 0);
                    break;
            }

            bulletOrigin = new Vector2(bulletSizeX / 2, bulletSizeY / 2);
        }

        public void Shoot(GameTime gameTime)
        {
            switch (charDirection)
            {
                case SpriteEffects.None:
                    bulletBody.ApplyForce(new Vector2(bulletSpeed, 0));
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletBody.ApplyForce(new Vector2(-bulletSpeed, 0));
                    break;
            }
            bossBulletStatus = BulletStatus.BULLETALIVE;
        }

        public void Update()
        {
            //if dead animation animationEnd
            if (animationDead)
            {
                bossBulletStatus = BulletStatus.BULLETEND;
            }

            if (preStatus != bossBulletStatus)
            {
                frames = 0;
            }

            sourceRect = new Rectangle((int)spriteVector.X, (int)spriteVector.Y, (int)spriteSize.X, (int)spriteSize.Y);
            preStatus = bossBulletStatus;
        }

        public bool IsContact()
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
                    return true;
                }

                // Check if the contact fixture is the ground
                contactEdge = contactEdge.Next;
            }
            return false;
        }

        public bool IsOutRange()
        {

            if (position.X - bulletBody.Position.X < -bulletDistance || position.X - bulletBody.Position.X > bulletDistance)
            {
                // The Bullet was Out of range
                bulletBody.Dispose();
                bossBulletStatus = BulletStatus.BULLETEND;
                return true;
            }

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(bulletBody.Position), new Rectangle(0,0,bulletSizeX,bulletSizeY), Color.White, 0, bulletOrigin, 1f, charDirection, 0f);
        }
    }
}

