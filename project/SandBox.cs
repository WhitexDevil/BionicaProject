using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    struct BattleData : ICloneable
        {
            public readonly Squad[,] Map;
            public readonly Squad[] EnemyArmy;
            public readonly Squad[] AllyArmy;

            public BattleData(Squad[] enemyArmy, Squad[] allyArmy, Squad[,] map)
            {
                this.EnemyArmy = enemyArmy;
                this.AllyArmy = allyArmy;
                this.Map = map;
            }
            /// <summary>
            /// Deep clone!
            /// </summary>
            /// <returns></returns>
            public BattleData Clone()
            {
                return  BattleData(EnemyArmy.Clone(), AllyArmy.Clone(), Map.Clone());
            }
        }
    class SandBox
    {
        private Player Enemy;
        private Player Player;

        private AI Side1;
        private AI Side2;

        private BattleData BattleData;

        public SandBox(Player enemy, Player player, Squad[] enemyArmy, Squad[] allyArmy)
        {
            // TODO: Complete member initialization
            this.Enemy = enemy;
            this.Player = player;
            BattleData = new BattleData(enemyArmy, allyArmy, new Squad[100, 100]);
            setMap();

            Side1 = new AI(Player, BattleData);
            Side2 = new AI(Enemy, BattleData);

        }
        void setMap()
        {
            for (int i = 0; i < BattleData.AllyArmy.Length / 100; i++)
            {
                int step = 100 / Math.Min((BattleData.AllyArmy.Length - i * 100), 100);
                for (int j = i * 100; j < Math.Min(BattleData.AllyArmy.Length, (i + 1) * 100); j++)
                {
                    BattleData.Map[i, (j % 100) * step] = BattleData.AllyArmy[j];

                }
            }

            int temp = BattleData.Map.GetLength(1);
            for (int i = 0; i < BattleData.EnemyArmy.Length / 100; i++)
            {
                int step = 100 / Math.Min((BattleData.EnemyArmy.Length - i * 100), 100);
                for (int j = i * 100; j < Math.Min(BattleData.EnemyArmy.Length, (i + 1) * 100); j++)
                {
                    BattleData.Map[temp - i, (j % 100) * step] = BattleData.EnemyArmy[j];

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
