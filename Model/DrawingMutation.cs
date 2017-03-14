using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Mutations;

namespace Model
{
    public class DrawingMutation : IMutation
    {
        static Random _rand = new Random();
        public bool IsOrdered { get { return false; } }
        public void Mutate(IChromosome chromosome, float probability)
        {
            if (_rand.NextDouble() < probability)
                ((DrawingChromosome) (chromosome)).Mutate();
        }
    }
}
