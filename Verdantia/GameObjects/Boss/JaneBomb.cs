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

        protected GameTime gameTime;

        //bullet state
        public BombStatus bossBombStatus;

        //wallOrigin
        private Vector2 bombOrigin;
        public int textureWidth;
        public int textureHeight;

        private bool animationDead = false;

        public Body bombBody;

        //animation
        private Vector2 bombSize;
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
            this.position = position;

            bombSpeed = 400;
            bombDistance = 10;

            //create wall hitbox
            bombBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(bombSize.X), ConvertUnits.ToSimUnits(bombSize.Y), 0, position, 0, BodyType.Dynamic, "EnemyBullet");
            bombBody.IgnoreGravity = true;
            bombBody.IgnoreCollisionWith(enemy.enemyHitBox);

            //animation
            bombSize = bombAliveSize;
            spriteVector = bombAliveAnimateList;

            spriteSize = bombSize;

            switch (charDirection)
            {
                case SpriteEffects.None:
                    bombBody.Position += new Vector2(-1f, 0);
                    break;
                case SpriteEffects.FlipHorizontally:
                    bombBody.Position += new Vector2(1f, 0);
                    break;
            }

            bombOrigin = new Vector2(bombSize.X / 2, bombSize.Y / 2);
        }

        public void Shoot()
        {
            bossBombStatus = BombStatus.BOMBALIVE;
        }

        public override void Update(GameTime gameTime)
        {
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
                    else if (frames >= allframes - 1)
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
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, bombOrigin, 1f, charDirection, 0f);
        }

        public bool IsContact()
        {
            ContactEdge contactEdge = bombBody.ContactList;
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
                    bombBody.Dispose();
                    return true;
                }

                // Check if the contact fixture is the ground
                contactEdge = contactEdge.Next;
            }
            return false;
        }
    }
}

