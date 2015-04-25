using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    using Microsoft.Xna.Framework;
    using Maneuver = Action<BattleData>;
    public class Strategy
    {
        public static Strategy Offensive = new Offensive();
        public static Strategy Deffensive = new Deffensive();

        public readonly Maneuver[] Maneuvers;

        public static int NearestToPoint(Point p1, Squad[] Army)
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
        public static int NearestToAll(Squad[] Army, Squad[] Targets)
        {
           int Temp = 0;
           double[] distances = new double[Targets.Length];

            for (int i = 0; i < Targets.Length; i++)
            {
                for (int j = 0; j < Army.Length; j++)
			    {
			        distances[i]+= DistanceAndPath.DistanceTo(Army[j].Position , Targets[i].Position);
			    }
            }

            double min = double.MaxValue;
            for (int i = 0; i < distances.Length; i++)
			{
			    if(distances[i]<min)
                {
                    min = distances[i];
                    Temp = i;
                }
			}
            
            return Temp;
        }

    }

    public class Offensive : Strategy
    {
        public static void Surround(BattleData battleData)
        {
            int TargetIndex = Strategy.NearestToAll(battleData.AllyArmy, battleData.EnemyArmy);
            for (int i = 0; i < battleData.AllyArmy.Length; i++)
            {

                KeyValuePair<Point, double>[] Path = DistanceAndPath.PathTo(
                    battleData.Map,
                    battleData.AllyArmy[i].Position,
                    battleData.EnemyArmy[TargetIndex].Position,
                    battleData.AllyArmy[i].Unit.Range);

                double movement = battleData.AllyArmy[i].Unit.MovementSpeed;
                Point temp = new Point(-1, -1);
                for (int k = 0; k < Path.Length; k++)
                {
                    if (Path[k].Value > movement)
                    {
                        temp = Path[k - 1].Key;
                        break;
                    }
                }
                if (temp == new Point(-1, -1))
                {
                    battleData.AllyArmy[i].Position = Path[Path.Length - 1].Key;
                    battleData.AllyArmy[i].Attack(ref battleData.EnemyArmy[TargetIndex]);
                }
                else
                    battleData.AllyArmy[i].Position = temp;
            }
        }
        public static void Rush(BattleData battleData)
        {
            for (int i = 0; i < battleData.AllyArmy.Length; i++)
            {
                int TargetIndex = Strategy.NearestToPoint(battleData.AllyArmy[i].Position, battleData.EnemyArmy);

                KeyValuePair<Point, double>[] Path = DistanceAndPath.PathTo(
                    battleData.Map,
                    battleData.AllyArmy[i].Position,
                    battleData.EnemyArmy[TargetIndex].Position,
                    battleData.AllyArmy[i].Unit.Range);

                double movement = battleData.AllyArmy[i].Unit.MovementSpeed;
                Point temp = new Point(-1, -1);
                for (int k = 0; k < Path.Length; k++)
                {
                    if (Path[k].Value > movement)
                    {
                        temp = Path[k - 1].Key;
                        break;
                    }
                }
                if (temp == new Point(-1, -1))
                {
                    battleData.AllyArmy[i].Position = Path[Path.Length - 1].Key;
                    battleData.AllyArmy[i].Attack(ref battleData.EnemyArmy[TargetIndex]);
                }
                else
                    battleData.AllyArmy[i].Position = temp;
            }
        }
        public Offensive()
        {
            Maneuvers[0] = Rush;
        }
    }

    class Deffensive : Strategy
    {
        public Deffensive()
        {
            //Maneuvers[0] = ;
        }
    }
}
