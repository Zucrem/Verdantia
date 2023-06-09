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
    public class DoctorBall : _GameObject
    {
        protected Texture2D texture; //enemy Texture (Animaiton)

        protected GameTime gameTime;

        //bullet state
        public BallStatus bossBombStatus;

        //wallOrigin
        private Vector2 ballOrigin;
        public int textureWidth;
        public int textureHeight;

        private bool animationDead = false;

        public Body ballBody;

        //animation
        private Vector2 ballSize;
        //sprite to run
        private Vector2 spriteSize;
        private List<Vector2> spriteVector;
        //frames 
        public int frames = 0;
        private int allframes;

        //all sprite position in spritesheet
        protected Rectangle sourceRect;

        //ball speed
        private int ballSpeed;

        //time
        private float elapsed;
        private float delay = 200f;

        private Vector2 ballAliveSize = new Vector2(140, 128);
        private List<Vector2> ballAliveAnimateList = new List<Vector2>() { new Vector2(240, 444), new Vector2(401, 444), new Vector2(580, 444) };

        private Enemy enemy;

        public enum BallStatus
        {
            BALLALIVE,
            BALLEND
        }

        public DoctorBall(Texture2D texture, Vector2 position, Enemy enemy) : base(texture)
        {
            this.texture = texture;
            this.position = position;
            this.enemy = enemy;
        }

        public void CreateBall()
        {
            //animation
            ballSize = ballAliveSize;
            spriteVector = ballAliveAnimateList;
            allframes = spriteVector.Count;

            spriteSize = ballSize;

            //create wall hitbox
            ballBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(ballSize.X), ConvertUnits.ToSimUnits(ballSize.Y), 0, position, 0, BodyType.Dynamic, "SkillBoss");
            ballBody.IgnoreGravity = true;
            ballBody.IsSensor = true;
            ballBody.IgnoreCollisionWith(enemy.enemyHitBox);

            ballOrigin = new Vector2(ballSize.X / 2, ballSize.Y / 2);

            bossBombStatus = BallStatus.BALLALIVE;
            ballBody.ApplyLinearImpulse(new Vector2(2, 0));
        }

        public override void Update(GameTime gameTime)
        {
            position = ballBody.Position;
            //if dead animation animationEnd
            if (animationDead)
            {
                bossBombStatus = BallStatus.BALLEND;
            }

            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsed >= delay)
            {
                if (bossBombStatus == BallStatus.BALLALIVE)
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
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, ballOrigin, 1f, charDirection, 0f);
        }
    }
}

