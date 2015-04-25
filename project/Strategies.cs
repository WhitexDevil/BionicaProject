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

    }

    public class Offensive : Strategy
    {
        public static void Rush(BattleData battleData)
        {
            for (int i = 0; i < battleData.AllyArmy.Length; i++)
			{
                
                    Squad Target;
                    //= new KeyValuePair<Point,double>[](null, double.MaxValue);
                    double minDistanse=Double.MaxValue;
                    foreach (var enemy in battleData.EnemyArmy)
                    {
                        double distance = DistanseAndPath.DistanceTo(battleData.AllyArmy[i].Position,enemy.Position);
                        if (distance < minDistanse)
                        {
                            minDistanse = distance;
                            Target = enemy;
                        }
                   
                    }

                    //int move = battleData.AllyArmy[i].Unit.MovementSpeed >Path.Key.Length- battleData.AllyArmy[i].Unit.Range?Path.Key.Length- battleData.AllyArmy[i].Unit.Range:battleData.AllyArmy[i].Unit.MovementSpeed; /// fix me TT_TT
                    
                KeyValuePair<Point, double>[] Path = DistanseAndPath.PathTo(battleData.Map);
                double movement =  battleData.AllyArmy[i].Unit.MovementSpeed;
                Point 
                foreach (var PathPoint in Path)
	            {
                    if (PathPoint.Value > movement)
                        break;

	            }
                       
                        
                                                                                                                     
                                                                                                      
                   battleData.AllyArmy[i].Position = Path.Key[move];
                   // if 
                      /// DISTANSE AND ATTTTTTTAAAAAACKKKK!!!!!! Waaghhh!!!
                

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
