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

namespace ScifiDruid.GameObjects
{
    public class Bullet : _GameObject
    {
        private Texture2D texture;
        public int jumpCount = 0;
        public bool isAttack = false;
        public Vector2 bulletPosition;
        public SpriteEffects bulletDirection;

        //bullet state
        public BulletStatus bulletStatus;

        //animation
        public SkillAnimation bulletAnimation;
        public enum BulletStatus
        {
            BULLETALIVE,
            BULLETDEAD
        }

        public Bullet(Texture2D texture , Vector2 bulletPosition, SpriteEffects bulletDirection) : base(texture)
        {
            this.texture = texture;
            this.bulletPosition = bulletPosition;
            this.bulletDirection = bulletDirection;
        }
        public void Initial(Rectangle position)
        {
            bulletAnimation = new SkillAnimation(this.texture);

            bulletStatus = BulletStatus.BULLETALIVE;


            bulletAnimation.Initialize();

            base.Initial();
        }

        public void UpdateBullet(GameTime gameTime, BulletStatus bulletState)
        {
            bulletStatus = bulletState;
            switch (bulletDirection)
            {
                case SpriteEffects.None:
                    bulletPosition.X += 2;
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletPosition.X -= 2;
                    break;
            }


            bulletAnimation.UpdateBullet(gameTime, bulletStatus);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            bulletAnimation.Draw(spriteBatch, new Rectangle((int)bulletPosition.X, (int)bulletPosition.Y, 15, 10), bulletDirection);
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
    }
}
