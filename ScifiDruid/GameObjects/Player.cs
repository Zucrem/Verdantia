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

        private Body hitBox;
        private Body _bulletBody;

        //player status
        public PlayerStatus playerStatus;

        private bool touchGround;

        private int jumpCount = 0;

        private int jumpTime;
        private int jumpDelay;

        private int skillTime;
        private int skillDelay;

        private int dashTime;
        private int dashDelay;

        private int attackTime;
        private int attackDelay;
        private int attackMaxTime;

        public static int health;
        public static int mana;
        public static int money;

        private float skill1Cooldown;
        private float skill2Cooldown;

        private Vector2 playerOrigin;

        private Vector2 bulletPosition;

        public bool isAttack = false;

        private bool animationEnd;

        private bool press = false;

        private SpriteEffects bulletDirection;
        public List<Bullet> bulletList;

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
            this.textureWidth = 46;
            this.textureHeight = 94;
            this.bulletTexture = bulletTexture;

        }

        public void Initial(Rectangle startRect)
        {
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

            base.Initial();
        }



        public override void Update(GameTime gameTime)
        {
            position = hitBox.Position;

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

            if (IsStepDeadBlock())
            {
                isAlive = false;
                playerStatus = PlayerStatus.DEAD;
            }

            //if dead animation animationEnd
            animationEnd = playerAnimation.GetAnimationDead();
            if (animationEnd)
            {
                playerStatus = PlayerStatus.END;
            }

            playerAnimation.Update(gameTime, playerStatus);
        }

        public void Action(GameTime gameTime)
        {
            currentKeyState = Keyboard.GetState();

            if (isAlive)
            {
                //check if player still on ground
                if (touchGround)
                {
                    playerStatus = PlayerStatus.IDLE;
                }
                Falling();
                Walking();
                Jump();
                Attack(gameTime);
                Dash();
                Skill();
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
            if (currentKeyState.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
            {
                hitBox.ApplyLinearImpulse(new Vector2(0, -5));

                //check if player still on ground
                if (touchGround)
                {
                    playerStatus = PlayerStatus.JUMP;
                }

            }
        }


        public void Attack(GameTime gameTime)
        {
            attackDelay = (int)gameTime.TotalGameTime.TotalMilliseconds - attackTime;

            if (currentKeyState.IsKeyDown(Keys.X) && oldKeyState.IsKeyUp(Keys.X) && attackDelay > attackMaxTime && Player.mana > 0)
            {
                bulletList.Add(new Bullet(bulletTexture, hitBox.Position,hitBox,charDirection));
                isAttack = true;
                attackTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
                bulletList[bulletList.Count - 1].Shoot(gameTime);
                Player.mana -= 5;
            }


            
            if (bulletList.Count == 0)
            {
                isAttack = false;
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
            if (currentKeyState.IsKeyDown(Keys.Z) && currentKeyState.IsKeyDown(Keys.Up) && !press)
            {
                Player.health++;
                press = true;
            }
            if (currentKeyState.IsKeyDown(Keys.Z) && currentKeyState.IsKeyDown(Keys.Down) && !press)
            {
                Player.health--;
                press = true;
            }
            if (currentKeyState.IsKeyDown(Keys.Z) && !press)
            {
                Debug.WriteLine("CC");
                press = true;
            }   


            if (currentKeyState.IsKeyUp(Keys.Z) && press)
            {
                Debug.WriteLine("DD");
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
            if (dashDelay > 200 && Keyboard.GetState().IsKeyDown(Keys.C))
            {

            }
        }

        public bool IsGround()
        {
            ContactEdge contactEdge = hitBox.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                // Check if the contact fixture is the ground
                if (contactFixture.IsTouching)
                {
                    Vector2 normal = contactFixture.Manifold.LocalNormal;
                    if (normal.Y < 0f || normal.Y > 0f)
                    {
                        return true;
                    }
                    // The character is on the ground

                }
                contactEdge = contactEdge.Next;
            }
            return false;
        }

        public bool IsStepDeadBlock()
        {
            ContactEdge contactEdge = hitBox.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;
                // Check if the contact fixture is the dead block
                if (contactFixture.IsTouching && contactEdge.Contact.FixtureA.Body.UserData != null && contactEdge.Contact.FixtureA.Body.UserData.Equals("dead"))
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
