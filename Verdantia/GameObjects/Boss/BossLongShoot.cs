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

        private float scaleSize;

        Vector2 janeLongShootSize = new Vector2(1200, 100);
        List<Vector2> janeLongShootAnimateList = new List<Vector2>() { new Vector2(0, 0), new Vector2(0, 100), new Vector2(0, 201), new Vector2(0, 301), new Vector2(0, 400), new Vector2(0, 500) };

        Vector2 doctorLongShootSize = new Vector2(1152, 96);
        List<Vector2> doctorLongShootAnimateList = new List<Vector2>() { new Vector2(240, 8), new Vector2(240, 126), new Vector2(234, 308) };

        public enum LongShotStatus
        {
            LONGSHOTALIVE,
            LONGSHOTEND
        }

        public BossLongShoot(Texture2D texture, Vector2 position, Enemy enemy, String nameBoss) : base(texture)
        {
            this.texture = texture;
            this.position = position;

            //animation
            switch (nameBoss)
            {
                case "Jane":
                    longShotSize = janeLongShootSize;
                    spriteVector = janeLongShootAnimateList;
                    break;
                case "Doctor":
                    longShotSize = doctorLongShootSize;
                    spriteVector = doctorLongShootAnimateList;
                    break;
            }

            //animation
            spriteSize = longShotSize;

            allframes = spriteVector.Count;

            //create wall hitbox
            longShotBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(longShotSize.X), ConvertUnits.ToSimUnits(longShotSize.Y), 0, position, 0, BodyType.Dynamic, "EnemyBullet");
            longShotBody.IgnoreGravity = true;
            longShotBody.IsSensor = true;
            longShotBody.IgnoreCollisionWith(enemy.enemyHitBox);

            longShotOrigin = new Vector2(longShotSize.X / 2, longShotSize.Y / 2);
            //scaleSize = 1200f / 386f;
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
                    else
                    {
                        animationDead = true;
                        return;
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
    }
}

