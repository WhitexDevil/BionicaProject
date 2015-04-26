using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project
{
    public sealed class GenomeComparer :  IComparer
    {
        public GenomeComparer()
        {
        }
        public int Compare(object x, object y)
        {
            if (!(x is Player) || !(y is Player))
                throw new ArgumentException("Not of type Genome");

            if (((Player)x).Fitness > ((Player)y).Fitness)
                return 1;
            else if (((Player)x).Fitness == ((Player)y).Fitness)
                return 0;
            else
                return -1;

        }
    }
}
