using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScifiDruid;
using ScifiDruid.GameObjects;
using ScifiDruid.GameScreen;
using ScifiDruid.Managers;
using System;
using System.Collections.Generic;
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

        //perk status
        private bool dashCD = false;
        private bool increaseHP = false;
        private bool atkDM = false;
        private bool atkNoMana = false;
        private bool crocCd = false;
        private bool crocDmg = false;
        private bool manaRe = false;
        private bool maxMana = false;

        private bool dashCDSkill = false;
        private bool increaseHPSkill = false;
        private bool atkDMSkill = false;
        private bool atkNoManaSkill = false;
        private bool crocCdSkill = false;
        private bool crocDmgSkill = false;
        private bool manaReSkill = false;
        private bool maxManaSkill = false;

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

            Initial();
        }
        public override void Update(GameTime gameTime)
        {
            Singleton.Instance.MousePrevious = Singleton.Instance.MouseCurrent;
            Singleton.Instance.MouseCurrent = Mouse.GetState();

            //all perk
            if (!confirmState)
            {
                //all perk
                if (dashCDButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !dashCDSkill)
                {
                    confirmState = true;
                    dashCD = true;
                }
                if (increaseHPButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !increaseHPSkill)
                {
                    confirmState = true;
                    increaseHP = true;
                }
                if (atkDMButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !atkDMSkill)
                {
                    confirmState = true;
                    atkDM = true;
                }
                if (atkNoManaButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !atkNoManaSkill)
                {
                    confirmState = true;
                    atkNoMana = true;
                }
                if (crocCdButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !crocCdSkill)
                {
                    confirmState = true;
                    crocCd = true;
                }
                if (crocDmgButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !crocDmgSkill)
                {
                    confirmState = true;
                    crocDmg = true;
                }
                if (manaReButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !manaReSkill)
                {
                    confirmState = true;
                    manaRe = true;
                }
                if (maxManaButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) && !maxManaSkill)
                {
                    confirmState = true;
                    maxMana = true;
                }

                //next map
                if (nextButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                {
                    ScreenManager.Instance.LoadScreen(ScreenManager.GameScreenName.PlayScreen);
                }
            }
            else if (confirmState)
            {
                if (dashCD)
                {
                    dashCDButton.SetCantHover(true);
                }
                if (increaseHP)
                {
                    increaseHPButton.SetCantHover(true);
                }
                if (atkDM)
                {
                    atkDMButton.SetCantHover(true);
                }
                if (atkNoMana)
                {
                    atkNoManaButton.SetCantHover(true);
                }
                if (crocCd)
                {
                    crocCdButton.SetCantHover(true);
                }
                if (crocDmg)
                {
                    crocDmgButton.SetCantHover(true);
                }
                if (manaRe)
                {
                    manaReButton.SetCantHover(true);
                }
                if (maxMana)
                {
                    maxManaButton.SetCantHover(true);
                }

                if (yesButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                {
                    if (dashCD)
                    {
                        dashCDSkill = true;
                    }
                    if (increaseHP)
                    {
                        increaseHPSkill = true;
                    }
                    if (atkDM)
                    {
                        atkDMSkill = true;
                    }
                    if (atkNoMana)
                    {
                        atkNoManaSkill = true;
                    }
                    if (crocCd)
                    {
                        crocCdSkill = true;
                    }
                    if (crocDmg)
                    {
                        crocDmgSkill = true;
                    }
                    if (manaRe)
                    {
                        manaReSkill = true;
                    }
                    if (maxMana)
                    {
                        maxManaSkill = true;
                    }
                    confirmState = false;
                }
                if (noButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime) || closeButton.IsClicked(Singleton.Instance.MouseCurrent, gameTime))
                {
                    if (!dashCDSkill)
                    {
                        dashCD = false;
                    }
                    else if (!increaseHPSkill)
                    {
                        increaseHP = false;
                    }
                    else if (!atkDMSkill)
                    {
                        atkDMSkill = false;
                    }
                    else if (!atkNoManaSkill)
                    {
                        atkNoMana = false;
                    }
                    else if (!crocCdSkill)
                    {
                        crocCd = false;
                    }
                    else if (!crocDmgSkill)
                    {
                        crocDmg = false;
                    }
                    else if (!manaReSkill)
                    {
                        manaRe = false;
                    }
                    else if (!maxManaSkill)
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

            //all perk
            if (dashCDSkill)
            {
                spriteBatch.Draw(dashCDRTex, new Vector2(236, 436), Color.White);
            }
            if(!dashCDSkill && !confirmState)
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
            }
            else
            {
                nextButton.SetCantHover(false);
            }
        }
    }
}
