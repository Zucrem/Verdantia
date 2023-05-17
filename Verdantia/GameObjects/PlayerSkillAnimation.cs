using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Box2DNet.Dynamics.Contacts;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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

        //animation
        //shoot and dead Size
        private Vector2 shootSymbolSize;
        private Vector2 healSymbolSize;
        private Vector2 birdSymbolSize;
        private Vector2 crocSymbolSize;
        private Vector2 lionSymbolSize;
        //Vector
        private List<Vector2> shootSymbolRectVector;
        private List<Vector2> healSymbolRectVector;
        private List<Vector2> birdSymbolRectVector;
        private List<Vector2> crocSymbolRectVector;
        private List<Vector2> lionSymbolRectVector;

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

        public PlayerSkillAnimation(Texture2D texture) : base(texture)
        {
            this.texture = texture;

            charDirection = SpriteEffects.FlipHorizontally;
        }

        public void ShootSymbol(Vector2 position)
        {
            this.position = position;
            delay = 200f;
            shootSymbolSize = new Vector2(16, 30);
            shootSymbolRectVector = new List<Vector2>() { new Vector2(247, 3), new Vector2(275, 1), new Vector2(306, 0) };
            spriteSize = shootSymbolSize;
            spriteVector = shootSymbolRectVector;
            symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
            allframes = shootSymbolRectVector.Count();

            animationDead = false;
            preStatus = SymbolStatus.SYMBOLSTART;
            curStatus = SymbolStatus.SYMBOLSTART;
        }
        public void HealSymbol(Vector2 position)
        {
            this.position = position;
            delay = 600f;
            healSymbolSize = new Vector2(46, 34);
            healSymbolRectVector = new List<Vector2>() { new Vector2(354, 0) };
            spriteSize = healSymbolSize;
            spriteVector = healSymbolRectVector;
            symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
            allframes = healSymbolRectVector.Count();
            elapsed = 0;

            animationDead = false;
            preStatus = SymbolStatus.SYMBOLSTART;
            curStatus = SymbolStatus.SYMBOLSTART;
            Debug.WriteLine(symbolOrigin);

        }

        public void BirdSymbol(Vector2 position)
        {
            this.position = position;
            delay = 200f;
            birdSymbolSize = new Vector2(100, 70);
            birdSymbolRectVector = new List<Vector2>() { new Vector2(272, 321), new Vector2(272, 465), new Vector2(272, 606) };
            spriteSize = birdSymbolSize;
            spriteVector = birdSymbolRectVector;
            symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
            allframes = birdSymbolRectVector.Count();

            animationDead = false;
            preStatus = SymbolStatus.SYMBOLSTART;
            curStatus = SymbolStatus.SYMBOLSTART;
        }

        public void CrocSymbol(Vector2 position)
        {
            this.position = position;
            delay = 200f;
            crocSymbolSize = new Vector2(69, 35);
            crocSymbolRectVector = new List<Vector2>() { new Vector2(28, 219), new Vector2(140, 220), new Vector2(278, 227) };
            spriteSize = crocSymbolSize;
            spriteVector = crocSymbolRectVector;
            symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
            allframes = crocSymbolRectVector.Count();

            animationDead = false;
            preStatus = SymbolStatus.SYMBOLSTART;
            curStatus = SymbolStatus.SYMBOLSTART;
        }

        public void LionSymbol(Vector2 position)
        {
            this.position = position;
            delay = 200f;
            lionSymbolSize = new Vector2(113, 100);
            lionSymbolRectVector = new List<Vector2>() { new Vector2(60, 306), new Vector2(62, 473), new Vector2(59, 583) };
            spriteSize = lionSymbolSize;
            spriteVector = lionSymbolRectVector;
            symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
            allframes = lionSymbolRectVector.Count();

            animationDead = false;
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
                //Debug.WriteLine("draw");
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, symbolOrigin, 1f, charDirection, 0f);
            }
        }
    }
}
