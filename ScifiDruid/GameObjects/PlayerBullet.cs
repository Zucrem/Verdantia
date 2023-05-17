using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Box2DNet.Dynamics.Contacts;
using System.Collections.Generic;
using System.Linq;

namespace ScifiDruid.GameObjects
{
    public class PlayerBullet : _GameObject
    {
        private Texture2D texture;

        private List<Vector2> spriteVector = new List<Vector2>();
        //time
        private float elapsed;
        private float delay;

        private Vector2 bulletOrigin;

        //bullet state
        public BulletStatus bulletStatus;

        public Body bulletBody;

        private int bulletSizeX;
        private int bulletSizeY;

        private int bulletSpeed;
        private int bulletDistance;

        //animation
        //shoot and dead Size
        private Vector2 bulletAliveSize, bulletDeadSize;
        //Vector
        private List<Vector2> bulletAliveRectVector;
        private List<Vector2> bulletDeadRectVector;

        private int bulletAliveCount;
        private int bulletDeadCount;

        //all sprite position in spritesheet
        private Rectangle sourceRect;

        private Vector2 spriteSize;

        //frames 
        public int frames = 0;
        private int allframes;

        //bullet status
        private BulletStatus preStatus;
        private BulletStatus curStatus;

        private bool animationDead = false;
        public enum BulletStatus
        {
            BULLETALIVE,
            BULLETDEAD,
            BULLETEND
        }

        public PlayerBullet(Texture2D texture, Vector2 position, Player player, SpriteEffects charDirection) : base(texture)
        {
            this.texture = texture;
            this.charDirection = charDirection;
            this.position = position;

            bulletSpeed = 400;
            bulletSizeX = 40;
            bulletSizeY = 8;
            bulletDistance = 10;

            //build object
            bulletBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(bulletSizeX), ConvertUnits.ToSimUnits(bulletSizeY), 0, position, 0, BodyType.Dynamic, "Bullet");
            bulletBody.IgnoreGravity = true;
            bulletBody.IgnoreCollisionWith(player.hitBox);

            //animation
            bulletAliveSize = new Vector2(40, 8);
            bulletDeadSize = new Vector2(16, 20);

            bulletAliveRectVector = new List<Vector2>() { new Vector2(0, 12), new Vector2(62, 12), new Vector2(120, 12) };
            bulletDeadRectVector = new List<Vector2>() { new Vector2(0, 6), new Vector2(210, 6) };

            bulletAliveCount = bulletAliveRectVector.Count();
            bulletDeadCount = bulletDeadRectVector.Count();

            switch (charDirection)
            {
                case SpriteEffects.None:
                    bulletBody.Position += new Vector2(-0.5f, 0);
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletBody.Position += new Vector2(1f, 0);
                    break;
            }

            bulletOrigin = new Vector2(bulletSizeX / 2, bulletSizeY / 2);
        }

        public void Initialize()
        {
            preStatus = BulletStatus.BULLETALIVE;
            curStatus = BulletStatus.BULLETALIVE;
        }

        public void Shoot(GameTime gameTime)
        {
            switch (charDirection)
            {
                case SpriteEffects.None:
                    bulletBody.ApplyForce(new Vector2(-bulletSpeed, 0));
                    bulletStatus = BulletStatus.BULLETALIVE;
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletBody.ApplyForce(new Vector2(bulletSpeed, 0));
                    bulletStatus = BulletStatus.BULLETALIVE;
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            //if dead animation animationEnd
            if (animationDead)
            {
                curStatus = BulletStatus.BULLETEND;
            }

            if (preStatus != curStatus)
            {
                frames = 0;
            }

            ChangeBulletAnimationStatus(bulletStatus);
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsed >= delay)
            {
                if (bulletStatus == BulletStatus.BULLETALIVE)
                {
                    if (frames < allframes - 1)
                    {
                        frames++;
                    }
                }
                else if (bulletStatus == BulletStatus.BULLETDEAD)
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
            preStatus = bulletStatus;
        }

        public bool IsContact()
        {
            ContactEdge contactEdge = bulletBody.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                Body fixtureA_Body = contactEdge.Contact.FixtureA.Body;
                Body fixtureB_Body = contactEdge.Contact.FixtureB.Body;

                bool contactA = (fixtureA_Body.UserData != null && fixtureA_Body.UserData.Equals("Ground"));
                bool contactB = (fixtureB_Body.UserData != null && fixtureB_Body.UserData.Equals("Ground"));
                bool contactGround = contactA || contactB;

                contactA = (fixtureA_Body.UserData != null && fixtureA_Body.UserData.Equals("Enemy"));
                contactB = (fixtureB_Body.UserData != null && fixtureB_Body.UserData.Equals("Enemy"));
                bool contactEnemy = contactA || contactB;

                if (contactFixture.IsTouching && (contactGround || contactEnemy))
                {
                    bulletBody.Dispose();
                    return true;
                }

                // Check if the contact fixture is the ground
                contactEdge = contactEdge.Next;
            }
            return false;
        }

        public bool IsOutRange()
        {

            if (position.X - bulletBody.Position.X < -bulletDistance || position.X - bulletBody.Position.X > bulletDistance)
            {
                // The Bullet was Out of range
                bulletBody.Dispose();
                curStatus = BulletStatus.BULLETDEAD;
                return true;
            }

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!animationDead)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(bulletBody.Position), sourceRect, Color.White, 0, bulletOrigin, 1f, charDirection, 0f);
            }

            base.Draw(spriteBatch);
        }
        public void ChangeBulletAnimationStatus(BulletStatus bulletStatus)
        {
            switch (bulletStatus)
            {
                case BulletStatus.BULLETALIVE:
                    delay = 150f;
                    spriteVector = bulletAliveRectVector;
                    spriteSize = bulletAliveSize;
                    allframes = bulletAliveCount;
                    break;
                case BulletStatus.BULLETDEAD:
                    delay = 150f;
                    spriteVector = bulletDeadRectVector;
                    spriteSize = bulletDeadSize;
                    allframes = bulletDeadCount;
                    break;
            }
        }
    }
}
