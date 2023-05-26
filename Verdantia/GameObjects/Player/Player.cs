using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Box2DNet.Dynamics.Contacts;
using Microsoft.Xna.Framework.Content;
using ScifiDruid.Managers;
using Microsoft.Xna.Framework.Media;

namespace ScifiDruid.GameObjects
{
    public class Player : _GameObject
    {
        Vector2 preCollisionLinearVelocity;

        private Texture2D texture;
        private Texture2D bulletTexture;
        private Texture2D regenTexture;
        private Texture2D lionTexture;
        private Texture2D crocodileTexture;

        private Rectangle characterDestRec;
        private Rectangle characterSouceRec;

        private KeyboardState currentKeyState;
        private KeyboardState oldKeyState;

        public Body hitBox;
        public Body lionBody;

        private List<Enemy> enemyContract;

        //player status
        public PlayerStatus playerStatus;
        public KnockbackStatus knockbackStatus;

        private bool touchGround;

        private int jumpCount = 0;
        private bool wasJumped;

        //time for animation
        private int jumpTime;
        private int jumpDelay;

        private int attackTimeDelay;
        private int attackDelay;
        public float manaRegenTime;

        private int healCount;

        private int dashTime;

        private bool isShootup;

        public int health;
        public float mana;

        //static for change in shop and apply to all Stage
        public static int maxHealth;
        public static int maxMana;
        public static int manaRegenAmount;
        public static float manaMaxRegenTime;

        public static int crocDmg;
        public static int skill2MaxCooldown;
        public static int dashMaxCooldown;

        public static int attackMaxTime;
        public static int attackMana;

        //Count cooldown of action
        public float skill1Cooldown; // Skill Regen
        public float skill2Cooldown; // skill Crocodile
        public float skill3Cooldown; // Skill Lion
        public float dashCooldown;

        private float skill3Time;
        private float skill3Active;

        //animation time for shoot and dash
        private float attackAnimationTime;
        private float dashAnimationTime;

        private float manaIdleRegen;
        private float manaIdleRegenTime;

        //Cooldown time make to static for change in shop and apply to all Stage
        public static int skill1MaxCooldown;
        public static int skill3MaxCooldown;

        public float hitCooldown;

        private Vector2 playerOrigin;

        private Vector2 bulletPosition;

        public static bool isAttack = false;

        private bool press = false;
        private bool startDash = false;
        private bool startCool = false;

        public static bool level2Unlock;
        public static bool level3Unlock;

        public float jumpHigh;

        private SpriteEffects enemyDirection;

        public List<Enemy> enemies;

        public List<PlayerBullet> bulletList;

        private GameTime gameTime;

        //player real size for hitbox
        private int textureWidth, textureHeight;

        //animation
        private PlayerAnimation playerAnimation;
        private PlayerSkillAnimation playerSkillAnimation;
        //check if animation animationEnd or not
        private bool animationEnd;

        private int playerDirectionInt = 1;

        //all sound effect
        private SoundEffect playerShoot, playerCrocShoot, playerDeadSound, playerJumpSound;

        //all boolean to check sound not repeat
        private bool playerDead = false;

        private bool isCroc = false;

        public int lionDmg;
        public int bulletDmg;

        private SoundEffect crocBusterSound, roarSound, jumpSound, dashSound, healSound, atkSound, shootSound, getHitSound;
        public enum PlayerStatus
        {
            IDLE,
            SHOOT,
            RUN,
            SHOOT_RUN,
            SHOOT_UP,
            SHOOT_UP_RUN,
            JUMP,
            SHOOT_AIR,
            FALLING,
            SKILL,
            TAKE_DAMAGE,
            DASH,
            DEAD,
            END
        }

        public enum KnockbackStatus
        {
            RIGHT,
            LEFT
        }

        public Player(Texture2D texture, Texture2D bulletTexture, Texture2D lionSKill) : base(texture)
        {
            this.texture = texture;
            this.bulletTexture = bulletTexture;
            this.lionTexture = lionSKill;

            LoadContent();
        }
        public void LoadContent()
        {
            ContentManager content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
            //sfx
            crocBusterSound = content.Load<SoundEffect>("Sounds/Player/crocBuster");
            atkSound = content.Load<SoundEffect>("Sounds/Player/crocBuster");
            dashSound = content.Load<SoundEffect>("Sounds/Player/dash");
            getHitSound = content.Load<SoundEffect>("Sounds/Player/gethit");
            healSound = content.Load<SoundEffect>("Sounds/Player/heal");
            jumpSound = content.Load<SoundEffect>("Sounds/Player/jump");
            roarSound = content.Load<SoundEffect>("Sounds/Player/roar");
            shootSound = content.Load<SoundEffect>("Sounds/Player/shoot");
        }

