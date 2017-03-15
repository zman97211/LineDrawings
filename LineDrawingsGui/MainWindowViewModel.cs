using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using Model;

namespace LineDrawingsGui
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _inputFilename = @"d:\pic.jpg";
        private string _outputFilename = @"d:\out.png";
        private int _numberOfGenerations = 2000;
        private int _numberOfLines = 100;
        private float _mutationProbability = 0.1f;
        private int _currentGeneration;
        private double _progress;

        public string InputFilename
        {
            get { return _inputFilename; }
            set { _inputFilename = value; OnPropertyChanged(); }
        }

        public string OutputFilename
        {
            get { return _outputFilename; }
            set { _outputFilename = value; OnPropertyChanged(); }
        }

        public int NumberOfGenerations
        {
            get { return _numberOfGenerations; }
            set { _numberOfGenerations = value; OnPropertyChanged(); }
        }

        public int NumberOfLines
        {
            get { return _numberOfLines; }
            set { _numberOfLines = value; OnPropertyChanged(); }
        }

        public float MutationProbability
        {
            get { return _mutationProbability; }
            set { _mutationProbability = value; OnPropertyChanged(); }
        }

        public int CurrentGeneration
        {
            get { return _currentGeneration; }
            set { _currentGeneration = value; OnPropertyChanged(); }
        }

        public double Progress
        {
            get { return _progress; }
            set { _progress = value; OnPropertyChanged(); }
        }

        public BitmapImage BestChromosome
        {
            get { return _bestChromosome; }
            set { _bestChromosome = value; OnPropertyChanged(); }
        }

        public RelayCommand StartCommand { get; set; }
        public RelayCommand BrowseInputFileCommand { get; set; }
        public RelayCommand BrowseOutputFileCommand { get; set; }

        public MainWindowViewModel()
        {
            StartCommand = new RelayCommand(o => { StartGa(); }, o => true);
            BrowseInputFileCommand = new RelayCommand(o => { BrowseInputFile(); }, o => true);
            BrowseOutputFileCommand = new RelayCommand(o => { BrowseOutputFile(); }, o => true);
        }

        private GeneticAlgorithm _ga;
        private Thread _gaThread;
        private BitmapImage _bestChromosome;

        private void StartGa()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover();
            var mutation = new DrawingMutation();
            var fitness = new LineDrawingTarget(InputFilename);
            var chromosome = new DrawingChromosome(NumberOfLines);
            var population = new Population(50, 60, chromosome);

            _ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            _ga.Termination = new GenerationNumberTermination(NumberOfGenerations);
            _ga.MutationProbability = MutationProbability;

            _ga.GenerationRan += OnGenerationComplete;
            _ga.TerminationReached += OnTerminationReached;

            _gaThread = new Thread(() => _ga.Start());
            _gaThread.Start();
        }

        private void OnTerminationReached(object sender, EventArgs eventArgs)
        {
            var bitmap = GenBitmap((DrawingChromosome)_ga.BestChromosome);
            bitmap.Save(OutputFilename);
        }

        private void OnGenerationComplete(object sender, EventArgs e)
        {
            CurrentGeneration = _ga.GenerationsNumber;
            Progress = (double)CurrentGeneration / NumberOfGenerations;
        }

        private void SetBestChromosome(DrawingChromosome c)
        {
            var bitmap = GenBitmap(c);

            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                BestChromosome = bitmapImage;
            }
        }

        private static Bitmap GenBitmap(DrawingChromosome c)
        {
            // REVISIT: This is a crappy way of doing this.
            var bitmap = new Bitmap(256, 256);
            var graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(Brushes.White, 0, 0, 255, 255);
            var ph = c.GetPhenotype();
            for (var y = 0; y < 256; ++y)
            for (var x = 0; x < 256; ++x)
                if (ph[x, y] < 128)
                    graphics.DrawLine(Pens.Black, x, y, x + 1, y + 1);
            return bitmap;
        }

        private void BrowseInputFile()
        {
            throw new NotImplementedException();
        }

        private void BrowseOutputFile()
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
