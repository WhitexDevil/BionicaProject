using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace project
{
	using Maneuver = Action<BattleData>;
	using Step = KeyValuePair<Point, float>;
	public class Strategy
	{
		public static Strategy Offensive = new Offensive();
		public static Strategy Deffensive = new Deffensive();

		public readonly Maneuver[] Maneuvers;

		protected Strategy()
		{
			Maneuvers = new Maneuver[2];
		}
		protected static int NearestToPoint(Point p1, Squad[] Army)
		{
			int Temp = 0;
			double minDistance = Double.MaxValue;
			for (int i = 0; i < Army.Length; i++)
			{
				double distance = DistanceAndPath.DistanceTo(p1, Army[i].Position);
				if (distance < minDistance)
				{
					minDistance = distance;
					Temp = i;
				}
			}
			return Temp;
		}
		/// <summary>
		/// Chose Nearest target to all army squads
		/// </summary>
		/// <param name="Army"></param>
		/// <param name="Targets"></param>
		/// <returns></returns>
		protected static int NearestToAll(Squad[] Army, Squad[] Targets)
		{
			int Temp = 0;
			double[] distances = new double[Targets.Length];

			for (int i = 0; i < Targets.Length; i++)
			{
				for (int j = 0; j < Army.Length; j++)
				{
					distances[i] += DistanceAndPath.DistanceTo(Army[j].Position, Targets[i].Position);
				}
			}

			double min = double.MaxValue;
			for (int i = 0; i < distances.Length; i++)
			{
				if (distances[i] < min)
				{
					min = distances[i];
					Temp = i;
				}
			}

			return Temp;
		}

		protected static void MoveAndAttak(Squad attaker,  Squad target, Step[] Path , BattleData bd)
		{

			if (Move(attaker, Path,bd)) attaker.Attack(ref target);

		}

      


        protected static bool Move(Squad mover, Step[] Path, BattleData bd)
		{
           if (Path.Length < 1)
                return true;
            double movement = mover.Unit.MovementSpeed;
			Point temp = new Point(-1, -1);
			int length = Path.Length;
			if (length < 0)
				return true;
            for (int k = length; k < 0; k--)
			{
				if (Path[k].Value > movement)
				{
					temp = Path[k + 1].Key;
					break;
				}
			}
			if (temp == new Point(-1, -1))
			{
                temp =Path[0].Key;
                bd.relocated(mover.Position,temp);
                mover.Position =temp ;
				return true;
			}
			else
                bd.relocated(mover.Position, temp);
                mover.Position = temp;
			return false;
		}

		protected static Point GetSafeFrom(Point Victum, Point Enemy)
		{
			Point temp = new Point(Victum.X - Enemy.X, Victum.Y - Enemy.Y);
			List<Point> SafePlaces = new List<Point>();
			if (temp.X <= 0)
				SafePlaces.Add(new Point(0, Victum.Y));
			if (temp.X >= 0)
				SafePlaces.Add(new Point(99, Victum.Y));
			if (temp.Y <= 0)
				SafePlaces.Add(new Point(Victum.X, 0));
			if (temp.Y >= 0)
				SafePlaces.Add(new Point(Victum.X, 99));

			double max = double.MinValue;
			foreach (var item in SafePlaces)
			{
				double dist = DistanceAndPath.DistanceTo(Victum, item);
				if (dist > max)
				{
					max = dist;
					temp = item;
				}
			}

			return temp;
		}

	}

	public class Offensive : Strategy
	{
		static void Surround(BattleData battleData)
		{
			int TargetIndex = Strategy.NearestToAll(battleData.AllyArmy, battleData.EnemyArmy);
			for (int i = 0; i < battleData.AllyArmy.Length; i++)
			{
				Step[] Path = DistanceAndPath.PathTo(
					battleData,
					battleData.AllyArmy[i].Position,
					battleData.EnemyArmy[TargetIndex].Position,
					battleData.AllyArmy[i].Unit.Range);

				Strategy.MoveAndAttak(battleData.AllyArmy[i], battleData.EnemyArmy[TargetIndex], Path , battleData);
			}

		}
		static void Rush(BattleData battleData)
		{
			for (int i = 0; i < battleData.AllyArmy.Length; i++)
			{
				int TargetIndex = Strategy.NearestToPoint(battleData.AllyArmy[i].Position, battleData.EnemyArmy);

				Step[] Path = DistanceAndPath.PathTo(
					battleData,
					battleData.AllyArmy[i].Position,
					battleData.EnemyArmy[TargetIndex].Position,
					battleData.AllyArmy[i].Unit.Range);

                Strategy.MoveAndAttak(battleData.AllyArmy[i], battleData.EnemyArmy[TargetIndex], Path, battleData);


			}
		}
		public Offensive()
		{

			Maneuvers[0] = Surround;
			Maneuvers[1] = Rush;
		}
	}

	class Deffensive : Strategy
	{
		static void Ambysh(BattleData battleData)
		{
			for (int i = 0; i < battleData.AllyArmy.Length; i++)
			{
				int TargetIndex = Strategy.NearestToPoint(battleData.AllyArmy[i].Position, battleData.EnemyArmy);

				Step[] Path = DistanceAndPath.PathTo(
					battleData,
					battleData.AllyArmy[i].Position,
					battleData.EnemyArmy[TargetIndex].Position,
					battleData.AllyArmy[i].Unit.Range);
                if (Path!=null && Path.Length>0)
				if (Path[Path.Length - 1].Value <= battleData.AllyArmy[i].Unit.MovementSpeed)
                    Strategy.MoveAndAttak(battleData.AllyArmy[i], battleData.EnemyArmy[TargetIndex], Path, battleData);
			}

		}
		static void HitAndRun(BattleData battleData)
		{
			for (int i = 0; i < battleData.AllyArmy.Length; i++)
			{
				int TargetIndex = Strategy.NearestToPoint(battleData.AllyArmy[i].Position, battleData.EnemyArmy);

				Step[] Path = DistanceAndPath.PathTo(
					battleData,
					battleData.AllyArmy[i].Position,
					battleData.EnemyArmy[TargetIndex].Position,
					battleData.AllyArmy[i].Unit.Range);

				if (Path.Length == 0)
				{
					battleData.AllyArmy[i].Attack(ref battleData.EnemyArmy[TargetIndex]);
					Point SafePoint = GetSafeFrom(battleData.AllyArmy[i].Position, battleData.EnemyArmy[TargetIndex].Position);

					Path = DistanceAndPath.PathTo(
					battleData,
					battleData.AllyArmy[i].Position,
					SafePoint,
					0);

                    Move(battleData.AllyArmy[i], Path, battleData);
				}
				else
                    Strategy.MoveAndAttak(battleData.AllyArmy[i],  battleData.EnemyArmy[TargetIndex], Path, battleData);
			}
		}


		public Deffensive()
		{
			Maneuvers[0] = Ambysh;
			Maneuvers[1] = HitAndRun;
		}
	}
}
