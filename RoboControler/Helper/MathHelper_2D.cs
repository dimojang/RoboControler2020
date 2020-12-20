using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoboControler.Helper
{
    class MathHelper_2D
    {
        public static double GetDistance(Vector2 startPoint, Vector2 endPoint)
        {
            float x = Math.Abs(endPoint.X - startPoint.X);
            float y = Math.Abs(endPoint.Y - startPoint.Y);
            return Math.Sqrt(x * x + y * y);
        }
    }
}
