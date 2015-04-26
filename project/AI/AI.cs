using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace project
{
    using Maneuver = Action<BattleData>;

    public static class Random
    {
        private static readonly List<System.Random> Randoms = new List<System.Random>();

		private static System.Random GetRandom()
        {
            int cId = System.Threading.Thread.CurrentThread.ManagedThreadId;
			if (cId < Randoms.Count) return Randoms[cId];
			lock (Randoms)
			{
				for (int i = cId - Randoms.Count + 1; i > 0; i--)
					Randoms.Add(new System.Random());
			}
			return Randoms[cId];
        }
        public static byte[] NextBytes(byte[] buffer)
        {
			GetRandom().NextBytes(buffer);
            return buffer;
        }

        public static double NextDouble()
        {
			return GetRandom().NextDouble();
        }
        public static int Next()
        {
			return GetRandom().Next();
        }
        public static int Next(int minValue, int maxValue)
        {
			return GetRandom().Next(minValue, maxValue);
        }
        public static int Next(int maxValue)
        {
			return GetRandom().Next(maxValue);
        }
    }

    class AI
    {
       

        private Player Player;
        private BattleData BattleData;

        private double StrategyTrust = -1;
        private double ManeuverTrust = -1;
        private Strategy Strategy;
        private Maneuver Maneuver;
        public int ForceBalance;
        public bool Alive = true;

        //public static Func<Point, Point, Squad[,], KeyValuePair<Point[],double>> PathFinder;
        public AI(Player player, BattleData battleData)
        {
            // TODO: Complete member initialization
            this.Player = player;
            this.BattleData = battleData;
        }

        private void ChooseStrategy(){
            if (StrategyTrust <= 0)
            {
                if (Random.NextDouble() <= Player.Aggression)
                    Strategy = Strategy.Offensive;
                else Strategy = Strategy.Deffensive;
                ManeuverTrust = 0;
                ChooseManeuver();
                StrategyTrust = 100*Player.Pride;
            }
        }

        private void ChooseManeuver()
        {
            if (ManeuverTrust <= 0)
            {
                double R = Random.NextDouble();
                if (R <= Player.Wairness)
                    Maneuver = Strategy.Maneuvers[0];
                else 
                    Maneuver = Strategy.Maneuvers[1];              


                ManeuverTrust = 20*Player.Pride;
            }
        }

        private int EvaluateForces(){
            EraseDeadSquads();
            int result = 0;
            foreach (var squad in BattleData.AllyArmy)
                result += squad.Amount * squad.Unit.MaxHitpoints;
            if (result == 0)
            {
                Alive = false;
                return 0;
            }
            foreach (var squad in BattleData.AllyArmy)
                result -= squad.Amount * squad.Unit.MaxHitpoints;
            return result;
        }

        private void EraseDeadSquads()
        {
             int aliveSq=0;
             bool change = false;
             foreach (var item in BattleData.AllyArmy)
	            {
                    if (item.Alive)
                        aliveSq++;
                    else
                    {
                        BattleData.EraseFromMap(item.Position);
                        change =true;
                    }
	            }
            if (!change)
                return;
            Squad[] temp = new Squad[aliveSq];
            int index=0;
            foreach (var item in BattleData.AllyArmy)
	            {
		            if (item.Alive)
                    {
                        temp[index]=item;
                        index++;
                    }
	            }

            BattleData.AllyArmy = temp;
        }

        public void NextTurn()
        {
            int NewForceBalance = EvaluateForces();
            int DeltaBalance = ForceBalance - NewForceBalance;
            double DeltaTrust = DeltaBalance * (Player.Perception) * (1 + Math.Sign(DeltaBalance) * Player.Pride);
            StrategyTrust += DeltaTrust;
            ManeuverTrust += DeltaTrust;
            ForceBalance = NewForceBalance;

            ChooseStrategy();
            ChooseManeuver();
            Maneuver(BattleData);
        }


    }
}
