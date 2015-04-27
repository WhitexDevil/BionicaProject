using Algorithms;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
	public class BattleData : ICloneable
	{
		public byte[] Map;
		public readonly int MapHeight;
		public readonly int MapWidth;
		public readonly int MapHeightLog2;

		public bool Reverse = false;
		private Squad[] ArmyA;
		private Squad[] ArmyB;


		public Squad[] EnemyArmy
		{
			get { return Reverse ? ArmyA : ArmyB; }
			set
			{
				if (Reverse) ArmyA = value;
				else ArmyB = value;
			}
		}
		public Squad[] AllyArmy
		{
			get { return Reverse ? ArmyB : ArmyA; }
			set
			{
				if (Reverse) ArmyB = value;
				else ArmyA = value;
			}
		}
		public readonly PathFinderFast PathFinder;
		public Visualization Visualization = null;

		public BattleData(Squad[] enemyArmy, Squad[] allyArmy, byte[] map, int mapWidth)
		{
			this.EnemyArmy = enemyArmy;
			this.AllyArmy = allyArmy;
			this.Map = map;
			this.MapWidth = mapWidth;
			this.MapHeight = map.Length / mapWidth;
			this.MapHeightLog2 = (int)(Math.Log(MapWidth, 2));
			this.PathFinder = new PathFinderFast(Map, MapWidth);
			PathFinder.PathFinderDebug += PathFinder_PathFinderDebug;
		}

	



		void PathFinder_PathFinderDebug(int fromX, int fromY, int x, int y, PathFinderNodeType type, int totalCost, float cost)
		{
			//	Console.WriteLine("{0}&{1}to{2}&{3} total {4} cost {5}", fromX, fromY, x, y, totalCost, cost);
		}

		public void Relocate(Point oldP, Point newP)
		{
			int oldIndex = (oldP.Y << MapHeightLog2) + oldP.X;
			int newIndex = (newP.Y << MapHeightLog2) + newP.X;
			Map[newIndex] = Map[oldIndex];
			Map[oldIndex] = 0;
		}

		public void EraseDeadSquads()
		{
			EraseDeadSquads(ref ArmyA);
			EraseDeadSquads(ref ArmyB);
		}
		private void EraseDeadSquads(ref Squad[] Army)
		{
			int aliveSq = 0;
			bool change = false;
			foreach (var item in Army)
			{
				if (item.Alive)
					aliveSq++;
				else
				{
					EraseFromMap(item.Position);
					change = true;
				}
			}
			if (!change)
				return;
			Squad[] temp = new Squad[aliveSq];
			int index = 0;
			foreach (var item in Army)
			{
				if (item.Alive)
				{
					temp[index] = item;
					index++;
				}
			}

			Army = temp;
		}

		private void EraseFromMap(Point p)
		{
			int Index = (p.Y << MapHeightLog2) + p.X;
			Map[Index] = 0;
		}
		/// <summary>
		/// Deep clone!
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new BattleData(EnemyArmy.Select(x => (Squad)x.Clone()).ToArray(),
				AllyArmy.Select(x => (Squad)x.Clone()).ToArray(), (byte[])Map.Clone(), MapWidth);
		}
	}
}
