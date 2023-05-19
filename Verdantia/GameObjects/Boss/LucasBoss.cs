using System;
using System.Collections.Generic;
using System.Linq;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace ScifiDruid.GameObjects
{
    public class LucasBoss : Boss
    {
        private Rectangle leftArea;
        private Rectangle rightArea;
        private Rectangle fieldBoss;

        private float centerFieldBoss;

        //framestate for dead animation
        private int frameState;
        private bool repeat;

        //random boss action
        private Random rand = new Random();
        private int randomAction;
        //attribute using for moving of boss
        private float timeElapsed;
        private float movingTime = 5f;

        //boolean to do action
        private bool action1 = false;
        private bool action2 = false;
        private bool action3 = false;

        private Texture2D drillTexture;

        private Body drillBody;

        public LucasBoss(Texture2D texture, Texture2D drillTexture) : base(texture)
        {
            this.texture = texture;
            this.drillTexture = drillTexture;


            idleSize = new Vector2(196, 186);
            action1Size = new Vector2(228, 185);
            action2Size = new Vector2(197, 187);
            action3Size = new Vector2(220, 184);
            deadSize = new Vector2(187, 188);

            idleSpriteVector = new List<Vector2>() { new Vector2(16, 0), new Vector2(264, 0) };

            action1SpriteVector = new List<Vector2>() { new Vector2(0, 215), new Vector2(230, 221) };
            action2SpriteVector = new List<Vector2>() { new Vector2(16, 436) };
            action3SpriteVector = new List<Vector2>() { new Vector2(230, 438), new Vector2(0, 898), new Vector2(230, 904) };

            deadSpriteVector = new List<Vector2>() { new Vector2(503, 0), new Vector2(737, 0), new Vector2(975, 0), new Vector2(1215, 0) };

            frames = 0;

            //animation dead state
            frameState = 0;
            repeat = false;
        }

        public override void Initial(Rectangle spawnPosition, Player player, Rectangle leftArea, Rectangle rightArea, Rectangle fieldBoss)
        {
            this.player = player;
            this.leftArea = leftArea;
            this.rightArea = rightArea;
            this.fieldBoss = fieldBoss;

            textureHeight = (int)size.Y;
            textureWidth = (int)size.X;
            centerFieldBoss = (fieldBoss.Width / 2) + fieldBoss.X;

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
                    if (drillBody != null)
                    {
                        drillBody.Dispose();
                    }
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
            switch (curBossStatus)
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
                    //randomAction = 3;

                    skillTime = 5;
                }
                else if (curBossStatus == BossStatus.IDLE)
                {
                    skillTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (randomAction < 4 && randomAction > 0)
                {
                    //if (position.X - )
                }
                else
                {
                    //Walk();
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

        public override void Walk()
        {
            if (curBossStatus != BossStatus.IDLE)
            {
                return;
            }

            if (position.X - playerPosition.X > 0)
            {
                charDirection = SpriteEffects.FlipHorizontally;
                enemyHitBox.ApplyForce(new Vector2(-75 * speed, 0));
            }
            else
            {
                charDirection = SpriteEffects.None;
                enemyHitBox.ApplyForce(new Vector2(75 * speed, 0));
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
                        charDirection = SpriteEffects.FlipHorizontally;
                        break;
                    case SpriteEffects.FlipHorizontally:
                        timeElapsed = 0f;
                        charDirection = SpriteEffects.None;
                        break;
                }
                action1 = false;
                curBossStatus = BossStatus.IDLE;
                randomAction = 0;
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
                drillBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(100), ConvertUnits.ToSimUnits(100), 0, enemyHitBox.Position, 0, BodyType.Dynamic, "SkillBoss");
                drillBody.IgnoreCollisionWith(enemyHitBox);
                drillBody.IsSensor = true;
                drillBody.IgnoreGravity = true;
                switch (charDirection)
                {
                    case SpriteEffects.None:
                        drillBody.ApplyLinearImpulse(new Vector2(3 * speed, 0));
                        break;
                    case SpriteEffects.FlipHorizontally:
                        drillBody.ApplyLinearImpulse(new Vector2(-3 * speed, 0));
                        break;
                }
            }
            else
            {
                if (IsContact(drillBody, "Ground"))
                {
                    drillBody.IsSensor = false;
                    drillBody.RestoreCollisionWith(enemyHitBox);
                    switch (charDirection)
                    {
                        case SpriteEffects.None:
                            enemyHitBox.ApplyForce(new Vector2(80 * speed, 0));
                            break;
                        case SpriteEffects.FlipHorizontally:
                            enemyHitBox.ApplyForce(new Vector2(-80 * speed, 0));
                            break;
                    }
                }

                if (IsContact(enemyHitBox, "SkillBoss"))
                {
                    drillBody.Dispose();
                    //drillBody.RestoreCollisionWith(enemyHitBox);
                    curBossStatus = BossStatus.IDLE;
                    randomAction = 0;
                    action2 = false;

                    switch (charDirection)
                    {
                        case SpriteEffects.None:
                            charDirection = SpriteEffects.FlipHorizontally;
                            break;
                        case SpriteEffects.FlipHorizontally:
                            charDirection = SpriteEffects.None;
                            break;
                    }
                }
            }
        }

        public void Skill3()
        {
            if (!action3)
            {
                action3 = true;
                curBossStatus = BossStatus.ACTION2;
                drillBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(100), ConvertUnits.ToSimUnits(100), 0, enemyHitBox.Position, 0, BodyType.Dynamic, "SkillBoss");
                drillBody.IgnoreCollisionWith(enemyHitBox);
                drillBody.IsSensor = true;
                drillBody.IgnoreGravity = true;
                switch (charDirection)
                {
                    case SpriteEffects.None:
                        drillBody.ApplyLinearImpulse(new Vector2(6 * speed, 0));
                        break;
                    case SpriteEffects.FlipHorizontally:
                        drillBody.ApplyLinearImpulse(new Vector2(-6 * speed, 0));
                        break;
                }
            }
            else
            {
                if (IsContact(drillBody, "Ground"))
                {
                    drillBody.IsSensor = false;
                    drillBody.RestoreCollisionWith(enemyHitBox);
                    switch (charDirection)
                    {
                        case SpriteEffects.None:
                            drillBody.ApplyLinearImpulse(new Vector2(-2 * speed, 0));
                            break;
                        case SpriteEffects.FlipHorizontally:
                            drillBody.ApplyLinearImpulse(new Vector2(2 * speed, 0));
                            break;
                    }
                }

                if (IsContact(enemyHitBox, "SkillBoss"))
                {
                    drillBody.Dispose();
                    curBossStatus = BossStatus.IDLE;
                    randomAction = 0;
                    action3 = false;
                }
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!animationDead)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, bossOrigin, 1f, charDirection, 0f);

                if (action2 || action3)
                {
                    spriteBatch.Draw(drillTexture, ConvertUnits.ToDisplayUnits(drillBody.Position), new Rectangle(0, 0, (int)ConvertUnits.ToDisplayUnits(ConvertUnits.ToSimUnits(100)), (int)ConvertUnits.ToDisplayUnits(ConvertUnits.ToSimUnits(100))), Color.Black, 0, new Vector2(100 / 2, 100 / 2), 1, charDirection, 0f);
                }
            }

        }
    }
}
