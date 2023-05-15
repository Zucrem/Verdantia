using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Box2DNet.Dynamics.Contacts;

namespace ScifiDruid.GameObjects
{
    public class Bullet : _GameObject
    {
        private Texture2D texture;
        private Vector2 bulletPosition;
        private SpriteEffects bulletDirection;

        private Vector2 bulletOrigin;

        //bullet state
        public BulletStatus bulletStatus;

        //animation
        private SkillAnimation bulletAnimation;

        public Body bulletBody;

        private int bulletSizeX;
        private int bulletSizeY;

        private int bulletSpd;
        private int bulletDistance;

        //check if animation end or not
        private bool end;

        public enum BulletStatus
        {
            BULLETALIVE,
            BULLETDEAD,
            BULLETEND
        }
      
        public Bullet(Texture2D texture , Vector2 position,Player player,SpriteEffects charDirection) : base(texture)
        {
            this.texture = texture;
            this.charDirection = charDirection;
            this.position = position;

            bulletSpd = 400;
            bulletSizeX = 40;
            bulletSizeY = 8;
            bulletDistance = 10;

            bulletAnimation = new SkillAnimation(this.texture);
            bulletBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(bulletSizeX), ConvertUnits.ToSimUnits(bulletSizeY),0,position,0,BodyType.Dynamic, "Bullet");
            bulletBody.IgnoreGravity = true;
            bulletBody.IgnoreCollisionWith(player.hitBox);

            switch (charDirection)
            {
                case SpriteEffects.None:
                    bulletBody.Position += new Vector2(-0.5f,0);
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletBody.Position += new Vector2(1f, 0);
                    break;
            }

            bulletOrigin = new Vector2(bulletSizeX / 2,bulletSizeY / 2);
        }

        public void Shoot(GameTime gameTime)
        {
            switch (charDirection)
            {
                case SpriteEffects.None:
                    bulletBody.ApplyForce(new Vector2(-bulletSpd, 0));
                    bulletStatus = BulletStatus.BULLETALIVE;
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletBody.ApplyForce(new Vector2(bulletSpd, 0));
                    bulletStatus = BulletStatus.BULLETALIVE;
                    break;
            }

            //bulletAnimation.UpdateBullet(gameTime, bulletStatus);

        }

        public override void Update(GameTime gameTime)
        {
            bulletAnimation.UpdateBullet(gameTime, bulletStatus);

            end = bulletAnimation.bulletAnimationDead;
            //if dead animation end
            if (end)
            {
                bulletStatus = BulletStatus.BULLETEND;
            }
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
                return true;
            }

            return false;
        }


        
        public override void Draw(SpriteBatch spriteBatch)
        {

            //spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(bulletBody.Position), Color.White);
            if (!end)
            {
                bulletAnimation.Draw(spriteBatch, bulletOrigin, charDirection, ConvertUnits.ToDisplayUnits(bulletBody.Position));
            }

            base.Draw(spriteBatch);
        }

    }
}


        /*
        public Bullet(Texture2D texture , Vector2 bulletPosition, SpriteEffects bulletDirection) : base(texture)
        {
            this.texture = texture;
            this.bulletPosition = bulletPosition;
            this.bulletDirection = bulletDirection;
        }
        public void Initial(Rectangle position)
        {
            //bulletAnimation = new PlayerAnimation(this.texture, new Vector2(startRect.X, startRect.Y));

            bulletStatus = BulletStatus.BULLETALIVE;
            //SkillAnimation.Initialize();

            base.Initial();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)bulletPosition.X, (int)bulletPosition.Y, 15, 10), null, Color.White, 0, Vector2.Zero, bulletDirection, 0);
            base.Draw(spriteBatch);
        }

        public void Shoot(Vector2 position, SpriteEffects charDirection)
        {
            bulletPosition.Y = position.Y + 10;
            bulletDirection = charDirection;
            switch (bulletDirection)
            {
                case SpriteEffects.None:
                    bulletPosition.X = position.X + 30;
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletPosition.X = position.X - 30;
                    break;
            }
        }

        public void Update()
        {
            switch (bulletDirection)
            {
                case SpriteEffects.None:
                    bulletPosition.X += 2;
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletPosition.X -= 2;
                    break;
            }
        }*/

