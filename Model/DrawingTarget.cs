using System;
using System.Drawing;
using System.Xml.Schema;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Model
{
    public class DrawingTarget : IFitness
    {
        private Bitmap _targetBitmap;

        public DrawingTarget(string filename)
        {
            _targetBitmap = new Bitmap(filename);
            _drawingRenderer = new DrawingRenderer();
        }

        public double Evaluate(IChromosome c)
        {
            var chromosome = (DrawingChromosome) c;
            var pheno = _drawingRenderer.GenBitmap(chromosome, _targetBitmap.Width, _targetBitmap.Height);
            return Score(pheno);
        }

        private double Score(Bitmap pheno)
        {
            double totalError = 0;
            for (int y = 0; y < pheno.Height; ++y)
                for (int x = 0; x < pheno.Width; ++x)
                {
                    var a = _targetBitmap.GetPixel(x, y);
                    var b = pheno.GetPixel(x, y);
                    var rError = (int)a.R - (int)b.R;
                    var gError = (int)a.G - (int)b.G;
                    var bError = (int)a.B - (int)b.B;
                    totalError += (Math.Abs(rError / 255f) + Math.Abs(gError / 255f) + Math.Abs(bError / 255f)) / 3;
                }

            return 1 - (totalError / (pheno.Height * pheno.Width));
        }

        private readonly DrawingRenderer _drawingRenderer;
    }
}
