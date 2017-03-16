using System;
using System.Drawing;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Model
{
    public class DrawingTarget : IFitness
    {
        public DrawingTarget(string filename)
        {
            throw new NotFiniteNumberException();
        }

        public double Evaluate(IChromosome c)
        {
            var chromosome = (DrawingChromosome) c;
            throw new NotImplementedException();
        }
    }
}
