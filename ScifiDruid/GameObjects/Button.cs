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

namespace ScifiDruid.GameObjects
{
    public class Button
    {
        private Texture2D texture;
        private Vector2 position;
        private Rectangle bounds;
        private bool isHovered;
        private bool isPressed;
        private int sizeX, sizeY;
        private bool cantHover = false;//button cant hover
        private bool noHover = false;//check for only 1 time using hover sound

        //sound
        private SoundEffect clickSound, whileHoveringSound;

        private const int MAX_CLICK_DELAY_MS = 200;

        public Button(Texture2D texture, Vector2 position, Vector2 size)
        {
            this.texture = texture;
            this.position = position;
            this.sizeX = (int)size.X;
            this.sizeY = (int)size.Y;
            this.bounds = new Rectangle((int)position.X, (int)position.Y, sizeX, sizeY);
            isHovered = false;
            this.isPressed = false;
            LoadContent();
        }

        public void LoadContent()
        {
            ContentManager content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");
            // Sounds
            /*clickSound = content.Load<SoundEffect>("Sounds/clickSound");
            whileHoveringSound = content.Load<SoundEffect>("Sounds/hoverSound");*/
        }
        public bool IsWhileHovering(MouseState mouseState)
        {
            isHovered = bounds.Contains(mouseState.Position);
            return isHovered;
        }
        public bool IsClicked(MouseState mouseState, GameTime gameTime)
        {
            bool wasPressed = isPressed;

            // bound.Contains use to active only if mouse is in Position && check if mouse was left click
            isPressed = bounds.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed;

            //if left mouse was click && check if mouse was holding
            if (isPressed && !wasPressed)
            {
                //count time from lastClickTime
                //ex. Program was run for 5 sec (5000 ms) and user doesn't click = 0 lastClickTime , then elapsedMs = 5000 - 0
                int elapsedMs = (int)gameTime.TotalGameTime.TotalMilliseconds - Singleton.Instance.lastClickTime;
                // if pass max click delay time since LastClickTime then lastClickTime was that Time
                if (elapsedMs > MAX_CLICK_DELAY_MS)
                {
                    //after hovering it will not hover anymore, so it could create sound when click, but when it cant hovering at the start = no sound
                    if (noHover)
                    {
                        //clickSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
                    }
                    //lastclickTime = TotalTime of program that time
                    Singleton.Instance.lastClickTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
                    return true;
                }
            }

            return false;
        }

        //for texture that does not need new texture while hover
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!cantHover)
            {
                if (IsWhileHovering(Singleton.Instance.MouseCurrent))
                {
                    if (!noHover)
                    {
                        //whileHoveringSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
                        noHover = true;
                    }
                    spriteBatch.Draw(texture, bounds, Color.LightGray);
                }
                else
                {
                    noHover = false; ;
                    spriteBatch.Draw(texture, bounds, Color.White);
                }
            }
            else
            {
                spriteBatch.Draw(texture, bounds, Color.White);
            }
        }

        //for button that want new texture when hover
        public void Draw(SpriteBatch spriteBatch, Texture2D hoverTexture)
        {
            if (!cantHover)
            {
                if (IsWhileHovering(Singleton.Instance.MouseCurrent))
                {
                    if (!noHover)
                    {
                        //whileHoveringSound.Play(volume: Singleton.Instance.soundMasterVolume, 0, 0);
                        noHover = true;
                    }
                    spriteBatch.Draw(hoverTexture, bounds, Color.White);
                }
                else
                {
                    noHover = false; ;
                    spriteBatch.Draw(texture, bounds, Color.White);
                }
            }
            else
            {
                spriteBatch.Draw(texture, bounds, Color.White);
            }
        }

        //for set Texture that change picture after click
        public void SetTexture(Texture2D htexture)
        {
            this.texture = htexture;
        }

        //set button cant hover no more
        public void SetCantHover(bool bol)
        {
            this.cantHover = bol;
        }
        //set new position for button
        public void SetPosition(Vector2 position)
        {
            this.bounds = new Rectangle((int)position.X, (int)position.Y, sizeX, sizeY); ;
        }
    }
}
