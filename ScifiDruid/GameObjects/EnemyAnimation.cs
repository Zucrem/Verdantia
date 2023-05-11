using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScifiDruid.GameObjects.Player;
using static ScifiDruid.GameObjects.Enemy;
using System.Diagnostics;

namespace ScifiDruid.GameObjects
{
    public class EnemyAnimation : _GameObject
    {
        private Texture2D texture;
        private List <Vector2> sizeList;
        private Vector2 walkSize;
        private Vector2 runSize;
        private Vector2 deadSize;
        private Vector2 spriteSize;
        private List<List<Vector2>> animateList = new List<List<Vector2>>();
        private List<Vector2> walkSpriteVector = new List<Vector2>();
        private List<Vector2> runSpriteVector = new List<Vector2>();
        private List<Vector2> deadSpriteVector = new List<Vector2>();
        private List<Vector2> spriteVector = new List<Vector2>();

        //get animation state if dead
        private bool animationDead = false;

        //time
        private float elapsed;
        private float delay;

        //all sprite position in spritesheet
        private Rectangle sourceRect;

        public int frames = 0;
        private int allframes;

        private EnemyStatus preStatus;
        public EnemyAnimation(Texture2D texture, List<Vector2> sizeList, List<List<Vector2>> animateList) : base(texture)
        {
            this.texture = texture;

            this.sizeList = sizeList;
            this.animateList = animateList;

            walkSize = sizeList[0];
            runSize = sizeList[1];
            deadSize = sizeList[2];
            walkSpriteVector = animateList[0];
            runSpriteVector = animateList[1];
            deadSpriteVector = animateList[2];
        }
        public void Initialize()
        {
            preStatus = EnemyStatus.WALK;
        }
        public void Update(GameTime gameTime, EnemyStatus enemyStatus)
        {
            if (preStatus != enemyStatus)
            {
                frames = 0;
            }
            EnemyStatus enemyAnimateSprite = enemyStatus;
            ChangeAnimationStatus(enemyAnimateSprite);
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            switch (enemyStatus)
            {
                case EnemyStatus.SHOOT:
                    if (elapsed >= delay)
                    {
                        if (frames < allframes - 1)
                        {
                            frames++;
                        }
                        elapsed = 0;
                    }
                    break;
                case EnemyStatus.DEAD:
                    if (elapsed >= delay)
                    {
                        if (frames >= allframes - 1)
                        {
                            animationDead = true;
                            return;
                        }
                        else
                        {
                            frames++;
                        }
                        elapsed = 0;
                    }
                    break;
                default:
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
            }
            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            preStatus = enemyStatus;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 enemyOrigin, SpriteEffects charDirection, Vector2 position)
        {
            spriteBatch.Draw(texture, position, sourceRect, Color.White, 0, enemyOrigin, 1f, charDirection, 0f);
        }
        public void ChangeAnimationStatus(EnemyStatus enemyStatus)
        {
            switch (enemyStatus)
            {
                case EnemyStatus.WALK:
                    delay = 300f;
                    spriteVector = walkSpriteVector;
                    spriteSize = new Vector2(walkSize.X, walkSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case EnemyStatus.SHOOT:
                    delay = 300f;
                    spriteVector = walkSpriteVector;
                    spriteSize = new Vector2(walkSize.X, walkSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case EnemyStatus.RUN:
                    delay = 300f;
                    spriteVector = runSpriteVector;
                    spriteSize = new Vector2(runSize.X, runSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case EnemyStatus.DEAD:
                    delay = 500f;
                    spriteVector = deadSpriteVector;
                    spriteSize = new Vector2(deadSize.X, deadSize.Y);
                    allframes = spriteVector.Count();
                    break;
            }
        }
        public bool GetAnimationDead()
        {
            return animationDead;
        }
    }
}
