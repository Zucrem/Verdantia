using ScifiDruid.Managers;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Xml;
using static ScifiDruid.GameObjects.Player;
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
        private BulletStatus bulletStatus;

        //animation
        private SkillAnimation bulletAnimation;

        private Body bulletBody;

        private int bulletSizeX;
        private int bulletSizeY;

        private int bulletSpd;
        private int bulletDistance;

        public enum BulletStatus
        {
            BULLETALIVE,
            BULLETDEAD
        }
      
        public Bullet(Texture2D texture , Vector2 position,Body playerBody,SpriteEffects charDirection) : base(texture)
        {
            this.texture = texture;
            this.charDirection = charDirection;
            this.position = position;

            bulletSpd = 400;
            bulletSizeX = 40;
            bulletSizeY = 8;
            bulletDistance = 10;

            bulletAnimation = new SkillAnimation(this.texture);
            bulletBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(bulletSizeX), ConvertUnits.ToSimUnits(bulletSizeY),0,position,0,BodyType.Dynamic,"bullet");
            bulletBody.IgnoreGravity = true;
            bulletBody.IgnoreCollisionWith(playerBody);

            switch (charDirection)
            {
                case SpriteEffects.None:
                    bulletBody.Position += new Vector2(-0.5f,0);
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletBody.Position += new Vector2(0.5f, 0);
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

            bulletAnimation.UpdateBullet(gameTime, bulletStatus);
        }

        public bool isContact()
        {
            ContactEdge contactEdge = bulletBody.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                // Check if the contact fixture is the ground
                if (contactEdge.Contact.FixtureB.Body.UserData != null)
                {
                    // The Bullet was contact anything
                    bulletBody.Dispose();
                    return true;
                }
                contactEdge = contactEdge.Next;
            }
            return false;
        }

        public bool isOutRange()
        {

            if (position.X - bulletBody.Position.X < -bulletDistance || position.X - bulletBody.Position.X > bulletDistance)
            {
                //bulletBody.IsStatic = true;
                bulletBody.IgnoreGravity = false;
                // The Bullet was Out of range
                bulletBody.Dispose();
                //bulletBody.Position
                return true;
            }

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            //spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(bulletBody.Position), Color.White);
            
            bulletAnimation.Draw(spriteBatch, bulletOrigin, charDirection, ConvertUnits.ToDisplayUnits(bulletBody.Position));

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

