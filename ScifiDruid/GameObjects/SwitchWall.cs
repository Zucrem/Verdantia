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
    public class SwitchWall : _GameObject
    {

        protected Texture2D texture; //enemy Texture (Animaiton)

        protected Player player;
        protected Vector2 playerPosition;

        protected GameTime gameTime;

        //switchOrigin
        protected Vector2 switchOrigin; 
        public int textureWidth;
        public int textureHeight;

        //for animation
        private Vector2 switchClosePos;
        private Vector2 switchOpenPos;

        private Vector2 spriteSize;
        private Vector2 spriteVector;

        //if player press
        public bool pressSwitch = false;

        public Body switchHitBox;

        private KeyboardState currentKeyState;

        //all sprite position in spritesheet
        protected Rectangle sourceRect;

        private SwitchStatus curStatus;


        public SwitchWall(Texture2D texture) : base(texture)
        {
            this.texture = texture;
        }

        protected enum SwitchStatus
        {
            CLOSE,
            OPEN
        }

        public void Initial(Rectangle spawnPosition)
        {
            spriteSize = new Vector2(32, 32);
            switchClosePos = new Vector2(32, 0);
            switchOpenPos = new Vector2(64, 0);

            textureHeight = (int)size.Y;
            textureWidth = (int)size.X;

            switchOrigin = new Vector2(textureWidth / 2, textureHeight / 2);

            //create switch hitbox
            switchHitBox = BodyFactory.CreateRectangle(Singleton.Instance.world, ConvertUnits.ToSimUnits(spawnPosition.Width), ConvertUnits.ToSimUnits(spawnPosition.Height), 1f, ConvertUnits.ToSimUnits(new Vector2(spawnPosition.X, spawnPosition.Y)));
            switchHitBox.UserData = "switch";
            switchHitBox.IsSensor = true;

            curStatus = SwitchStatus.CLOSE;
        }

        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            position = switchHitBox.Position;
            if (IsContact("Player", "A"))
            {
                curStatus = SwitchStatus.OPEN;
                pressSwitch = true;
            }

            //animation
            if (curStatus == SwitchStatus.CLOSE)
            {
                spriteVector = switchClosePos;
            }
            else if (curStatus == SwitchStatus.OPEN)
            {
                spriteVector = switchOpenPos;
            }
            sourceRect = new Rectangle((int)spriteVector.X, (int)spriteVector.Y, (int)spriteSize.X, (int)spriteSize.Y);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, ConvertUnits.ToDisplayUnits(position), sourceRect, Color.White, 0, switchOrigin, 1f, charDirection, 0f);
        }

        public bool IsContact(String contact, String fixture)
        {
            ContactEdge contactEdge = switchHitBox.ContactList;
            while (contactEdge != null)
            {
                Contact contactFixture = contactEdge.Contact;
                switch (fixture)
                {
                    case "A":
                        // Check if the contact fixture is the ground
                        if (contactFixture.IsTouching && contactEdge.Contact.FixtureA.Body.UserData != null && contactEdge.Contact.FixtureA.Body.UserData.Equals(contact))
                        {
                            return true;
                        }
                        break;
                    case "B":
                        // Check if the contact fixture is the ground
                        if (contactFixture.IsTouching && contactEdge.Contact.FixtureB.Body.UserData != null && contactEdge.Contact.FixtureB.Body.UserData.Equals(contact))
                        {
                            return true;
                        }
                        break;
                }

                contactEdge = contactEdge.Next;
            }
            return false;
        }
    }
}
