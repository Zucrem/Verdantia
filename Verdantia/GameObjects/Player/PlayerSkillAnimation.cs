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

        //symbol state
        private SymbolStatus symbolStatus;

        //symbol position
        private Vector2 symbolPosition;
        private int direction = 0;

        //time
        private float elapsed;
        private float delay;

        private Vector2 symbolOrigin;

        private string symbolName;

        //animation
        //shoot and dead Size
        private Vector2 shootSymbolSize = new Vector2(16, 33);
        private Vector2 healSymbolSize = new Vector2(46, 37);
        private Vector2 dashSymbolSize = new Vector2(100, 77);
        private Vector2 birdSymbolSize = new Vector2(100, 77);
        private Vector2 crocSymbolSize = new Vector2(69, 39);
        private Vector2 lionSymbolSize = new Vector2(230, 220);
        //Vector
        private List<Vector2> shootSymbolRectVector = new List<Vector2>() { new Vector2(247, 2), new Vector2(275, 1), new Vector2(306, 0) };
        private List<Vector2> healSymbolRectVector = new List<Vector2>() { new Vector2(354, 0) };
        private List<Vector2> dashSymbolRectVector = new List<Vector2>() { new Vector2(272, 367), new Vector2(272, 510), new Vector2(272, 660) };
        private List<Vector2> birdSymbolRectVector = new List<Vector2>() { new Vector2(272, 660), new Vector2(272, 510), new Vector2(272, 367) };
        private List<Vector2> crocSymbolRectVector = new List<Vector2>() { new Vector2(28, 238), new Vector2(140, 239), new Vector2(278, 247) };
        private List<Vector2> lionSymbolRectVector = new List<Vector2>() { new Vector2(0, 284), new Vector2(0, 456), new Vector2(0, 707), new Vector2(0, 980) };

        //all sprite position in spritesheet
        private Rectangle sourceRect;

        private Vector2 spriteSize;
        private List<Vector2> spriteVector = new List<Vector2>();

        //frames 
        public int frames = 0;
        private int allframes = 0;

        //bullet status
        private SymbolStatus preStatus;
        public SymbolStatus curStatus;

        private bool animationDead = false;
        public enum SymbolStatus
        {
            SYMBOLSTART,
            SYMBOLEND
        }

        public PlayerSkillAnimation(Texture2D texture, Vector2 position, string name) : base(texture)
        {
            this.texture = texture;
            this.position = position;
            symbolName = name;

            charDirection = SpriteEffects.FlipHorizontally;

            preStatus = SymbolStatus.SYMBOLSTART;
            curStatus = SymbolStatus.SYMBOLSTART;
        }

        public void Update(GameTime gameTime, Vector2 position, SpriteEffects playerSprite)
        {
            this.position = position;

            //check symbol name
            checkSymbol();

            //skill direction
            switch (playerSprite)
            {
                case SpriteEffects.None:
                    charDirection = SpriteEffects.None;
                    direction = -1;
                    break;
                case SpriteEffects.FlipHorizontally:
                    charDirection = SpriteEffects.FlipHorizontally;
                    direction = 1;
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

            if (allframes != 0)
            {
                sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            }
            preStatus = curStatus;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!animationDead)
            {
                spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(symbolPosition), sourceRect, Color.White, 0, symbolOrigin, 1f, charDirection, 0f);
            }
        }

        public void checkSymbol()
        {
            switch (symbolName)
            {
                case "Shoot":
                    symbolPosition = position + (new Vector2(0.8f * direction, -0.12f));
                    delay = 150f;
                    spriteSize = shootSymbolSize;
                    spriteVector = shootSymbolRectVector;
                    symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
                    allframes = spriteVector.Count();
                    break;
                case "Heal":
                    symbolPosition = position + (new Vector2(0, -1f));
                    delay = 550f;
                    spriteSize = healSymbolSize;
                    spriteVector = healSymbolRectVector;
                    symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
                    allframes = spriteVector.Count();
                    break;
                case "Dash":
                    symbolPosition = position + (new Vector2(1.2f * direction * (-1), -0.12f));
                    delay = 200f;
                    spriteSize = dashSymbolSize;
                    spriteVector = dashSymbolRectVector;
                    symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
                    allframes = spriteVector.Count();
                    break;
                case "Bird":
                    symbolPosition = position + (new Vector2(1.2f * direction * (-1), -0.12f));
                    delay = 200f;
                    spriteSize = birdSymbolSize;
                    spriteVector = birdSymbolRectVector;
                    symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
                    allframes = spriteVector.Count();
                    break;
                case "Croc":
                    symbolPosition = position + (new Vector2(0.8f * direction, -0.7f));
                    delay = 300f;
                    spriteSize = crocSymbolSize;
                    spriteVector = crocSymbolRectVector;
                    symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
                    allframes = spriteVector.Count();
                    break;
                case "Lion":
                    symbolPosition = position + (new Vector2(0, 0));
                    delay = 200f;
                    spriteSize = lionSymbolSize;
                    spriteVector = lionSymbolRectVector;
                    symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
                    allframes = spriteVector.Count();
                    break;
                default:
                    symbolPosition = position;
                    delay = 200f;
                    spriteSize = new Vector2(0, 0);
                    spriteVector = new List<Vector2>();
                    symbolOrigin = new Vector2(spriteSize.X / 2, spriteSize.Y / 2);
                    allframes = spriteVector.Count();
                    break;
            }
        }
    }
}
