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
    }
}
