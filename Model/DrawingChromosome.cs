using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace Model
{
    public class CircleGene
    {
        private static Random _rand = new Random();
        public enum Colors
        {
            Red, Green, Blue
        }

        public class Point
        {
            public double X { get; set; } = _rand.NextDouble();
            public double Y { get; set; } = _rand.NextDouble();
        }

        public Point Center { get; set; } = new Point();

        public double Radius { get; set; } = _rand.NextDouble();
        public Colors Color { get; set; }

        public CircleGene()
        {
            var colorValues = Enum.GetValues(typeof(Colors));
            Color = (Colors)colorValues.GetValue(_rand.Next(colorValues.Length));
        }
    }

    public class DrawingChromosome : HasIdBase, IChromosome
    {
        private readonly List<Gene> _genes;

        public static int NewChromosomeLength { get; set; } = 20;

        public DrawingChromosome(int length)
        {
            _genes = new List<Gene>(length);
            for (var i = 0; i < length; ++i)
                _genes.Add(new Gene(new CircleGene()));
        }

        public int CompareTo(IChromosome other)
        {
            return Id.CompareTo(((DrawingChromosome)other).Id);
        }

        public Gene GenerateGene(int geneIndex)
        {
            return new Gene(new CircleGene());
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
                    _genes.Add(new Gene(new CircleGene()));
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
            return new DrawingChromosome(NewChromosomeLength);
        }

        public IChromosome Clone()
        {
            throw new NotImplementedException();
        }

        public double? Fitness { get; set; }

        public int Length => _genes.Count;

        public IEnumerable<CircleGene> GetCircleGenes()
        {
            var circleGenes = new List<CircleGene>();
            foreach (var genericGene in _genes)
                circleGenes.Add((CircleGene)genericGene.Value);
            return circleGenes;
        }

        private static readonly Random _rand = new Random();
        public void Mutate()
        {
            _genes[_rand.Next(_genes.Count)] = new Gene(new CircleGene());
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
