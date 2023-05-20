﻿using System;
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
    public class BossBullet : _GameObject
    {
        //time
        private float elapsed;
        private float delay;

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
        private List<Vector2> bulletRectVector;
        //frames 
        public int frames = 0;
        private int allframes;

        //bullet status
        private BulletStatus preStatus = BulletStatus.BULLETALIVE;

        private int bulletSpeed;
        private int bulletDistance;

        //all sprite position in spritesheet
        protected Rectangle sourceRect;

        //srpite to run
        private Vector2 spriteSize;
        private List<Vector2> spriteVector = new List<Vector2>();

        private enum BulletStatus
        {
            BULLETALIVE,
            BULLETEND
        }

        public BossBullet(Texture2D texture, Vector2 position, Enemy enemy, SpriteEffects charDirection, string name) : base(texture)
        {
            this.texture = texture;
            this.charDirection = charDirection;
            this.position = position;
            this.name = name;

            if (name.Equals("Jane"))
            {
                bulletSpeed = 400;
                bulletSizeX = 40;
                bulletSizeY = 9;
                bulletDistance = 10;

                //create wall hitbox
                bulletBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(bulletSizeX), ConvertUnits.ToSimUnits(bulletSizeY), 0, position, 0, BodyType.Dynamic, "EnemyBullet");
                bulletBody.IgnoreGravity = true;
                bulletBody.IgnoreCollisionWith(enemy.enemyHitBox);

                //animation
                bulletSize = new Vector2(bulletSizeX, bulletSizeY); 
                bulletRectVector = new List<Vector2>() { new Vector2(0, 13)};

                //delay
                delay = 150f;
                spriteVector = bulletRectVector;
                spriteSize = bulletSize;
            }


            allframes = bulletRectVector.Count();

            switch (charDirection)
            {
                case SpriteEffects.None:
                    bulletBody.Position += new Vector2(-1f, 0);
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletBody.Position += new Vector2(1f, 0);
                    break;
            }

            bulletOrigin = new Vector2(bulletSizeX / 2, bulletSizeY / 2);
        }

        public void Shoot(GameTime gameTime)
        {
            switch (charDirection)
            {
                case SpriteEffects.None:
                    bulletBody.ApplyForce(new Vector2(-bulletSpeed, 0));
                    bossBulletStatus = BulletStatus.BULLETALIVE;
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletBody.ApplyForce(new Vector2(bulletSpeed, 0));
                    bossBulletStatus = BulletStatus.BULLETALIVE;
                    break;
            }
        }

        public void Update(GameTime gameTime, Vector2 position)
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

            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsed >= delay)
            {
                if (bossBulletStatus == BulletStatus.BULLETALIVE)
                {
                    if (frames < allframes - 1)
                    {
                        frames++;
                    }
                }
                elapsed = 0;
            }

            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            preStatus = bossBulletStatus;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, bulletOrigin, 1f, charDirection, 0f);
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
    }
}

