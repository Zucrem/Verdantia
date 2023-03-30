using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScifiDruid.GameObjects
{
    public class _GameObject
    {
        protected Texture2D _texture;//รูป
        protected Texture2D[] _allTextures;

        public Vector2 position;
        public float rotation;
        public Vector2 size;
        public Color color;
        public Vector2 origin;
        public SpriteEffects charDirection;
        public float speed;

        public string name;

        public bool isAlive;

        public Rectangle rectangle
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            }
        }

        public _GameObject(Texture2D texture)
        {
            _texture = texture;
            position = Vector2.Zero;
            rotation = 0f;
            isAlive = true;
        }

        public virtual void Initial() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

    }
}
