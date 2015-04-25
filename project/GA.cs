using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    struct Player
    {
        public double Aggression;
        public double Wairness;
        public double Perception;
        public double Pride;

        public int fitness;
        public Player(params double[] gens)
        {
            Aggression = 0;
            Wairness = 0;
            Perception = 0;
            Pride = 0;
            fitness = 0;
            for (int i = 0; i < gens.Length; i++)
            {
                this[i] = gens[i];
            }
        }
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Aggression;
                    case 1: return Wairness;
                    case 2: return Perception;
                    default: return Pride;
                }
            }
            set
            {
                switch (index)
                {
                    case 0: Aggression = value; break;
                    case 1: Wairness = value; break;
                    case 2: Perception = value; break;
                    default: Pride = value; break;
                }
            }
        }
    }

    class GA
    {
        Player Enemy;
        Player[] generation;
        int BattleCount;

        void fitness()
        {
            for (int i = 0; i < generation.Length; i++)
            {
                int wins = 0;
                SandBox sb = new SandBox(Enemy, generation[i]);
                for (int j = 0; j < BattleCount; j++)
                    if (sb.Fight())
                        wins++;

                generation[i].fitness = wins;
            }
        }

    }
}
