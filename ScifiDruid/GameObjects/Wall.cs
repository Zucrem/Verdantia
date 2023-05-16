using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2DNet.Dynamics.Contacts;
using Box2DNet.Dynamics;
using Microsoft.Xna.Framework.Input;
using Box2DNet.Factories;
using Box2DNet;
using System.Diagnostics;

namespace ScifiDruid.GameObjects
{
    public class Wall : _GameObject
    {

        protected Texture2D texture; //enemy Texture (Animaiton)

        protected Player player;
        protected Vector2 playerPosition;

        protected GameTime gameTime;

        //wallOrigin
        protected Vector2 wallOrigin;
        public int textureWidth;
        public int textureHeight;

        private Vector2 spriteSize;
        private Vector2 spriteVector;

        public Body wallHitBox;

        //all sprite position in spritesheet
        protected Rectangle sourceRect;


        public Wall(Texture2D texture) : base(texture)
        {
            this.texture = texture;
        }

        public void Initial(Rectangle spawnPosition)
        {
            spriteSize = new Vector2(32, 192);
            spriteVector = new Vector2(0, 0);

            textureHeight = (int)size.Y;
            textureWidth = (int)size.X;

            wallOrigin = new Vector2(textureWidth / 2, textureHeight / 2);

            //create wall hitbox
            wallHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(spawnPosition.Width), ConvertUnits.ToSimUnits(spawnPosition.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(spawnPosition.X, spawnPosition.Y)));
            wallHitBox.UserData = "Ground";
            wallHitBox.Restitution = 0.0f;
            wallHitBox.Friction = 0.3f;
        }

        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = wallHitBox.Position;

            sourceRect = new Rectangle((int)spriteVector.X, (int)spriteVector.Y, (int)spriteSize.X, (int)spriteSize.Y);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, wallOrigin, 1f, charDirection, 0f);
        }
    }
}

