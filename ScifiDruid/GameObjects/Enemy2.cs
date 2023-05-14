using Box2DNet;
using Box2DNet.Dynamics;
using Box2DNet.Dynamics.Contacts;
using Box2DNet.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Timers;
using static ScifiDruid.GameObjects.Player;

namespace ScifiDruid.GameObjects
{
    public class Enemy2 : _GameObject
    {
        private Texture2D texture;    //enemy Texture (Animaiton)

        protected Player player;
        protected Vector2 playerPosition;

        private Rectangle enemyDestRect;  //where postion
        private Rectangle enemySourceRec; //where read
        private Vector2 enemyOrigin;  //start draw enemy point

        public Body enemyHitBox;       // to check the hit of bullet

        private bool isPlayerinArea = false;       // to check is player in the area 
        private bool isGoingToFall = false;        // check is there are hole infront of this enemy

        public int health;                          // reduce when get hit by bullet
        public int damage;

        public int textureWidth;
        public int textureHeight;

        private GameTime gameTime;

        //attribute using for moving of enemy
        private float timeElapsed;
        private bool isMovingLeft;

        public List<Vector2> sizeList;
        public List<List<Vector2>> animateList;

        private EnemyAnimation enemyAnimation;

        private KeyboardState currentKeyState;

        //animation
        private EnemyStatus enemyStatus;
        //check if animation animationEnd or not
        private bool animationEnd;

        protected float playerCheckTime;

        public enum EnemyStatus
        {
            WALK,
            RUN,
            SHOOT,
            DEAD,
            END
        }


        public Enemy2(Texture2D texture) : base(texture)
        {
            this.texture = texture;
            //characterSouceRec = new Rectangle(0, 0, sizeX, sizeY);
        }

        public void Initial(Rectangle startRect,Player player)
        {
            textureHeight = (int)size.Y;
            textureWidth= (int)size.X;

            enemyHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 1f, ConvertUnits.ToSimUnits(new Vector2(startRect.X, startRect.Y - 1)), 0, BodyType.Dynamic,"Enemy");
            enemyHitBox.FixedRotation = true;
            enemyHitBox.Friction = 1.0f;
            enemyHitBox.AngularDamping = 2.0f;
            enemyHitBox.LinearDamping = 2.0f;
            //enemyHitBox.IsSensor = true;

            //foreach (Body item in Singleton.Instance.world.BodyList)
            //{
            //    if (!item.UserData.Equals("Player"))
            //    {
            //        enemyHitBox.IgnoreCollisionWith(item);
            //    }
            //}
            isAlive = true;

            charDirection = SpriteEffects.FlipHorizontally;  // heading direction

            enemyOrigin = new Vector2(textureWidth/2,textureHeight/2);  //draw in the middle

            enemyStatus = EnemyStatus.WALK;
            //enemyAnimation = new EnemyAnimation(texture, sizeList, animateList);

            enemyAnimation.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            /*this.gameTime = gameTime;
            position = enemyHitBox.Position;
            if (isAlive)
            {
                if (Player.isAttack && GotHit())
                {
                    health--;
                }

                if (health <= 0)
                {
                    isAlive = false;
                    enemyHitBox.Dispose();
                    enemyStatus = EnemyStatus.DEAD;
                }
            }

            //if dead animation animationEnd
            animationEnd = enemyAnimation.GetAnimationDead();
            if (animationEnd)
            {
                enemyStatus = EnemyStatus.END;
            }

            enemyAnimation.Update(gameTime, enemyStatus);*/
       }

        public virtual bool GotHit()
        {
            ContactEdge contactEdge = enemyHitBox.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                Body fixtureB_Body = contactEdge.Contact.FixtureB.Body;
                Body fixtureA_Body = contactEdge.Contact.FixtureA.Body;

                bool contactB = (fixtureB_Body.UserData != null && fixtureB_Body.UserData.Equals("Bullet"));
                bool contactA = (fixtureA_Body.UserData != null && fixtureA_Body.UserData.Equals("Bullet"));

                if (contactFixture.IsTouching && (contactB || contactA))
                {
                    return true;
                }
                // Check if the contact fixture is the ground

                contactEdge = contactEdge.Next;
            }
            return false;
        }

        public void CheckPlayerPosition(GameTime gameTime)
        {
            if (playerCheckTime <= 0)
            {
                playerCheckTime = 1;
                playerPosition = player.position;
            }
            else
            {
                playerCheckTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void EnemyAction()
        {
            currentKeyState = Keyboard.GetState();
            if (isAlive)
            {
                EnemyWalking();
            }
            //EnemyAlertWalking();
        }

        private void EnemyWalking()
        {
            //do normal walking left and right
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(health > 0)
            {
                if (timeElapsed >= 5f)
                {
                    timeElapsed = 0f;
                    isMovingLeft= !isMovingLeft;
                }

                if(isMovingLeft)
                {
                    charDirection = SpriteEffects.None;
                    enemyHitBox.ApplyForce(new Vector2(-100*speed,0));
                }
                else
                {
                    charDirection = SpriteEffects.FlipHorizontally;
                    enemyHitBox.ApplyForce(new Vector2(100 * speed, 0));
                }
            }


        }

        

        private void EnemyAlertWalking()
        {

            //player on (right ,mid,left)
            //got to that direction of player
            //stop when player go out of detect area


            //do alert condition follow Player and Track Player down to death
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!animationEnd)
            {
                enemyAnimation.Draw(spriteBatch, enemyOrigin, charDirection, ConvertUnits.ToDisplayUnits(position));
            }

            //if shoot
            /*if (_bulletBody != null && !_bulletBody.IsDisposed)
            {
                bullet.Draw(spriteBatch);
            }*/

            base.Draw(spriteBatch);
        }
    }
}
