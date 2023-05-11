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
    public class Boss : _GameObject
    {
        protected Texture2D texture;

        protected Rectangle bossDestRect;  //where postion
        protected Rectangle bossSourceRec; //where read
        protected Vector2 bossOrigin;  //start draw boss point

        public Body bossHitBox;       // to check the hit of bullet

        protected bool isPlayerinArea = false;       // to check is player in the area 
        protected bool isGoingToFall = false;        // check is there are hole infront of this enemy

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

        //all sprite position in spritesheet
        protected Rectangle sourceRect;

        protected int frames;
        protected int allframes;

        protected BossStatus preStatus;
        protected BossStatus curStatus;

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

        public virtual void Initial()
        {
            curStatus = BossStatus.IDLE;
            preStatus = BossStatus.IDLE;
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public bool GotHit()
        {
            ContactEdge contactEdge = bossHitBox.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                //if (contactEdge.Contact.FixtureB.Body.UserData.Equals("Bullet") || contactEdge.Contact.FixtureA.Body.UserData.Equals("Bullet"))
                //{
                //    Debug.WriteLine("Count " + Singleton.Instance.world.BodyList.Count);

                //    Debug.WriteLine("A " + contactEdge.Contact.FixtureA.Body.UserData);

                //    Debug.WriteLine("B " + contactEdge.Contact.FixtureB.Body.UserData);
                //}

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
    }
}
