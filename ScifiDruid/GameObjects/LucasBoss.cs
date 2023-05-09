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

namespace ScifiDruid.GameObjects
{
    public class LucasBoss : Boss
    {
        //framestate for dead animation
        private int frameState;
        private bool repeat;
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
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!animationDead)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, bossOrigin, 1f, charDirection, 0f);
            }
        }
    }
}
