using System;
using System.Drawing;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Model
{
    public class DrawingTarget : IFitness
    {
        public DrawingTarget(string imageFilename)
        {
            throw new NotImplementedException();
            var target = new Bitmap(imageFilename);
        }

        private double CalcFitness(DrawingChromosome c)
        {
            throw new NotImplementedException();
        }

        public double Evaluate(IChromosome chromosome)
        {
            return CalcFitness((DrawingChromosome)chromosome);
        }
    }
}
