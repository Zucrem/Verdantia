using Box2DNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScifiDruid.Singleton;

namespace ScifiDruid.GameObjects
{
    internal class DroneEnemy : Enemy
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
        public bool isdrone = true;

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

        private float attackTimeDelayEnemy = 0.8f;     //time of delay 
        private float attackTimeEnemy;          //timer for delay          
        private enum EnemyStatus
        {
            IDLE,
            WALK,
            SHOOT,
            DEAD,
            END
        }

        public DroneEnemy(Texture2D texture, List<Vector2> sizeList, List<List<Vector2>> animateList) : base(texture)
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
            enemyHitBox.IgnoreGravity = true;
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

            if (animationDead)
            {
                curStatus = EnemyStatus.END;
            }
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
            if (isAlive && isPlayerinDroneArea)
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
            curStatus = EnemyStatus.WALK;
            //fly left and right 
            if ((xspawnPosition - enemyHitBox.Position.X) > pathWalkLength && isMovingLeft)
            {
                enemyHitBox.LinearVelocity = Vector2.Zero;
                isMovingLeft = !isMovingLeft;
            }
            else if ((xspawnPosition - enemyHitBox.Position.X) < pathWalkLength * -1 && !isMovingLeft)
            {
                enemyHitBox.LinearVelocity = Vector2.Zero;
                isMovingLeft = !isMovingLeft;
            }

            if (isMovingLeft)
            {
                charDirection = SpriteEffects.None;
                enemyHitBox.ApplyForce(new Vector2(-50 * speed, 0));
            }
            else
            {
                charDirection = SpriteEffects.None;
                enemyHitBox.ApplyForce(new Vector2(50 * speed, 0));
            }

            //enemyHitBox.LinearVelocity = Vector2.Zero;  // make drone stop
        }

        private void EnemyAlertWalking()
        {

            if (playerPosition.X - position.X > 0.4 && (xspawnPosition - enemyHitBox.Position.X) > pathWalkLength * -1)  // run(right) to player but not out of area
            {
                curStatus = EnemyStatus.WALK;
                charDirection = SpriteEffects.None;
                enemyHitBox.ApplyForce(new Vector2(80 * speed, 0));
            }
            else if (playerPosition.X - position.X < -0.1 && (xspawnPosition - enemyHitBox.Position.X) < pathWalkLength) // run(left) to player but not out of area
            {
                curStatus = EnemyStatus.WALK;
                charDirection = SpriteEffects.FlipHorizontally;
                enemyHitBox.ApplyForce(new Vector2(-80 * speed, 0));
            }
            else if(playerPosition.X - position.X >= -0.1 && playerPosition.X - position.X <= 0.4)
            {
                curStatus = EnemyStatus.SHOOT;
                Enemyshoot();               
            }
            
        }

        public override void ChangeAnimationStatus()
        {
            switch (curStatus)
            {
                case EnemyStatus.IDLE:
                    delay = 100;
                    spriteVector = idleSpriteVector;
                    spriteSize = new Vector2(idleSize.X, idleSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case EnemyStatus.WALK:
                    delay = 100;
                    spriteVector = walkSpriteVector;
                    spriteSize = new Vector2(walkSize.X, walkSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case EnemyStatus.SHOOT:
                    delay = 100;
                    spriteVector = shootSpriteVector;
                    spriteSize = new Vector2(shootSize.X, shootSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case EnemyStatus.DEAD:
                    delay = 100;
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
                EnemyBullet bullet = new EnemyBullet(this.texture, enemyHitBox.Position, this, charDirection)
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
