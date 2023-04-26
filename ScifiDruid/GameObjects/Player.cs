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

        public Rectangle characterDestRec;
        public Rectangle characterSouceRec;

        private KeyboardState currentKeyState;
        private KeyboardState oldKeyState;

        public Body hitBox;
        public Body _bulletBody;

        public int jumpCount = 0;
        
        private bool isJumpPress = false;
        private float jumpPosition;
        private bool gravityActive = false;
        private Vector2 firstPosition;
        private GameTime gameTime;

        //player status
        public PlayerStatus playerStatus;

        private bool touchGround;

        private int jumpTime;
        private int jumpDelay;

        private int skillTime;
        private int skillDelay;

        private int dashTime;
        private int dashDelay;

        private int attackTime;
        private int attackDelay;

        public Vector2 playerOrigin;

        public bool isAttack = false;
        public Vector2 bulletPosition;
        public SpriteEffects bulletDirection;
        public List<Bullet> bulletList;

        //player real size for hitbox
        public int textureWidth, textureHeight;

        //animation
        public PlayerAnimation playerAnimation;
        //check if animation end or not
        bool end;

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
            //characterSouceRec = new Rectangle(0, 0, sizeX, sizeY);

        }

        public void Initial(Rectangle startRect)
        {
            //ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);
            playerAnimation = new PlayerAnimation(this.texture, new Vector2(startRect.X, startRect.Y));

            //characterDestRec = rectangle;
            bulletList = new List<Bullet>();
            //hitBox = BodyFactory.CreateRectangle(Singleton.Instance.world,ConvertUnits.ToSimUnits(textureWidth),ConvertUnits.ToSimUnits(textureHeight),1f,ConvertUnits.ToSimUnits(new Vector2(500,100)),0,BodyType.Dynamic);
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

            base.Initial();
        }



        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = hitBox.Position;
            //characterDestRec.X = (int)hitBox.Position.X;
            //characterDestRec.Y = (int)hitBox.Position.Y;

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

            //if dead animation end
            end = playerAnimation.GetAnimationDead();
            if (end)
            {
                playerStatus = PlayerStatus.END;
            }

            //Debug.WriteLine("touch ground = " + touchGround);
            //Debug.WriteLine("Vel x,y = " + hitBox.LinearVelocity);


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
            /*else if (oldKeyState.IsKeyDown(Keys.Left) && currentKeyState.IsKeyUp(Keys.Left))
            {
                playerAnimation.frames = 0;
            }*/
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

                //jumpCount++;
            }
        }


        public void Attack(GameTime gameTime)
        {
            if (currentKeyState.IsKeyDown(Keys.K) && oldKeyState.IsKeyUp(Keys.K))
            {
                bulletList.Add(new Bullet(bulletTexture, hitBox.Position,hitBox,charDirection));
                isAttack = true;

                bulletList[bulletList.Count - 1].Shoot(gameTime);
            }

            if (bulletList.Count == 0)
            {
                isAttack = false;
            }
            else if (bulletList.Count > 0)
            {
                foreach (Bullet bullet in bulletList)
                {
                    if (bullet.isContact() || bullet.isOutRange())
                    {
                        bulletList.Remove(bullet);
                        break;
                    }
                }
            }

            Debug.WriteLine(Singleton.Instance.world.BodyList.Count);

        }

        public void Skill()
        {
            if (skillDelay > 200)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Z) && Keyboard.GetState().IsKeyDown(Keys.Up))
                {

                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Z) && Keyboard.GetState().IsKeyDown(Keys.Down))
                {

                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Z))
                {

                }
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
            if (!end)
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
