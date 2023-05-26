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
    public class StageObject : _GameObject
    {

        protected Texture2D texture; //enemy Texture (Animaiton)

        protected GameTime gameTime;

        //wallOrigin
        protected Vector2 wallOrigin;
        public int textureWidth;
        public int textureHeight;

        private Vector2 spriteSize;
        private Vector2 spriteVector;

        public Rectangle spawnPosition;

        public Body wallHitBox;

        //all sprite position in spritesheet
        protected Rectangle sourceRect;


        public StageObject(Texture2D texture, Vector2 size, Vector2 texturePosition) : base(texture)
        {
            this.texture = texture;
            spriteSize = size;
            spriteVector = texturePosition;
        }

        public void Initial(Rectangle spawnPosition)
        {
            this.spawnPosition = spawnPosition;
            textureHeight = (int)size.Y;
            textureWidth = (int)size.X;

            wallOrigin = new Vector2(textureWidth / 2, textureHeight / 2);

            //create wall hitbox
            wallHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(size.X), ConvertUnits.ToSimUnits(size.Y), 1f, ConvertUnits.ToSimUnits(new Vector2(spawnPosition.X, spawnPosition.Y)));
            wallHitBox.UserData = "Ground";
            wallHitBox.Restitution = 0.0f;
            wallHitBox.Friction = 0.3f;
        }
        public void WallInitial(Rectangle spawnPosition)
        {
            this.spawnPosition = spawnPosition;
            textureHeight = (int)size.Y;
            textureWidth = (int)size.X;

            wallOrigin = new Vector2(textureWidth / 2, textureHeight / 2);

            //create wall hitbox
            wallHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(spawnPosition.X), ConvertUnits.ToSimUnits(spawnPosition.Y), 1f, ConvertUnits.ToSimUnits(new Vector2(spawnPosition.X, spawnPosition.Y)));
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

