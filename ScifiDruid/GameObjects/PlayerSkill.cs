using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Box2DNet.Dynamics.Contacts;
using System.Collections.Generic;
using System.Linq;
using static ScifiDruid.GameObjects.PlayerSkill;
using static ScifiDruid.GameObjects.PlayerBullet;

namespace ScifiDruid.GameObjects
{
    public class PlayerSkill : _GameObject
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
        private List<Vector2> symbolStartRectVector;
        private List<Vector2> symbolEndRectVector;

        private int symbolStartCount;
        private int symbolEndCount;

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

        public PlayerSkill(Texture2D texture, Vector2 position, SpriteEffects charDirection, string name) : base(texture)
        {
            this.texture = texture;
            this.charDirection = charDirection;
            this.position = position;
            this.name = name;


            //animation
            symbolStartSize = new Vector2(40, 8);
            symbolEndSize = new Vector2(16, 20);

            symbolStartRectVector = new List<Vector2>() { new Vector2(0, 12), new Vector2(62, 12), new Vector2(120, 12) };
            symbolEndRectVector = new List<Vector2>() { new Vector2(0, 6), new Vector2(210, 6) };

            symbolStartCount = symbolStartRectVector.Count();
            symbolEndCount = symbolEndRectVector.Count();

            symbolOrigin = new Vector2(symbolSizeX / 2, symbolSizeY / 2);
        }

        public void Initialize()
        {
            preStatus = SymbolStatus.SYMBOLSTART;
            curStatus = SymbolStatus.SYMBOLEND;
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
            ChangeSymbolAnimationStatus(symbolStatus);
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsed >= delay)
            {
                if (symbolStatus == SymbolStatus.SYMBOLSTART)
                {
                    if (frames < allframes - 1)
                    {
                        frames++;
                    }
                }
                else if (symbolStatus == SymbolStatus.SYMBOLEND)
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
        public void ChangeSymbolAnimationStatus(SymbolStatus symbolStatus)
        {
            switch (symbolStatus)
            {
                case SymbolStatus.SYMBOLSTART:
                    delay = 150f;
                    spriteVector = symbolStartRectVector;
                    spriteSize = symbolStartSize;
                    allframes = symbolStartCount;
                    break;
                case SymbolStatus.SYMBOLEND:
                    delay = 150f;
                    spriteVector = symbolEndRectVector;
                    spriteSize = symbolEndSize;
                    allframes = symbolEndCount;
                    break;
            }
        }
    }
}
