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

namespace ScifiDruid.GameObjects
{
    public class Player : _GameObject
    {
        private Texture2D texture;
        private Texture2D bulletTexture;

        private Rectangle characterDestRec;
        private Rectangle characterSouceRec;

        private KeyboardState currentKeyState;
        private KeyboardState oldKeyState;

        public Body hitBox;

        //player status
        public PlayerStatus playerStatus;

        private bool touchGround;

        private int jumpCount = 0;
        private bool wasJumped;

        private int jumpTime;
        private int jumpDelay;

        private int attackTimeDelay;
        private int attackDelay;
        private int attackMaxTime;

        private float attackTime;

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

        private float attackAnimationTime;

        //Cooldown time make to static for change in shop and apply to all Stage
        public static int skill1CoolTime;
        public static int skill2CoolTime;
        public static int skill3CoolTime;
        public static int dashCoolTime;

        public float hitCooldown;

        private Vector2 playerOrigin;

        private Vector2 bulletPosition;

        public static bool isAttack = false;
        
        private bool animationEnd;

        private bool press = false;

        private bool startCool = false;

        public int jumpHigh;

        private SpriteEffects bulletDirection;
        public List<Bullet> bulletList;

        private GameTime gameTime;

        //player real size for hitbox
        private int textureWidth, textureHeight;

        //animation
        private PlayerAnimation playerAnimation;
        //check if animation animationEnd or not

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

        public Player(Texture2D texture ,Texture2D bulletTexture) : base(texture)
        {
            this.texture = texture;
            this.bulletTexture = bulletTexture;

            
        }

        public void Initial(Rectangle startRect)
        {
            textureWidth = (int)size.X;
            textureHeight = (int)size.Y;

            playerAnimation = new PlayerAnimation(this.texture, new Vector2(startRect.X, startRect.Y));

            bulletList = new List<Bullet>();

            hitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 1f, ConvertUnits.ToSimUnits(new Vector2(startRect.X, startRect.Y - 1)), 0, BodyType.Dynamic);
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
                skill3CoolTime = 60;
            }

            if (dashCoolTime == 0)
            {
                dashCoolTime = 5;
            }

