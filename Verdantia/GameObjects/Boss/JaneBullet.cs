using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DNet.Dynamics.Contacts;
using Box2DNet.Dynamics;
using Microsoft.Xna.Framework.Input;
using Box2DNet.Factories;
using Box2DNet;
using System.Diagnostics;
using static ScifiDruid.GameObjects.PlayerSkillAnimation;
using System.Linq;
using static ScifiDruid.GameObjects.PlayerBullet;

namespace ScifiDruid.GameObjects
{
    public class JaneBullet : _GameObject
    {
        protected Texture2D texture; //enemy Texture (Animaiton)

        protected GameTime gameTime;

        //bullet state
        public BulletStatus bossBulletStatus;

        //wallOrigin
        private Vector2 bulletOrigin;
        public int textureWidth;
        public int textureHeight;

        private bool animationDead = false;

        public Body bulletBody;

        //animation
        private int bulletSizeX;
        private int bulletSizeY;
        private Vector2 bulletSize;
        //frames 
        public int frames = 0;

        private int bulletSpeed;
        private int bulletDistance;

        //all sprite position in spritesheet
        protected Rectangle sourceRect;

        //srpite to run
        private Vector2 spriteSize;
        private Vector2 spriteVector;

        private SpriteEffects ammoDirect;

        public enum BulletStatus
        {
            BULLETALIVE,
            BULLETEND
        }

        public JaneBullet(Texture2D texture, Vector2 position, Enemy self, SpriteEffects charDirection) : base(texture)
        {
            this.texture = texture;
            this.charDirection = charDirection;
            this.position = position;

            bulletSpeed = 400;
            bulletSizeX = 36;
            bulletSizeY = 10;
            bulletDistance = 10;

            //create wall hitbox
            bulletBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(bulletSizeX), ConvertUnits.ToSimUnits(bulletSizeY), 0, position, 0, BodyType.Dynamic, "SkillBoss");
            bulletBody.IgnoreGravity = true;
            bulletBody.IsSensor = true;
            bulletBody.IgnoreCollisionWith(self.enemyHitBox);

            //animation
            bulletSize = new Vector2(bulletSizeX, bulletSizeY);
            spriteVector = new Vector2(28, 718);

            spriteSize = bulletSize;

            switch (charDirection)
            {
                case SpriteEffects.None:
                    bulletBody.Position += new Vector2(0.5f, 0);
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletBody.Position += new Vector2(-0.5f, 0);
                    break;
            }

            sourceRect = new Rectangle((int)spriteVector.X, (int)spriteVector.Y, (int)spriteSize.X, (int)spriteSize.Y);

            bulletOrigin = new Vector2(bulletSizeX / 2, bulletSizeY / 2);
        }

        public void Shoot()
        {
            switch (charDirection)
            {
                case SpriteEffects.None:
                    bulletBody.ApplyForce(new Vector2(bulletSpeed, 0));
                    ammoDirect = SpriteEffects.FlipHorizontally;
                    break;
                case SpriteEffects.FlipHorizontally:
                    bulletBody.ApplyForce(new Vector2(-bulletSpeed, 0));
                    ammoDirect = SpriteEffects.None;
                    break;
            }
            bossBulletStatus = BulletStatus.BULLETALIVE;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(bulletBody.Position), sourceRect, Color.White, 0, bulletOrigin, 1f, ammoDirect, 0f);
        }
    }
}

