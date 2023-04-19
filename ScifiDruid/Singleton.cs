using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DNet.Dynamics;

namespace ScifiDruid
{
    internal class Singleton
    {
        public World world = new World(new Vector2(0, 9.82f));
        public Vector2 Dimensions = new(1280, 720);
        public Vector2 CenterScreen;
        public float bgMusicVolume = 1f;
        public float soundMasterVolume = 0.6f;
        public bool cmdExit = false;
        public bool gameOver = false;

        public Matrix _view;

        public int lastClickTime = 0;
        public bool firsttime = true;//first time playing this game

        //camera
        public Matrix tfMatrix;

        //Audio State
        public AudioState bgmState = AudioState.FULL;
        public AudioState sfxState = AudioState.FULL;

        //Level State
        public LevelState levelState = LevelState.NULL;
        public int stageunlock = 1;

        //Mouse State
        public MouseState MousePrevious, MouseCurrent;

        private static Singleton instance;

        public enum AudioState
        {
            MUTE,
            MEDIUM,
            FULL
        }
        public enum LevelState
        {
            NULL,
            FOREST,
            CITY,
            LAB
        }
        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }
    }
}
