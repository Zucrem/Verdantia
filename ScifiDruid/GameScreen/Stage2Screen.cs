using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;
using ScifiDruid.GameObjects;

namespace ScifiDruid.GameScreen
{
    class Stage2Screen : PlayScreen
    {
        public override void Initial()
        {

            Player.level2Unlock = true;

            base.Initial();
        }
        public override void LoadContent()
        {
            base.LoadContent();

            Initial();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
