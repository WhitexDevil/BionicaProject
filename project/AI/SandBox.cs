using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Algorithms;

namespace project
{
	using Maneuver = Action<BattleData>;

	class SandBox
	{
		private Player Enemy;
		private Player Player;
		public int ForceBalance;
		private AI Side1;
		private AI Side2;
		private int MapSize;
        private BattleData CurrentBattleData;

        public BattleData BattleData
        {
            get { return CurrentBattleData; }
           private set { CurrentBattleData = value; }
        }
		bool Finish;
		bool Win;
		//private BattleData BattleDataSide2;

		public SandBox(Player enemy, Player player, Squad[] enemyArmy, Squad[] allyArmy, int mapSize)
		{
			// TODO: Complete member initialization
			this.Enemy = enemy;
			this.Player = player;
			MapSize = mapSize;

			CurrentBattleData = new BattleData(
				enemyArmy.Select(x => (Squad)x.Clone()).ToArray(),
				allyArmy.Select(x => (Squad)x.Clone()).ToArray(),
				new byte[mapSize * mapSize], mapSize);

			//BattleDataSide2 = new BattleData(BattleDataSide1.AllyArmy, BattleDataSide1.EnemyArmy, BattleDataSide1.Map, BattleDataSide1.MapWidth);
			setMap();
			if (CurrentBattleData.Visualization != null)
				CurrentBattleData.Visualization.InitZeroState(CurrentBattleData);
			//		var p  = DistanceAndPath.PathTo(BattleData, BattleData.AllyArmy[0].Position, BattleData.EnemyArmy[0].Position, 2);
			Side1 = new AI(Player, CurrentBattleData);
			Side2 = new AI(Enemy, CurrentBattleData);

		}
		void setMap()
		{

			for (int i = 0; i <= CurrentBattleData.AllyArmy.Length / MapSize; i++)
			{
				int step = MapSize / Math.Min((CurrentBattleData.AllyArmy.Length - i * MapSize), MapSize);


				for (int j = i * MapSize; j < Math.Min(CurrentBattleData.AllyArmy.Length, (i + 1) * MapSize); j++)
				{
					CurrentBattleData.Map[i + (((j % MapSize) * step) << CurrentBattleData.MapHeightLog2)] = 1;
					CurrentBattleData.AllyArmy[j].Position = new Point(i, (j % MapSize) * step);
				}
			}

			for (int i = 0; i <= CurrentBattleData.EnemyArmy.Length / MapSize; i++)
			{
				int step = MapSize / Math.Min((CurrentBattleData.EnemyArmy.Length - i * MapSize), MapSize);
				for (int j = i * MapSize; j < Math.Min(CurrentBattleData.EnemyArmy.Length, (i + 1) * MapSize); j++)
				{
					CurrentBattleData.Map[(CurrentBattleData.MapWidth - 1 - i) + (((j % MapSize) * step) << CurrentBattleData.MapHeightLog2)] = 1;
					CurrentBattleData.EnemyArmy[j].Position = new Point((CurrentBattleData.MapWidth - 1 - i), (j % MapSize) * step);
				}
			}
		}

		private int EvaluateForces()
		{
			CurrentBattleData.EraseDeadSquads();

			int sumAlly = 0;
			foreach (var squad in CurrentBattleData.AllyArmy)
				sumAlly += squad.Amount * squad.Unit.MaxHitpoints;
			if (sumAlly == 0)
			{
				Finish = true;
				Win = false;
			}

			int sumEnemy = 0;
			foreach (var squad in CurrentBattleData.EnemyArmy)
				sumEnemy += squad.Amount * squad.Unit.MaxHitpoints;
			if (sumEnemy == 0)
			{
				Finish = true;
				Win = true;
			}
			return sumAlly - sumEnemy;
		}



		public bool Fight()
		{
			int Turns = 0;
			while (!Finish)
			{
				Turns++;
				int NewForceBalance = EvaluateForces();
				int DeltaBalance = ForceBalance - NewForceBalance;
				
				if (Turns % 10 == 0)
				Console.WriteLine("Turn {0}, force balance {1}, delta force {2}", Turns, NewForceBalance, DeltaBalance);

				ForceBalance = NewForceBalance;

				Side1.NextTurn(DeltaBalance)(CurrentBattleData);
				CurrentBattleData.Reverse = !CurrentBattleData.Reverse;
				Side2.NextTurn(-DeltaBalance)(CurrentBattleData);
				CurrentBattleData.Reverse = !CurrentBattleData.Reverse;

				if (CurrentBattleData.Visualization != null)
					CurrentBattleData.Visualization.RecordEndOfTurn();
			}
			return Win;
		}
	}
}
