using System;
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
using static ScifiDruid.Singleton;

namespace ScifiDruid.GameObjects
{
    public class MeleeEnemy : Enemy
    {

        //random boss action
        private Random rand = new Random();
        private int randomAction;
        //attribute using for moving of boss
        private float timeElapsed;
        private bool isMovingLeft = true;
        private float idleTime = 0;



        //for animation
        protected Vector2 idleSize;
        protected Vector2 walkSize;
        protected Vector2 runSize;
        protected Vector2 detectPlayerSize;
        protected Vector2 deadSize;

        private List<Vector2> idleSpriteVector;
        private List<Vector2> walkSpriteVector;
        private List<Vector2> runSpriteVector;
        private List<Vector2> detectPlayerSpriteVector;
        private List<Vector2> deadSpriteVector;


        private EnemyStatus preStatus;
        private EnemyStatus curStatus;
        private enum EnemyStatus
        {
            IDLE,
            WALK,
            RUN,
            DETECT,
            DEAD,
            END
        }
        public MeleeEnemy(Texture2D texture, List<Vector2> sizeList, List<List<Vector2>> animateList) : base(texture)
        {
            this.texture = texture;

            idleSize = sizeList[0];
            walkSize = sizeList[1];
            runSize = sizeList[2];
            detectPlayerSize = sizeList[3];
            deadSize = sizeList[4];

            idleSpriteVector = animateList[0];
            walkSpriteVector = animateList[1];
            runSpriteVector = animateList[2];
            detectPlayerSpriteVector = animateList[3];
            deadSpriteVector = animateList[4];

            frames = 0;
        }

        public override void Initial(Rectangle spawnPosition, Player player)
        {
            base.Initial(spawnPosition, player);
            curStatus = EnemyStatus.RUN;
            preStatus = EnemyStatus.RUN;
        }

        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = enemyHitBox.Position;

            if (isAlive)
            {
                CheckPlayerPosition(gameTime, 1);
                
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
                case EnemyStatus.RUN:
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
                case EnemyStatus.DETECT:
                    if (elapsed >= delay)
                    {
                        if (frames >= allframes - 1)
                        {
                            curStatus = EnemyStatus.RUN;
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
            if (IsContact(enemyHitBox, "Enemy"))
            {
                foreach (var item in bodyList)
                {
                    if (enemyHitBox.BodyId != item.BodyId)
                    {
                        Debug.WriteLine("position item =" + item.Position.X + "\n" + "position enemyhitbox " + enemyHitBox.Position.X);
                        if (item.Position.X - enemyHitBox.Position.X > 0)
                        {
                            isMovingLeft= true;
                            bodyList.Clear();
                            break;
                        }
                        else
                        {
                            isMovingLeft= false;
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
            


            curStatus = EnemyStatus.IDLE;

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
            curStatus = EnemyStatus.RUN;

            if (playerPosition.X - position.X > 1 && (xspawnPosition - enemyHitBox.Position.X) > pathWalkLength * -1)
            {
                if (Singleton.Instance.levelState == LevelState.FOREST)
                {
                    charDirection = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    charDirection = SpriteEffects.None;
                }

            enemyHitBox.ApplyForce(new Vector2(150 * speed, 0));

            }
            else if (playerPosition.X - position.X < -1 && (xspawnPosition - enemyHitBox.Position.X) < pathWalkLength)
            {

                if (Singleton.Instance.levelState == LevelState.FOREST)
                {
                    charDirection = SpriteEffects.None;
                }
                else
                {
                    charDirection = SpriteEffects.FlipHorizontally;
                }
                enemyHitBox.ApplyForce(new Vector2(-120 * speed, 0));

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
                    delay = 200f;
                    spriteVector = walkSpriteVector;
                    spriteSize = new Vector2(walkSize.X, walkSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case EnemyStatus.RUN:
                    delay = 150f;
                    spriteVector = runSpriteVector;
                    spriteSize = new Vector2(runSize.X, runSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case EnemyStatus.DETECT:
                    delay = 200f;
                    spriteVector = detectPlayerSpriteVector;
                    spriteSize = detectPlayerSize;
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
