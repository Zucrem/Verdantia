using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Box2DNet.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Box2DNet;
using static ScifiDruid.GameObjects.Player;

namespace ScifiDruid.GameObjects
{
    public class PlayerAnimation : _GameObject
    {
        private Texture2D texture;
        private Vector2 spriteSize;
        private List <Vector2> spriteVector= new List<Vector2>();

        //get animation state if dead
        private bool animationDead = false;

        //time
        private float elapsed;
        private float delay;

        //every player state sprite size
        //idle
        private int idleSrcWidth, idleSrcHeight;
        //shoot
        private int shootSrcWidth, shootSrcHeight;
        //run
        private int runSrcWidth, runSrcHeight;
        //shoot and run
        private int shootAndRunSrcWidth, shootAndRunSrcHeight;
        //shoot up
        private int shootUpSrcWidth, shootUpSrcHeight;
        //shoot up and run
        private int shootUpAndRunSrcWidth, shootUpAndRunSrcHeight;
        //jump
        private int jumpSrcWidth, jumpSrcHeight;
        //shoot on air
        private int shootOnAirSrcWidth, shootOnAirSrcHeight;
        //falling
        private int fallingSrcWidth, fallingSrcHeight;
        //skill
        private int skillSrcWidth, skillSrcHeight;
        //take damage
        private int takeDamageSrcWidth, takeDamageSrcHeight;
        //take damage
        private int dashSrcWidth, dashSrcHeight;
        //dead
        private int deadSrcWidth, deadSrcHeight;

        //all sprite position in spritesheet
        private Rectangle sourceRect;

        private List<Vector2> idleRectVector = new List<Vector2>();
        private List<Vector2> shootRectVector = new List<Vector2>();
        private List<Vector2> runRectVector = new List<Vector2>();
        private List<Vector2> shootAndRunRectVector = new List<Vector2>();
        private List<Vector2> shootUpRectVector = new List<Vector2>();
        private List<Vector2> shootUpAndRunRectVector = new List<Vector2>();
        private List<Vector2> jumpRectVector = new List<Vector2>();
        private List<Vector2> shootOnAirRectVector = new List<Vector2>();
        private List<Vector2> fallingRectVector = new List<Vector2>();
        private List<Vector2> skillRectVector = new List<Vector2>();
        private List<Vector2> takeDamageRectVector = new List<Vector2>();
        private List<Vector2> dashRectVector = new List<Vector2>();
        private List<Vector2> deadRectVector = new List<Vector2>();

        private int idleFrames, shootFrames, runFrames, shootAndRunFrames, shootUpFrames, shootUpAndRunFrames, jumpFrames, shootOnAirFrames, fallingFrames, skillFrames, takeDamageFrames, dashFrames, deadFrames;

        public int frames = 0;
        private int allframes;

        private PlayerStatus preStatus;

