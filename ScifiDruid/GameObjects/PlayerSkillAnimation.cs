using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Box2DNet.Dynamics.Contacts;
using System.Collections.Generic;
using System.Linq;
using static ScifiDruid.GameObjects.PlayerSkillAnimation;
using static ScifiDruid.GameObjects.PlayerBullet;

namespace ScifiDruid.GameObjects
{
    public class PlayerSkillAnimation : _GameObject
    {
        private Texture2D texture;

        //bullet state
        public SymbolStatus symbolStatus;

        //time
        private float elapsed;
        private float delay;

        private Vector2 symbolOrigin;

        public Body symbolBody;

        private int symbolSizeX;
        private int symbolSizeY;

        //animation
        //shoot and dead Size
        private Vector2 shootSymbolSize;
        private Vector2 birdSymbolSize;
        private Vector2 crocSymbolSize;
        private Vector2 lionSymbolSize;
        private Vector2 symbolStartSize, symbolEndSize;
        //Vector
        private List<Vector2> shootSymbolRectVector;
        private List<Vector2> birdSymbolRectVector;
        private List<Vector2> crocSymbolRectVector;
        private List<Vector2> lionSymbolRectVector;
        private List<Vector2> symbolRectVector;

        //all sprite position in spritesheet
        private Rectangle sourceRect;

        private Vector2 spriteSize;
        private List<Vector2> spriteVector = new List<Vector2>();

        //frames 
        public int frames = 0;
        private int allframes;

        //bullet status
        private SymbolStatus preStatus;
        private SymbolStatus curStatus;

        private bool animationDead = false;
        public enum SymbolStatus
        {
            SYMBOLSTART,
            SYMBOLEND
        }

        public PlayerSkillAnimation(Texture2D texture, Vector2 position, SpriteEffects charDirection, Vector2 size, List<Vector2> rectVector, float delay) : base(texture)
        {
            this.texture = texture;
            this.charDirection = charDirection;
            this.position = position;
            this.spriteSize = size;
            this.spriteVector = rectVector;
            this.delay = delay;

            allframes = rectVector.Count();

            //animation
            shootSymbolSize = new Vector2(16, 30);
            birdSymbolSize = new Vector2(100, 70);
            crocSymbolSize = new Vector2(69, 35);
            lionSymbolSize = new Vector2(113, 100);

            shootSymbolRectVector = new List<Vector2>() { new Vector2(247, 3), new Vector2(275, 1), new Vector2(306, 0) };
            birdSymbolRectVector = new List<Vector2>() { new Vector2(272, 321), new Vector2(272, 465), new Vector2(272, 606) };
            crocSymbolRectVector = new List<Vector2>() { new Vector2(28, 219), new Vector2(140, 220), new Vector2(278, 227) };
            lionSymbolRectVector = new List<Vector2>() { new Vector2(60, 306), new Vector2(62, 473), new Vector2(59, 583) };

            symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);

            preStatus = SymbolStatus.SYMBOLSTART;
            curStatus = SymbolStatus.SYMBOLSTART;
        }

        public void Update(GameTime gameTime, SpriteEffects playerSprite)
        {
            //skill direction
            switch (playerSprite)
            {
                case SpriteEffects.None:
                    charDirection = SpriteEffects.None;
                    break;
                case SpriteEffects.FlipHorizontally:
                    charDirection = SpriteEffects.FlipHorizontally;
                    break;
            }

            //if dead animation animationEnd
            if (animationDead)
            {
                curStatus = SymbolStatus.SYMBOLEND;
            }

            if (preStatus != curStatus)
            {
                frames = 0;
            }

            //animation
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsed >= delay)
            {
                if (symbolStatus == SymbolStatus.SYMBOLSTART)
                {
                    if (frames < allframes - 1)
                    {
                        frames++;
                    }
                    else if (frames >= allframes - 1)
                    {
                        animationDead = true;
                        return;
                    }
                }
                elapsed = 0;
            }

            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            preStatus = curStatus;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!animationDead)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, symbolOrigin, 1f, charDirection, 0f);
            }
        }
    }
}
