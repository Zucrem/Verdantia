using ScifiDruid.Managers;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Xml;

namespace ScifiDruid.GameObjects
{
    public class Player : _GameObject
    {
        private Texture2D texture;
        public Rectangle characterDestRec;
        public Rectangle characterSouceRec;

        public Player(Texture2D texture ,int sizeX , int sizeY) : base(texture)
        {
            this.texture = texture;
            characterSouceRec = new Rectangle(0, 0, sizeX, sizeY);
        }

        public override void Initial()
        {
            characterDestRec = rectangle;
            base.Initial();
        }

        public void Update()
        {
            characterDestRec.X = (int)position.X;
            characterDestRec.Y = (int)position.Y;
        }

        public void Walking()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                position.X -= 0.1f * speed;
                charDirection = SpriteEffects.FlipHorizontally;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                position.X += 0.1f * speed;
                charDirection = SpriteEffects.None;
            }
        }

        public void Jump()
        {
            Vector2 jumpPosition = position + new Vector2(0, -50);


        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, characterDestRec, characterSouceRec, Color.White, rotation, origin, charDirection,0);
            //spriteBatch.Draw(texture, characterDestRec, characterSouceRec, Color.White);

            base.Draw(spriteBatch);
        }
    }
}
