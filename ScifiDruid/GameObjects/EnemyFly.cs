using Box2DNet.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScifiDruid.GameObjects
{
    internal class EnemyFly : Enemy
    {
        
        /*
        public List<Vector2> patternfly;
        public List<List<Vector2>> listpattern;
        */

        public EnemyFly(Texture2D texture, int patternflynum) : base(texture)
        {
            this.texture = texture;
           /* List<Vector2> list1 = new List<Vector2>() { new Vector2(230, 438), new Vector2(0, 898), new Vector2(230, 904) }; //walk pattern 1 //value in this must change this is only example
            List<Vector2> list2 = new List<Vector2>() { new Vector2(230, 438), new Vector2(0, 898), new Vector2(230, 904) }; //walk pattern 2 //value in this must change this is only example
            List<Vector2> list3 = new List<Vector2>() { new Vector2(230, 438), new Vector2(0, 898), new Vector2(230, 904) }; //walk pattern 3 //value in this must change this is only example
           
            listpattern = new List<List<Vector2>>() { list1, list2, list3 };
            patternfly = listpattern[patternflynum];
           */


            //characterSouceRec = new Rectangle(0, 0, sizeX, sizeY);
        }
        public override void Initial()
        {

            base.Initial();
            charDirection = SpriteEffects.FlipHorizontally;  // heading direction
            enemyAnimation = new EnemyAnimation(texture, deadSize, runSize, deadSize, walkList, runList, deadList);
            enemyAnimation.Initialize();
        }

        public override void EnemyWalking()
        {
            //patternfly
            //change position(not adding force)
            //do nothing 
        }



    }


}