            base.Initial();
        }



        public override void Update(GameTime gameTime)
        {
            position = hitBox.Position;
            this.gameTime = gameTime;
            //check touch ground condition
            if (IsGround())
            {
                touchGround = true;
            }
            else
            {
                touchGround = false;
            }

            //all animation
            //if step on dead block
            if (IsContact("dead","B") || (hitCooldown <= 1.7 && touchGround && Player.health == 0 ))
            {
                isAlive = false;
                playerStatus = PlayerStatus.DEAD;
            }

            if (IsContact("Enemy","B"))
            {
                GotHit();
            }
            else if (hitCooldown > 0)
            {
                hitCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            //if dead animation animationEnd
            animationEnd = playerAnimation.GetAnimationDead();
            if (animationEnd)
            {
                playerStatus = PlayerStatus.END;
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

            playerAnimation.Update(gameTime, playerStatus);
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

                if (currentKeyState.IsKeyDown(Keys.K) && oldKeyState.IsKeyUp(Keys.K))
                {
                    GotHit();
                }
            }
            
            oldKeyState = currentKeyState;
        }

        private void Walking()
        {
            if (currentKeyState.IsKeyDown(Keys.Left))
            {
                hitBox.ApplyForce(new Vector2(-100 * speed, 0));
                charDirection = SpriteEffects.None;

                //check if player still on ground
                if (touchGround)
                {
                    playerStatus = PlayerStatus.RUN;
                }
            }
            if (currentKeyState.IsKeyDown(Keys.Right))
            {
                hitBox.ApplyForce(new Vector2(100 * speed, 0));
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
                }
                else 
                {
                    hitBox.LinearVelocity = new Vector2(hitBox.LinearVelocity.X,0f);
                    wasJumped = true;
                }

                hitBox.ApplyLinearImpulse(new Vector2(0, -jumpHigh));
            }

            if (wasJumped && touchGround)
            {
                wasJumped = false;
            }

        }
        public void Attack()
        {
            attackDelay = (int)gameTime.TotalGameTime.TotalMilliseconds - attackTimeDelay;

            if (currentKeyState.IsKeyDown(Keys.X) && oldKeyState.IsKeyUp(Keys.X) && attackDelay > attackMaxTime && Player.mana > 0)
            {
                bulletList.Add(new Bullet(bulletTexture, hitBox.Position + new Vector2(0,-0.12f),hitBox,charDirection));
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

            
            if (bulletList.Count == 0)
            {
                Player.isAttack = false;
            }

            else if (bulletList.Count > 0)
            {
                foreach (Bullet bullet in bulletList)
                {
                    bullet.Update(gameTime);
                    if (bullet.IsContact() || bullet.IsOutRange())
                    {
                        if (bullet.bulletStatus != Bullet.BulletStatus.BULLETEND)
                        {
                            bullet.bulletStatus = Bullet.BulletStatus.BULLETDEAD;
                        }
                    }
                    //if animation end
                    if (bullet.bulletStatus == Bullet.BulletStatus.BULLETEND)
                    {
                        bulletList.Remove(bullet);
                        break;
                    }
                }
            }
        }
        public void Skill()
        {
            if (currentKeyState.IsKeyDown(Keys.Z) && currentKeyState.IsKeyDown(Keys.Up) && !press && skill1Cooldown <= 0)
            {
                if (Player.health < Player.maxHealth)
                {
                    RegenSkill();
                }
            }
            if (currentKeyState.IsKeyDown(Keys.Z) && currentKeyState.IsKeyDown(Keys.Down) && !press && skill2Cooldown <= 0)
            {
                CrocodileSkill();
            }
            if (currentKeyState.IsKeyDown(Keys.Z) && currentKeyState.IsKeyUp(Keys.Down) && currentKeyState.IsKeyUp(Keys.Up) && !press)
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
                switch (charDirection)
                {
                    case SpriteEffects.None:
                        hitBox.ApplyLinearImpulse(new Vector2(-5, 0));

                        break;
                    case SpriteEffects.FlipHorizontally:
                        hitBox.ApplyLinearImpulse(new Vector2(5, 0));

                        break;
                }

                dashCooldown = dashCoolTime;
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
            
            press = true;
        }

        public void LionSkill()
        {
            skill3Cooldown = skill3CoolTime;
            
            press = true;
        }

        public void CrocodileSkill()
        {
            skill2Cooldown = skill2CoolTime;
            press = true;
        }

        public void GotHit()
        {
            if (hitCooldown <= 0)
            {
                Player.health--;
                switch (charDirection)
                {
                    case SpriteEffects.None:
                        hitBox.ApplyLinearImpulse(new Vector2(10, -jumpHigh));

                        break;
                    case SpriteEffects.FlipHorizontally:
                        hitBox.ApplyLinearImpulse(new Vector2(-10, -jumpHigh));

                        break;
                }
                hitCooldown = 2;
            }
            
        }

        public bool IsContact(String contact,String fixture)
        {
            ContactEdge contactEdge = hitBox.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;
                switch (fixture)
                {
                    case "A":
                        // Check if the contact fixture is the ground
                        if (contactFixture.IsTouching && contactEdge.Contact.FixtureA.Body.UserData != null && contactEdge.Contact.FixtureA.Body.UserData.Equals(contact))
                        {
                            //if Contact thing in parameter it will return True
                            return true;
                        }
                        break; 
                    case "B":
                        // Check if the contact fixture is the ground
                        if (contactFixture.IsTouching && contactEdge.Contact.FixtureB.Body.UserData != null && contactEdge.Contact.FixtureB.Body.UserData.Equals(contact))
                        {
                            //if Contact thing in parameter it will return True
                            return true;
                        }
                        break;
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
                if (contactFixture.IsTouching && contactEdge.Contact.FixtureA.Body.UserData != null && contactEdge.Contact.FixtureA.Body.UserData.Equals("ground"))
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
                playerAnimation.Draw(spriteBatch, playerOrigin, charDirection, ConvertUnits.ToDisplayUnits(position));
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
