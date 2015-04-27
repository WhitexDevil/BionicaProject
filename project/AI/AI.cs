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
                StrategyTrust = 500*Player.Pride;
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


                ManeuverTrust = 50*Player.Pride;
            }
        }

        public Maneuver NextTurn(double DeltaBalance)
        {
            if (DeltaBalance == 0)
                DeltaBalance = -5; 
            double DeltaTrust =DeltaBalance * ((Player.Perception) + (1 + Math.Sign(DeltaBalance) * Player.Pride));
            StrategyTrust += DeltaTrust;
            ManeuverTrust += DeltaTrust;

            ChooseStrategy();
            ChooseManeuver();
            return Maneuver;
        }


    }
}
