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
		private BattleData BattleData;

		public SandBox(Player enemy, Player player, Squad[] enemyArmy, Squad[] allyArmy, int mapSize)
		{
			// TODO: Complete member initialization
			this.Enemy = enemy;
			this.Player = player;
			MapSize = mapSize;
			BattleData = new BattleData(
				enemyArmy.Select(x => (Squad)x.Clone()).ToArray(),
				allyArmy.Select(x => (Squad)x.Clone()).ToArray(),
				new byte[mapSize * mapSize], mapSize);
			setMap();
	//		var p  = DistanceAndPath.PathTo(BattleData, BattleData.AllyArmy[0].Position, BattleData.EnemyArmy[0].Position, 2);
			Side1 = new AI(Player, BattleData);
			Side2 = new AI(Enemy, BattleData);

		}
		void setMap()
		{

			for (int i = 0; i <= BattleData.AllyArmy.Length / MapSize; i++)
			{
				int step = MapSize / Math.Min((BattleData.AllyArmy.Length - i * MapSize), MapSize);


				for (int j = i * MapSize; j < Math.Min(BattleData.AllyArmy.Length, (i + 1) * MapSize); j++)
				{
					BattleData.Map[i + (((j % MapSize) * step) << BattleData.MapHeightLog2)] = 1;
					BattleData.AllyArmy[j].Position = new Microsoft.Xna.Framework.Point(i, (j % MapSize) * step);
				}
			}

			for (int i = 0; i <= BattleData.EnemyArmy.Length / MapSize; i++)
			{
				int step = MapSize / Math.Min((BattleData.EnemyArmy.Length - i * MapSize), MapSize);
				for (int j = i * MapSize; j < Math.Min(BattleData.EnemyArmy.Length, (i + 1) * MapSize); j++)
				{
					BattleData.Map[(BattleData.MapWidth - i) + (((j % MapSize) * step) << BattleData.MapHeightLog2)] = 1;
					BattleData.EnemyArmy[j].Position = new Microsoft.Xna.Framework.Point((BattleData.MapWidth - i), (j % MapSize) * step);
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
