using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace project
{
    using Step = KeyValuePair<Point, float>;
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

