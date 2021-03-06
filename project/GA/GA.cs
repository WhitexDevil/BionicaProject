﻿using project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
namespace project
{
    public delegate double GAFunction(Player player, Player Enemy, Squad[] allyArmy, Squad[] enemyArmy);

    /// <summary>
    /// Genetic Algorithm class
    /// </summary>
    public class GA
    {
        /// <summary>
        /// Default constructor sets mutation rate to 5%, crossover to 80%, population to 10,
        /// and generations to 10.
        /// </summary>
        public GA()
        {
            InitialValues();
            m_mutationRate = 0.05;
            m_crossoverRate = 0.80;
            m_populationSize = 2;
            m_generationSize = 1;
            m_battleCount = 1;
            m_goalFitness = 1;




        }
        /// <summary>
        /// Sets the Equal army to both sides
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="Army"></param>
        /// <param name="crossoverRate"></param>
        /// <param name="mutationRate"></param>
        /// <param name="populationSize"></param>
        /// <param name="generationSize"></param>
        /// <param name="genomeSize"></param>
        public GA(
            Player enemy,
            Squad[] Army,
            double crossoverRate = 0.8,
            double mutationRate = 0.05,
            double goalFitness = 0.9,
            int populationSize = 2,
            int generationSize = 2,
            int battleCount = 5,
            int mapSize = 64)
            : this(enemy, Army, Army, crossoverRate, mutationRate, goalFitness, populationSize, generationSize, battleCount, mapSize) { }

        /// <summary>
        /// Sets different army to each side;
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="enemyArmy"></param>
        /// <param name="allyArmy"></param>
        /// <param name="crossoverRate"></param>
        /// <param name="mutationRate"></param>
        /// <param name="populationSize"></param>
        /// <param name="generationSize"></param>
        /// <param name="genomeSize"></param>
        public GA(
            Player enemy,
            Squad[] enemyArmy,
            Squad[] allyArmy,
            double crossoverRate = 0.8,
            double mutationRate = 0.05,
            double goalFitness = 0.9,
            int populationSize = 2,
            int generationSize = 2,
            int battleCount = 5,
            int mapSize = 64)
        {
            InitialValues();
            m_mutationRate = mutationRate;
            m_crossoverRate = crossoverRate;
            m_goalFitness = goalFitness;
            m_populationSize = populationSize + populationSize % 2;
            m_generationSize = generationSize;
            // m_strFitness = "";
            Enemy = enemy;
            AllyArmy = allyArmy;
            EnemyArmy = enemyArmy;
            m_battleCount = battleCount;
            m_mapSize = mapSize;

        }




        public void InitialValues()
        {
            m_elitism = false;
        }


        /// <summary>
        /// Method which starts the GA executing.
        /// </summary>
        public void Go()
        {



            //  Create the fitness table.
            m_fitnessTable = new ArrayList();
            m_thisGeneration = new ArrayList(m_generationSize);
            m_nextGeneration = new ArrayList(m_generationSize);
            Player.MutationRate = m_mutationRate;


            CreateGenomes();
            RankPopulation();

            for (m_currentGeneration = 1; m_currentGeneration <= m_generationSize; m_currentGeneration++)
            {
                CreateNextGeneration();
                RankPopulation();

            }



        }

        /// <summary>
        /// After ranking all the genomes by fitness, use a 'roulette wheel' selection
        /// method.  This allocates a large probability of selection to those with the 
        /// highest fitness.
        /// </summary>
        /// <returns>Random individual biased towards highest fitness</returns>
        private int RouletteSelection()
        {
            double randomFitness = Random.NextDouble() * (double)m_fitnessTable[m_populationSize - 1];
            int idx = -1;
            for (int i = 0; i < m_populationSize; i++)
                if (randomFitness <= (double)m_fitnessTable[i])
                {
                    idx = i;
                    break;
                }

            return idx;
        }

        /// <summary>
        /// Rank population and sort in order of fitness.
        /// </summary>
        private void RankPopulation()
        {
            m_totalFitness = 0.0;
            for (int i = 0; i < m_populationSize; i++)
            {
                Player g = ((Player)m_thisGeneration[i]);
                g.WinRate = FitnessFunction(g);

                m_totalFitness += g.Fitness;
            }
            m_thisGeneration.Sort(new GenomeComparer());

            //  now sorted in order of fitness.
            double fitness = 0.0;
            m_fitnessTable.Clear();
            for (int i = 0; i < m_populationSize; i++)
            {
                fitness += ((Player)m_thisGeneration[i]).Fitness;
                m_fitnessTable.Add((double)fitness);
            }
        }

