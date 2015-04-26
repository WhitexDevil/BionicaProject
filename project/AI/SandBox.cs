using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    CurrentBattleData.AllyArmy[j].Position = new Microsoft.Xna.Framework.Point(i, (j % MapSize) * step);
				}
			}

            for (int i = 0; i <= CurrentBattleData.EnemyArmy.Length / MapSize; i++)
			{
                int step = MapSize / Math.Min((CurrentBattleData.EnemyArmy.Length - i * MapSize), MapSize);
                for (int j = i * MapSize; j < Math.Min(CurrentBattleData.EnemyArmy.Length, (i + 1) * MapSize); j++)
				{
                    CurrentBattleData.Map[(CurrentBattleData.MapWidth - 1 - i) + (((j % MapSize) * step) << CurrentBattleData.MapHeightLog2)] = 1;
                    CurrentBattleData.EnemyArmy[j].Position = new Microsoft.Xna.Framework.Point((CurrentBattleData.MapWidth - 1 - i), (j % MapSize) * step);
				}
			}
		}

        private int EvaluateForces()
        {
            EraseDeadSquads(ref CurrentBattleData.AllyArmy);
            
            int sumAlly= 0;
            foreach (var squad in CurrentBattleData.AllyArmy)
                sumAlly += squad.Amount * squad.Unit.MaxHitpoints;
            if (sumAlly == 0)
            {
                Finish = true;
                Win = false;
                
            }
            EraseDeadSquads(ref CurrentBattleData.EnemyArmy);
            int sumEnemy = 0;
            foreach (var squad in CurrentBattleData.EnemyArmy)
                sumEnemy += squad.Amount * squad.Unit.MaxHitpoints;
            if (sumEnemy == 0)
            {
                Finish = true;
                Win = true;

            }
            return sumAlly-sumEnemy;
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
                    CurrentBattleData.EraseFromMap(item.Position);
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

		public bool Fight()
		{
			while (!Finish)
			{
                int NewForceBalance = EvaluateForces();
                int DeltaBalance = ForceBalance - NewForceBalance;
               
                ForceBalance = NewForceBalance;
                Side1.NextTurn(DeltaBalance)(CurrentBattleData);
                Side2.NextTurn(-DeltaBalance)(BattleData.Reverse(CurrentBattleData));
			}
			return Win;
		}
	}
}
