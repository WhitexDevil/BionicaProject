using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    class DistanceAndPath
    {
        static public  double DistanceTo(Point p1 , Point p2)
        {
            int a = (p1.X - p2.X);
            int b = (p1.Y - p2.Y);
            return Math.Sqrt(a * a + b * b);
        }
        static public KeyValuePair<Point, double>[] PathTo(Squad[,] map,Point p1,Point p2,Double range)
        {
            return  null;
        }
    }
}
