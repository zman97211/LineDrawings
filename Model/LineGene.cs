using System;
using System.Collections.Generic;

namespace Model
{
    public class LineGene
    {
        public byte x1 { get; set; }
        public byte y1 { get; set; }
        public byte x2 { get; set; }
        public byte y2 { get; set; }

        static Random _rand = new Random();

        public LineGene()
        {
            x1 = (byte)_rand.Next(255);
            y1 = (byte)_rand.Next(255);
            x2 = (byte)_rand.Next(255);
            y2 = (byte)_rand.Next(255);
        }

        public static IEnumerable<LineGene> GetRandomChromosome(int length)
        {
            var chromosome = new List<LineGene>(length);
            for (var i = 0; i < length; ++i)
                chromosome.Add(new LineGene());
            return chromosome;
        }

        public static byte[,] GetPhenotype(IEnumerable<LineGene> chromosome)
        {
            var phenotype = new byte[256, 256];
            for (var y = 0; y < 256; ++y)
                for (var x = 0; x < 256; ++x)
                    phenotype[x, y] = 255;
            foreach (var line in chromosome)
                Algorithms.Line(line.x1, line.y1, line.x2, line.y2, (x, y) => { phenotype[x, y] = 0; return true; });
            return phenotype;
        }
    }
}
