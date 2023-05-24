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

        private Vector2 ballAliveSize = new Vector2(68, 64);
        private List<Vector2> ballAliveAnimateList = new List<Vector2>() { new Vector2(132, 222), new Vector2(216, 222), new Vector2(302, 222) };

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

        }

        public void Initial(String direction)
        {

            ballSpeed = 400;

            //create wall hitbox
            ballBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(ballSize.X), ConvertUnits.ToSimUnits(ballSize.Y), 0, position, 0, BodyType.Dynamic, "EnemyBullet");
            ballBody.IgnoreGravity = true;
            ballBody.IgnoreCollisionWith(enemy.enemyHitBox);

            //animation
            ballSize = ballAliveSize;
            spriteVector = ballAliveAnimateList;

            spriteSize = ballSize;

            switch (charDirection)
            {
                case SpriteEffects.None:
                    ballBody.Position += new Vector2(-1f, 0);
                    break;
                case SpriteEffects.FlipHorizontally:
                    ballBody.Position += new Vector2(1f, 0);
                    break;
            }

            ballOrigin = new Vector2(ballSize.X / 2, ballSize.Y / 2);
        }

        public void Shoot()
        {
            bossBombStatus = BallStatus.BALLALIVE;
        }

        public override void Update(GameTime gameTime)
        {
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

        public bool IsContact()
        {
            ContactEdge contactEdge = ballBody.ContactList;
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
                    ballBody.Dispose();
                    return true;
                }

                // Check if the contact fixture is the ground
                contactEdge = contactEdge.Next;
            }
            return false;
        }
    }
}

