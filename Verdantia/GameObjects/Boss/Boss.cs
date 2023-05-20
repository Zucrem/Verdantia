using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScifiDruid.GameObjects
{
    public class Boss : Enemy
    {
        protected Rectangle bossDestRect;  //where postion
        protected Rectangle bossSourceRec; //where read
        protected Vector2 bossOrigin;  //start draw boss point


        public float skillTime;

        protected SpriteEffects bossSkilDirection;

        public Boss(Texture2D texture) : base(texture)
        {
            this.texture = texture;
        }

        

        public virtual void Initial(Rectangle spawnPosition,Player player, Rectangle fieldBoss)
        {
            
        }
        
        public override void Action() { }

        public override void Walk() { }

        public override void ChangeAnimationStatus() { }
    }
}
