using ScifiDruid.Managers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace ScifiDruid.GameScreen
{
    class _GameScreen
    {
        protected ContentManager content;

        public virtual void LoadContent()
        {
            content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
        }
        public virtual void UnloadContent()
        {
            content.Unload();
        }
        public virtual void Update(GameTime gameTime)
        {
        }
        public virtual void Update()
        {
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
        public virtual void DrawHUD(SpriteBatch spriteBatch)
        {

        }
        public virtual void DrawFixScreen(SpriteBatch spriteBatch)
        {

        }
    }
}
