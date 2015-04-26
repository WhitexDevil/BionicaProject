using Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    public struct BattleData : ICloneable
    {
        public readonly byte[] Map;
		public readonly int MapHeight;
		public readonly int MapWidth;
		public readonly int MapHeightLog2;

        public readonly Squad[] EnemyArmy;
        public readonly Squad[] AllyArmy;
        public readonly PathFinderFast PathFinder;

        public BattleData(Squad[] enemyArmy, Squad[] allyArmy, byte[] map, int mapWidth)
        {
            this.EnemyArmy = enemyArmy;
            this.AllyArmy = allyArmy;
            this.Map = map;
			this.MapWidth = mapWidth;
			this.MapHeight = map.Length / mapWidth;
			this.MapHeightLog2 = (int)(Math.Log(MapWidth, 2));
            this.PathFinder = new PathFinderFast(Map, MapWidth);
        }
        /// <summary>
        /// Deep clone!
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new BattleData((Squad[])EnemyArmy.Clone(), 
				(Squad[])AllyArmy.Clone(), (byte[])Map.Clone(), MapWidth);
        }
    }
}
