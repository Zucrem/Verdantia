using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScifiDruid.GameObjects
{
    internal class EnemyFlame : Enemy
    {

        float initpoint;
        float walkLenght;
     
        public EnemyFlame(Texture2D texture) : base(texture)
        {
            this.texture = texture;
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
            //get initposition
            //calculate leght walk 
            //go walk like shit

        }


    }
}
