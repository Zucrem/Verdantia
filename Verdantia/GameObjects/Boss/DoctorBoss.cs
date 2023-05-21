﻿using System;
using System.Collections.Generic;
using System.Linq;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScifiDruid.GameObjects
{
    public class DoctorBoss : Boss
    {
        //framestate for dead animation
        private int frameState;
        private bool repeat;

        //random boss action
        private Random rand = new Random();
        private int randomAction;
        //attribute using for moving of boss
        private float movingTime = 3.4f;

        //boolean to do action
        private bool action1 = false;
        private bool action2 = false;
        private bool action3 = false;
        //for animation
        protected Vector2 action1Size;
        protected Vector2 action2Size;
        protected Vector2 action3Size;
        protected List<Vector2> action1SpriteVector;
        protected List<Vector2> action2SpriteVector;
        protected List<Vector2> action3SpriteVector;

        private BossStatus preBossStatus;
        private BossStatus curBossStatus;

        private enum BossStatus
        {
            IDLE,
            ACTION1,
            ACTION2,
            ACTION3,
            DEAD,
            END
        }

        public DoctorBoss(Texture2D texture) : base(texture)
        {
            this.texture = texture;

            idleSize = new Vector2(38, 88);
            action1Size = new Vector2(60, 82);
            action2Size = new Vector2(62, 86);
            action3Size = new Vector2(60, 82);
            deadSize = new Vector2(32, 82);

            //idle spritevector
            idleSpriteVector = new List<Vector2>() { new Vector2(14, 2), new Vector2(100, 2), new Vector2(186, 2)};

            //action1 spritevector
            action1SpriteVector = new List<Vector2>() { new Vector2(272, 8), new Vector2(358, 8), new Vector2(452, 8)};

            //action2 spritevector
            action2SpriteVector = new List<Vector2>() { new Vector2(0, 116), new Vector2(86, 116)};

            //action3 spritevector
            action3SpriteVector = new List<Vector2>() { new Vector2(272, 8), new Vector2(358, 8), new Vector2(452, 8) };

            //dead spritevector
            deadSpriteVector = new List<Vector2>() { new Vector2(194, 120), new Vector2(280, 120) };

            frames = 0;

            //animation dead state
            frameState = 0;
            repeat = false;
        }

        public override void Initial(Rectangle spawnPosition, Player player)
        {
            this.player = player;

            textureHeight = (int)size.Y;
            textureWidth = (int)size.X;

            enemyHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 1f, ConvertUnits.ToSimUnits(new Vector2(spawnPosition.X, spawnPosition.Y - 1)), 0, BodyType.Dynamic, "Enemy");
            enemyHitBox.FixedRotation = true;
            enemyHitBox.Friction = 1.0f;
            enemyHitBox.AngularDamping = 2.0f;
            enemyHitBox.LinearDamping = 2.0f;

            isAlive = false;

            charDirection = SpriteEffects.FlipHorizontally;  // heading direction

            bossOrigin = new Vector2(textureWidth / 2, textureHeight / 2);  //draw in the middle

            skillTime = 5;
        }

        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = enemyHitBox.Position;

            if (isAlive)
            {
                CheckPlayerPosition(gameTime);

                takeDMG(1, "Bullet");

                if (health <= 0)
                {
                    enemyHitBox.UserData = "Died";
                    isAlive = false;
                    enemyHitBox.Dispose();
                    curBossStatus = BossStatus.DEAD;
                }
            }

            //boss action
            Action();

            //if dead animation animationEnd
            if (animationDead)
            {
                curBossStatus = BossStatus.END;
            }

            //animation
            if (preBossStatus != curBossStatus)
            {
                frames = 0;
                frameState = 0;
            }

            ChangeAnimationStatus();
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (curBossStatus == BossStatus.DEAD)
            {
                if (elapsed >= delay)
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
                    elapsed = 0;
                }
            }
            else
            {
                if (elapsed >= delay)
                {
                    if (frames >= allframes - 1)
                    {
                        frames = 0;
                    }
                    else
                    {
                        frames++;
                    }
                    elapsed = 0;
                }
            }
            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            preBossStatus = curBossStatus;
        }


        public override void Action()
        {
            if (isAlive && Player.health > 0)
            {

                //timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (skillTime <= 0 && curBossStatus == BossStatus.IDLE)
                {
                    randomAction = rand.Next(1, 6);
                    skillTime = 5;
                }
                else if (curBossStatus == BossStatus.IDLE)
                {
                    skillTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //do action 1
                switch (randomAction)
                {
                    case 1:
                        Skill1();
                        break;
                    case 2:
                        Skill2();
                        break;
                    case 3:
                        Skill3();
                        break;
                }
            }
        }

        public void Skill1()
        {
            action1 = true;
            //clear random action number
            //do animation1
            curBossStatus = BossStatus.ACTION1;
            //do normal walking left and right
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeElapsed >= movingTime)
            {
                switch (charDirection)
                {
                    case SpriteEffects.None:
                        timeElapsed = 0f;
                        action1 = false;
                        charDirection = SpriteEffects.FlipHorizontally;
                        curBossStatus = BossStatus.IDLE;
                        randomAction = 0;
                        break;
                    case SpriteEffects.FlipHorizontally:
                        timeElapsed = 0f;
                        charDirection = SpriteEffects.None;
                        break;
                }
            }
            else
            {
                switch (charDirection)
                {
                    case SpriteEffects.None:
                        enemyHitBox.ApplyForce(new Vector2(100 * speed, 0));
                        break;
                    case SpriteEffects.FlipHorizontally:
                        enemyHitBox.ApplyForce(new Vector2(-100 * speed, 0));
                        break;
                }
            }
        }

        public void Skill2()
        {
            if (!action2)
            {
                action2 = true;
                curBossStatus = BossStatus.ACTION2;
            }
        }

        public void Skill3()
        {
            if (!action3)
            {
                action3 = true;
                curBossStatus = BossStatus.ACTION3;
            }
        }

        public override void ChangeAnimationStatus()
        {
            switch (curBossStatus)
            {
                case BossStatus.IDLE:
                    delay = 200f;
                    spriteVector = idleSpriteVector;
                    spriteSize = new Vector2(idleSize.X, idleSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case BossStatus.ACTION1:
                    delay = 200f;
                    spriteVector = action1SpriteVector;
                    spriteSize = new Vector2(action1Size.X, action1Size.Y);
                    allframes = spriteVector.Count();
                    break;
                case BossStatus.ACTION2:
                    delay = 300f;
                    spriteVector = action2SpriteVector;
                    spriteSize = new Vector2(action2Size.X, action2Size.Y);
                    allframes = spriteVector.Count();
                    break;
                case BossStatus.ACTION3:
                    delay = 300f;
                    spriteSize = new Vector2(action3Size.X, action3Size.Y);
                    spriteVector = action3SpriteVector;
                    allframes = spriteVector.Count();
                    break;
                case BossStatus.DEAD:
                    delay = 300f;
                    spriteVector = deadSpriteVector;
                    spriteSize = new Vector2(deadSize.X, deadSize.Y);
                    allframes = spriteVector.Count();
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!animationDead)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, bossOrigin, 1f, charDirection, 0f);
            }
        }
    }
}
