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
    public class JaneBoss : Boss
    {
        Texture2D ammoTexture;

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

        private List<JaneBomb> rockets;
        private List<JaneBullet> bulletLists;
        private Body beam;

        public JaneStatus preBossStatus;
        public JaneStatus curBossStatus;

        private int countTeleport;

        private Rectangle fieldBoss;

        private Vector2 firstPosition;

        //Action all states
        public enum JaneStatus
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

        public JaneBoss(Texture2D texture, Texture2D ammoTexture) : base(texture)
        {
            this.texture = texture;
            this.ammoTexture = ammoTexture;

            idleSize = new Vector2(74, 112);
            shootGunSize = new Vector2(74, 112);
            shootPlasmaSize = new Vector2(88, 112);
            action2Size = new Vector2(88, 112);
            prepareSize = new Vector2(78, 112);
            dashInSize = new Vector2(78, 98);
            dashOutSize = new Vector2(90, 112);
            punchSize = new Vector2(90, 112);
            deadSize = new Vector2(74, 112);

            //idle spritevector
            idleSpriteVector = new List<Vector2>() { new Vector2(8, 0), new Vector2(140, 0), new Vector2(256, 0) };

            //action1 spritevector
            shootGunSpriteVector = new List<Vector2>() { new Vector2(372, 0), new Vector2(488, 0)};
            shootPlasmaSpriteVector = new List<Vector2>() { new Vector2(0, 146), new Vector2(134, 146), new Vector2(256, 146)};

            //action2 spritevector
            callDownBombSpriteVector = new List<Vector2>() { new Vector2(358, 146), new Vector2(474, 146), new Vector2(580, 146)};


            //action3 spritevector
            prepareSpriteVector = new List<Vector2>() { new Vector2(0, 284) };
            dashInSpriteVector = new List<Vector2>() { new Vector2(134, 294), new Vector2(251, 294), new Vector2(363, 294) };
            dashOutSpriteVector = new List<Vector2>() { new Vector2(475, 284), new Vector2(589, 286), new Vector2(707, 286) };
            punchSpriteVector = new List<Vector2>() { new Vector2(4, 434), new Vector2(112, 434), new Vector2(250, 434) };

            //dead spritevector
            deadSpriteVector = new List<Vector2>() { new Vector2(368, 434), new Vector2(472, 434) };

            frames = 0;

            //animation dead state
            frameState = 0;
            repeat = false;
        }

        public override void Initial(Rectangle spawnPosition, Player player , Rectangle fieldBoss)
        {
            this.player = player;
            this.fieldBoss = fieldBoss;

            textureHeight = (int)size.Y;
            textureWidth = (int)size.X;

            enemyHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 100f, ConvertUnits.ToSimUnits(new Vector2(spawnPosition.X, spawnPosition.Y - 1)), 0, BodyType.Dynamic, "Enemy");
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

            rockets = new List<JaneBomb>();
            bulletLists = new List<JaneBullet>();

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
                    //randomAction = rand.Next(1, 4);
                    randomAction = 2;
                    skillTime = 3;
                }
                else if (curBossStatus == JaneStatus.IDLE)
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

        public void Skill1()
        {
            if (skillTime <= 0 && bulletLists.Count < 3)
            {
                action1 = true;
                curBossStatus = JaneStatus.SHOOTGUN;
                JaneBullet bullet = new JaneBullet(ammoTexture, enemyHitBox.Position, this, charDirection);
                skillTime = 1;
                bulletLists.Add(bullet);
                bulletLists[bulletLists.Count - 1].Shoot();
            }

            if (skillTime <= 0 && bulletLists.Count == 3 && curBossStatus == JaneStatus.SHOOTGUN)
            {
                curBossStatus = JaneStatus.SHOOTPLASMA;
                skillTime = 2;
            }
            else if (skillTime <= 0 && bulletLists.Count == 3 && curBossStatus == JaneStatus.SHOOTPLASMA && beam == null)
            {
                Vector2 positionBeam = Vector2.Zero;

                switch (charDirection)
                {
                    case SpriteEffects.None:
                        positionBeam = enemyHitBox.Position + new Vector2(10, -0.5f);
                        break;
                    case SpriteEffects.FlipHorizontally:
                        positionBeam = enemyHitBox.Position + new Vector2(-10, -0.5f);
                        break;
                }

                beam = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(1200), ConvertUnits.ToSimUnits(shootPlasmaSize.Y), 1, positionBeam, 0, BodyType.Static, "SkillBoss");
                beam.IsSensor = true;
                skillTime = 0.3f;
            }
            else if (skillTime <= 0 && beam != null)
            {
                foreach (var item in bulletLists)
                {
                    item.bulletBody.Dispose();
                }
                bulletLists.Clear();
                beam.Dispose();
                beam = null;
                curBossStatus = JaneStatus.IDLE;
                action1 = false;
                randomAction = 0;
                skillTime = 3;
            }

            if (skillTime > 0)
            {
                skillTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void Skill2()
        {
            if (skillTime <= 0 && rockets.Count < 2)
            {
                action2 = true;
                curBossStatus = JaneStatus.CALLDOWNBOMB;
                JaneBomb dropBomb = new JaneBomb(ammoTexture, playerPosition + new Vector2(0, -10), this, prepareSize);
                //Body dropRocket = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(prepareSize.X), ConvertUnits.ToSimUnits(prepareSize.Y), 1, , 0, BodyType.Dynamic, "SkillBoss");
                //dropRocket.IsSensor = false;
                skillTime = 3;
                rockets.Add(dropBomb);
            }
            else if (skillTime > 0)
            {
                skillTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                foreach (var item in rockets)
                {
                    item.bombBody.Dispose();
                }
                rockets.Clear();
                curBossStatus = JaneStatus.IDLE;
                randomAction = 0;
                skillTime = 3;
            }

            if (rockets.Count > 0)
            {
                foreach (var item in rockets)
                {
                    item.Update(gameTime);
                    if (IsContact(item.bombBody, "Ground"))
                    {
                        item.bossBombStatus = JaneBomb.BombStatus.BOMBDEAD;
                        item.bombBody.IsStatic = true;
                    }

                    if (IsContact(item.bombBody, "Player"))
                    {
                        item.bombBody.IsSensor = true;
                    }

                    if (item.bossBombStatus == JaneBomb.BombStatus.BOMBEND)
                    {
                        item.bombBody.Dispose();
                    }
                }
            }

            //clear random action number
            //do animation1

        }

        public void Skill3()
        {
            if (!action3)
            {
                action3 = true;
                curBossStatus = JaneStatus.PREPARE;
                firstPosition = enemyHitBox.Position;
                skillTime = 1;
            }

            if (skillTime > 0)
            {
                skillTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (skillTime <= 0)
            {
                if (curBossStatus == JaneStatus.PREPARE && countTeleport == 0)
                {
                    curBossStatus = JaneStatus.DASHIN;
                    countTeleport++;
                    skillTime = 0.5f;
                }
                else if (curBossStatus == JaneStatus.DASHIN && countTeleport == 1)
                {
                    enemyHitBox.IsStatic = true;
                    switch (charDirection)
                    {
                        case SpriteEffects.None:
                            enemyHitBox.Position = playerPosition + new Vector2(-1, 0);
                            curBossStatus = JaneStatus.DASHOUT;
                            break;
                        case SpriteEffects.FlipHorizontally:
                            enemyHitBox.Position = playerPosition + new Vector2(1f, 0);
                            curBossStatus = JaneStatus.DASHOUT;
                            break;
                    }
                    skillTime = 0.5f;
                }
                else if (curBossStatus == JaneStatus.DASHOUT && countTeleport == 1)
                {
                    curBossStatus = JaneStatus.PUNCH;
                    float playerPosX = player.position.X;
                    float playerPosY = player.position.Y;
                    float enemyPosX = enemyHitBox.Position.X;
                    float enemyPosY = enemyHitBox.Position.Y;

                    bool xPositive = enemyPosX - playerPosX < 3 && enemyPosX - playerPosX > 0;
                    bool xNegative = enemyPosX - playerPosX > -3 && enemyPosX - playerPosX < 0;

                    bool yPositive = enemyPosY - playerPosY < 2 && enemyPosY - playerPosY > -2;

                    if (xPositive && yPositive && charDirection == SpriteEffects.FlipHorizontally)
                    {
                        player.GotHit(Player.KnockbackStatus.LEFT);
                    }
                    else if (xNegative && yPositive && charDirection == SpriteEffects.None)
                    {
                        player.GotHit(Player.KnockbackStatus.RIGHT);
                    }
                    skillTime = 0.5f;
                }
                else if (curBossStatus == JaneStatus.PUNCH)
                {
                    countTeleport++;
                    curBossStatus = JaneStatus.DASHIN;
                    skillTime = 0.5f;
                }
                else if (curBossStatus == JaneStatus.DASHIN && countTeleport == 2)
                {
                    enemyHitBox.IsStatic = false;

                    curBossStatus = JaneStatus.DASHOUT;
                    //enemyHitBox.Position = new Vector2(ConvertUnits.ToSimUnits(fieldBoss.X),enemyHitBox.Position.Y);
                    if (ConvertUnits.ToSimUnits(fieldBoss.X) - enemyHitBox.Position.X < 0 )
                    {
                        enemyHitBox.Position = new Vector2(ConvertUnits.ToSimUnits(fieldBoss.X) + ConvertUnits.ToSimUnits((fieldBoss.Width / 2) - 100), firstPosition.Y);
                        charDirection = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        enemyHitBox.Position = new Vector2(ConvertUnits.ToSimUnits(fieldBoss.X) - ConvertUnits.ToSimUnits((fieldBoss.Width / 2) - 100), firstPosition.Y);
                        charDirection = SpriteEffects.None;
                    }
                    skillTime = 0.5f;
                }
                else if (curBossStatus == JaneStatus.DASHOUT && countTeleport == 2)
                {
                    curBossStatus = JaneStatus.IDLE;
                    action3 = false;
                    randomAction = 0;
                    skillTime = 3;
                    countTeleport = 0;
                }
            }
        }

        /*
        public void DashOut()
        {

        }

        public void DashIn()
        {

        }

        public void Prepare()
        {
            curBossStatus = JaneStatus.DASHIN;
            countTeleport++;
            skillTime = 0.5f;
        }

        public void Punch()
        {

        }
        */

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

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!animationDead)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, bossOrigin, 1f, charDirection, 0f);
            }

            if (action2)
            {
                foreach (JaneBomb item in rockets)
                {
                    item.Draw(spriteBatch);
                    //spriteBatch.Draw(ammoTexture, ConvertUnits.ToDisplayUnits(item.bombBody.Position), new Rectangle(0, 0, (int)ConvertUnits.ToDisplayUnits(ConvertUnits.ToSimUnits(100)), (int)ConvertUnits.ToDisplayUnits(ConvertUnits.ToSimUnits(100))), Color.White, 0, new Vector2(prepareSize.X / 2, prepareSize.Y / 2), 1f, SpriteEffects.None, 0f);
                }
            }

            if (action1)
            {
                foreach (JaneBullet bullet in bulletLists)
                {
                    bullet.Draw(spriteBatch);
                }

                if (beam != null)
                {
                    spriteBatch.Draw(ammoTexture, ConvertUnits.ToDisplayUnits(beam.Position), new Rectangle(0, 0, (int)ConvertUnits.ToDisplayUnits(ConvertUnits.ToSimUnits(1200)), (int)ConvertUnits.ToDisplayUnits(ConvertUnits.ToSimUnits(shootPlasmaSize.Y))), Color.White, 0, new Vector2(1200 / 2, shootPlasmaSize.Y / 2), 1f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
