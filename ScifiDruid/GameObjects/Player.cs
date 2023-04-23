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

        public int jumpCount = 0;
        
        private bool isJumpPress = false;
        private float jumpPosition;
        private bool gravityActive = false;
        private Vector2 firstPosition;
        private GameTime gameTime;

        //player status
        private PlayerStatus playerStatus;

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
        public List<Bullet> bullet;

        //player real size for hitbox
        public int textureWidth, textureHeight;

        //animation
        public PlayerAnimation playerAnimation;

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
            DEAD
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
            bullet = new List<Bullet>();
            //hitBox = BodyFactory.CreateRectangle(Singleton.Instance.world,ConvertUnits.ToSimUnits(textureWidth),ConvertUnits.ToSimUnits(textureHeight),1f,ConvertUnits.ToSimUnits(new Vector2(500,100)),0,BodyType.Dynamic);
            hitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 1f, ConvertUnits.ToSimUnits(new Vector2(startRect.X, startRect.Y - 1)), 0, BodyType.Dynamic);
            hitBox.FixedRotation = true;
            hitBox.Friction = 1.0f;
            hitBox.AngularDamping = 2.0f;
            hitBox.LinearDamping = 2.0f;

            /*if (isGround())
            {
                touchGround = isGround();
            }*/

            playerOrigin = new Vector2(textureWidth / 2, textureHeight / 2);

            playerStatus = PlayerStatus.IDLE;
            playerAnimation.Initialize();

            base.Initial();
        }



        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = hitBox.Position;
            //characterDestRec.X = (int)hitBox.Position.X;
            //characterDestRec.Y = (int)hitBox.Position.Y;

            //check touch ground condition
            /*if (isGround())
            {
                touchGround = isGround();
            }*/

            //isGround();
            Debug.WriteLine(isGround());

            playerAnimation.Update(gameTime, playerStatus);
            Action();
        }

        public void Action()
        {
            currentKeyState = Keyboard.GetState();

            playerStatus = PlayerStatus.IDLE;
            /*if (touchGround)
            {
                playerStatus = PlayerStatus.IDLE;
            }*/
            Walking();
            Jump();
            Attack();
            Dash();
            Skill();

            oldKeyState = currentKeyState;
        }

        private void Walking()
        {
            if (currentKeyState.IsKeyDown(Keys.Left))
            {
                hitBox.ApplyForce(new Vector2(-100 * speed, 0));
                charDirection = SpriteEffects.None;
                playerStatus = PlayerStatus.RUN;
                /*if (isGround())
                {
                    playerStatus = PlayerStatus.RUN;
                }*/
            } 
            /*else if (oldKeyState.IsKeyDown(Keys.Left) && currentKeyState.IsKeyUp(Keys.Left))
            {
                playerAnimation.frames = 0;
            }*/
            if (currentKeyState.IsKeyDown(Keys.Right))
            {
                hitBox.ApplyForce(new Vector2(100 * speed, 0));
                charDirection = SpriteEffects.FlipHorizontally;
                playerStatus = PlayerStatus.RUN;
                /*if (isGround())
                {
                    playerStatus = PlayerStatus.RUN;
                }*/
            }


        }
        private void Jump()
        {
            currentKeyState = Keyboard.GetState();

            if (currentKeyState.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
            {
                hitBox.ApplyLinearImpulse(new Vector2(0, -5));

                playerStatus = PlayerStatus.JUMP;
                /*if (isGround())
                {
                    playerStatus = PlayerStatus.JUMP;
                }*/

                //jumpCount++;
            }
            //Debug.WriteLine(hitBox.LinearVelocity);

            /*
            if (hitBox.LinearVelocity.Equals(Vector2.Zero))
            {
                jumpCount = 0;
            }*/

            oldKeyState = currentKeyState;
            /*jumpTime = (int)gameTime.TotalGameTime.TotalMilliseconds - jumpDelay;

            if (jumpCount < 2 && jumpTime > 200 && (Keyboard.GetState().IsKeyDown(Keys.Space)))
            {
                isJumpPress = true;
                gravityActive = false;
                if (jumpCount == 0)
                {
                    firstPosition = position;
                }
                jumpDelay = (int)gameTime.TotalGameTime.TotalMilliseconds;
                jumpPosition = position.Y - 100f;
                jumpCount++;
            }

            if (isJumpPress && !gravityActive)
            {
                position.Y -= 300f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (!gravityActive && jumpPosition >= position.Y)
            {
                gravityActive = true;
            }

            if (gravityActive)
            {
                position.Y += 300f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (gravityActive && firstPosition.Y <= position.Y)
            {
                isJumpPress = false;
                gravityActive = false;
                jumpCount = 0;
            }
            */
        }

        /*public void Attack()
        {
            if (elapsedMs > 200 && Keyboard.GetState().IsKeyDown(Keys.X))
            {
                isAttack = true;
                
                bulletPosition.Y = position.Y + 10;
                bulletDirection = charDirection;
                switch (bulletDirection)
                {
                    case SpriteEffects.None:
                        bulletPosition.X = position.X + 30;
                        break;
                    case SpriteEffects.FlipHorizontally:
                        bulletPosition.X = position.X - 30;
                        break;
                    case SpriteEffects.FlipVertically:
                        break;
                }
            }
            
            if (isAttack)
            {
                switch (bulletDirection)
                {
                    case SpriteEffects.None:
                        bulletPosition.X += 2;
                        break;
                    case SpriteEffects.FlipHorizontally:
                        bulletPosition.X -= 2;
                        break;
                }
            }
        }*/

        public void Attack()
        {
            attackTime = (int)gameTime.TotalGameTime.TotalMilliseconds - attackDelay;

            if (attackTime > 1000 && Keyboard.GetState().IsKeyDown(Keys.X))
            {
                isAttack = true;
                bullet.Add(new Bullet(bulletTexture, bulletPosition, charDirection));
                bullet[^1].Shoot(position, charDirection);
                attackDelay = (int)gameTime.TotalGameTime.TotalMilliseconds;
            }

            if (bullet.Count > 0)
            {
                foreach (Bullet bulletE in bullet)
                {
                    if (bulletE.bulletPosition.X > 1280 || bulletE.bulletPosition.X < 0)
                    {
                        bullet.Remove(bulletE);
                        break;
                    }
                }
            }

            if (isAttack)
            {
                foreach (Bullet bulletE in bullet)
                {
                    bulletE.Update();
                }
            }
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
        public void Dash()
        {
            if (dashDelay > 200 && Keyboard.GetState().IsKeyDown(Keys.C))
            {

            }
        }

        private bool isGround()
        {
            ContactEdge contactEdge = hitBox.ContactList;
            while (contactEdge != null)
            {
                Contact contact = contactEdge.Contact;

                // Check if the contact fixture is the ground
                if (contact.IsTouching && contactEdge.Contact.FixtureA.Body.UserData != null && contactEdge.Contact.FixtureA.Body.UserData.Equals("ground"))
                {
                    // Get the contact normal vector and check if it's approximately upwards
                    Vector2 normal = contact.Manifold.LocalNormal;
                    //Debug.WriteLine(normal.Y);
                    if (normal.Y < 0f)
                    {
                        //The character is standing on the ground
                        return true;
                    }
                }

                    contactEdge = contactEdge.Next;
            }

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            playerAnimation.Draw(spriteBatch, playerOrigin, charDirection, ConvertUnits.ToDisplayUnits(position));

            base.Draw(spriteBatch);
        }
    }
}
