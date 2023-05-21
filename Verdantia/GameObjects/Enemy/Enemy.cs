using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DNet.Dynamics.Contacts;
using Box2DNet.Dynamics;
using Microsoft.Xna.Framework.Input;
using static ScifiDruid.GameObjects.Player;

namespace ScifiDruid.GameObjects
{
    public class Enemy : _GameObject
    {

        protected Texture2D texture; //enemy Texture (Animaiton)

        protected Player player;
        protected Vector2 playerPosition;

        private Rectangle enemyDestRect;  //where postion
        private Rectangle enemySourceRec; //where read
        protected Vector2 enemyOrigin;  //start draw enemy point

        protected bool isPlayerinArea = false;       // to check is player in the area 
        //protected bool isGoingToFall = false;        // check is there are hole infront of this enemy

        public int health;                          // reduce when get hit by bullet
        public int damage;

        public int textureWidth;
        public int textureHeight;

        protected GameTime gameTime;

        //for animation
        protected Vector2 idleSize;
        protected Vector2 runSize;
        protected Vector2 detectPlayerSize;
        protected Vector2 deadSize;

        public Body enemyHitBox;       // to check the hit of bullet

        protected Vector2 spriteSize;

        protected List<Vector2> idleSpriteVector = new List<Vector2>();
        protected List<Vector2> runSpriteVector = new List<Vector2>();
        protected List<Vector2> detectPlayerSpriteVector = new List<Vector2>();
        protected List<Vector2> deadSpriteVector = new List<Vector2>();

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

        protected EnemyStatus preStatus;
        protected EnemyStatus curStatus;

        //check player position
        protected float playerCheckTime;

        public Enemy(Texture2D texture) : base(texture)
        {
            this.texture = texture;
        }

        protected enum EnemyStatus
        {
            IDLE,
            RUN,
            DETECT,
            DEAD,
            END
        }

        public virtual void Initial(Rectangle spawnPosition, Player player)
        {
            curStatus = EnemyStatus.IDLE;
            preStatus = EnemyStatus.IDLE;
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

        public void takeDMG(int dmg,String dmgType)
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


                // Check if the contact fixture is the ground
                if (contactFixture.IsTouching && (fixtureA_Check || fixtureB_Check))
                {
                    //if Contact thing in parameter it will return True
                    return true;
                }

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
                if (playerPosition.X - position.X < 6 && playerPosition.X - position.X > -6 && playerPosition.Y - position.Y < 3 && playerPosition.Y - position.Y > -3)
                {
                    isPlayerinArea = true;
                }
                else { isPlayerinArea = false; }
            }
            else
            {
                playerCheckTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public virtual void Action() { }

        public virtual void Walk() { }

        public virtual void ChangeAnimationStatus() { }
    }
}

