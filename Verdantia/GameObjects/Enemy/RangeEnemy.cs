using System;
using System.Collections.Generic;
using System.Linq;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static ScifiDruid.Singleton;
using System.Diagnostics;

namespace ScifiDruid.GameObjects
{
    public class RangeEnemy : Enemy
    {
        //framestate for dead animation
        private int frameState;
        private bool repeat;

        //random boss action
        private float idleTime = 0;

        public int bulletSizeX;
        public int bulletSizeY;

        public int bulletSpeed;
        public int bulletDistance;

        public List<EnemyBullet> bulletList = new List<EnemyBullet>();

        private int worldLevel;
        public bool isdrone = false;



        //for animation
        protected Vector2 idleSize;
        protected Vector2 walkSize;
        protected Vector2 shootSize;
        protected Vector2 deadSize;

        private List<Vector2> idleSpriteVector;
        private List<Vector2> walkSpriteVector;
        private List<Vector2> shootSpriteVector;
        private List<Vector2> deadSpriteVector;


        private EnemyStatus preStatus;
        private EnemyStatus curStatus;

        private float attackTimeDelayEnemy = 2;     //time of delay //plz set enemy delay time
        private float attackTimeEnemy;          //timer for delay          
        private enum EnemyStatus
        {
            IDLE,
            WALK,
            SHOOT,
            DEAD,
            END
        }
        public RangeEnemy(Texture2D texture, List<Vector2> sizeList, List<List<Vector2>> animateList) : base(texture)
        {
            this.texture = texture;

            idleSize = sizeList[0];
            walkSize = sizeList[1];
            shootSize = sizeList[2];
            deadSize = sizeList[3];

            idleSpriteVector = animateList[0];
            walkSpriteVector = animateList[1];
            shootSpriteVector = animateList[2];
            deadSpriteVector = animateList[3];

            frames = 0;

            //animation dead state
            frameState = 0;
            repeat = false;
        }

        public override void Initial(Rectangle spawnPosition, Player player)
        {
            base.Initial(spawnPosition, player);
            curStatus = EnemyStatus.WALK;
            preStatus = EnemyStatus.WALK;

            switch (Singleton.Instance.levelState)
            {
                case LevelState.FOREST:
                    worldLevel = 1;
                    break;
                case LevelState.CITY:
                    worldLevel = 2;
                    break;
                case LevelState.LAB:
                    worldLevel = 3;
                    break;
            }

        }

        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = enemyHitBox.Position;

            if (isAlive)
            {
                foreach (var item in bulletList)
                {
                    item.Update(gameTime);
                }
                CheckPlayerPosition(gameTime, 1);

                if (health <= 0)
                {
                    enemyHitBox.UserData = "Died";
                    isAlive = false;
                    enemyHitBox.Dispose();
                    bulletList.Clear();
                    curStatus = EnemyStatus.DEAD;
                }
            }

