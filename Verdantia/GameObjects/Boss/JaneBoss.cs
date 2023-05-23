using System;
using System.Collections.Generic;
using System.Linq;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScifiDruid.GameObjects
{
    public class JaneBoss : Boss
    {
        //framestate for dead animation
        private int frameState;
        private bool repeat;

        //random boss action
        private Random rand = new Random();
        private int randomAction;
        //attribute using for moving of boss
        private float movingTime = 3.4f;

        //boss size
        private Vector2 idleSize;

        private Vector2 shootGunSize;
        private Vector2 shootPlasmaSize;

        private Vector2 action2Size;

        private Vector2 prepareSize;
        private Vector2 dashInSize;
        private Vector2 dashOutSize;
        private Vector2 punchSize;

        private Vector2 deadSize;

        //boolean to do action
        private bool action1 = false;
        private bool action2 = false;
        private bool action3 = false;

        //boolean action1 in 2 part and action3

        //unique jane animation
        private List<Vector2> idleSpriteVector;
        private List<Vector2> shootGunSpriteVector;

        private List<Vector2> shootPlasmaSpriteVector;

        private List<Vector2> callDownBombSpriteVector;

        private List<Vector2> prepareSpriteVector;
        private List<Vector2> dashInSpriteVector;
        private List<Vector2> dashOutSpriteVector;
        private List<Vector2> punchSpriteVector;
        private List<Vector2> deadSpriteVector;


        private JaneStatus preBossStatus;
        private JaneStatus curBossStatus;

        public JaneBoss(Texture2D texture) : base(texture)
        {
            this.texture = texture;

            idleSize = new Vector2(74, 112);
            shootGunSize = new Vector2(74, 112);
            shootPlasmaSize = new Vector2(88, 88);
            action2Size = new Vector2(84, 110);
            prepareSize = new Vector2(78, 98);
            dashInSize = new Vector2(78, 98);
            dashOutSize = new Vector2(90, 104);
            punchSize = new Vector2(92, 120);
            deadSize = new Vector2(74, 112);

            //idle spritevector
            idleSpriteVector = new List<Vector2>() { new Vector2(8, 0), new Vector2(140, 0), new Vector2(256, 0)};

            //action1 spritevector
            shootGunSpriteVector = new List<Vector2>() { new Vector2(372, 0), new Vector2(488, 0)};
            shootPlasmaSpriteVector = new List<Vector2>() { new Vector2(0, 170), new Vector2(134, 170), new Vector2(256, 170)};

            //action2 spritevector
            callDownBombSpriteVector = new List<Vector2>() { new Vector2(362, 148), new Vector2(478, 148), new Vector2(584, 148)};

            //action3 spritevector
            prepareSpriteVector = new List<Vector2>() { new Vector2(0, 298) };
            dashInSpriteVector = new List<Vector2>() { new Vector2(134, 298), new Vector2(251, 298), new Vector2(363, 298) };
            dashOutSpriteVector = new List<Vector2>() { new Vector2(488, 292), new Vector2(589, 294), new Vector2(707, 294) };
            punchSpriteVector = new List<Vector2>() { new Vector2(4, 426), new Vector2(114, 426), new Vector2(242, 426) };

            //dead spritevector
            deadSpriteVector = new List<Vector2>() { new Vector2(368, 434), new Vector2(472, 434)};

            frames = 0;

            //animation dead state
            frameState = 0;
            repeat = false;
        }

        //Action all states
        private enum JaneStatus
        {
            IDLE,
            SHOOTGUN,
            SHOOTPLASMA,
            CALLDOWNBOMB,
            PREPARE,
            DASHIN,
            DASHOUT,
            PUNCH,
            DEAD,
            END
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

            isAlive = false;

            charDirection = SpriteEffects.FlipHorizontally;  // heading direction

            bossOrigin = new Vector2(textureWidth / 2, textureHeight / 2);  //draw in the middle

            skillTime = 5;

            curBossStatus = JaneStatus.IDLE;
            preBossStatus = JaneStatus.IDLE;
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
                    curBossStatus = JaneStatus.DEAD;
                }
            }

            //boss action
            Action();

            //if dead animation animationEnd
            if (animationDead)
            {
                curBossStatus = JaneStatus.END;
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
                case JaneStatus.IDLE:
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
                case JaneStatus.DEAD:
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
                case JaneStatus.SHOOTGUN:
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
                case JaneStatus.SHOOTPLASMA:
                    if (elapsed >= delay)
                    {
                        if (frames < allframes - 1)
                        {
                            frames++;
                        }
                        elapsed = 0;
                    }
                    break;
                case JaneStatus.CALLDOWNBOMB:
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
                case JaneStatus.PREPARE:
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

                if (skillTime <= 0 && curBossStatus == JaneStatus.IDLE)
                {
                    randomAction = rand.Next(1, 6);
                    skillTime = 5;
                }
                else if (curBossStatus == JaneStatus.IDLE)
                {
                    skillTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                //do action 1
                switch (randomAction)
                {
                    case 1:
                        //Skill1();
                        break;
                    case 2:
                        //Skill2();
                        break;
                    case 3:
                        //Skill3();
                        break;
                }
            }
        }

        public void Skill1()
        {
            if (!action1)
            {
                action1 = true;
                curBossStatus = JaneStatus.CALLDOWNBOMB;
            }

            //clear random action number
            //do animation1
            curBossStatus = JaneStatus.IDLE;
            //do normal walking left and right
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeElapsed >= movingTime)
            {
                switch (charDirection)
                {
                    case SpriteEffects.None:
                        timeElapsed = 0f;
                        action1 = false;
                        charDirection = SpriteEffects.FlipHorizontally;
                        curBossStatus = JaneStatus.IDLE;
                        randomAction = 0;
                        break;
                    case SpriteEffects.FlipHorizontally:
                        timeElapsed = 0f;
                        charDirection = SpriteEffects.None;
                        break;
                }
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
                curBossStatus = JaneStatus.CALLDOWNBOMB;
            }
        }

        public void Skill3()
        {
            if (!action3)
            {
                action3 = true;
                curBossStatus = JaneStatus.PREPARE;
            }
        }

        public override void ChangeAnimationStatus()
        {
            switch (curBossStatus)
            {
                case JaneStatus.IDLE:
                    delay = 300f;
                    spriteVector = idleSpriteVector;
                    spriteSize = new Vector2(idleSize.X, idleSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case JaneStatus.SHOOTGUN:
                    delay = 200f;
                    spriteVector = shootGunSpriteVector;
                    spriteSize = new Vector2(shootGunSize.X, shootGunSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case JaneStatus.SHOOTPLASMA:
                    delay = 200f;
                    spriteVector = shootPlasmaSpriteVector;
                    spriteSize = new Vector2(shootPlasmaSize.X, shootPlasmaSize.Y);
                    allframes = spriteVector.Count();
                    break;
                case JaneStatus.CALLDOWNBOMB:
                    delay = 300f;
                    spriteVector = callDownBombSpriteVector;
                    spriteSize = new Vector2(action2Size.X, action2Size.Y);
                    allframes = spriteVector.Count();
                    break;
                case JaneStatus.PREPARE:
                    delay = 300f;
                    spriteSize = prepareSize;
                    spriteVector = prepareSpriteVector;
                    allframes = spriteVector.Count();
                    break;
                case JaneStatus.DASHIN:
                    delay = 300f;
                    spriteSize = dashInSize;
                    spriteVector = dashInSpriteVector;
                    allframes = spriteVector.Count();
                    break;
                case JaneStatus.DASHOUT:
                    delay = 300f; 
                    spriteSize = dashOutSize;
                    spriteVector = dashOutSpriteVector;
                    allframes = spriteVector.Count();
                    break;
                case JaneStatus.PUNCH:
                    delay = 300f; 
                    spriteSize = punchSize;
                    spriteVector = punchSpriteVector;
                    allframes = spriteVector.Count();
                    break;
                case JaneStatus.DEAD:
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
            }
        }

        public bool IsBossDead()
        {
            if (curBossStatus == JaneStatus.DEAD)
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
            if (curBossStatus == JaneStatus.END)
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
