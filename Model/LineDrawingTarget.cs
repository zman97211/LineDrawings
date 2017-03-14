using System.Drawing;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Model
{
    class LineDrawingTarget : IFitness
    {
        private readonly byte[,] _targetPhenotype = new byte[256,256];

        public LineDrawingTarget(string imageFilename)
        {
            var target = new Bitmap(imageFilename);
            for (int y = 0; y < 256; y++)
                for (int x = 0; x < 256; x++)
                    _targetPhenotype[x, y] = (byte) (target.GetPixel(x, y).GetBrightness() * 256);
        }

        private double CalcFitness(DrawingChromosome c)
        {
            var phenotype = c.GetPhenotype();
            int totalError = 0;
            for (int y = 0; y < 256; y++)
                for (int x = 0; x < 256; x++)
                {
                    var aPixel = _targetPhenotype[x, y];
                    var bPixel = phenotype[x, y];
                    totalError += System.Math.Abs(aPixel - bPixel);
                }
            return 1 - ((double)totalError / int.MaxValue);
        }

        public double Evaluate(IChromosome chromosome)
        {
            return CalcFitness((DrawingChromosome)chromosome);
        }
    }
}
