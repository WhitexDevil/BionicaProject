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
        public readonly Squad[,] Map;
        public readonly Squad[] EnemyArmy;
        public readonly Squad[] AllyArmy;
        public readonly PathFinderFast PathFinder;

        public BattleData(Squad[] enemyArmy, Squad[] allyArmy, Squad[,] map)
        {
            this.EnemyArmy = enemyArmy;
            this.AllyArmy = allyArmy;
            this.Map = map;
            this.PathFinder = new PathFinderFast(Map);
        }
        /// <summary>
        /// Deep clone!
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new BattleData((Squad[])EnemyArmy.Clone(), (Squad[])AllyArmy.Clone(), (Squad[,])Map.Clone());
        }
    }
}
