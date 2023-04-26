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
        public List<Bullet> bulletList;

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
            bulletList = new List<Bullet>();
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
            //Debug.WriteLine(isGround());

            playerAnimation.Update(gameTime, playerStatus);
            Action(gameTime);
        }

        public void Action(GameTime gameTime)
        {
            currentKeyState = Keyboard.GetState();

            //playerStatus = PlayerStatus.IDLE;
            if (isGround())
            {
                playerStatus = PlayerStatus.IDLE;
            }
            Walking();
            Jump();
            Attack(gameTime);
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
            if (currentKeyState.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
            {
                hitBox.ApplyLinearImpulse(new Vector2(0, -5));

                playerStatus = PlayerStatus.JUMP;
            }
        }


        public void Attack(GameTime gameTime)
        {
            if (currentKeyState.IsKeyDown(Keys.K) && oldKeyState.IsKeyUp(Keys.K))
            {
                bulletList.Add(new Bullet(bulletTexture, hitBox.Position,hitBox,charDirection));
                isAttack = true;
                //if (_bulletBody != null)
                //{
                //    _bulletBody.Dispose();
                //}

                //switch (charDirection)
                //{
                //    case SpriteEffects.None:
                //        _bulletBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(54), ConvertUnits.ToSimUnits(54f), 0, _bulletPosition, 0, BodyType.Dynamic);
                //        _bulletBody.IsBullet = true;
                //        _bulletBody.IgnoreGravity = true;
                //        _bulletBody.IgnoreCollisionWith(_circleBody);
                //        _bulletBody.ApplyForce(new Vector2(-400, 0));
                //        break;
                //    case SpriteEffects.FlipHorizontally:
                //        _bulletBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(54), ConvertUnits.ToSimUnits(54f), 0, _bulletPosition, 0, BodyType.Dynamic);
                //        _bulletBody.IsBullet = true;
                //        _bulletBody.IgnoreGravity = true;
                //        _bulletBody.IgnoreCollisionWith(_circleBody);
                //        _bulletBody.ApplyForce(new Vector2(400, 0));
                //        break;
                //}
                //shoot = true;
            }

            if (isAttack)
            {
                foreach (Bullet bullet in bulletList)
                {
                    bullet.Shoot(gameTime);
                }
            }

            if (bulletList.Count == 0)
            {
                isAttack = false;
            }
            else if (bulletList.Count > 0)
            {
                foreach (Bullet bullet in bulletList)
                {
                    if (bullet.isContractEnemy())
                    {
                        bullet.BulletDispose();
                        bulletList.Remove(bullet);
                        break;
                    }
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

        public bool isGround()
        {
            ContactEdge contactEdge = hitBox.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;

                //Vector2 normal = contactFixture.Manifold.LocalNormal;
                //Debug.WriteLine(normal.Y);
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            playerAnimation.Draw(spriteBatch, playerOrigin, charDirection, ConvertUnits.ToDisplayUnits(position));

            base.Draw(spriteBatch);
        }
    }
}
