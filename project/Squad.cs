using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace project
{
     public class Squad
    {
        public readonly Unit Unit;
        private int amount;

        public int Amount
        {
            get { return amount; }
            private set { amount = value; }
        }
        public int DamageLeft;
        public Point Position;
     
        public Squad(Unit unit, int _amount)
        {
            Unit = unit;
            amount = _amount;
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
     }
}