        public PlayerAnimation(Texture2D texture) : base(texture)
        {
            this.texture = texture;

            //get size of sprite
            //idle
            idleSrcWidth = 46;
            idleSrcHeight = 96;
            //shoot
            shootSrcWidth = 80;
            shootSrcHeight = 94;
            //run
            runSrcWidth = 68;
            runSrcHeight = 94;
            //shoot and run
            shootAndRunSrcWidth = 96;
            shootAndRunSrcHeight = 94;
            //shoot up
            shootUpSrcWidth = 40;
            shootUpSrcHeight = 110;
            //shoot up and run
            shootUpAndRunSrcWidth = 62;
            shootUpAndRunSrcHeight = 110;
            //jump
            jumpSrcWidth = 44;
            jumpSrcHeight = 96;
            //shoot on air
            shootOnAirSrcWidth = 62;
            shootOnAirSrcHeight = 98;
            //falling
            fallingSrcWidth = 74;
            fallingSrcHeight = 106;
            //skill
            skillSrcWidth = 64;
            skillSrcHeight = 94;
            //dash
            dashSrcWidth = 72;
            dashSrcHeight = 78;
            //take damage
            takeDamageSrcWidth = 54;
            takeDamageSrcHeight = 92;
            //dead
            deadSrcWidth = 100;
            deadSrcHeight = 92;

            //position of spritesheet
            //idle vector to list
            idleRectVector.Add(new Vector2(0, 16));
            idleRectVector.Add(new Vector2(46, 16));
            idleRectVector.Add(new Vector2(92, 16));

            idleFrames = idleRectVector.Count();

            //shoot vector to list
            shootRectVector.Add(new Vector2(0, 292));

            shootFrames = shootRectVector.Count();

            //run vector to list
            runRectVector.Add(new Vector2(0, 148));
            runRectVector.Add(new Vector2(68, 148));
            runRectVector.Add(new Vector2(136, 148));
            runRectVector.Add(new Vector2(204, 148));
            runRectVector.Add(new Vector2(272, 148));
            runRectVector.Add(new Vector2(340, 148));
            runRectVector.Add(new Vector2(408, 148));

            runFrames = runRectVector.Count();

            //shoot and run vector to list
            shootAndRunRectVector.Add(new Vector2(81, 292));
            shootAndRunRectVector.Add(new Vector2(177, 292));
            shootAndRunRectVector.Add(new Vector2(273, 292));
            shootAndRunRectVector.Add(new Vector2(369, 292));
            shootAndRunRectVector.Add(new Vector2(465, 292));
            shootAndRunRectVector.Add(new Vector2(561, 292));
            shootAndRunRectVector.Add(new Vector2(657, 292));

            shootAndRunFrames = shootAndRunRectVector.Count();

            //shoot up vector to list
            shootUpRectVector.Add(new Vector2(260, 0));

            shootUpFrames = shootUpRectVector.Count();

            //shoot up and run vector to list
            shootUpAndRunRectVector.Add(new Vector2(404, 0));
            shootUpAndRunRectVector.Add(new Vector2(466, 0));
            shootUpAndRunRectVector.Add(new Vector2(528, 0));
            shootUpAndRunRectVector.Add(new Vector2(590, 0));
            shootUpAndRunRectVector.Add(new Vector2(652, 0));
            shootUpAndRunRectVector.Add(new Vector2(714, 0));
            shootUpAndRunRectVector.Add(new Vector2(776, 0));

            shootUpAndRunFrames = shootUpAndRunRectVector.Count();

            //jump vector to list
            jumpRectVector.Add(new Vector2(0, 420));
            jumpRectVector.Add(new Vector2(44, 420));

            jumpFrames = jumpRectVector.Count();

            //shoot on air vector to list
            shootOnAirRectVector.Add(new Vector2(52, 418));
            shootOnAirRectVector.Add(new Vector2(234, 418));

            shootOnAirFrames = shootOnAirRectVector.Count();

            //falling vector to list
            fallingRectVector.Add(new Vector2(432, 416));
            fallingRectVector.Add(new Vector2(504, 416));

            fallingFrames = fallingRectVector.Count();

            //skill vector to list
            skillRectVector.Add(new Vector2(672, 422));
            skillRectVector.Add(new Vector2(736, 422));

            skillFrames = skillRectVector.Count();

            //take damage vector to list
            takeDamageRectVector.Add(new Vector2(32, 570));

            takeDamageFrames = takeDamageRectVector.Count();

            //dash vector to list
            dashRectVector.Add(new Vector2(686, 162));
            dashRectVector.Add(new Vector2(778, 162));

            dashFrames = dashRectVector.Count();

            //dead vector to list
            deadRectVector.Add(new Vector2(0, 570));
            deadRectVector.Add(new Vector2(100, 570));
            deadRectVector.Add(new Vector2(200, 570));
            deadRectVector.Add(new Vector2(300, 570));
            deadRectVector.Add(new Vector2(400, 570));
            deadRectVector.Add(new Vector2(500, 570));
            deadRectVector.Add(new Vector2(600, 570));
            deadRectVector.Add(new Vector2(700, 570));
            deadRectVector.Add(new Vector2(800, 570));

            deadFrames = deadRectVector.Count();

        }

