using System;
using System.Collections.Generic;
using System.Linq;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static ScifiDruid.Singleton;

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
        private float movingTime = 3.4f;

        private float pathWalkLength;
        private float xspawnPosition;

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

        public void Initial(Rectangle spawnPosition, Player player)
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

            // heading direction
            if (Singleton.Instance.levelState == LevelState.FOREST)
            {
                charDirection = SpriteEffects.FlipHorizontally;
            }
            else
            {
                charDirection = SpriteEffects.None;
            } 

            enemyOrigin = new Vector2(textureWidth / 2, textureHeight / 2);  //draw in the middle

            curStatus = EnemyStatus.WALK;
            preStatus = EnemyStatus.WALK;
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
        private void EnemyAlertWalking()
        {

            //player on (right ,mid,left)
            //got to that direction of player
            //stop when player go out of detect area

            if (playerPosition.X - position.X > 2)
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
            else if (playerPosition.X - position.X < -2)
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
    }
}