        /// <summary>
        /// Create the *initial* genomes by repeated calling the supplied fitness function
        /// </summary>
        private void CreateGenomes()
        {
            for (int i = 0; i < m_populationSize; i++)
            {
                Player g = new Player(true, 4);
                m_thisGeneration.Add(g);
            }
        }

        private void CreateNextGeneration()
        {
            m_nextGeneration.Clear();
            Player g = new Player();
            if (m_elitism)
                g = (Player)m_thisGeneration[m_populationSize - 1];

            ConcurrentBag<Player> bag = new ConcurrentBag<Player>();
            Parallel.For(0, m_populationSize / 2, i =>
            {

                int pidx1 = RouletteSelection();
                int pidx2 = RouletteSelection();
                Player parent1, parent2, child1, child2;
                parent1 = ((Player)m_thisGeneration[pidx1]);
                parent2 = ((Player)m_thisGeneration[pidx2]);

                if (Random.NextDouble() < m_crossoverRate)
                {
                    parent1.Crossover(ref parent2, out child1, out child2);
                }
                else
                {
                    child1 = parent1;
                    child2 = parent2;
                }
                child1.Mutate();
                child2.Mutate();

                bag.Add(child1);
                bag.Add(child2);
            });

            foreach (var item in bag)
            {
                m_nextGeneration.Add(item);
            }


            if (m_elitism)
                m_nextGeneration[0] = g;

            m_thisGeneration.Clear();
            for (int i = 0; i < m_populationSize; i++)
                m_thisGeneration.Add(m_nextGeneration[i]);
        }


        private double m_mutationRate;
        private double m_crossoverRate;
        private int m_populationSize;
        private int m_generationSize;
        private double m_totalFitness;
        private double m_goalFitness;
        private bool m_elitism;
        private Player Enemy;
        private Squad[] AllyArmy;
        private Squad[] EnemyArmy;
        private int m_battleCount;
        private int m_currentGeneration = 0;
        private ArrayList m_thisGeneration;
        private ArrayList m_nextGeneration;
        private ArrayList m_fitnessTable;
        private int m_mapSize = 64;

        public int MapSize
        {
            get { return m_mapSize; }
            set { m_mapSize = value; }
        }
        static private GAFunction getFitness;


        double FitnessFunction(Player player)
        {

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            int temp = 0;
            Parallel.For(0, m_battleCount, i =>
             {

                 SandBox sb = new SandBox(Enemy, player, EnemyArmy, AllyArmy, m_mapSize);
                 if (sb.Fight(m_currentGeneration))
                     Interlocked.Add(ref temp, 1);
             }
           );


            //sw.Stop();
            //System.Windows.MessageBox.Show(sw.Elapsed.TotalMilliseconds.ToString());
            //sw.Reset();
            player.WinRate = (double)temp / (double)m_battleCount;
            double result = 1 - Math.Abs(m_goalFitness - (double)temp / (double)m_battleCount);
            return result;
        }



        //  Properties
        public int PopulationSize
        {
            get
            {
                return m_populationSize;
            }
            set
            {
                m_populationSize = value;
            }
        }

        public int Generations
        {
            get
            {
                return m_generationSize;
            }
            set
            {
                m_generationSize = value;
            }
        }



        public double CrossoverRate
        {
            get
            {
                return m_crossoverRate;
            }
            set
            {
                m_crossoverRate = value;
            }
        }
        public double MutationRate
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



        /// <summary>
        /// Keep previous generation's fittest individual in place of worst in current
        /// </summary>
        public bool Elitism
        {
            get
            {
                return m_elitism;
            }
            set
            {
                m_elitism = value;
            }
        }

        public void GetBest(out double[] values, out double fitness)
        {
            Player g = ((Player)m_thisGeneration[m_populationSize - 1]);
            values = new double[g.Length];
            g.GetValues(ref values);
            fitness = (double)g.Fitness;
        }
        public Player GetBest()
        {
            return ((Player)m_thisGeneration[m_populationSize - 1]);
        }

        public ArrayList GetLastGeneration()
        {
            return m_thisGeneration;
        }


        

        public Player GetNthGenome(int n)
        {
            if (n < 0 || n > m_populationSize - 1)
                throw new ArgumentOutOfRangeException("n too large, or too small");
            return ((Player)m_thisGeneration[n]);
            
        }
    }
}