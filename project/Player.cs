using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    public class Player
    {

        public Player()
        {
            m_length = 4;
            Aggression = 0;
            Wairness = 0;
            Perception = 0;
            Pride = 0;
            CreateGenes();
        }
   
        public double Aggression;
        public double Wairness;
        public double Perception;
        public double Pride;

      //  public double[] m_genes;
        private int m_length;
        private double m_fitness;
        private static double m_mutationRate;

        public Player( double[] gens)
        {
            Aggression = 0;
            Wairness = 0;
            Perception = 0;
            Pride = 0;
            m_fitness = 0;
            m_length = gens.Length;
            for (int i = 0; i < gens.Length; i++)
            {
                this[i] = gens[i];
            }
        }
        /// <summary>
        /// Create genom with random gens
        /// </summary>
        /// <param name="create"></param>
        public Player(bool create,int count)
        {
            Aggression = 0;
            Wairness = 0;
            Perception = 0;
            Pride = 0;
            m_fitness = 0;
            m_length = count;
            if (create)
                CreateGenes();
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

        private void CreateGenes()
        {
            // DateTime d = DateTime.UtcNow;
            for (int i = 0; i < m_length; i++)
                this[i] = Random.NextDouble();
        }

        public void Crossover(ref Player genome2, out Player child1, out Player child2)
        {
            int pos = (int)(Random.NextDouble() * (double)m_length);
            double[] a = new double[4];
            double[] b = new double[4];
            child1 = new Player();
            child2 = new Player();
            for (int i = 0; i < m_length; i++)
            {
                if (i < pos)
                {
                    child1[i] = this[i];
                    child2[i] = genome2[i];
                }
                else
                {
                    child1[i] = genome2[i];
                    child2[i] = this[i];
                }
            }
        }

        public void Mutate()
        {
            for (int pos = 0; pos < m_length; pos++)
            {
                if (Random.NextDouble() < m_mutationRate)
                    this[pos] = (this[pos] + Random.NextDouble()) / 2.0;
            }
        }

        public double[] Genes()
        {
            double[] temp = new double[m_length];
            for (int i = 0; i < m_length; i++)
            {
                temp[i] = this[i];
            }
            return temp;
        }

        public void Output()
        {
            for (int i = 0; i < m_length; i++)
            {
                System.Console.WriteLine("{0:F4}", this[i]);
            }
            System.Console.Write("\n");
        }

        public void GetValues(ref double[] values)
        {
            for (int i = 0; i < m_length; i++)
                values[i] = this[i];
        }

        public double Fitness
        {
            get
            {
                return m_fitness;
            }
            set
            {
                m_fitness = value;
            }
        }

        public static double MutationRate
        {
            get
            {
                return m_mutationRate;
            }
            set
            {
                m_mutationRate = value;
            }
        }

        public int Length
        {
            get
            {
                return m_length;
            }
        }
    }
}
