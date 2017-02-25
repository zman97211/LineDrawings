using System.Collections.Generic;

namespace Model
{
    public class LineGene
    {
        public byte x1 { get; set; }
        public byte y1 { get; set; }
        public byte x2 { get; set; }
        public byte y2 { get; set; }

        public static byte[,] GetPhenotype(IEnumerable<LineGene> lines)
        {
            var phenotype = new byte[256, 256];
            foreach (var line in lines)
                Algorithms.Line(line.x1, line.y1, line.x2, line.y2, (x, y) => { phenotype[x, y] = 0; return true; });
            return phenotype;
        }
    }
}
