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

        public LineGene()
        {
            x1 = (byte)_rand.Next(255);
            y1 = (byte)_rand.Next(255);
            x2 = (byte)_rand.Next(255);
            y2 = (byte)_rand.Next(255);
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
