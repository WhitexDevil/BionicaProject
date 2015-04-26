using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace project
{
     public class Squad : ICloneable
    {
        public readonly Unit Unit;
        private int amount;
        public int DamageLeft;
        private Point position;
       
        public int Amount
        {
            get { return amount; }
            private set { amount = value; }
        }

        public Point Position
        {
            get { return position; }
            set { position = value; }
        }
     
        public Squad(Unit unit)
        {
            Unit = unit;
            amount = unit.MaxAmount;
            DamageLeft = 0;
            Position = new Point();
        }

        public void Attack(ref Squad target){
            int Damage = 0;
            for (int i = 0; i < Amount; i++){
                if ((1 + Random.Next(20) + Unit.Attack) >= target.Unit.Defense)
                    Damage += Unit.Damage;
             }

            target.TakeDamage(Damage);
        }

        private void TakeDamage(int Damage)
        {
            Damage += DamageLeft;
            Amount -= (Damage / Unit.MaxHitpoints);
            DamageLeft = Damage%Unit.MaxHitpoints;

            if (Amount < 0)
                Amount = 0;
        }
		/// <summary>
		/// Deep clone!
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new Squad(Unit) { DamageLeft = 0, Amount = amount, Position = position };
		}
	}
}

