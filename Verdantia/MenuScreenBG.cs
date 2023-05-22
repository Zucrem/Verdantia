using Box2DNet.Dynamics;
using Box2DNet.Factories;
using Box2DNet;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ScifiDruid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verdantia
{
    public class MenuScreenBG
    {
        private Texture2D texture;
        private GameTime gameTime;

        //all sprite position in spritesheet
        private Rectangle desRect;
        private Rectangle sourceRect;

        //srpite to run
        private Vector2 spriteSize = new Vector2(1280,720);
        private List<Vector2> spriteVector = new List<Vector2> { new Vector2(0,0), new Vector2(1280, 0), new Vector2(2560, 0), new Vector2(3840, 0), new Vector2(0, 720), new Vector2(0, 720), new Vector2(0, 720), new Vector2(0, 720), new Vector2(0, 720), new Vector2(0, 720), new Vector2(0, 720), new Vector2(0, 720), new Vector2(0, 720), new Vector2(0, 720), new Vector2(0, 720), new Vector2(0, 720), new Vector2(0, 720), new Vector2(1280, 720), new Vector2(2560, 720)};

        //time
        private float elapsed;
        private float delay = 190f;

        //frames 
        private int frames = 0;
        private int allframes;
        public MenuScreenBG(Texture2D texture)
        {
            this.texture = texture;

            //animation
            desRect = new Rectangle(0 ,0 , (int)spriteSize.X, (int)spriteSize.Y);
            allframes = spriteVector.Count();
        }

        public void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;

            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsed >= delay)
            {
                if (frames >= allframes - 1)
                {
                    frames = 0;
                }
                else
                {
                    frames++;
                }
                elapsed = 0;
            }
            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, desRect, sourceRect, Color.White);
        }
    }
}
