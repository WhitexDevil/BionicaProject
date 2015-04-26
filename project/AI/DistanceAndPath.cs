using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algorithms;

namespace project
{
	using Step = KeyValuePair<Point, float>;
	class DistanceAndPath
	{
		static public double DistanceTo(Point p1, Point p2)
		{
			int a = (p1.X - p2.X);
			int b = (p1.Y - p2.Y);
			return Math.Sqrt(a * a + b * b);
		}
		static public Step[] PathTo(BattleData battleData, Point p1, Point p2, float range)
		{
			return battleData.PathFinder.FindPath(p1, p2, range);
		}
	}
}
 