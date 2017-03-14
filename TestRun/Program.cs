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

namespace TestRun
{
    class Program
    {
        static void Main(string[] args)
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover();
            var mutation = new DrawingMutation();
            var fitness = new LineDrawingTarget(@"d:\pic.jpg");
            var chromosome = new DrawingChromosome(200);
            var population = new Population(50, 60, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Termination = new GenerationNumberTermination(2200);

            Console.WriteLine("GA running...");
            ga.Start();

            Console.WriteLine("Best solution found has {0} fitness.", ga.BestChromosome.Fitness);

            var best = (DrawingChromosome)ga.BestChromosome;
            var bitmap = GenBitmap(best);
            bitmap.Save(@"d:\out3.png");
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
