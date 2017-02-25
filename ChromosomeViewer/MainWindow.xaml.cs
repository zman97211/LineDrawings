using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Model;

namespace ChromosomeViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static Bitmap GenBitmap(byte[,] c)
        {
            var width = c.GetLength(0);
            var height = c.GetLength(1);
            var bitmap = new Bitmap(width, height);
            for (var y = 0; y < height; ++y)
                for (var x = 0; x < width; ++x)
                {
                    var val = c[x, y];
                    bitmap.SetPixel(x, y, Color.FromArgb(val, val, val));
                }
            return bitmap;
        }

        public BitmapImage Phenotype { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            var c = LineGene.GetRandomChromosome(100);
            var p = LineGene.GetPhenotype(c);
            var b = GenBitmap(p);

            using (MemoryStream memory = new MemoryStream())
            {
                b.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                Phenotype = new BitmapImage();
                Phenotype.BeginInit();
                Phenotype.StreamSource = memory;
                Phenotype.CacheOption = BitmapCacheOption.OnLoad;
                Phenotype.EndInit();
            }

            DataContext = this;
        }
    }
}
