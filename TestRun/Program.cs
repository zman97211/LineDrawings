using GeneticSharp.Domain.Selections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Terminations;
using Model;
using System.Drawing;
using System.Threading;

namespace TestRun
{
    class Program
    {
        private static GeneticAlgorithm _ga;
        private const int GenerationLimit = 2000;

        static void Main(string[] args)
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover();
            var mutation = new DrawingMutation();
            var fitness = new LineDrawingTarget(@"d:\pic.jpg");
            var chromosome = new DrawingChromosome(200);
            var population = new Population(50, 60, chromosome);

            _ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            _ga.Termination = new GenerationNumberTermination(GenerationLimit);

            _ga.GenerationRan += OnGenerationComplete;

            var t = new Thread(() => _ga.Start());
            t.Start();
            Console.WriteLine("GA running...");

            t.Join();

            Console.WriteLine("Best solution found has {0} fitness.", _ga.BestChromosome.Fitness);

            var best = (DrawingChromosome)_ga.BestChromosome;
            var bitmap = GenBitmap(best);
            bitmap.Save(@"d:\out3.png");
        }

        private static void OnGenerationComplete(object sender, EventArgs e)
        {
            Console.WriteLine("Generation complete. Current generation: {0} ({1}%). Best fitness: {2}", _ga.GenerationsNumber, 100 * _ga.GenerationsNumber / GenerationLimit, _ga.BestChromosome.Fitness);
        }

        static Bitmap GenBitmap(DrawingChromosome c)
        {
            // REVISIT: This is a crappy way of doing this.
            var bitmap = new Bitmap(256, 256);
            var graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.White, 0, 0, 255, 255);
            var ph = c.GetPhenotype();
            for (var y = 0; y < 256; ++y)
                for (var x = 0; x < 256; ++x)
                    if (ph[x,y]<128)
                        graphics.DrawLine(Pens.Black, x, y, x+1, y+1);
            return bitmap;
        }
    }
}