        public override void Initial()
        {
            playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position + new Vector2(0, 0), "Null");

            textureWidth = (int)size.X;
            textureHeight = (int)size.Y;

            playerAnimation = new PlayerAnimation(texture);
            bulletList = new List<PlayerBullet>();
            enemyContract = new List<Enemy>();

            hitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 1f, Vector2.Zero, 0, BodyType.Dynamic, "Player");
            hitBox.FixedRotation = true;
            hitBox.Friction = 1.0f;
            hitBox.AngularDamping = 2.0f;
            hitBox.LinearDamping = 2.0f;

            //check touch ground condition
            touchGround = true;

            playerOrigin = new Vector2(textureWidth / 2, textureHeight / 2);

            playerStatus = PlayerStatus.IDLE;
            playerAnimation.Initialize();

            charDirection = SpriteEffects.FlipHorizontally;

            
            
            if (Singleton.Instance.stageunlock == 1)
            {
                if (maxHealth == 0)
                {
                    maxHealth = 3;
                }

                if (maxMana == 0)
                {
                    maxMana = 100;
                }

                if (manaMaxRegenTime == 0)
                {
                    manaMaxRegenTime = 5;
                }

                if (manaRegenAmount == 0)
                {
                    manaRegenAmount = 0;
                }

                if (attackMana == 0)
                {
                    attackMana = 5;
                }

                if (attackMaxTime == 0)
                {
                    attackMaxTime = 1000;
                }

                if (crocDmg == 0)
                {
                    crocDmg = 4;
                }

                if (skill1MaxCooldown == 0)
                {
                    skill1MaxCooldown = 10;
                }

                if (skill2MaxCooldown == 0)
                {
                    skill2MaxCooldown = 30;
                }

                if (skill3MaxCooldown == 0)
                {
                    skill3MaxCooldown = 30;
                }

                if (dashMaxCooldown == 0)
                {
                    dashMaxCooldown = 10;
                }
            }

