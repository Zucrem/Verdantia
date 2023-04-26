using Box2DNet;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public Enemy(Texture2D texture) : base(texture)
        {
            this.texture = texture;
            //characterSouceRec = new Rectangle(0, 0, sizeX, sizeY);
        }

        public void Initial(Rectangle startRect)
        {
            size = new Vector2(textureWidth, textureHeight);

            enemyHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(textureWidth), ConvertUnits.ToSimUnits(textureHeight), 1f, ConvertUnits.ToSimUnits(new Vector2(startRect.X, startRect.Y - 1)), 0, BodyType.Dynamic,"Enemy");
            enemyHitBox.FixedRotation = true;
            enemyHitBox.Friction = 1f;
            enemyHitBox.AngularDamping = 2f;
            enemyHitBox.LinearDamping = 2f;

            enemyOrigin = new Vector2(textureWidth/2,textureHeight/2);  //draw in the middle
        }

       /* public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = hitBox.Position;
            //characterDestRec.X = (int)hitBox.Position.X;
            //characterDestRec.Y = (int)hitBox.Position.Y;
        }*/

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(enemyHitBox.Position), null, Color.White, 0, enemyOrigin, 1f, SpriteEffects.None, 0f);

            //spriteBatch.Draw(texture, characterDestRec, characterSouceRec, Color.White, rotation, new Vector2(characterDestRec.Width / 2, characterDestRec.Height /2), charDirection,0);
            //spriteBatch.Draw(texture, characterDestRec, characterSouceRec, Color.White);

            base.Draw(spriteBatch);
        }
        public void EnemyAction()
        {
            EnemyWalking();
            EnemyAlertWalking();
        }

        private void EnemyWalking()
        {
            //do normal walking left and right
        }

        private void EnemyAlertWalking()
        {
            //do alert condition follow Player and Track Player down to death like shit
        }
    }
}
