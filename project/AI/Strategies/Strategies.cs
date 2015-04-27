using System;
using System.Collections.Generic;
using System.Drawing;

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
			int Temp = -1;
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
			int Temp = -1;
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

		protected static void AttackAndMove(Squad attacker, Squad target, Step[] Path, BattleData bd)
		{
			int dmg = target.Amount;
			attacker.Attack(target);

			if (bd.Visualization != null)
				bd.Visualization.RecordAttack(attacker, target, dmg - target.Amount);

			Move(attacker, Path, bd);
		}

		protected static void MoveAndAttack(Squad attacker, Squad target, Step[] Path, BattleData bd)
		{

			if (Move(attacker, Path, bd))
			{
				int dmg = target.Amount;
				attacker.Attack(target);

				if (bd.Visualization != null)
					bd.Visualization.RecordAttack(attacker, target, dmg - target.Amount);
			}

		}

		protected static bool Move(Squad mover, Step[] Path, BattleData bd)
		{
			if (Path == null)
				return false;
			int length = Path.Length;
			if (length < 1)
				return true;
			double movement = mover.Unit.MovementSpeed;
			Point temp = Path[0].Key;

			for (int k = length - 1; k > 0; k--)
			{
				if (Path[k].Value > movement)
				{
					temp = Path[k + 1].Key;

					if (bd.Visualization != null)
						bd.Visualization.RecordMove(mover, mover.Position, temp, Path);

					bd.Relocate(mover.Position, temp);
					mover.Position = temp;
					return false;
				}
			}

			if (bd.Visualization != null)
				bd.Visualization.RecordMove(mover, mover.Position, temp, Path);

			bd.Relocate(mover.Position, temp);
			mover.Position = temp;
			return true;
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
}


