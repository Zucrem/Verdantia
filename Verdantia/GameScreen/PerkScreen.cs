using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;
using ScifiDruid;
using ScifiDruid.GameObjects;
using ScifiDruid.GameScreen;
using ScifiDruid.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Verdantia.GameScreen
{
    class PerkScreen : _GameScreen
    {
        //bg
        private Texture2D perkBG, cfPopUp, blackBG;
        //have
        private Texture2D dashCDRTex, increaseHPRTex, atkDMRTex, atkNoManaRTex, crocCdRTex, crocDmgRTex, manaReRTex, maxManaRTex;
        //not have
        private Texture2D dashCDNotRTex, increaseHPNotRTex, atkDMNotRTex, atkNoManaNotRTex, crocCdNotRTex, crocDmgNotRTex, manaReNotRTex, maxManaNotRTex;
        //CF
        private Texture2D closeTex, yesTex, noTex, nextTex;

        //all button
        private Button dashCDButton, increaseHPButton, atkDMButton, atkNoManaButton, crocCdButton, crocDmgButton, manaReButton, maxManaButton;
        private Button closeButton, yesButton, noButton, nextButton;

        //confirm state
        private bool confirmState = false;

        //fonts
        private SpriteFont smallfonts, mediumfonts, bigfonts, kongfonts;//กำหนดชื่อ font
        private Vector2 fontSize;//ขนาด font ที่เอามา

        //perk status
        private bool dashCD = false;
        private bool increaseHP = false;
        private bool atkDM = false;
        private bool atkNoMana = false;
        private bool crocCd = false;
        private bool crocDmg = false;
        private bool manaRe = false;
        private bool maxMana = false;

        private int countAviliableSkill;

        public static bool dashCDSkill = false;
        public static bool increaseHPSkill = false;
        public static bool atkDMSkill = false;
        public static bool atkNoManaSkill = false;
        public static bool crocCdSkill = false;
        public static bool crocDmgSkill = false;
        public static bool manaReSkill = false;
        public static bool maxManaSkill = false;

        //fade
        private int alpha = 255;
        private bool fadeFinish = false;
        private bool changeScreen = false;
        //timer
        private float _timer = 0f;
        private float _scrollTime = 0f;
        private float Timer = 0f;
        private float timerPerUpdate = 0.05f;
        protected float tickPerUpdate = 30f;

        //color for fade in and out
        private Color colorStart;
        private Color colorEnd;

        //change to go another screen
        protected bool nextScreen = false;

        private string skillName;

        public void Initial()
        {
            dashCDButton = new Button(dashCDNotRTex, new Vector2(236, 436), new Vector2(124, 88));
            increaseHPButton = new Button(increaseHPNotRTex, new Vector2(556, 128), new Vector2(144, 96));
            atkDMButton = new Button(atkDMNotRTex, new Vector2(132, 196), new Vector2(116, 76));
            atkNoManaButton = new Button(atkNoManaNotRTex, new Vector2(360, 196), new Vector2(116, 76));
            crocCdButton = new Button(crocCdNotRTex, new Vector2(784, 192), new Vector2(116, 76));
            crocDmgButton = new Button(crocDmgNotRTex, new Vector2(1012, 192), new Vector2(116, 76));
            manaReButton = new Button(manaReNotRTex, new Vector2(892, 440), new Vector2(124, 88));
            maxManaButton = new Button(maxManaNotRTex, new Vector2(556, 468), new Vector2(144, 96));

            closeButton = new Button(closeTex, new Vector2(764, 172), new Vector2(20, 20));

            yesButton = new Button(yesTex, new Vector2(508, 420), new Vector2(108, 48));
            noButton = new Button(noTex, new Vector2(652, 420), new Vector2(108, 48));

            nextButton = new Button(nextTex, new Vector2(1096, 588), new Vector2(112, 68));

            countAviliableSkill = 2;
        }
        public override void LoadContent()
        {
            base.LoadContent();
            perkBG = content.Load<Texture2D>("Pictures/Play/PerkScreen/perkBG");
            blackBG = content.Load<Texture2D>("Pictures/Main/MainMenu/Black");

            //already have
            dashCDRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/DashCDR");
            increaseHPRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/IncreaseHPR");
            atkDMRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/AtkDMR");
            atkNoManaRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/AtkNoManaR");
            crocCdRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/crocCdR");
            crocDmgRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/crocDmgR");
            manaReRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/manaReR");
            maxManaRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/maxManaR");

            //not have
            dashCDNotRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/DashCDNotR");
            increaseHPNotRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/IncreaseHPNotR");
            atkDMNotRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/AtkDMNotR");
            atkNoManaNotRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/AtkNoManaNotR");
            crocCdNotRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/crocCdNotR");
            crocDmgNotRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/crocDmgNotR");
            manaReNotRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/manaReNotR");
            maxManaNotRTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/maxManaNotR");

            //CF
            cfPopUp = content.Load<Texture2D>("Pictures/Play/PerkScreen/CfWindow");

            closeTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/Close");
            noTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/No");
            yesTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/Yes");

            //next game
            nextTex = content.Load<Texture2D>("Pictures/Play/PerkScreen/Next");

            //fonts
            smallfonts = content.Load<SpriteFont>("Fonts/font20");
            bigfonts = content.Load<SpriteFont>("Fonts/font60");
            mediumfonts = content.Load<SpriteFont>("Fonts/font30");
            kongfonts = content.Load<SpriteFont>("Fonts/KongFont");

            Initial();
        }
        public override void Update(GameTime gameTime)
        {
            Singleton.Instance.MousePrevious = Singleton.Instance.MouseCurrent;
            Singleton.Instance.MouseCurrent = Mouse.GetState();

            //fade in
            if (!fadeFinish)
            {
                _timer += (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                // fade out when start
                if (_timer >= timerPerUpdate)
                {
                    alpha -= 10;
                    _timer -= timerPerUpdate;
                    if (alpha <= 10)
                    {
                        fadeFinish = true;
                    }
                    colorStart.A = (byte)alpha;
                }
            }

            if (countAviliableSkill <= 0)
            {
                //next map
                if (nextButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                {
                    changeScreen = true;
                }
            }
            //fade in
            if (changeScreen)
            {
                if (fadeFinish)
                {
                    _timer += (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
                    // fade out when start
                    if (_timer >= timerPerUpdate)
                    {
                        alpha += 10;
                        _timer -= timerPerUpdate;
                        if (alpha > 245)
                        {
                            fadeFinish = false;
                            nextScreen = true;
                        }
                        colorEnd.A = (byte)alpha;
                    }
                }
            }
            if (nextScreen)
            {
                ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                //ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.MenuScreen);
            }

            //all perk
            if (!confirmState)
            {
                //all perk
                if (dashCDButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !dashCDSkill)
                {
                    confirmState = true;
                    dashCD = true;
                    skillName = "DashCD";
                }
                if (increaseHPButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !increaseHPSkill)
                {
                    confirmState = true;
                    increaseHP = true;
                    skillName = "IncreaseHP";
                }
                if (atkDMButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !atkDMSkill)
                {
                    confirmState = true;
                    atkDM = true;
                    skillName = "AtkDM";
                }
                if (atkNoManaButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !atkNoManaSkill)
                {
                    confirmState = true;
                    atkNoMana = true;
                    skillName = "AtkNoMana";
                }
                if (crocCdButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !crocCdSkill)
                {
                    confirmState = true;
                    crocCd = true;
                    skillName = "CrocCd";
                }
                if (crocDmgButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !crocDmgSkill)
                {
                    confirmState = true;
                    crocDmg = true;
                    skillName = "CrocDmg";
                }
                if (manaReButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !manaReSkill)
                {
                    confirmState = true;
                    manaRe = true;
                    skillName = "ManaRe";
                }
                if (maxManaButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !maxManaSkill)
                {
                    confirmState = true;
                    maxMana = true;
                    skillName = "MaxMana";
                }
            }
            else
            {
                if (yesButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                {
                    switch (skillName)
                    {
                        case "DashCD":
                            dashCDSkill = true;
                            Player.dashMaxCooldown = 5;
                            countAviliableSkill--;
                            break;

                        case "IncreaseHP":
                            increaseHPSkill = true;
                            Player.maxHealth = 5;
                            countAviliableSkill--;
                            break;

                        case "AtkDM":
                            atkDMSkill = true;
                            Player.attackDmg = 3;
                            countAviliableSkill--;
                            break;

                        case "AtkNoMana":
                            atkNoManaSkill = true;
                            Player.attackMana = 0;
                            countAviliableSkill--;
                            break;

                        case "CrocCd":
                            crocCdSkill = true;
                            Player.skill2MaxCooldown = 20;
                            countAviliableSkill--;
                            break;

                        case "CrocDmg":
                            crocDmgSkill = true;
                            Player.skill2Dmg = 5;
                            countAviliableSkill--;
                            break;

                        case "ManaRe":
                            manaReSkill = true;
                            Player.manaMaxRegenTime = 3;
                            Player.manaRegenAmount = 3;
                            countAviliableSkill--;
                            break;

                        case "MaxMana":
                            maxManaSkill = true;
                            Player.maxMana = 150;
                            countAviliableSkill--;
                            break;
                    }
                    confirmState = false;
                }
                if (noButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) || closeButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                {
                    if (!dashCDSkill)
                    {
                        dashCD = false;
                    }
                    if (!increaseHPSkill)
                    {
                        increaseHP = false;
                    }
                    if (!atkDMSkill)
                    {
                        atkDM = false;
                    }
                    if (!atkNoManaSkill)
                    {
                        atkNoMana = false;
                    }
                    if (!crocCdSkill)
                    {
                        crocCd = false;
                    }
                    if (!crocDmgSkill)
                    {
                        crocDmg = false;
                    }
                    if (!manaReSkill)
                    {
                        manaRe = false;
                    }
                    if (!maxManaSkill)
                    {
                        maxMana = false;
                    }

                    confirmState = false;
                }
            }

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //bg
            spriteBatch.Draw(perkBG, Vector2.Zero, Color.White);

            fontSize = bigfonts.MeasureString("Perk");
            spriteBatch.DrawString(bigfonts, "Perk", new Vector2((Singleton.Instance.Dimensions.X - fontSize.X) / 2, 30), Color.White);

            spriteBatch.DrawString(smallfonts, "Skill Point : " + countAviliableSkill, new Vector2(30, 620), Color.White);

            //all perk
            if (dashCDSkill)
            {
                spriteBatch.Draw(dashCDRTex, new Vector2(236, 436), Color.White);
            }
            if (!dashCDSkill && !confirmState)
            {
                dashCDButton.SetCantHover(false);
                dashCDButton.Draw(spriteBatch);
            }

            if (increaseHPSkill)
            {
                spriteBatch.Draw(increaseHPRTex, new Vector2(556, 128), Color.White);
            }
            if (!increaseHPSkill && !confirmState)
            {
                increaseHPButton.SetCantHover(false);
                increaseHPButton.Draw(spriteBatch);
            }

            if (atkDMSkill)
            {
                spriteBatch.Draw(atkDMRTex, new Vector2(132, 196), Color.White);
            }
            if (!atkDMSkill && !confirmState)
            {
                atkDMButton.SetCantHover(false);
                atkDMButton.Draw(spriteBatch);
            }

            if (atkNoManaSkill)
            {
                spriteBatch.Draw(atkNoManaRTex, new Vector2(360, 196), Color.White);
            }
            if (!atkNoManaSkill && !confirmState)
            {
                atkNoManaButton.SetCantHover(false);
                atkNoManaButton.Draw(spriteBatch);
            }

            if (crocCdSkill)
            {
                spriteBatch.Draw(crocCdRTex, new Vector2(784, 192), Color.White);
            }
            if (!crocCdSkill && !confirmState)
            {
                crocCdButton.SetCantHover(false);
                crocCdButton.Draw(spriteBatch);
            }

            if (crocDmgSkill)
            {
                spriteBatch.Draw(crocDmgRTex, new Vector2(1012, 192), Color.White);
            }
            if (!crocDmgSkill && !confirmState)
            {
                crocDmgButton.SetCantHover(false);
                crocDmgButton.Draw(spriteBatch);
            }

            if (manaReSkill)
            {
                spriteBatch.Draw(manaReRTex, new Vector2(892, 440), Color.White);
            }
            if (!manaReSkill && !confirmState)
            {
                manaReButton.SetCantHover(false);
                manaReButton.Draw(spriteBatch);
            }

            if (maxManaSkill)
            {
                spriteBatch.Draw(maxManaRTex, new Vector2(556, 468), Color.White);
            }
            if (!maxManaSkill && !confirmState)
            {
                maxManaButton.SetCantHover(false);
                maxManaButton.Draw(spriteBatch);
            }

            //next
            nextButton.Draw(spriteBatch, nextTex);

            if (confirmState)
            {
                nextButton.SetCantHover(true);
                //cf
                spriteBatch.Draw(cfPopUp, new Vector2(452, 156), Color.White);
                closeButton.Draw(spriteBatch);
                yesButton.Draw(spriteBatch);
                noButton.Draw(spriteBatch);

                //write skill desc
                if (dashCD && !dashCDSkill)
                {
                    spriteBatch.DrawString(smallfonts, "Reduce Dash", new Vector2(493, 270), Color.White);
                    spriteBatch.DrawString(smallfonts, "Cooldown", new Vector2(523, 310), Color.White);
                }
                if (increaseHP && !increaseHPSkill)
                {
                    spriteBatch.DrawString(smallfonts, "Increase", new Vector2(531, 270), Color.White);
                    spriteBatch.DrawString(smallfonts, "More HPs", new Vector2(531, 310), Color.White);
                }
                if (atkDM && !atkDMSkill)
                {
                    spriteBatch.DrawString(smallfonts, "Increase", new Vector2(531, 270), Color.White);
                    spriteBatch.DrawString(smallfonts, "Attack Damage", new Vector2(465, 310), Color.White);
                }
                if (atkNoMana && !atkNoManaSkill)
                {
                    spriteBatch.DrawString(smallfonts, "No Mana Costs", new Vector2(464, 270), Color.White);
                    spriteBatch.DrawString(smallfonts, "When Attack", new Vector2(493, 310), Color.White);
                }
                if (crocCd && !crocCdSkill)
                {
                    spriteBatch.DrawString(smallfonts, "Reduce Croc", new Vector2(493, 270), Color.White);
                    spriteBatch.DrawString(smallfonts, "SkillCooldown", new Vector2(465, 310), Color.White);
                }
                if (crocDmg && !crocDmgSkill)
                {
                    spriteBatch.DrawString(smallfonts, "Increase Croc", new Vector2(461, 270), Color.White);
                    spriteBatch.DrawString(smallfonts, "Skill Damange", new Vector2(461, 310), Color.White);
                }
                if (manaRe && !manaReSkill)
                {
                    spriteBatch.DrawString(smallfonts, "Regenerate", new Vector2(500, 270), Color.White);
                    spriteBatch.DrawString(smallfonts, "Mana Faster", new Vector2(495, 310), Color.White);
                }
                if (maxMana && !maxManaSkill)
                {
                    spriteBatch.DrawString(smallfonts, "Increase", new Vector2(531, 270), Color.White);
                    spriteBatch.DrawString(smallfonts, "More Mana", new Vector2(505, 310), Color.White);
                }
            }
            else
            {
                nextButton.SetCantHover(false);


                //fade
                if (fadeFinish)
                {
                    spriteBatch.Draw(blackBG, Vector2.Zero, colorEnd);
                }
                if (!fadeFinish)
                {
                    spriteBatch.Draw(blackBG, Vector2.Zero, colorStart);
                }
            }

        }
    }
}