            //if step on dead block
            if (IsContact(enemyHitBox, "Dead"))
            {
                isAlive = false;
                enemyHitBox.Dispose();
                curStatus = EnemyStatus.DEAD;
                health = 0;
                enemyHitBox.Dispose();
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
                case EnemyStatus.WALK:
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
                case EnemyStatus.SHOOT:
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
            if (IsContact(enemyHitBox, "Enemy"))
            {
                foreach (var item in bodyList)
                {
                    if (enemyHitBox.BodyId != item.BodyId)
                    {
                        if (item.Position.X - enemyHitBox.Position.X > 0)
                        {
                            isMovingLeft = true;
                            bodyList.Clear();
                            break;
                        }
                        else
                        {
                            isMovingLeft = false;
                            bodyList.Clear();
                            break;
                        }
                    }
                }
            }

            if (idleTime == 0)
            {
                IsIdle();
            }






            //do normal walking left and right
            if ((xspawnPosition - enemyHitBox.Position.X) > pathWalkLength && isMovingLeft)
            {
                isMovingLeft = !isMovingLeft;
            }
            else if ((xspawnPosition - enemyHitBox.Position.X) < pathWalkLength * -1 && !isMovingLeft)
            {
                isMovingLeft = !isMovingLeft;
            }

            if (!isIdle)
            {
                curStatus = EnemyStatus.WALK;

                if (isMovingLeft)
                {
                    if (Singleton.Instance.levelState == LevelState.FOREST)
                    {
                        charDirection = SpriteEffects.None;
                    }
                    else
                    {
                        charDirection = SpriteEffects.FlipHorizontally;
                    }
                    enemyHitBox.ApplyForce(new Vector2(-100 * speed, 0));
                }
                else
                {
                    if (Singleton.Instance.levelState == LevelState.FOREST)
                    {
                        charDirection = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        charDirection = SpriteEffects.None;
                    }
                    enemyHitBox.ApplyForce(new Vector2(100 * speed, 0));
                }
            }
            else
            {

                if (idleTime < 5)
                {
                    curStatus = EnemyStatus.IDLE;
                    idleTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    isIdle = false;
                    idleTime = 0;
                }

            }




        }
        private void EnemyAlertWalking()
        {

            //player on (right ,mid,left)
            //got to that direction of player
            //stop when player go out of detect area
            Enemyshoot();

            if (playerPosition.X - position.X > 2 && (xspawnPosition - enemyHitBox.Position.X) > pathWalkLength * -1)//(xspawnPosition - enemyHitBox.Position.X) > pathWalkLength*-1
            {
                if (Singleton.Instance.levelState == LevelState.FOREST)
                {
                    charDirection = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    charDirection = SpriteEffects.None;
                }
                enemyHitBox.ApplyForce(new Vector2(100 * speed, 0));
            }
            else if (playerPosition.X - position.X < -2 && (xspawnPosition - enemyHitBox.Position.X) < pathWalkLength)//(xspawnPosition - enemyHitBox.Position.X) < pathWalkLength
            {
                if (Singleton.Instance.levelState == LevelState.FOREST)
                {
                    charDirection = SpriteEffects.None;
                }
                else
                {
                    charDirection = SpriteEffects.FlipHorizontally;
                }
                enemyHitBox.ApplyForce(new Vector2(-100 * speed, 0));
            }


            //do alert condition follow Player and Track Player down to death
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
                case EnemyStatus.WALK:
                    delay = 300f;
                    spriteVector = walkSpriteVector;
                    spriteSize = new Vector2(walkSize.X, walkSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case EnemyStatus.SHOOT:
                    delay = 300f;
                    spriteVector = shootSpriteVector;
                    spriteSize = new Vector2(shootSize.X, shootSize.Y);
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

        public void Enemyshoot()
        {
            attackTimeEnemy += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (attackTimeEnemy > attackTimeDelayEnemy)
            {
                Vector2 bulletpositionPlus = Vector2.Zero;
                if(Singleton.Instance.levelState == LevelState.LAB)
                {
                    bulletpositionPlus = new Vector2(0,-0.58f);
                }else if(Singleton.Instance.levelState == LevelState.CITY)
                {
                    bulletpositionPlus = new Vector2(0, -0.1f);
                }

                EnemyBullet bullet = new EnemyBullet(this.texture, enemyHitBox.Position+bulletpositionPlus, this, charDirection)
                {
                    bulletSpeed = this.bulletSpeed,
                    bulletDistance = this.bulletDistance,
                    bulletSizeX = this.bulletSizeX,
                    bulletSizeY = this.bulletSizeY,
                };
                bullet.CreateBullet(worldLevel, isdrone);
                bulletList.Add(bullet);
                bulletList[bulletList.Count - 1].Shoot();
                attackTimeEnemy = 0;


            }


        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!animationDead)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, enemyOrigin, 1f, charDirection, 0f);
            }
        }

    }
}

