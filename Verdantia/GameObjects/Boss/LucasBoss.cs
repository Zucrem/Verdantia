using System;
using System.Collections.Generic;
using System.Linq;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using ScifiDruid.Managers;

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
        private float movingTime = 5f;

        //boolean to do action
        private bool action1 = false;
        private bool action2 = false;
        private bool action3 = false;

        private Texture2D drillTexture;

        private Body drillBody;

        private int countBounce = 0;

        //for animation

        private Vector2 idleSize;
        private Vector2 action1Size;
        private Vector2 action2_1Size;
        private Vector2 action2_2Size;
        private Vector2 action3Size;
        private Vector2 deadSize;
        
        private List<Vector2> idleSpriteVector;
        private List<Vector2> action1SpriteVector;
        private List<Vector2> action2_1SpriteVector;
        private List<Vector2> action2_2SpriteVector;
        private List<Vector2> action3SpriteVector;
        private List<Vector2> deadSpriteVector;

        private LucasStatus preBossStatus;
        private LucasStatus curBossStatus;

        private SoundEffect chargeSound, deathSound;

        //charge sfx
        private bool chargeSoundBool = false;

        private enum LucasStatus
        {
            IDLE,
            ACTION1,
            ACTION2_1,
            ACTION2_2,
            ACTION3,
            DEAD,
            END
        }

        public LucasBoss(Texture2D texture, Texture2D drillTexture) : base(texture)
        {
            this.texture = texture;
            this.drillTexture = drillTexture;

            idleSize = new Vector2(196, 186);
            action1Size = new Vector2(228, 185);
            action2_1Size = new Vector2(220, 184);
            action2_2Size = new Vector2(220, 184);
            action3Size = new Vector2(197, 186); 
            deadSize = new Vector2(187, 188);

            idleSpriteVector = new List<Vector2>() { new Vector2(16, 0), new Vector2(264, 0) };

            action1SpriteVector = new List<Vector2>() { new Vector2(0, 215), new Vector2(230, 221) };
            action2_1SpriteVector = new List<Vector2>() { new Vector2(230, 438) };
            action2_2SpriteVector = new List<Vector2>(){ new Vector2(0, 898), new Vector2(230, 904) };
            action3SpriteVector = new List<Vector2>() { new Vector2(16, 436) };

            deadSpriteVector = new List<Vector2>() { new Vector2(503, 0), new Vector2(737, 0), new Vector2(975, 0), new Vector2(1215, 0) };

            frames = 0;

            //animation dead state
            frameState = 0;
            repeat = false;

            LoadContent();
        }
        public void LoadContent()
        {
            ContentManager content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
            //sfx
            chargeSound = content.Load<SoundEffect>("Sounds/Boss1/lucasCharge");
            deathSound = content.Load<SoundEffect>("Sounds/Boss1/lucasDeath");
        }

        public override void Initial(Rectangle spawnPosition, Player player, Rectangle fieldBoss)
        {
            this.player = player;
            //this.leftArea = leftArea;
            //this.rightArea = rightArea;
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

            curBossStatus = LucasStatus.IDLE;
            preBossStatus = LucasStatus.IDLE;
        }

        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = enemyHitBox.Position;

            if (isAlive)
            {
                CheckPlayerPosition(gameTime,1);

                //takeDMG(1, "Bullet");

                if (health <= 0)
                {
                    enemyHitBox.UserData = "Died";
                    isAlive = false;
                    enemyHitBox.Dispose();
                    if (drillBody != null)
                    {
                        drillBody.Dispose();
                    }
                    curBossStatus = LucasStatus.DEAD;
                    deathSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
                }
            }

            //boss action
            Action();

            //if dead animation animationEnd
            if (animationDead)
            {
                curBossStatus = LucasStatus.END;
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
                case LucasStatus.IDLE:
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
                case LucasStatus.DEAD:
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

                case LucasStatus.ACTION1:
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
            if (isAlive && player.health > 0)
            {

                //timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (skillTime <= 0 && curBossStatus == LucasStatus.IDLE)
                {
                    //randomAction = rand.Next(1, 4);
                    //randomAction = 3;
                    randomAction = 1;

                    skillTime = 5;
                }
                else if (curBossStatus == LucasStatus.IDLE)
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

        public override void Walk()
        {
            if (curBossStatus != LucasStatus.IDLE)
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
            if (!action1)
            {
                chargeSoundBool = false;
                action1 = true;
                curBossStatus = LucasStatus.ACTION1;
            }
            //clear random action number
            //do animation1
            //do normal walking left and right
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (!chargeSoundBool)
            {
                chargeSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
                chargeSoundBool = true;
            }

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
                curBossStatus = LucasStatus.IDLE;
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
            //Drill
            if (!action2)
            {
                action2 = true;
                curBossStatus = LucasStatus.ACTION2_1;
                drillBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(62), ConvertUnits.ToSimUnits(28), 0, enemyHitBox.Position + new Vector2(0, 0.3f), 0, BodyType.Dynamic, "SkillBoss");
                drillBody.IgnoreCollisionWith(enemyHitBox);
                drillBody.IsSensor = true;
                drillBody.IgnoreGravity = true;
                bossSkilDirection = charDirection;
                switch (bossSkilDirection)
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
                    curBossStatus = LucasStatus.ACTION2_2;
                    drillBody.LinearVelocity = Vector2.Zero;
                    drillBody.RestoreCollisionWith(enemyHitBox);
                    switch (bossSkilDirection)
                    {
                        case SpriteEffects.None:
                            enemyHitBox.ApplyForce(new Vector2(80 * speed, 0));
                            //drillBody.IsStatic = true;
                            break;
                        case SpriteEffects.FlipHorizontally:
                            enemyHitBox.ApplyForce(new Vector2(-80 * speed, 0));
                            //drillBody.IsStatic = true;
                            break;
                    }
                    //drillBody.IsSensor = true;
                }

                if (IsContact(enemyHitBox, "SkillBoss"))
                {
                    drillBody.Dispose();
                    //drillBody.RestoreCollisionWith(enemyHitBox);
                    curBossStatus = LucasStatus.IDLE;
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
            //Saw
            if (!action3)
            {
                action3 = true;
                curBossStatus = LucasStatus.ACTION3;
                drillBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(56), ConvertUnits.ToSimUnits(56), 0, enemyHitBox.Position + new Vector2(0, -0.2f), 0, BodyType.Dynamic, "SkillBoss");
                drillBody.IgnoreCollisionWith(enemyHitBox);
                drillBody.IsSensor = true;
                drillBody.IgnoreGravity = true;
                bossSkilDirection = charDirection;
                switch (bossSkilDirection)
                {
                    case SpriteEffects.None:
                        drillBody.ApplyLinearImpulse(new Vector2(3f * speed, 0));
                        break;
                    case SpriteEffects.FlipHorizontally:
                        drillBody.ApplyLinearImpulse(new Vector2(-3f * speed, 0));
                        break;
                }
            }
            else
            {
                if (IsContact(drillBody, "Ground"))
                {
                    if (countBounce > 3)
                    {
                        switch (bossSkilDirection)
                        {
                            case SpriteEffects.None:
                                bossSkilDirection = SpriteEffects.FlipHorizontally;
                                break;
                            case SpriteEffects.FlipHorizontally:
                                bossSkilDirection = SpriteEffects.None;
                                break;
                        }
                    }
                    drillBody.RestoreCollisionWith(enemyHitBox);
                    switch (bossSkilDirection)
                    {
                        case SpriteEffects.None:
                            drillBody.ApplyLinearImpulse(new Vector2(-1.8f * speed, 0));
                            break;
                        case SpriteEffects.FlipHorizontally:
                            drillBody.ApplyLinearImpulse(new Vector2(1.8f * speed, 0));
                            break;
                    }
                    countBounce++;
                }

                if (IsContact(enemyHitBox, "SkillBoss"))
                {
                    drillBody.Dispose();
                    curBossStatus = LucasStatus.IDLE;
                    randomAction = 0;
                    action3 = false;
                    countBounce = 0;
                }
            }
        }


        public override void ChangeAnimationStatus()
        {
            switch (curBossStatus)
            {
                case LucasStatus.IDLE:
                    delay = 200f;
                    spriteVector = idleSpriteVector;
                    spriteSize = new Vector2(idleSize.X, idleSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case LucasStatus.ACTION1:
                    delay = 200f;
                    spriteVector = action1SpriteVector;
                    spriteSize = new Vector2(action1Size.X, action1Size.Y);
                    allframes = spriteVector.Count();
                    break;
                case LucasStatus.ACTION2_1:
                    delay = 300f;
                    spriteVector = action2_1SpriteVector;
                    spriteSize = new Vector2(action2_1Size.X, action2_1Size.Y);
                    allframes = spriteVector.Count();
                    break;
                case LucasStatus.ACTION2_2:
                    delay = 300f;
                    spriteVector = action2_2SpriteVector;
                    spriteSize = new Vector2(action2_2Size.X, action2_2Size.Y);
                    allframes = spriteVector.Count();
                    break;
                case LucasStatus.ACTION3:
                    delay = 300f;
                    spriteVector = action3SpriteVector;
                    spriteSize = new Vector2(action3Size.X, action3Size.Y);
                    allframes = spriteVector.Count();
                    break;
                case LucasStatus.DEAD:
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

                //if boss not ded yet
                if (!IsBossDead())
                {
                    if (action2)
                    {
                        spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(drillBody.Position), new Rectangle(50, 1262, 62, 28), Color.White, 0, new Vector2(62 / 2, 28 / 2), 1, bossSkilDirection, 0f);
                    }
                    else if (action3)
                    {
                        spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(drillBody.Position), new Rectangle(152, 1248, 56, 56), Color.White, 0, new Vector2(56 / 2, 56 / 2), 1, bossSkilDirection, 0f);
                    }
                }
            }
        }
        public bool IsBossDead()
        {
            if (curBossStatus == LucasStatus.DEAD)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsBossEnd()
        {
            if (curBossStatus == LucasStatus.END)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
