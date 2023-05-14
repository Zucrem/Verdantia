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
        protected Rectangle bossDestRect;  //where postion
        protected Rectangle bossSourceRec; //where read
        protected Vector2 bossOrigin;  //start draw boss point

        //for animation
        protected Vector2 action1Size;
        protected Vector2 action2Size;
        protected Vector2 action3Size;
        protected Vector2 action4Size;
        protected List<Vector2> action1SpriteVector = new List<Vector2>();
        protected List<Vector2> action2SpriteVector = new List<Vector2>();
        protected List<Vector2> action3SpriteVector = new List<Vector2>();
        protected List<Vector2> action4SpriteVector = new List<Vector2>();

        protected BossStatus preBossStatus;
        protected BossStatus curBossStatus;

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

        public override void Initial(Rectangle spawnPosition,Player player)
        {
            curBossStatus = BossStatus.IDLE;
            preBossStatus = BossStatus.IDLE;
            
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
        
        public override void Action() { }

        public override void Walk() { }

        public override void ChangeAnimationStatus() { }
    }
}
