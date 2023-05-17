using Box2DNet;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScifiDruid
{
    public class Camera
    {
        public Matrix Transform;

        public Matrix Follow(Vector2 target, float startCamX, float endCamX)
        {
            target.X = MathHelper.Clamp(target.X , startCamX, endCamX);//player position in tile ,min value to make camera focus on player tile.X in Screen.Width = 40/2 and /2, max value to make camera not focus at last screen in map
            target.Y = 5.625f;//camera focus on player tile.Y in Screen.Height = 22.5/2 and /2
            Vector3 translation = new Vector3((target * - 64), 0f);
            Vector3 offset = new Vector3(Singleton.Instance.CenterScreen,0f);

            Transform = Matrix.CreateTranslation(translation) * Matrix.CreateTranslation(offset);

            return Transform;
        }
    }
}
