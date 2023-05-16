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

        //for animation
        protected Vector2 action1Size;
        protected Vector2 action2Size;
        protected Vector2 action3Size;
        protected Vector2 action4Size;
        protected List<Vector2> action1SpriteVector = new List<Vector2>();
        protected List<Vector2> action2SpriteVector = new List<Vector2>();
        protected List<Vector2> action3SpriteVector = new List<Vector2>();
        protected List<Vector2> action4SpriteVector = new List<Vector2>();

        protected BossStatus preBossStatus;
        protected BossStatus curBossStatus;

        public float skillTime;

        public Boss(Texture2D texture) : base(texture)
        {
            this.texture = texture;
        }

        protected enum BossStatus
        {
            IDLE,
            ACTION1,
            ACTION2,
            ACTION3,
            ACTION4,
            DEAD,
            END
        }

        public override void Initial(Rectangle spawnPosition,Player player)
        {
            curBossStatus = BossStatus.IDLE;
            preBossStatus = BossStatus.IDLE;
            
        }
        
        public override void Action() { }

        public override void Walk() { }

        public override void ChangeAnimationStatus() { }
    }
}
