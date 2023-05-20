using ScifiDruid.Managers;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Xml;
using System.Collections;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Box2DNet.Dynamics.Contacts;
using System.Data;
using System.Reflection.Metadata;

namespace ScifiDruid.GameObjects
{
    public class Player : _GameObject
    {
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
        private int attackMaxTime;

        private float attackTime;
        private int dashTime;

        private bool isShootup;

        //static for change in shop and apply to all Stage
        public static int health;
        public static float mana;
        public static int money;
        public static int maxHealth;
        public static int maxMana;

        //Count cooldown of action
        private float skill1Cooldown;
        private float skill2Cooldown;
        private float skill3Cooldown;
        private float dashCooldown;

        private float skill3Time;
        private float skill3Active;

        //animation time for shoot and dash
        private float attackAnimationTime;
        private float dashAnimationTime;

        //Cooldown time make to static for change in shop and apply to all Stage
        public static int skill1CoolTime;
        public static int skill2CoolTime;
        public static int skill3CoolTime;
        public static int dashCoolTime;

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
            FONT,
            BACK
        }

        public Player(Texture2D texture, Texture2D bulletTexture, Texture2D lionSKill) : base(texture)
        {
            this.texture = texture;
            this.bulletTexture = bulletTexture;
            this.lionTexture = lionSKill;
        }

        public void Initial(Rectangle startRect)
        {

            playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position + new Vector2(0, 0), "Null");

            textureWidth = (int)size.X;
            textureHeight = (int)size.Y;

            playerAnimation = new PlayerAnimation(this.texture);
            bulletList = new List<PlayerBullet>();
            enemyContract = new List<Enemy>();

            hitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 1f, ConvertUnits.ToSimUnits(new Vector2(startRect.X, startRect.Y - 1)), 0, BodyType.Dynamic, "Player");
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

            attackMaxTime = 500;

            if (skill1CoolTime == 0)
            {
                skill1CoolTime = 5;
            }

            if (skill2CoolTime == 0)
            {
                skill2CoolTime = 60;
            }

            if (skill3CoolTime == 0)
            {
                skill3CoolTime = 10;
            }

            if (dashCoolTime == 0)
            {
                dashCoolTime = 5;
            }

            base.Initial();
            LoadContent();
        }

        public void LoadContent()
        {
            ContentManager content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
            //all sound effetc for player
            //playerDeadSound = content.Load<SoundEffect>("Sounds/Player/PlayerDead");
            //playerJumpSound = content.Load<SoundEffect>("Sounds/Player/PlayerJump");
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

                if ( (IsContact(hitBox, "Enemy") || IsContact(hitBox, "SkillBoss")) && playerStatus != PlayerStatus.DASH)
                {
                    GotHit();
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
                        if (item.UserData.Equals("Enemy") || item.UserData.Equals("SkillBoss"))
                        {
                            hitBox.RestoreCollisionWith(item);
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
            if ((IsContact(hitBox, "Dead") || (hitCooldown <= 1.7 && touchGround && Player.health == 0)) && playerDead == false)
            {
                isAlive = false;
                playerStatus = PlayerStatus.DEAD; 
                //playerDeadSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
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

        public void Action()
        {
            currentKeyState = Keyboard.GetState();

            if (isAlive && Player.health > 0)
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
                    //playerJumpSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
                }
                else
                {
                    hitBox.LinearVelocity = new Vector2(hitBox.LinearVelocity.X, 0f);
                    playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Bird");
                    //wasJumped = true;
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
            attackDelay = (int)gameTime.TotalGameTime.TotalMilliseconds - attackTimeDelay;

            //Normal Shoot left or right
            if (currentKeyState.IsKeyDown(Keys.X) && currentKeyState.IsKeyUp(Keys.Up) && oldKeyState.IsKeyUp(Keys.X) && attackDelay > attackMaxTime && Player.mana > 0)
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

                if (isCroc)
                {
                    bullet.bulletSpeed = 600;
                    bullet.bulletSizeX = 52;
                    bullet.bulletSizeY = 39;
                    bullet.bulletDistance = 10;
                    bullet.CreateBullet(isCroc);
                    isCroc = false;
                }
                else
                {
                    bullet.CreateBullet(isCroc);
                }

                bulletList.Add(bullet);
                //bulletList.Add(new PlayerBullet(bulletTexture, hitBox.Position + new Vector2(0.43f *  playerDirectionInt , -0.12f), this, charDirection , isCroc, isShootup));

                Player.isAttack = true;
                attackAnimationTime = 0.3f;
                attackTimeDelay = (int)gameTime.TotalGameTime.TotalMilliseconds;
                attackTime = 5;
                bulletList[bulletList.Count - 1].Shoot(gameTime);
                Player.mana -= 5;
            }
            //Shoot Up
            else if (currentKeyState.IsKeyDown(Keys.X) && currentKeyState.IsKeyDown(Keys.Up) && oldKeyState.IsKeyUp(Keys.X) && attackDelay > attackMaxTime && Player.mana > 0)
            {
                isShootup = true;

                PlayerBullet bullet = new PlayerBullet(bulletTexture, hitBox.Position + new Vector2(0, -1f), this, charDirection, isShootup)
                {

                    bulletSpeed = 400,
                    bulletSizeX = 40,
                    bulletSizeY = 9,
                    bulletDistance = 10,
                };
                bullet.CreateBullet(false);
                bulletList.Add(bullet);

                //bulletList.Add(new PlayerBullet(bulletTexture, hitBox.Position + new Vector2(0, -1f), this, charDirection, isShootup));

                Player.isAttack = true;
                attackAnimationTime = 0.3f;
                attackTimeDelay = (int)gameTime.TotalGameTime.TotalMilliseconds;
                attackTime = 5;
                bulletList[bulletList.Count - 1].Shoot(gameTime);
                Player.mana -= 5;
            }

            if (attackTime > 0)
            {
                attackTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (attackTime <= 0 && Player.mana < Player.maxMana)
            {
                attackTime = 0;
                Player.mana += (float)gameTime.ElapsedGameTime.TotalSeconds;
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
                            bullet.bulletStatus = PlayerBullet.BulletStatus.BULLETDEAD;
                        }
                    }
                    if (bullet.IsContact())
                    {
                        bullet.bulletStatus = PlayerBullet.BulletStatus.BULLETEND;
                    }
                    //if animation end
                    if (bullet.bulletStatus == PlayerBullet.BulletStatus.BULLETEND)
                    {
                        bulletList.Remove(bullet);
                        break;
                    }
                }
            }
        }
        public void Skill()
        {
            if (currentKeyState.IsKeyDown(Keys.Z) && currentKeyState.IsKeyUp(Keys.Down) && currentKeyState.IsKeyUp(Keys.Up) && !press && skill1Cooldown <= 0)
            {
                if (Player.health < Player.maxHealth)
                {
                    RegenSkill();
                }
            }

            CrocodileSkill();

            LionSkill();

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
            if (currentKeyState.IsKeyDown(Keys.C) && oldKeyState.IsKeyUp(Keys.C) && dashCooldown <= 0)
            {
                playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Dash");
                dashAnimationTime = 0.3f;
                dashTime = (int)gameTime.TotalGameTime.TotalMilliseconds;

                switch (charDirection)
                {
                    case SpriteEffects.None:
                        hitBox.ApplyLinearImpulse(new Vector2(-hitBox.Mass * (speed - 3), 0));

                        break;
                    case SpriteEffects.FlipHorizontally:
                        hitBox.ApplyLinearImpulse(new Vector2(hitBox.Mass * (speed - 3), 0));

                        break;
                }
                hitCooldown = 0.5f;
                dashCooldown = dashCoolTime;
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
            skill1Cooldown = skill1CoolTime;
            Player.health++;
            playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Heal");
            press = true;
        }

        public void LionSkill()
        {

            if (currentKeyState.IsKeyDown(Keys.Z) && currentKeyState.IsKeyDown(Keys.Up) && !press && skill3Cooldown <= 0)
            {
                lionBody = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(500), ConvertUnits.ToSimUnits(200), 0, hitBox.Position + new Vector2(0, textureHeight / 2), 0, BodyType.Static, "Lion");
                lionBody.IgnoreCollisionWith(hitBox);
                lionBody.IsSensor = true;

                skill3Cooldown = skill3CoolTime;
                skill3Time = 1;
                press = true;

                playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Lion");
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
            if (lionBody != null && skill3Active <= 0 && IsContact(lionBody, "Enemy"))
            {
                if (enemyContract != null)
                {
                    foreach (Enemy enemy in enemyContract)
                    {
                        enemy.takeDMG(4, "Lion");
                    }
                    //lionBody.Dispose();
                }
            }

        }

        public void CrocodileSkill()
        {
            if (currentKeyState.IsKeyDown(Keys.Z) && currentKeyState.IsKeyDown(Keys.Down) && !press && skill2Cooldown <= 0)
            {
                //isAlive = false;
                press = true;
                skill2Cooldown = skill2CoolTime;
                isCroc = true;
                playerSkillAnimation = new PlayerSkillAnimation(bulletTexture, position, "Croc");
            }

            if (!isAlive && skill2Cooldown > 0)
            {
                playerStatus = PlayerStatus.RUN;
                hitBox.ApplyForce(new Vector2(10, 0));
            }
        }

        public void GotHit()
        {
            if (hitCooldown <= 0)
            {
                if (Player.health > 0)
                {
                    Player.health--;
                }

                switch (knockbackStatus)
                {
                    case KnockbackStatus.FONT:
                        hitBox.ApplyLinearImpulse(new Vector2(hitBox.Mass * (speed - 6), -hitBox.Mass * jumpHigh));
                        break;
                    case KnockbackStatus.BACK:
                        hitBox.ApplyLinearImpulse(new Vector2(-hitBox.Mass * (speed - 6), -hitBox.Mass * jumpHigh));
                        break;
                }
                hitCooldown = 1f;

            }
            else
            {
                foreach (Body item in Singleton.Instance.world.BodyList)
                {
                    if (item.UserData.Equals("Enemy") || item.UserData.Equals("SkillBoss"))
                    {
                        enemyContract.Clear();
                        hitBox.IgnoreCollisionWith(item);
                    }
                }
            }


        }

        public bool IsContact(Body box, String contact)
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
                        if (lionBody == null)
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
                                Debug.WriteLine("SS");
                                return false;
                            }
                        }
                    }

                    foreach (Body body in Singleton.Instance.world.BodyList)
                    {
                        if (body.UserData == null || ( !body.UserData.Equals("Enemy") && !body.UserData.Equals("SkillBoss") ) || !(body.BodyId == fixtureA.BodyId || body.BodyId == fixtureB.BodyId))
                        {
                            continue;
                        }

                        if (body.Position.X - box.Position.X > 0)
                        {
                            knockbackStatus = KnockbackStatus.BACK;
                        }
                        else
                        {
                            knockbackStatus = KnockbackStatus.FONT;
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

                // Check if the contact fixture is the ground
                if (contactFixture.IsTouching && contactEdge.Contact.FixtureA.Body.UserData != null && contactEdge.Contact.FixtureA.Body.UserData.Equals("Ground"))
                {
                    Vector2 normal = contactFixture.Manifold.LocalNormal;
                    if (normal.Y < 0f)
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

            if (lionBody != null)
            {
                spriteBatch.Draw(lionTexture, ConvertUnits.ToDisplayUnits(lionBody.Position), new Rectangle(0, 0, (int)ConvertUnits.ToDisplayUnits(ConvertUnits.ToSimUnits(500)), (int)ConvertUnits.ToDisplayUnits(ConvertUnits.ToSimUnits(200))), Color.Black, 0, new Vector2(500 / 2, 200 / 2), 1, SpriteEffects.None, 0);
            }
            //if shoot
            /*if (_bulletBody != null && !_bulletBody.IsDisposed)
            {
                bullet.Draw(spriteBatch);
            }*/
            base.Draw(spriteBatch);
        }
    }
}
