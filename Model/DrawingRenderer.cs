using System;
using System.Drawing;

namespace Model
{
    public class DrawingRenderer
    {
        private const int MaxCircleSize = 40;
        private readonly Brush RedBrush = new SolidBrush(Color.FromArgb(128, 255, 0, 0));
        private readonly Brush GreenBrush = new SolidBrush(Color.FromArgb(128, 0, 255, 0));
        private readonly Brush BlueBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));

        public DrawingRenderer()
        {
        }

        public Bitmap GenBitmap(DrawingChromosome c, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.White, 0, 0, 255, 255);
            var genes = c.GetCircleGenes();
            foreach (var gene in genes)
            {
                Brush brush;
                switch (gene.Color)
                {
                    case CircleGene.Colors.Red:
                        brush = RedBrush;
                        break;
                    case CircleGene.Colors.Green:
                        brush = GreenBrush;
                        break;
                    case CircleGene.Colors.Blue:
                        brush = BlueBrush;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                graphics.FillEllipse(brush, (float)(gene.Center.X - gene.Radius) * width, (float)(gene.Center.Y - gene.Radius) * height, (float)(gene.Radius * 2) * MaxCircleSize, (float)(gene.Radius * 2) * MaxCircleSize);
            }
            return bitmap;
        }
    }
}