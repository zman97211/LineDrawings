using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace Model
{
    public class LineGene
    {
        public byte x1 { get; set; }
        public byte y1 { get; set; }
        public byte x2 { get; set; }
        public byte y2 { get; set; }

        static Random _rand = new Random();

        private double Clamp(double a, double min, double max)
        {
            if (a < min)
                return min;
            if (a > max)
                return max;
            return a;
        }

        public LineGene()
        {
            var length = (_rand.NextDouble() * 0.5 + 0.2) * 256;
            var cx = _rand.NextDouble() * 256;
            var cy = _rand.NextDouble() * 256;
            var degrees = _rand.NextDouble() * 360;
            var radians = 2 * Math.PI * degrees / 360;
            var wOver2 = length * Math.Cos(radians) / 2;
            var hOver2 = length * Math.Sin(radians) / 2;

            var ix1 = Clamp(cx - wOver2, 0, 256);
            var iy1 = Clamp(cy - hOver2, 0, 256);
            var ix2 = Clamp(cx + wOver2, 0, 256);
            var iy2 = Clamp(cy + hOver2, 0, 256);

            x1 = (byte)ix1;
            y1 = (byte)iy1;
            x2 = (byte)ix2;
            y2 = (byte)iy2;
        }
    }

    public class DrawingChromosome : HasIdBase, IChromosome
    {
        private readonly List<Gene> _genes;

        public DrawingChromosome(int length)
        {
            _genes = new List<Gene>(length);
            for (var i = 0; i < length; ++i)
                _genes.Add(new Gene(new LineGene()));
        }

        public int CompareTo(IChromosome other)
        {
            return Id.CompareTo(((DrawingChromosome)other).Id);
        }

        public Gene GenerateGene(int geneIndex)
        {
            return new Gene(new LineGene());
        }

        public void ReplaceGene(int index, Gene gene)
        {
            _genes[index] = gene;
        }

        public void ReplaceGenes(int startIndex, Gene[] genes)
        {
            _genes.RemoveRange(startIndex, genes.Length);
            _genes.InsertRange(startIndex, genes);
        }

        public void Resize(int newLength)
        {
            if (_genes.Count > newLength)
                _genes.RemoveRange(newLength, _genes.Count - newLength);
            else
            {
                var nToAdd = newLength - _genes.Count;
                for (int i = 0; i < nToAdd; ++i)
                    _genes.Add(new Gene(new LineGene()));
            }
        }

        public Gene GetGene(int index)
        {
            return _genes[index];
        }

        public Gene[] GetGenes()
        {
            return _genes.ToArray();
        }

        public IChromosome CreateNew()
        {
            return new DrawingChromosome(100);
        }

        public IChromosome Clone()
        {
            throw new NotImplementedException();
        }

        public double? Fitness { get; set; }

        public int Length => _genes.Count;

        public byte[,] GetPhenotype()
        {
            var phenotype = new byte[256, 256];
            for (var y = 0; y < 256; ++y)
                for (var x = 0; x < 256; ++x)
                    phenotype[x, y] = 255;
            foreach (var lineGene in _genes)
            {
                var line = (LineGene) lineGene.Value;
                Algorithms.Line(line.x1, line.y1, line.x2, line.y2, (x, y) =>
                {
                    phenotype[x, y] = 0;
                    return true;
                });
            }
            return phenotype;
        }

        private static readonly Random _rand = new Random();
        public void Mutate()
        {
            _genes[_rand.Next(100)] = new Gene(new LineGene());
        }
    }

    public class HasIdBase
    {
        private static int _nextId;
        private static readonly Object _nextIdLock = new Object();
        public HasIdBase()
        {
            lock (_nextIdLock)
            {
                Id = _nextId++;
            }
        }

        public int Id { get; private set; }
    }
}
