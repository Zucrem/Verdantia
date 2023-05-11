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

namespace ScifiDruid.GameObjects
{
    public class LucasBoss : Boss
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

        //boolean to do action
        private bool action1 = false;
        private bool action2 = false;
        private bool action3 = false;
        public LucasBoss(Texture2D texture) : base(texture)
        {
            this.texture = texture;
            idleSize = new Vector2(196, 186);
            action1Size = new Vector2(228, 185);
            action2Size = new Vector2(197, 187);
            action3Size = new Vector2(220, 184);
            action4Size = new Vector2(0, 0);
            deadSize = new Vector2(187, 188);

            idleSpriteVector = new List<Vector2>() { new Vector2(16, 0), new Vector2(264, 0) };
            action1SpriteVector = new List<Vector2>() { new Vector2(0, 215), new Vector2(230, 221) };
            action2SpriteVector = new List<Vector2>() { new Vector2(16, 436) };
            action3SpriteVector = new List<Vector2>() { new Vector2(230, 438), new Vector2(0, 898), new Vector2(230, 904) };
            action4SpriteVector = new List<Vector2>();

            deadSpriteVector = new List<Vector2>() { new Vector2(503, 0), new Vector2(737, 0), new Vector2(975, 0), new Vector2(1215, 0) };

            frames = 0;

            //animation dead state
            frameState = 0;
            repeat = false;
        }
        public void Initial(Rectangle spawnPosition)
        {
            textureHeight = (int)size.Y;
            textureWidth = (int)size.X;

            bossHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 1f, ConvertUnits.ToSimUnits(new Vector2(spawnPosition.X, spawnPosition.Y - 1)), 0, BodyType.Dynamic, "Boss");
            bossHitBox.FixedRotation = true;
            bossHitBox.Friction = 1.0f;
            bossHitBox.AngularDamping = 2.0f;
            bossHitBox.LinearDamping = 2.0f;

            isAlive = true;

            charDirection = SpriteEffects.FlipHorizontally;  // heading direction

            bossOrigin = new Vector2(textureWidth / 2, textureHeight / 2);  //draw in the middle
        }
        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = bossHitBox.Position;

            if (isAlive)
            {
                if (Player.isAttack && GotHit())
                {
                    health--;
                }

                if (health <= 0)
                {
                    isAlive = false;
                    curStatus = BossStatus.DEAD;
                }
            }

            //boss action
            Action();

            //if dead animation animationEnd
            if (animationDead)
            {
                curStatus = BossStatus.END;
                bossHitBox.Dispose();
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
                case BossStatus.IDLE:
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
                case BossStatus.DEAD:
                    if (elapsed >= delay)
                    {
                        if (frames >= allframes - 1 && frameState != 2)
                        {
                            frameState++;
                            frames = 0;
                        }
                        else if (frames >= allframes - 1 && frameState == 2)
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

                case BossStatus.ACTION1:
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
                default:
                    if (elapsed >= delay)
                    {
                        if (frames < allframes - 1)
                        {
                            frames++;
                        }
                        elapsed = 0;
                    }
                    break;
            }
            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            preStatus = curStatus;
        }

        public void Action()
        {
            if (isAlive && Player.health > 0)
            {
                if (curStatus == BossStatus.IDLE)
                {
                    randomAction = rand.Next(1, 300);
                }
                //do action 1
                if (randomAction == 1)
                {
                    action1 = true;
                    //clear random action number
                    //do animation1
                    curStatus = BossStatus.ACTION1;
                    //do normal walking left and right
                    timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (charDirection == SpriteEffects.FlipHorizontally && timeElapsed >= movingTime)
                    {
                        timeElapsed = 0f;
                        bossHitBox.ApplyForce(new Vector2(0, 0));
                        charDirection = SpriteEffects.None;
                    }
                    else if (charDirection == SpriteEffects.None && timeElapsed >= movingTime)
                    {
                        bossHitBox.ApplyForce(new Vector2(0, 0));
                        timeElapsed = 0f;
                        action1 = false;
                        charDirection = SpriteEffects.FlipHorizontally;
                        curStatus = BossStatus.IDLE;
                        randomAction = 0;
                    }
                    else if (charDirection == SpriteEffects.FlipHorizontally && timeElapsed <= movingTime)
                    {
                        bossHitBox.ApplyForce(new Vector2(-100 * speed, 0));
                    }
                    else if (charDirection == SpriteEffects.None && timeElapsed <= movingTime)
                    {
                        bossHitBox.ApplyForce(new Vector2(100 * speed, 0));
                    }
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!animationDead)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, bossOrigin, 1f, charDirection, 0f);
            }
        }

        public void ChangeAnimationStatus()
        {
            switch (curStatus)
            {
                case BossStatus.IDLE:
                    delay = 300f;
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
                    spriteVector = action3SpriteVector;
                    spriteSize = new Vector2(action3Size.X, action3Size.Y);
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
    }
}
