using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DNet.Dynamics.Contacts;
using Box2DNet.Dynamics;
using Microsoft.Xna.Framework.Input;
using Box2DNet.Factories;
using Box2DNet;
using static ScifiDruid.GameObjects.Player;

namespace ScifiDruid.GameObjects
{
    public class Enemy : _GameObject
    {
        protected Texture2D texture;

        protected Player player;
        protected Vector2 playerPosition;

        private Rectangle enemyDestRect;  //where postion
        private Rectangle enemySourceRec; //where read
        protected Vector2 enemyOrigin;  //start draw enemy point

        protected bool isPlayerinArea = false;       // to check is player in the area 
        protected bool isGoingToFall = false;        // check is there are hole infront of this enemy

        public int health;                          // reduce when get hit by bullet
        public int damage;

        public int textureWidth;
        public int textureHeight;

        protected GameTime gameTime;

        //for animation
        protected Vector2 idleSize;
        protected Vector2 runSize;
        protected Vector2 deadSize;

        public Body enemyHitBox;       // to check the hit of bullet

        protected Vector2 spriteSize;

        protected List<Vector2> idleSpriteVector = new List<Vector2>();
        protected List<Vector2> runSpriteVector = new List<Vector2>();
        protected List<Vector2> deadSpriteVector = new List<Vector2>();

        protected List<Vector2> spriteVector = new List<Vector2>();

        //get animation state if dead
        protected bool animationDead = false;

        //attribute using for moving of enemy
        private float timeElapsed;
        private bool isMovingLeft;

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
            DEAD,
            END
        }

        public virtual void Initial(Rectangle spawnPosition, Player player)
        {
            curStatus = EnemyStatus.IDLE;
            preStatus = EnemyStatus.IDLE;
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

        public bool IsContact(String contact, String fixture)
        {
            ContactEdge contactEdge = enemyHitBox.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;
                switch (fixture)
                {
                    case "A":
                        // Check if the contact fixture is the ground
                        if (contactFixture.IsTouching && contactEdge.Contact.FixtureA.Body.UserData != null && contactEdge.Contact.FixtureA.Body.UserData.Equals(contact))
                        {
                            return true;
                        }
                        break;
                    case "B":
                        // Check if the contact fixture is the ground
                        if (contactFixture.IsTouching && contactEdge.Contact.FixtureB.Body.UserData != null && contactEdge.Contact.FixtureB.Body.UserData.Equals(contact))
                        {
                            return true;
                        }
                        break;
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

