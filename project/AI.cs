﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace project
{
    using Maneuver = Action<BattleData>;

    public static class Random
    {
        private static readonly List<System.Random> Randoms = new List<System.Random>();

        private static void InitRandoms()
        {
            int max = System.Threading.Thread.CurrentThread.ManagedThreadId;
            if (max < Randoms.Count) return;
			lock (Randoms)
			{
				max = max - Randoms.Count + 1;
				for (int i = 0; i < max; i++)
					Randoms.Add(new System.Random());
			}
        }
        public static byte[] NextBytes(byte[] buffer)
        {
            InitRandoms();
            Random.NextBytes(buffer);
            return buffer;
        }

        public static double NextDouble()
        {
            InitRandoms();
            return Randoms[System.Threading.Thread.CurrentThread.ManagedThreadId].NextDouble();
        }
        public static int Next()
        {
            InitRandoms();
            return Randoms[System.Threading.Thread.CurrentThread.ManagedThreadId].Next();
        }
        public static int Next(int minValue, int maxValue)
        {
            InitRandoms();
            return Random.Next(minValue, maxValue);
        }
        public static int Next(int maxValue)
        {
            InitRandoms();
            return Random.Next(maxValue);
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
                double R = Random.NextDouble() * 2d;
                if ( R >= Player.Aggression)
                    Maneuver = Strategy.Maneuvers[0];
                else if (R >= Player.Wairness+Player.Aggression)
                    Maneuver = Strategy.Maneuvers[1];              
                else
                    Maneuver = Strategy.Maneuvers[2];
                ManeuverTrust = 20*Player.Pride;
            }
        }

        private int EvaluateForces(){
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
