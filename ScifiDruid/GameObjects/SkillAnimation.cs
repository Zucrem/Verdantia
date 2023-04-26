using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ScifiDruid.GameObjects.Player;
using static ScifiDruid.GameObjects.Bullet;

namespace ScifiDruid.GameObjects
{
    public class SkillAnimation : _GameObject
    {
        private Texture2D texture;
        private Vector2 spriteSize;
        private List<Vector2> spriteVector = new List<Vector2>();
        //time
        private float elapsed;
        private float delay;

        //every bullet state sprite size
        //shoot
        private int bulletSrcWidth, bulletSrcHeight;
        //dead
        private int bulletDeadSrcWidth, bulletDeadSrcHeight;
        //skill
        private int skillSrcWidth, skillSrcHeight;

        //all sprite position in spritesheet
        private Rectangle sourceRect;

        public int frames = 0;
        private int allframes;

        private List<Vector2> bulletRectVector = new List<Vector2>();
        private List<Vector2> bulletDeadRectVector = new List<Vector2>();
        private List<Vector2> skillRectVector = new List<Vector2>();

        private int bulletFrames, bulletDeadFrames, skillFrames;

        public SkillAnimation(Texture2D texture, Vector2 position) : base(texture)
        {
            this.texture = texture;

            //get size of sprite
            //bullet
            bulletSrcWidth = 40;
            bulletSrcHeight = 8;
            //bullet dead
            bulletDeadSrcWidth = 16;
            bulletDeadSrcHeight = 20;
            //skill
            skillSrcWidth = 16;
            skillSrcHeight = 30;

            //position of spritesheet
            //bullet vector to list
            bulletRectVector.Add(new Vector2(0, 12));
            bulletRectVector.Add(new Vector2(62, 12));
            bulletRectVector.Add(new Vector2(120, 12));

            bulletFrames = bulletRectVector.Count();

            //bullet dead vector to list
            bulletDeadRectVector.Add(new Vector2(170, 6));
            bulletDeadRectVector.Add(new Vector2(216, 6));

            bulletDeadFrames = bulletDeadRectVector.Count();

            //skill vector to list
            skillRectVector.Add(new Vector2(252, 12));
            skillRectVector.Add(new Vector2(276, 2));
            skillRectVector.Add(new Vector2(306, 0));

            skillFrames = skillRectVector.Count();
        }
        public void Initialize()
        {
        }

        public void Update(GameTime gameTime, BulletStatus bulletStatus)
        {
            changeBulletAnimationStatus(bulletStatus);
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsed >= delay)
            {
                if (frames < allframes - 1)
                {
                    frames++;
                }
                elapsed = 0;
            }

            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
        }
        public void Update(GameTime gameTime)
        {
            changeSkillAnimationStatus();
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsed >= delay)
            {
                if (frames < allframes - 1)
                {
                    frames++;
                }
                elapsed = 0;
            }

            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 playerOrigin, SpriteEffects charDirection, Vector2 position)
        {
            spriteBatch.Draw(texture, position, sourceRect, Color.White, 0, playerOrigin, 1f, charDirection, 0f);
        }

        public void changeBulletAnimationStatus(BulletStatus bulletStatus)
        {
            switch (bulletStatus)
            {
                case BulletStatus.BULLETALIVE:
                    delay = 300f;
                    spriteVector = bulletRectVector;
                    spriteSize = new Vector2(bulletSrcWidth, bulletSrcHeight);
                    allframes = bulletFrames;
                    break;
                case BulletStatus.BULLETDEAD:
                    delay = 300f;
                    spriteVector = bulletDeadRectVector;
                    spriteSize = new Vector2(bulletDeadSrcWidth, bulletDeadSrcHeight);
                    allframes = bulletDeadFrames;
                    break;
            }
        }
        public void changeSkillAnimationStatus()
        {
            delay = 300f;
            spriteVector = skillRectVector;
            spriteSize = new Vector2(skillSrcWidth, skillSrcHeight);
            allframes = skillFrames;
        }
    }
}
