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
    public class BossLongShoot : _GameObject
    {
        protected Texture2D texture; //enemy Texture (Animaiton)

        protected GameTime gameTime;

        //bullet state
        public LongShotStatus bossLongShotStatus;

        //wallOrigin
        private Vector2 longShotOrigin;
        public int textureWidth;
        public int textureHeight;

        private bool animationDead = false;

        public Body longShotBody;

        //animation
        private int longShotSizeX;
        private int longShotSizeY;
        private Vector2 longShotSize;
        //frames 
        public int frames = 0;
        private int allframes;

        //bullet status
        private LongShotStatus preStatus = LongShotStatus.LONGSHOTALIVE;


        //all sprite position in spritesheet
        protected Rectangle sourceRect;

        //srpite to run
        private Vector2 spriteSize;
        private List<Vector2> spriteVector;

        //time
        private float elapsed;
        private float delay = 200f;

        /*Vector2 janeLongShootSize = new Vector2(1200, 100);
        List<Vector2> janeLongShootAnimateList = new List<Vector2>(){new Vector2(0, 0), new Vector2(0, 100), new Vector2(0, 201), new Vector2(0, 301), new Vector2(0, 400), new Vector2(0, 500)};

        Vector2 doctorLongShootSize = new Vector2(386, 49);
        List<Vector2> doctorLongShootAnimateList = new List<Vector2>() { new Vector2(114, 4), new Vector2(114, 80), new Vector2(114, 140)};*/
        public enum LongShotStatus
        {
            LONGSHOTALIVE,
            LONGSHOTEND
        }

        public BossLongShoot(Texture2D texture, Vector2 position, Enemy enemy, SpriteEffects charDirection, Vector2 sizeVector, List<Vector2> animateList) : base(texture)
        {
            this.texture = texture;
            this.charDirection = charDirection;
            this.position = position;

            //animation
            spriteSize = sizeVector;
            spriteVector = animateList;

            longShotSizeX = 36;
            longShotSizeY = 10;

            //create wall hitbox
            longShotBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(longShotSizeX), ConvertUnits.ToSimUnits(longShotSizeY), 0, position, 0, BodyType.Dynamic, "EnemyBullet");
            longShotBody.IgnoreGravity = true;
            longShotBody.IgnoreCollisionWith(enemy.enemyHitBox);

            //animation
            longShotSize = new Vector2(longShotSizeX, longShotSizeY);
            spriteVector = new List<Vector2>() { };

            spriteSize = longShotSize;

            switch (charDirection)
            {
                case SpriteEffects.None:
                    longShotBody.Position += new Vector2(-1f, 0);
                    break;
                case SpriteEffects.FlipHorizontally:
                    longShotBody.Position += new Vector2(1f, 0);
                    break;
            }

            longShotOrigin = new Vector2(longShotSizeX / 2, longShotSizeY / 2);
        }

        public void Shoot()
        {
            bossLongShotStatus = LongShotStatus.LONGSHOTALIVE;
        }

        public override void Update(GameTime gameTime)
        {
            //if dead animation animationEnd
            if (animationDead)
            {
                bossLongShotStatus = LongShotStatus.LONGSHOTEND;
            }

            if (preStatus != bossLongShotStatus)
            {
                frames = 0;
            }
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsed >= delay)
            {
                if (bossLongShotStatus == LongShotStatus.LONGSHOTALIVE)
                {
                    if (frames < allframes - 1)
                    {
                        frames++;
                    }
                }
                elapsed = 0;
            }

            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            preStatus = bossLongShotStatus;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, longShotOrigin, 1f, charDirection, 0f);
        }

        public bool IsContact()
        {
            ContactEdge contactEdge = longShotBody.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                Body fixtureA_Body = contactEdge.Contact.FixtureA.Body;
                Body fixtureB_Body = contactEdge.Contact.FixtureB.Body;

                bool contactA = (fixtureA_Body.UserData != null && fixtureA_Body.UserData.Equals("Ground"));
                bool contactB = (fixtureB_Body.UserData != null && fixtureB_Body.UserData.Equals("Ground"));
                bool contactGround = contactA || contactB;

                contactA = (fixtureA_Body.UserData != null && fixtureA_Body.UserData.Equals("Player"));
                contactB = (fixtureB_Body.UserData != null && fixtureB_Body.UserData.Equals("Player"));
                bool contactEnemy = contactA || contactB;

                if (contactFixture.IsTouching && (contactGround || contactEnemy))
                {
                    longShotBody.Dispose();
                    return true;
                }

                // Check if the contact fixture is the ground
                contactEdge = contactEdge.Next;
            }
            return false;
        }
    }
}

