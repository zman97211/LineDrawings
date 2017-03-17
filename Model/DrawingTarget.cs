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

        private double Score(FastImage pheno)
        {
            double totalError = 0;
            for (int y = 0; y < pheno.Height; ++y)
                for (int x = 0; x < pheno.Width; ++x)
                {
                    var a = _targetBitmap.GetPixel(x, y);
                    var b = pheno.GetPixel(x, y);
                    var rError = (float)a.R - b.R;
                    var gError = (float)a.G - b.G;
                    var bError = (float)a.B - b.B;
                    var result = Math.Pow(rError * rError + gError * gError + bError * bError, (1f / 3));
                    totalError += result;
                }

            return 1 - (totalError / (pheno.Height * pheno.Width));
        }

        private readonly DrawingRenderer _drawingRenderer;
    }
}
