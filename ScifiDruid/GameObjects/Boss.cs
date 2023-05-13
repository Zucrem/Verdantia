using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DNet.Dynamics.Contacts;
using Box2DNet.Dynamics;

namespace ScifiDruid.GameObjects
{
    public class Boss : Enemy
    {
        protected Texture2D texture;

        protected Rectangle bossDestRect;  //where postion
        protected Rectangle bossSourceRec; //where read
        protected Vector2 bossOrigin;  //start draw boss point

        protected bool isPlayerinArea = false;       // to check is player in the area 
        protected bool isGoingToFall = false;        // check is there are hole infront of this enemy

        public bool isAlive;

        public int health;                          // reduce when get hit by bullet
        public int damage;

        public int textureWidth;
        public int textureHeight;

        protected GameTime gameTime;

        //for animation
        protected Vector2 idleSize;
        protected Vector2 action1Size;
        protected Vector2 action2Size;
        protected Vector2 action3Size;
        protected Vector2 action4Size;
        protected Vector2 deadSize;

        protected Vector2 spriteSize;
        public Vector2 position;
        public Vector2 size;

        protected List<Vector2> idleSpriteVector = new List<Vector2>();
        protected List<Vector2> action1SpriteVector = new List<Vector2>();
        protected List<Vector2> action2SpriteVector = new List<Vector2>();
        protected List<Vector2> action3SpriteVector = new List<Vector2>();
        protected List<Vector2> action4SpriteVector = new List<Vector2>();
        protected List<Vector2> deadSpriteVector = new List<Vector2>();

        protected List<Vector2> spriteVector = new List<Vector2>();

        //get animation state if dead
        protected bool animationDead = false;

        //time
        protected float elapsed;
        protected float delay;

        public float speed;

        //all sprite position in spritesheet
        protected Rectangle sourceRect;

        protected int frames;
        protected int allframes;

        protected BossStatus preStatus;
        protected BossStatus curStatus;

        public SpriteEffects charDirection;

        public Boss(Texture2D texture) : base(texture)
        {
            this.texture = texture;
        }

        protected enum BossStatus
        {
            IDLE,
            ACTION1,
            ACTION2,
            ACTION3,
            ACTION4,
            DEAD,
            END
        }

        public virtual void Initial(Rectangle spawnPosition,Player player)
        {
            curStatus = BossStatus.IDLE;
            preStatus = BossStatus.IDLE;
            
        }

        public override bool GotHit()
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
        
        public virtual void Action() { }

        public virtual void Walk() { }

        public virtual void ChangeAnimationStatus() { }
    }
}
