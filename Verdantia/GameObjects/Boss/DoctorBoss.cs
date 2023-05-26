using System;
using System.Collections.Generic;
using System.Linq;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using ScifiDruid.Managers;

namespace ScifiDruid.GameObjects
{
    public class DoctorBoss : Boss
    {
        private Rectangle fieldBoss;

        private int frameState;
        private bool repeat;

        //random boss action
        private Random rand = new Random();
        private int randomAction;
        //attribute using for moving of boss
        private float ballDelay;

        //boolean to do action
        private bool action1 = false;
        private bool action2 = false;

        private int countLoop;

        //for animation
        private Vector2 idleSize;
        private Vector2 action1Size;
        private Vector2 action2Size;
        private Vector2 action3Size;
        private Vector2 deadSize;

        private List<Vector2> idleSpriteVector;
        private List<Vector2> action1SpriteVector;
        private List<Vector2> action2SpriteVector;
        private List<Vector2> action3SpriteVector;
        private List<Vector2> deadSpriteVector;

        private DoctorStatus preBossStatus;
        private DoctorStatus curBossStatus;

        private List<DoctorBall> lightningBall;
        private Texture2D skillBossTexture;
        private BossLongShoot redLightning;
        private DoctorLaser laser;

        private float warningSkillTime;

        private SoundEffect laughSound, redElectricSound, laserSound;
        private enum DoctorStatus
        {
            IDLE,
            ACTION1,
            ACTION2,
            ACTION3,
            DEAD,
            END
        }

        public DoctorBoss(Texture2D texture, Texture2D skillBossTexture) : base(texture)
        {
            this.texture = texture;
            this.skillBossTexture = skillBossTexture;

            idleSize = new Vector2(38, 88);
            action1Size = new Vector2(60, 88);
            action2Size = new Vector2(62, 88);
            action3Size = new Vector2(60, 88);
            deadSize = new Vector2(38, 88);

            //idle spritevector
            idleSpriteVector = new List<Vector2>() { new Vector2(14, 2), new Vector2(100, 2), new Vector2(186, 2) };

            //action1 spritevector
            action1SpriteVector = new List<Vector2>() { new Vector2(272, 2), new Vector2(358, 2), new Vector2(452, 2) };

            //action2 spritevector
            action2SpriteVector = new List<Vector2>() { new Vector2(0, 114), new Vector2(86, 114) };

            //action3 spritevector
            action3SpriteVector = new List<Vector2>() { new Vector2(272, 2), new Vector2(358, 2), new Vector2(452, 2) };

            //dead spritevector
            deadSpriteVector = new List<Vector2>() { new Vector2(191, 114), new Vector2(277, 114) };

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
            laserSound = content.Load<SoundEffect>("Sounds/Boss3/laser");
            laughSound = content.Load<SoundEffect>("Sounds/Boss3/laughViroj");
            redElectricSound = content.Load<SoundEffect>("Sounds/Boss3/redElectric");
        }

        public override void Initial(Rectangle spawnPosition, Player player, Rectangle fieldBoss)
        {
            this.player = player;
            this.fieldBoss = fieldBoss;

            textureHeight = (int)size.Y;
            textureWidth = (int)size.X;

            enemyHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 1f, ConvertUnits.ToSimUnits(new Vector2(spawnPosition.X, spawnPosition.Y - 1)), 0, BodyType.Dynamic, "Enemy");
            enemyHitBox.FixedRotation = true;
            enemyHitBox.Friction = 1.0f;
            enemyHitBox.AngularDamping = 2.0f;
            enemyHitBox.LinearDamping = 2.0f;

            isAlive = false;

            charDirection = SpriteEffects.FlipHorizontally;  // heading direction

            bossOrigin = new Vector2(textureWidth / 2, textureHeight / 2);  //draw in the middle

            lightningBall = new List<DoctorBall>();