            base.Initial();
        }

        public override void Update(GameTime gameTime)
        {
            position = hitBox.Position;
            this.gameTime = gameTime;

            if (isAlive)
            {
                //check touch ground condition
                if (IsGround())
                {
                    touchGround = true;
                }
                else
                {
                    touchGround = false;
                }

                bool hitByEnemy = (IsContact(hitBox, "Enemy") || IsContact(hitBox, "EnemyBullet"));

                if ( (IsContact(hitBox, "SkillBoss") || hitByEnemy) && playerStatus != PlayerStatus.DASH)
                {
                    GotHit(knockbackStatus);
                }
                else if (hitCooldown > 0)
                {
                    hitCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (!touchGround)
                {
                    hitBox.LinearVelocity = new Vector2(hitBox.LinearVelocity.X * 0.97f, hitBox.LinearVelocity.Y);
                    hitBox.GravityScale = 2;
                }
                else
                {
                    hitBox.GravityScale = 1;
                }

                if (hitCooldown <= 0)
                {
                    foreach (Body item in Singleton.Instance.world.BodyList)
                    {
                        if (item.UserData.Equals("Enemy") || item.UserData.Equals("SkillBoss") || item.UserData.Equals("EnemyBullet"))
                        {
                            hitBox.RestoreCollisionWith(item);
                        }
                    }
                }

                if (playerStatus == PlayerStatus.IDLE)
                {
                    manaIdleRegen = 3;
                    manaIdleRegenTime = 3;
                }
                else
                {
                    manaIdleRegen = 1;
                    manaIdleRegenTime = 1;
                }

                //After attack x time mana will regeneration
                if (manaRegenTime > 0)
                {
                    manaRegenTime -= (float)gameTime.ElapsedGameTime.TotalSeconds * manaIdleRegenTime;
                }
                else if (manaRegenTime <= 0 && mana < maxMana)
                {
                    manaRegenTime = 0;
                    mana += (float)gameTime.ElapsedGameTime.TotalSeconds * (manaRegenAmount + manaIdleRegen);
                }

                if (bulletList.Count > 0)
                {
                    foreach (PlayerBullet bullet in bulletList)
                    {
                        //Do damge to every enemy that contact with lionBody
                        if (bullet.bulletBody != null && IsContact(bullet.bulletBody, "Enemy"))
                        {
                            bullet.bulletStatus = PlayerBullet.BulletStatus.BULLETDEAD;
                            if (enemyContract.Count > 0)
                            {
                                foreach (Enemy enemy in enemyContract)
                                {
                                    if (bullet.bulletBody.UserData.Equals("Bullet"))
                                    {
                                        enemy.takeDMG(1, "Bullet");
                                        break;
                                    }
                                    else if (bullet.bulletBody.UserData.Equals("Croc"))
                                    {
                                        enemy.takeDMG(crocDmg, "Croc");
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }

            }

            //player direction
            switch (charDirection)
            {
                case SpriteEffects.None:
                    playerDirectionInt = -1;
                    break;
                case SpriteEffects.FlipHorizontally:
                    playerDirectionInt = 1;
                    break;
            }

            //all animation
            //if step on dead block
            if ((IsContact(hitBox, "Dead") || (hitCooldown <= 1.7 && touchGround && health == 0)) && playerDead == false)
            {
                isAlive = false;
                playerStatus = PlayerStatus.DEAD;
                playerDead = true;
            }

            //if dead animation animationEnd
            animationEnd = playerAnimation.GetAnimationDead();
            if (animationEnd)
            {
                playerStatus = PlayerStatus.END;
            }

            playerAnimation.Update(gameTime, playerStatus);
            playerSkillAnimation.Update(gameTime, position, charDirection);

        }

        public void SetSpawn(Rectangle startRect)
        {
            hitBox.Position = ConvertUnits.ToSimUnits(new Vector2(startRect.X, startRect.Y - 1));
        }

        public void Action()
        {
            currentKeyState = Keyboard.GetState();

            if (isAlive && health > 0)
            {
                //check if player still on ground
                if (touchGround)
                {
                    playerStatus = PlayerStatus.IDLE;
                }
                Falling();
                Walking();
                Jump();
                Attack();
                Dash();
                Skill();
            }

            oldKeyState = currentKeyState;
        }

        private void Walking()
        {
            if (currentKeyState.IsKeyDown(Keys.Left))
            {
                hitBox.ApplyForce(new Vector2(-hitBox.Mass * speed, 0));
                charDirection = SpriteEffects.None;

                //check if player still on ground
                if (touchGround)
                {
                    playerStatus = PlayerStatus.RUN;
                }
            }
            if (currentKeyState.IsKeyDown(Keys.Right))
            {
                hitBox.ApplyForce(new Vector2(hitBox.Mass * speed, 0));
                charDirection = SpriteEffects.FlipHorizontally;

                //check if player still on ground
                if (touchGround)
                {
                    playerStatus = PlayerStatus.RUN;
                }
            }
        }
        
        private void Jump()
        {
            if (currentKeyState.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space) && !wasJumped)
            {

                //check if player still on ground
                if (touchGround)
                {
                    playerStatus = PlayerStatus.JUMP;
                    jumpSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
                }
                else
                {
                    hitBox.LinearVelocity = new Vector2(hitBox.LinearVelocity.X, 0f);
                    playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Bird");
                    jumpSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
                    wasJumped = true;
                }

                hitBox.ApplyLinearImpulse(new Vector2(0, -hitBox.Mass * jumpHigh));
            }

            if (wasJumped && touchGround)
            {
                wasJumped = false;
            }

        }
       
        public void Attack()
        {
            //Attack animation
            attackDelay = (int)gameTime.TotalGameTime.TotalMilliseconds - attackTimeDelay;

            //Normal Shoot left or right
            if (currentKeyState.IsKeyDown(Keys.X) && currentKeyState.IsKeyUp(Keys.Up) && oldKeyState.IsKeyUp(Keys.X) && attackDelay > attackMaxTime && mana - attackMana > 0)
            {
                isShootup = false;
                PlayerBullet bullet = new PlayerBullet(bulletTexture, hitBox.Position + new Vector2(0.43f * playerDirectionInt, -0.12f), this, charDirection, isShootup)
                {
                    bulletSpeed = 400,
                    bulletSizeX = 40,
                    bulletSizeY = 9,
                    bulletDistance = 10,
                };
                playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Shoot");
                bullet.CreateBullet(isCroc, "Bullet");
                bulletList.Add(bullet);
                Player.isAttack = true;
                attackAnimationTime = 0.3f;//Update Animation
                attackTimeDelay = (int)gameTime.TotalGameTime.TotalMilliseconds; //Delay before shoot again
                manaRegenTime = manaMaxRegenTime; //Delay before Regen Mana
                bulletList[bulletList.Count - 1].Shoot();
                mana -= attackMana;

                shootSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
            }
            //Shoot Up
            else if (currentKeyState.IsKeyDown(Keys.X) && currentKeyState.IsKeyDown(Keys.Up) && oldKeyState.IsKeyUp(Keys.X) && attackDelay > attackMaxTime && mana - attackMana > 0)
            {
                isShootup = true;

                PlayerBullet bullet = new PlayerBullet(bulletTexture, hitBox.Position + new Vector2(0, -1f), this, charDirection, isShootup)
                {

                    bulletSpeed = 400,
                    bulletSizeX = 40,
                    bulletSizeY = 9,
                    bulletDistance = 10,
                };
                bullet.CreateBullet(false, "Bullet");
                bulletList.Add(bullet);
                Player.isAttack = true;
                attackAnimationTime = 0.3f;
                attackTimeDelay = (int)gameTime.TotalGameTime.TotalMilliseconds;
                manaRegenTime = manaMaxRegenTime;
                bulletList[bulletList.Count - 1].Shoot();
                mana -= attackMana;

                shootSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
            }

            if (attackAnimationTime > 0)
            {
                attackAnimationTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (isShootup)
                {
                    switch (playerStatus)
                    {
                        case PlayerStatus.IDLE:
                            playerStatus = PlayerStatus.SHOOT_UP;
                            break;
                        case PlayerStatus.RUN:
                            playerStatus = PlayerStatus.SHOOT_UP_RUN;
                            break;
                    }
                }
                else
                {
                    switch (playerStatus)
                    {
                        case PlayerStatus.IDLE:
                            playerStatus = PlayerStatus.SHOOT;

                            break;
                        case PlayerStatus.RUN:
                            playerStatus = PlayerStatus.SHOOT_RUN;
                            break;
                    }
                }

            }

            if (bulletList.Count == 0)
            {
                Player.isAttack = false;
            }

            else if (bulletList.Count > 0)
            {
                foreach (PlayerBullet bullet in bulletList)
                {
                    bullet.Update(gameTime);
                    if (bullet.IsOutRange())
                    {
                        if (bullet.bulletStatus != PlayerBullet.BulletStatus.BULLETEND)
                        {
                            bulletList.Remove(bullet);
                            bullet.bulletStatus = PlayerBullet.BulletStatus.BULLETDEAD;
                            break;
                        }
                    }
                    if (bullet.IsContact(false))
                    {
                        bullet.bulletStatus = PlayerBullet.BulletStatus.BULLETEND;
                    }
                    //if animation end
                    if (bullet.bulletStatus == PlayerBullet.BulletStatus.BULLETEND)
                    {
                        bulletList.Remove(bullet);
                        enemyContract.Clear();
                        break;
                    }
                }
            }
        }
        
        public void Skill()
        {
            if (currentKeyState.IsKeyDown(Keys.Z) && currentKeyState.IsKeyUp(Keys.Down) && currentKeyState.IsKeyUp(Keys.Up) && !press && skill1Cooldown <= 0)
            {
                while (health < Player.maxHealth && healCount < 2)
                {
                    RegenSkill();
                    healCount++;
                }
                healCount = 0;
            }

            if (Singleton.Instance.stageunlock > 1)
            {
                CrocodileSkill();
            }

            if (Singleton.Instance.stageunlock > 2)
            {
                LionSkill();
            }

            if (press)
            {
                startCool = true;
            }

            if (startCool)
            {

                if (skill1Cooldown > 0)
                {
                    skill1Cooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (skill2Cooldown > 0)
                {
                    skill2Cooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (skill3Cooldown > 0)
                {
                    skill3Cooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (skill1Cooldown <= 0 && skill2Cooldown <= 0 && skill3Cooldown <= 0)
                {
                    startCool = false;
                }
            }

            if (currentKeyState.IsKeyUp(Keys.Z) && press)
            {
                press = false;
            }
        }

        public void Falling()
        {
            Vector2 velocity = hitBox.LinearVelocity;
            if (!touchGround && (int)velocity.Y > 0)
            {
                playerStatus = PlayerStatus.FALLING;
            }
            else if (!touchGround && (int)velocity.Y < 0)
            {
                playerStatus = PlayerStatus.JUMP;
            }
        }
       
        public void Dash()
        {
            if (currentKeyState.IsKeyDown(Keys.C) && oldKeyState.IsKeyUp(Keys.C) && dashCooldown <= 0 && mana - 10 >= 0)
            {
                playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Dash");
                dashAnimationTime = 0.3f;
                dashTime = (int)gameTime.TotalGameTime.TotalMilliseconds;

                switch (charDirection)
                {
                    case SpriteEffects.None:
                        hitBox.ApplyLinearImpulse(new Vector2(-hitBox.Mass * (speed - 2), 0));

                        break;
                    case SpriteEffects.FlipHorizontally:
                        hitBox.ApplyLinearImpulse(new Vector2(hitBox.Mass * (speed - 2), 0));

                        break;
                }

                mana -= 10;
                manaRegenTime = manaMaxRegenTime;
                hitCooldown = 0.5f;
                dashCooldown = dashMaxCooldown;

                dashSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
            }

            if (dashAnimationTime > 0)
            {
                dashAnimationTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                playerStatus = PlayerStatus.DASH;
                foreach (Body item in Singleton.Instance.world.BodyList)
                {
                    if (item.UserData != null && (item.UserData.Equals("Enemy") || item.UserData.Equals("SkillBoss")))
                    {
                        hitBox.IgnoreCollisionWith(item);
                    }
                }
            }

            if (dashCooldown > 0)
            {
                dashCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void RegenSkill()
        {
            skill1Cooldown = skill1MaxCooldown;
            health++;
            playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Heal");
            press = true;

            healSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
        }

        public void LionSkill()
        {
            if (currentKeyState.IsKeyDown(Keys.Z) && currentKeyState.IsKeyDown(Keys.Up) && !press && skill3Cooldown <= 0 && Singleton.Instance.stageunlock > 2 && mana - 20 >= 0)
            {
                lionBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(500), ConvertUnits.ToSimUnits(200), 0, hitBox.Position + new Vector2(0, textureHeight / 2), 0, BodyType.Static, "Lion");
                lionBody.IgnoreCollisionWith(hitBox);

                lionBody.IsSensor = true;

                skill3Cooldown = skill3MaxCooldown;
                skill3Time = 1;
                press = true;
                manaRegenTime = manaMaxRegenTime;
                mana -= 20;
                playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Lion");

                roarSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
            }

            //Time Active for 1 sec
            if (lionBody != null && skill3Time > 0)
            {

                lionBody.Position = hitBox.Position;
                skill3Time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (lionBody != null)
            {
                enemyContract.Clear();
                lionBody.Dispose();
                lionBody = null;
            }

            //Do damge to every enemy that contact with lionBody
            if (lionBody != null)

                if (lionBody != null && skill3Active <= 0 && IsContact(lionBody, "Enemy"))
                {
                    if (enemyContract != null)
                    {
                        foreach (Enemy enemy in enemyContract)
                        {
                            enemy.takeDMG(7, "Lion");
                        }
                    }
                }
        }

        public void CrocodileSkill()
        {
            if (currentKeyState.IsKeyDown(Keys.Z) && currentKeyState.IsKeyDown(Keys.Down) && !press && skill2Cooldown <= 0 && Singleton.Instance.stageunlock > 1)
            {
                press = true;
                skill2Cooldown = skill2MaxCooldown;
                isCroc = true;
                playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Croc");
                manaRegenTime = manaMaxRegenTime;
                mana -= 10;

                crocBusterSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
            }

            if (skill2Cooldown > 0 && isCroc)
            {
                if (playerSkillAnimation.curStatus == PlayerSkillAnimation.SymbolStatus.SYMBOLEND && skill2Cooldown > 0)
                {
                    PlayerBullet bullet = new PlayerBullet(bulletTexture, hitBox.Position + new Vector2(0.43f * playerDirectionInt, -0.12f), this, charDirection, false)
                    {
                        bulletSpeed = 600,
                        bulletSizeX = 52,
                        bulletSizeY = 39,
                        bulletDistance = 10,
                    };
                    bullet.CreateBullet(isCroc, "Croc");
                    bulletList.Add(bullet);
                    playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Shoot");
                    Player.isAttack = true;
                    attackAnimationTime = 0.3f;
                    attackTimeDelay = (int)gameTime.TotalGameTime.TotalMilliseconds;
                    bulletList[bulletList.Count - 1].Shoot();
                    isCroc = false;
                }
            }
        }

        public void GotHit(KnockbackStatus knockback)
        {
            if (hitCooldown <= 0)
            {
                if (health > 0)
                {
                    health--;

                    getHitSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
                }

                switch (knockback)
                {
                    case KnockbackStatus.RIGHT:
                        hitBox.ApplyLinearImpulse(new Vector2(hitBox.Mass * (speed - 10), -hitBox.Mass * (jumpHigh - 6)));
                        break;
                    case KnockbackStatus.LEFT:
                        hitBox.ApplyLinearImpulse(new Vector2(-hitBox.Mass * (speed - 10), -hitBox.Mass * (jumpHigh - 6)));
                        break;
                }
                hitCooldown = 1.3f;

            }
            else
            {
                foreach (Body item in Singleton.Instance.world.BodyList)
                {
                    if (item.UserData.Equals("Enemy") || item.UserData.Equals("SkillBoss") || item.UserData.Equals("EnemyBullet"))
                    {
                        enemyContract.Clear();
                        hitBox.IgnoreCollisionWith(item);
                    }
                }
            }


        }

        public bool IsContact(Body box, string contact)
        {
            ContactEdge contactEdge = box.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                Body fixtureA = contactEdge.Contact.FixtureA.Body;
                Body fixtureB = contactEdge.Contact.FixtureB.Body;

                bool fixtureA_Check = fixtureA.UserData != null && fixtureA.UserData.Equals(contact);
                bool fixtureB_Check = fixtureB.UserData != null && fixtureB.UserData.Equals(contact);

                // Check if the contact fixture is the ground
                if (contactFixture.IsTouching && (fixtureA_Check || fixtureB_Check))
                {
                    //if Contact thing in parameter it will return True
                    foreach (Enemy item in Singleton.Instance.enemiesInWorld)
                    {
                        //if Doesn't use skill lion now it will not process in foreach
                        if (lionBody == null && !box.UserData.Equals("Croc") && !box.UserData.Equals("Bullet"))
                        {
                            break;
                        }

                        if (item != null && (item.enemyHitBox.BodyId == fixtureA.BodyId || item.enemyHitBox.BodyId == fixtureB.BodyId))
                        {
                            if (!enemyContract.Contains(item))
                            {
                                enemyContract.Add(item);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    foreach (Body body in Singleton.Instance.world.BodyList)
                    {
                        if (body.UserData == null || (!body.UserData.Equals("Enemy") && !body.UserData.Equals("SkillBoss")) || !(body.BodyId == fixtureA.BodyId || body.BodyId == fixtureB.BodyId))
                        {
                            continue;
                        }

                        if (body.Position.X - box.Position.X > 0)
                        {
                            knockbackStatus = KnockbackStatus.LEFT;
                        }
                        else
                        {
                            knockbackStatus = KnockbackStatus.RIGHT;
                        }
                    }
                    return true;
                }

                contactEdge = contactEdge.Next;
            }
            return false;
        }

        public bool IsGround()
        {
            ContactEdge contactEdge = hitBox.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                Body fixtureA = contactEdge.Contact.FixtureA.Body;
                Body fixtureB = contactEdge.Contact.FixtureB.Body;

                bool fixtureACheck = fixtureA.UserData != null && fixtureA.UserData.Equals("Ground");
                bool fixtureBCheck = fixtureB.UserData != null && fixtureB.UserData.Equals("Ground");

                bool standOnPlatformA = fixtureA.UserData != null && fixtureA.UserData.Equals("Platform");
                bool standOnPlatformB = fixtureA.UserData != null && fixtureB.UserData.Equals("Platform");

                // Check if the contact fixture is the ground
                if (contactFixture.IsTouching && (fixtureACheck || fixtureBCheck || standOnPlatformA || standOnPlatformB))
                {
                    Vector2 normal = contactFixture.Manifold.LocalNormal;
                    if ((standOnPlatformA || standOnPlatformB) && normal.Y < 0f)
                    {
                        return true;
                    }
                    if (normal.Y > 0f)
                    {
                        return true;
                    }
                    // The character is on the ground

                }
                contactEdge = contactEdge.Next;
            }
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw player
            if (!animationEnd)
            {
                //draw player skill
                if (isAlive)
                {
                    playerSkillAnimation.Draw(spriteBatch);
                }
                playerAnimation.Draw(spriteBatch, playerOrigin, charDirection, ConvertUnits.ToDisplayUnits(position));
            }

            base.Draw(spriteBatch);
        }
    }
}
