using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace project
{
    public enum Race
    {
        Human, Elf, Dwarf,
        Undead, Daemon, Barbarian
    };


	public class Obstacle : Unit {}

    public class Unit
    {
        public int MaxHitpoints;
        public int MovementSpeed;
       // public Race Race;
        public int Defense;
        public int Attack;
        public float Range;
        public int Damage;
        public int MaxAmount;
        public int InitiativeMod;
       // public bool Magic;
       // public Texture2D Texture = null;


        public Unit(int initiative, int maxHitpoints, int defense, int attack, int damage, int movSpeed, int maxAmount, float range)
        {
            MaxHitpoints = maxHitpoints;
            Defense = defense;
            Attack = attack;
            Range = range;
            Damage = damage;
            InitiativeMod = initiative;
            MovementSpeed = movSpeed;
            MaxAmount = maxAmount;
            //Race = race;
           // Magic = magic;
        }
        public Unit()
        {
            MaxAmount = 0;
            MaxHitpoints = 0;
            Defense = 0;
            Attack = 0;
            Range = 0;
            Damage = 0;
            InitiativeMod = 0;

        }

    }
}