            skillTime = 5;
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
                    curBossStatus = DoctorStatus.DEAD;
                }
            }

            //boss action
            Action();

            //if dead animation animationEnd
            if (animationDead)
            {
                curBossStatus = DoctorStatus.END;
            }

            //animation
            if (preBossStatus != curBossStatus)
            {
                frames = 0;
                frameState = 0;
            }

            if (lightningBall.Count > 0)
            {
                foreach (DoctorBall item in lightningBall)
                {
                    item.Update(gameTime);
                }
            }

            if (action1 && redLightning != null)
            {
                redLightning.Update(gameTime);
            }

            if (action2 && laser != null)
            {
                laser.Update(gameTime);
            }

            ChangeAnimationStatus();
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (curBossStatus == DoctorStatus.DEAD)
            {
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
            }
            else
            {
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
            }
            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            preBossStatus = curBossStatus;
        }

        public override void Action()
        {
            if (isAlive && player.health > 0)
            {
                if (skillTime <= 0 && curBossStatus == DoctorStatus.IDLE)
                {
                    randomAction = rand.Next(1, 4);
                    //randomAction = 2;
                    skillTime = 5;
                }
                else if (curBossStatus == DoctorStatus.IDLE)
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
                        Skill1();
                        break;
                    case 3:
                        Skill2();
                        break;
                }

                if (ballDelay <= 0)
                {
                    ballDelay = 3f;
                    Vector2 ballPosition = new Vector2((fieldBoss.X - fieldBoss.Width / 2) + 5, fieldBoss.Y - 30);
                    DoctorBall ball = new DoctorBall(skillBossTexture, ConvertUnits.ToSimUnits(ballPosition), this);
                    ball.CreateBall();
                    lightningBall.Add(ball);
                }
                else
                {
                    ballDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (lightningBall.Count > 7)
                {
                    lightningBall[0].ballBody.Dispose();
                    lightningBall.RemoveAt(0);
                }
            }
            else
            {
                foreach (DoctorBall ball in lightningBall)
                {
                    ball.ballBody.Dispose();
                }
                lightningBall.Clear();
            }
        }

        public void Skill1()
        {
            if (!action1)
            {
                action1 = true;
                curBossStatus = DoctorStatus.ACTION1;
                warningSkillTime = 1;

                redElectricSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);

            }
            else if (warningSkillTime <= 0 && redLightning == null)
            {
                Vector2 redLightingPosition = position + new Vector2(-10, 0);
                redLightning = new BossLongShoot(skillBossTexture, redLightingPosition, this, "Doctor");
            }
            else if (warningSkillTime <= 0 && redLightning != null)
            {
                warningSkillTime = 1;
                if (countLoop == 1 && redLightning.bossLongShotStatus == BossLongShoot.LongShotStatus.LONGSHOTEND)
                {
                    redLightning.longShotBody.Dispose();
                    redLightning = null;
                    action1 = false;
                    randomAction = 0;
                    curBossStatus = DoctorStatus.IDLE;
                    countLoop = 0;
                    return;
                }
                countLoop++;
            }
            else if (warningSkillTime > 0)
            {
                warningSkillTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void Skill2()
        {
            if (!action2)
            {
                action2 = true;
                curBossStatus = DoctorStatus.ACTION2;
                warningSkillTime = 1;

                laughSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);

            }
            else if (warningSkillTime <= 0 && laser == null)
            {
                Vector2 laserPosition = position + new Vector2(-5, -2.2f);
                laser = new DoctorLaser(skillBossTexture, laserPosition, this);
                laser.CreateLaser();

                laserSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
            }
            else if (laser != null)
            {
                float leftPosition = ConvertUnits.ToSimUnits((fieldBoss.X - fieldBoss.Width / 2) + 150);  
                if (laser.position.X <= leftPosition)
                {
                    laser.laserBody.Dispose();
                    laser = null;
                    action2 = false;
                    randomAction = 0;
                    curBossStatus = DoctorStatus.IDLE;
                }
            }
            else if (warningSkillTime > 0)
            {
                warningSkillTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override void ChangeAnimationStatus()
        {
            switch (curBossStatus)
            {
                case DoctorStatus.IDLE:
                    delay = 100;
                    spriteVector = idleSpriteVector;
                    spriteSize = new Vector2(idleSize.X, idleSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case DoctorStatus.ACTION1:
                    delay = 100;
                    spriteVector = action1SpriteVector;
                    spriteSize = new Vector2(action1Size.X, action1Size.Y);
                    allframes = spriteVector.Count();
                    break;
                case DoctorStatus.ACTION2:
                    delay = 100;
                    spriteVector = action2SpriteVector;
                    spriteSize = new Vector2(action2Size.X, action2Size.Y);
                    allframes = spriteVector.Count();
                    break;
                case DoctorStatus.ACTION3:
                    delay = 100;
                    spriteSize = new Vector2(action3Size.X, action3Size.Y);
                    spriteVector = action3SpriteVector;
                    allframes = spriteVector.Count();
                    break;
                case DoctorStatus.DEAD:
                    delay = 900f;
                    spriteVector = deadSpriteVector;
                    spriteSize = new Vector2(deadSize.X, deadSize.Y);
                    allframes = spriteVector.Count();
                    break;
            }
        }

        public bool IsBossDead()
        {
            if (curBossStatus == DoctorStatus.DEAD)
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
            if (curBossStatus == DoctorStatus.END)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //if (!animationDead)
            //{
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, bossOrigin, 1f, charDirection, 0f);
            //}

            if (lightningBall.Count > 0)
            {
                foreach (DoctorBall ball in lightningBall)
                {
                    ball.Draw(spriteBatch);
                }
            }

            if (action1 && redLightning != null)
            {
                redLightning.Draw(spriteBatch);
            }

            if (action2 && laser != null)
            {
                laser.Draw(spriteBatch);
            }


        }
    }
}

