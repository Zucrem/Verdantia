﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DNet.Dynamics.Contacts;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Input;
using static ScifiDruid.GameObjects.Player;

namespace ScifiDruid.GameObjects
{
    public class RangeEnemy : Enemy
    {
        //framestate for dead animation
        private int frameState;
        private bool repeat;

        //random boss action
        private Random rand = new Random();
        private int randomAction;
        //attribute using for moving of boss
        private float timeElapsed;
        private bool isMovingLeft = true;
        private float movingTime = 3.4f;

        private float pathWalkLength;
        private float xspawnPosition;

        public RangeEnemy(Texture2D texture, List<Vector2> sizeList, List<List<Vector2>> animateList) : base(texture)
        {
            this.texture = texture;

            idleSize = sizeList[0];
            deadSize = sizeList[1];

            idleSpriteVector = animateList[0];
            deadSpriteVector = animateList[1];

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

            pathWalkLength = ConvertUnits.ToSimUnits((spawnPosition.Width / 2) - 64);
            xspawnPosition = ConvertUnits.ToSimUnits(spawnPosition.X);

            isAlive = true;

            charDirection = SpriteEffects.FlipHorizontally;  // heading direction

            enemyOrigin = new Vector2(textureWidth / 2, textureHeight / 2);  //draw in the middle
        }

        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = enemyHitBox.Position;

            if (isAlive)
            {
                CheckPlayerPosition(gameTime);

                if (Player.isAttack && GotHit())
                {
                    health--;
                }

                if (health <= 0)
                {
                    enemyHitBox.UserData = "Dead";
                    isAlive = false;
                    enemyHitBox.Dispose();
                    curStatus = EnemyStatus.DEAD;
                }

            }

            //if step on dead block
            if (IsContact("dead", "A"))
            {
                isAlive = false;
                curStatus = EnemyStatus.DEAD;
            }

            //if dead animation animationEnd
            if (animationDead)
            {
                curStatus = EnemyStatus.END;
            }

            //animation
            if (preStatus != curStatus)
            {
                frames = 0;
                frameState = 0;
            }
            ChangeAnimationStatus();
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            switch (curStatus)
            {
                case EnemyStatus.IDLE:
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
                    break;
                case EnemyStatus.DEAD:
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
                    break;
            }
            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            preStatus = curStatus;
        }
        public override void Action()
        {
            if (isAlive && isPlayerinArea)
            {
                EnemyAlertWalking();
            }
            else if (isAlive)
            {
                EnemyWalking();
            }
        }

        private void EnemyWalking()
        {
            //do normal walking left and right
            if ((xspawnPosition - enemyHitBox.Position.X) > pathWalkLength && isMovingLeft)
            {
                isMovingLeft = !isMovingLeft;
            }
            else if ((xspawnPosition - enemyHitBox.Position.X) < pathWalkLength * -1 && !isMovingLeft)
            {
                isMovingLeft = !isMovingLeft;
            }

            if (isMovingLeft)
            {
                charDirection = SpriteEffects.None;
                enemyHitBox.ApplyForce(new Vector2(-100 * speed, 0));
            }
            else
            {
                charDirection = SpriteEffects.FlipHorizontally;
                enemyHitBox.ApplyForce(new Vector2(100 * speed, 0));
            }


        }
        private void EnemyAlertWalking()
        {

            //player on (right ,mid,left)
            //got to that direction of player
            //stop when player go out of detect area

            if (playerPosition.X - position.X > 2)
            {
                charDirection = SpriteEffects.FlipHorizontally;
                enemyHitBox.ApplyForce(new Vector2(100 * speed, 0));
            }
            else if (playerPosition.X - position.X < -2)
            {
                
                charDirection = SpriteEffects.None;
                enemyHitBox.ApplyForce(new Vector2(-100 * speed, 0));
                
            }
           

            //do alert condition follow Player and Track Player down to death
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!animationDead)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, enemyOrigin, 1f, charDirection, 0f);
            }
        }

        public override void ChangeAnimationStatus()
        {
            switch (curStatus)
            {
                case EnemyStatus.IDLE:
                    delay = 200f;
                    spriteVector = idleSpriteVector;
                    spriteSize = new Vector2(idleSize.X, idleSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case EnemyStatus.DEAD:
                    delay = 300f;
                    spriteVector = deadSpriteVector;
                    spriteSize = new Vector2(deadSize.X, deadSize.Y);
                    allframes = spriteVector.Count();
                    break;
            }
        }
    }
}