        public void Initialize()
        {
            preStatus = PlayerStatus.IDLE;
        }
        public void Update(GameTime gameTime, PlayerStatus playerStatus)
        {
            if (preStatus != playerStatus)
            {
                frames = 0;
            }
            PlayerStatus playerAnimateSprite = playerStatus;
            ChangeAnimationStatus(playerAnimateSprite);
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            switch (playerStatus)
            {
                case PlayerStatus.SHOOT:
                    if (elapsed >= delay)
                    {
                        if (frames < allframes - 1)
                        {
                            frames++;
                        }
                        elapsed = 0;
                    }
                    break;
                case PlayerStatus.DASH:
                    if (elapsed >= delay)
                    {
                        if (frames < allframes - 1)
                        {
                            frames++;
                        }
                        elapsed = 0;
                    }
                    break;
                case PlayerStatus.DEAD:
                    if (elapsed >= delay)
                    {
                        if (frames >= allframes - 1)
                        {
                            animationDead = true;
                            return;
                        }
                        else
                        {
                            frames++;
                        }
                        elapsed = 0;
                    }
                    break;
                default:
                    if (elapsed >= delay)
                    {
                        if (frames >= allframes - 1)
                        {
                            frames = 0;
                        }
                        else
                        {
                            frames++;
                        }
                        elapsed = 0;
                    }
                    break;
            }
            sourceRect = new Rectangle((int)spriteVector[frames].X, (int)spriteVector[frames].Y, (int)spriteSize.X, (int)spriteSize.Y);
            preStatus = playerStatus;
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 playerOrigin, SpriteEffects charDirection, Vector2 position)
        {
            spriteBatch.Draw(texture, position, sourceRect, Color.White, 0, playerOrigin, 1f, charDirection, 0f);
        }

        public void ChangeAnimationStatus(PlayerStatus playerStatus)
        {
            switch (playerStatus)
            {
                case PlayerStatus.IDLE:
                    delay = 300f;
                    spriteVector = idleRectVector;
                    spriteSize = new Vector2(idleSrcWidth, idleSrcHeight);
                    allframes = idleFrames;
                    break;
                case PlayerStatus.SHOOT:
                    delay = 300f;
                    spriteVector = shootRectVector;
                    spriteSize = new Vector2(shootSrcWidth, shootSrcHeight);
                    allframes = shootFrames;
                    break;
                case PlayerStatus.RUN:
                    delay = 200f;
                    spriteVector = runRectVector;
                    spriteSize = new Vector2(runSrcWidth, runSrcHeight);
                    allframes = runFrames;
                    break;
                case PlayerStatus.SHOOT_RUN:
                    delay = 300f;
                    spriteVector = shootAndRunRectVector;
                    spriteSize = new Vector2(shootAndRunSrcWidth, shootAndRunSrcHeight);
                    allframes = shootAndRunFrames;
                    break;
                case PlayerStatus.SHOOT_UP:
                    delay = 300f;
                    spriteVector = shootUpRectVector;
                    spriteSize = new Vector2(shootUpSrcWidth, shootUpSrcHeight);
                    allframes = shootUpFrames;
                    break;
                case PlayerStatus.SHOOT_UP_RUN:
                    delay = 300f;
                    spriteVector = shootUpAndRunRectVector;
                    spriteSize = new Vector2(shootUpAndRunSrcWidth, shootUpAndRunSrcHeight);
                    allframes = shootUpAndRunFrames;
                    break;
                case PlayerStatus.JUMP:
                    delay = 300f;
                    spriteVector = jumpRectVector;
                    spriteSize = new Vector2(jumpSrcWidth, jumpSrcHeight);
                    allframes = jumpFrames;
                    break;
                case PlayerStatus.SHOOT_AIR:
                    delay = 300f;
                    spriteVector = shootOnAirRectVector;
                    spriteSize = new Vector2(shootOnAirSrcWidth, shootOnAirSrcHeight);
                    allframes = shootOnAirFrames;
                    break;
                case PlayerStatus.FALLING:
                    delay = 300f;
                    spriteVector = fallingRectVector;
                    spriteSize = new Vector2(fallingSrcWidth, fallingSrcHeight);
                    allframes = fallingFrames;
                    break;
                case PlayerStatus.SKILL:
                    delay = 300f;
                    spriteVector = skillRectVector;
                    spriteSize = new Vector2(skillSrcWidth, skillSrcHeight);
                    allframes = skillFrames;
                    break;
                case PlayerStatus.TAKE_DAMAGE:
                    delay = 300f;
                    spriteVector = takeDamageRectVector;
                    spriteSize = new Vector2(takeDamageSrcWidth, takeDamageSrcHeight);
                    allframes = takeDamageFrames;
                    break;
                case PlayerStatus.DASH:
                    delay = 150f;
                    spriteVector = dashRectVector;
                    spriteSize = new Vector2(dashSrcWidth, dashSrcHeight);
                    allframes = dashFrames;
                    break;
                case PlayerStatus.DEAD:
                    delay = 150f;
                    spriteVector = deadRectVector;
                    spriteSize = new Vector2(deadSrcWidth, deadSrcHeight);
                    allframes = deadFrames;
                    break;
            }
        }

        public bool GetAnimationDead()
        {
            return animationDead;
        }
    }
}
