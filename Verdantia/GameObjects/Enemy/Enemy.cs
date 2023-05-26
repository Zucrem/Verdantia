using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DNet.Dynamics.Contacts;
using Box2DNet.Dynamics;
using Microsoft.Xna.Framework.Input;
using static ScifiDruid.GameObjects.Player;
using System.Diagnostics;
using Box2DNet.Factories;
using Box2DNet;
using static ScifiDruid.Singleton;

namespace ScifiDruid.GameObjects
{
    public class Enemy : _GameObject
    {

        protected Texture2D texture; //enemy Texture (Animaiton)

        protected Player player;
        protected Vector2 playerPosition;
        protected Boolean isIdle = false;

        private Rectangle enemyDestRect;  //where postion
        private Rectangle enemySourceRec; //where read
        protected Vector2 enemyOrigin;  //start draw enemy point

        protected bool isPlayerinArea = false;       // to check is player in the area 
        protected bool isPlayerinDroneArea = false;

        public int health;                          // reduce when get hit by bullet
        public int damage;

        public int textureWidth;
        public int textureHeight;

        protected GameTime gameTime;

        public Body enemyHitBox;       // to check the hit of bullet

        protected Vector2 spriteSize;

        protected List<Vector2> spriteVector = new List<Vector2>();

        //get animation state if dead
        protected bool animationDead = false;

        //attribute using for moving of enemy
        protected float timeElapsed;
        protected bool isMovingLeft;

        public List<Vector2> sizeList;
        public List<List<Vector2>> animateList;

        private KeyboardState currentKeyState;

        //time
        protected float elapsed;
        protected float delay;

        //all sprite position in spritesheet
        protected Rectangle sourceRect;

        protected int frames;
        protected int allframes;

        //check player position
        protected float playerCheckTime;

        public List<Body> bodyList;

        protected float pathWalkLength;
        protected float xspawnPosition;

        public Enemy(Texture2D texture) : base(texture)
        {
            this.texture = texture;
        }
        public virtual void Initial(Rectangle spawnPosition, Player player)
        {
            this.player = player;

            textureHeight = (int)size.Y;
            textureWidth = (int)size.X;

            enemyHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 1f, ConvertUnits.ToSimUnits(new Vector2(spawnPosition.X, spawnPosition.Y - 1)), 0, BodyType.Dynamic, "Enemy");
            enemyHitBox.FixedRotation = true;
            enemyHitBox.Friction = 1.0f;
            enemyHitBox.AngularDamping = 2.0f;
            enemyHitBox.LinearDamping = 2.0f;

            pathWalkLength = ConvertUnits.ToSimUnits((spawnPosition.Width / 2) - 64);
            xspawnPosition = ConvertUnits.ToSimUnits(spawnPosition.X);

            isAlive = true;

            // heading direction
            if (Singleton.Instance.levelState == LevelState.FOREST)
            {
                charDirection = SpriteEffects.FlipHorizontally;
            }
            else
            {
                charDirection = SpriteEffects.None;
            }

            enemyOrigin = new Vector2(textureWidth / 2, textureHeight / 2);

            bodyList = new List<Body>();

        }

        public bool GotHit(String dmgType)
        {
            ContactEdge contactEdge = enemyHitBox.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                Body fixtureB_Body = contactEdge.Contact.FixtureB.Body;
                Body fixtureA_Body = contactEdge.Contact.FixtureA.Body;

                bool contactB = (fixtureB_Body.UserData != null && fixtureB_Body.UserData.Equals(dmgType));
                bool contactA = (fixtureA_Body.UserData != null && fixtureA_Body.UserData.Equals(dmgType));

                if (contactFixture.IsTouching && (contactB || contactA))
                {
                    return true;
                }
                // Check if the contact fixture is the ground

                contactEdge = contactEdge.Next;
            }
            return false;
        }

        public void takeDMG(int dmg, String dmgType)
        {
            if (GotHit(dmgType))
            {
                health -= dmg;
            }

        }

        public bool IsContact(Body box, String contact)
        {

            ContactEdge contactEdge = box.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                Body fixtureA = contactEdge.Contact.FixtureA.Body;
                Body fixtureB = contactEdge.Contact.FixtureB.Body;

                bool fixtureA_Check = fixtureA.UserData != null && fixtureA.UserData.Equals(contact);
                bool fixtureB_Check = fixtureB.UserData != null && fixtureB.UserData.Equals(contact);



                if (box.UserData.Equals("Enemy")&&contact.Equals("Enemy")) {

                    // Check if the contact fixture is the ground
                    if (contactFixture.IsTouching && (fixtureA_Check))
                    {
                        //if Contact thing in parameter it will return True
                        if (!bodyList.Contains(fixtureA))
                        {
                            bodyList.Add(fixtureA);
                        }
                    }
                    else if (contactFixture.IsTouching && (fixtureB_Check))
                    {
                        if (!bodyList.Contains(fixtureB))
                        {
                            bodyList.Add(fixtureB);
                        }
                    }
                    if (bodyList.Count > 1) { return true; }

                    return false;
                }
                
                if (contactFixture.IsTouching && (fixtureA_Check || fixtureB_Check))
                {
                    //if Contact thing in parameter it will return True
                    return true;
                }
                

                contactEdge = contactEdge.Next;
            }
            return false;
        }

        public void CheckPlayerPosition(GameTime gameTime,int checkPosTime)
        {
            if (playerCheckTime <= 0)
            {
                playerCheckTime = checkPosTime;
                playerPosition = player.position;
                if (playerPosition.X - position.X < 6 && playerPosition.X - position.X > -6 && playerPosition.Y - position.Y < 3 && playerPosition.Y - position.Y > -3)
                {
                    isPlayerinArea = true;
                }
                else { isPlayerinArea = false; }

                if(playerPosition.X - position.X < 8 && playerPosition.X - position.X > -8 && playerPosition.Y - position.Y < 8 && playerPosition.Y - position.Y > -1)
                {
                    isPlayerinDroneArea = true;
                } else
                {
                    isPlayerinDroneArea = false;
                }
            }
            else
            {
                playerCheckTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public virtual void Action() { }

        public virtual void Walk() { }

        public virtual void ChangeAnimationStatus() { }

        public void IsIdle()
        {
            Random random = new Random();
            if (random.Next(500) == 1)
            {
                isIdle = true;
            }
            else { isIdle = false; }
        }
    }
}

