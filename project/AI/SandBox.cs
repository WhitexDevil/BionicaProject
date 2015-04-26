using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algorithms;

namespace project
{

	class SandBox
	{
		private Player Enemy;
		private Player Player;

		private AI Side1;
		private AI Side2;
		private int MapSize;
		private BattleData BattleDataSide1;
        private BattleData BattleDataSide2;

		public SandBox(Player enemy, Player player, Squad[] enemyArmy, Squad[] allyArmy, int mapSize)
		{
			// TODO: Complete member initialization
			this.Enemy = enemy;
			this.Player = player;
			MapSize = mapSize;
            
			BattleDataSide1 = new BattleData(
				enemyArmy.Select(x => (Squad)x.Clone()).ToArray(),
				allyArmy.Select(x => (Squad)x.Clone()).ToArray(),
				new byte[mapSize * mapSize], mapSize);
         
            BattleDataSide2 = new BattleData(BattleDataSide1.AllyArmy, BattleDataSide1.EnemyArmy, BattleDataSide1.Map, BattleDataSide1.MapWidth);
            setMap();

	//		var p  = DistanceAndPath.PathTo(BattleData, BattleData.AllyArmy[0].Position, BattleData.EnemyArmy[0].Position, 2);
			Side1 = new AI(Player, BattleDataSide1);
			Side2 = new AI(Enemy, BattleDataSide2);

		}
		void setMap()
		{

            for (int i = 0; i <= BattleDataSide1.AllyArmy.Length / MapSize; i++)
			{
                int step = MapSize / Math.Min((BattleDataSide1.AllyArmy.Length - i * MapSize), MapSize);


                for (int j = i * MapSize; j < Math.Min(BattleDataSide1.AllyArmy.Length, (i + 1) * MapSize); j++)
				{
                    BattleDataSide1.Map[i + (((j % MapSize) * step) << BattleDataSide1.MapHeightLog2)] = 1;
                    BattleDataSide1.AllyArmy[j].Position = new Microsoft.Xna.Framework.Point(i, (j % MapSize) * step);
				}
			}

            for (int i = 0; i <= BattleDataSide1.EnemyArmy.Length / MapSize; i++)
			{
                int step = MapSize / Math.Min((BattleDataSide1.EnemyArmy.Length - i * MapSize), MapSize);
                for (int j = i * MapSize; j < Math.Min(BattleDataSide1.EnemyArmy.Length, (i + 1) * MapSize); j++)
				{
                    BattleDataSide1.Map[(BattleDataSide1.MapWidth - 1 - i) + (((j % MapSize) * step) << BattleDataSide1.MapHeightLog2)] = 1;
                    BattleDataSide1.EnemyArmy[j].Position = new Microsoft.Xna.Framework.Point((BattleDataSide1.MapWidth - 1 - i), (j % MapSize) * step);
				}
			}
		}

		public bool Fight()
		{
			while (Side1.Alive && Side2.Alive)
			{
				Side1.NextTurn();
				Side2.NextTurn();
			}
			return Side1.Alive;
		}
	}
}
