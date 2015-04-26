using System;
using System.Collections.Generic;
using System.Drawing;

namespace project
{
    using Step = KeyValuePair<Point, float>;
   class Deffensive : Strategy
	{
		static void Ambysh(BattleData battleData)
		{
            if (battleData.EnemyArmy.Length < 1)
                return;
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
                    Strategy.MoveAndAttack(battleData.AllyArmy[i], battleData.EnemyArmy[TargetIndex], Path, battleData);
			}

		}
		static void HitAndRun(BattleData battleData)
		{
            if (battleData.EnemyArmy.Length < 1)
                return;
			for (int i = 0; i < battleData.AllyArmy.Length; i++)
			{
				int TargetIndex = Strategy.NearestToPoint(battleData.AllyArmy[i].Position, battleData.EnemyArmy);

				Step[] Path = DistanceAndPath.PathTo(
					battleData,
					battleData.AllyArmy[i].Position,
					battleData.EnemyArmy[TargetIndex].Position,
					battleData.AllyArmy[i].Unit.Range);
                if (Path!=null)
				if (Path.Length == 0)
				{
					Point SafePoint = GetSafeFrom(battleData.AllyArmy[i].Position, battleData.EnemyArmy[TargetIndex].Position);

					Path = DistanceAndPath.PathTo(
					battleData,
					battleData.AllyArmy[i].Position,
					SafePoint,
					0);

					Strategy.AttackAndMove(battleData.AllyArmy[i], battleData.EnemyArmy[TargetIndex], Path, battleData);
				}
				else
                    Strategy.MoveAndAttack(battleData.AllyArmy[i],  battleData.EnemyArmy[TargetIndex], Path, battleData);
			}
		}


		public Deffensive()
		{
			Maneuvers[0] = Ambysh;
			Maneuvers[1] = HitAndRun;
		}
	}
}

