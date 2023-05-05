using Box2DNet;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Timers;
using static ScifiDruid.GameObjects.Player;

namespace ScifiDruid.GameObjects
{
    public class Enemy : _GameObject
    {
        private Texture2D texture;    //enemy Texture (Animaiton)

        private Rectangle enemyDestRect;  //where postion
        private Rectangle enemySourceRec; //where read
        private Vector2 enemyOrigin;  //start draw enemy point

        public Body enemyHitBox;       // to check the hit of bullet

        private bool isPlayerinArea = false;       // to check is player in the area 
        private bool isGoingToFall = false;        // check is there are hole infront of this enemy

        public int health;                          // reduce when get hit by bullet
        public int damage;

        public int textureWidth;
        public int textureHeight;

        private GameTime gameTime;

        //attribute using for moving of enemy
        private float timeElapsed;
        private bool isMovingLeft;

        public Vector2 walkSize;
        public Vector2 runSize;
        public Vector2 deadSize;
        public List<Vector2> walkList;
        public List<Vector2> runList;
        public List<Vector2> deadList;

        private EnemyAnimation enemyAnimation;

        private KeyboardState currentKeyState;

        //animation
        private EnemyStatus enemyStatus;
        //check if animation animationEnd or not

        public enum EnemyStatus
        {
            WALK,
            RUN,
            SHOOT,
            DEAD,
            END
        }


        public Enemy(Texture2D texture) : base(texture)
        {
            this.texture = texture;
            //characterSouceRec = new Rectangle(0, 0, sizeX, sizeY);
        }

        public void Initial(Rectangle startRect)
        {
            textureHeight = (int)size.Y;
            textureWidth= (int)size.X;

            enemyHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 1f, ConvertUnits.ToSimUnits(new Vector2(startRect.X, startRect.Y - 1)), 0, BodyType.Dynamic,"Enemy");
            enemyHitBox.FixedRotation = true;
            enemyHitBox.Friction = 1.0f;
            enemyHitBox.AngularDamping = 2.0f;
            enemyHitBox.LinearDamping = 2.0f;

            charDirection = SpriteEffects.FlipHorizontally;  // heading direction

            enemyOrigin = new Vector2(textureWidth/2,textureHeight/2);  //draw in the middle

            enemyStatus = EnemyStatus.WALK;
            enemyAnimation = new EnemyAnimation(texture, deadSize, runSize, deadSize, walkList, runList, deadList);

            enemyAnimation.Initialize();



        }

        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = enemyHitBox.Position;
            //characterDestRec.X = (int)hitBox.Position.X;
            //characterDestRec.Y = (int)hitBox.Position.Y;
            enemyAnimation.Update(gameTime, Enemy.EnemyStatus.WALK);
            
          
        }

        
        public void EnemyAction()
        {
            currentKeyState = Keyboard.GetState();
            EnemyWalking();
            //EnemyAlertWalking();
        }

        private void EnemyWalking()
        {
            
            
            //do normal walking left and right
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            

            int pokemon = 0;
            if(health > 0)
            {
                if (timeElapsed >= 5f)
                {
                    timeElapsed = 0f;
                    isMovingLeft= !isMovingLeft;
                }

                if(isMovingLeft)
                {
                    charDirection = SpriteEffects.None;
                    enemyHitBox.ApplyForce(new Vector2(-100*speed,0));
                }
                else
                {
                    charDirection = SpriteEffects.FlipHorizontally;
                    enemyHitBox.ApplyForce(new Vector2(100 * speed, 0));
                }
              
            }


        }

        

        private void EnemyAlertWalking()
        {

            //player on (right ,mid,left)
            //got to that direction of player
            //stop when player go out of detect area


            //do alert condition follow Player and Track Player down to death
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            enemyAnimation.Draw(spriteBatch, enemyOrigin, charDirection, ConvertUnits.ToDisplayUnits(position));

            //if shoot
            /*if (_bulletBody != null && !_bulletBody.IsDisposed)
            {
                bullet.Draw(spriteBatch);
            }*/

            base.Draw(spriteBatch);
        }
    }
}
