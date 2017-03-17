using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
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
        private int _numberOfGenerations = 180000;
        private int _numberOfGenes = 250;
        private float _mutationProbability = 0.15f;
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

        public int NumberOfGenes
        {
            get { return _numberOfGenes; }
            set { _numberOfGenes = value; OnPropertyChanged(); }
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
            DrawingChromosome.NewChromosomeLength = NumberOfGenes;

            var selection = new EliteSelection();
            var crossover = new OnePointCrossover();
            var mutation = new DrawingMutation();
            var fitness = new DrawingTarget(InputFilename);
            var chromosome = new DrawingChromosome(NumberOfGenes);
            var population = new Population(50, 60, chromosome);

            _ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            _ga.Termination = new GenerationNumberTermination(NumberOfGenerations);
            _ga.MutationProbability = MutationProbability;

            _ga.GenerationRan += OnGenerationComplete;
            _ga.TerminationReached += OnTerminationReached;

            _gaThread = new Thread(() => _ga.Start());
            _gaThread.Start();
            TimeStarted = DateTime.Now;
        }

        private void OnTerminationReached(object sender, EventArgs eventArgs)
        {
            MessageBox.Show("Done");
        }
        
        private void OnGenerationComplete(object sender, EventArgs e)
        {
            CurrentGeneration = _ga.GenerationsNumber;
            Progress = (double)CurrentGeneration / NumberOfGenerations;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                SetBestChromosome((DrawingChromosome)_ga.BestChromosome);
                _drawingRenderer.GenBitmap((DrawingChromosome)_ga.BestChromosome, 1024, 1024).Save($@"d:\balloon\{CurrentGeneration}.png");
            }));
            UpdateTimeStats();
        }

        public DateTime TimeStarted
        {
            get { return _timeStarted; }
            private set { _timeStarted = value; OnPropertyChanged(); }
        }

        public TimeSpan TimeElapsed
        {
            get { return _timeElapsed; }
            private set { _timeElapsed = value; OnPropertyChanged(); }
        }

        public TimeSpan AverageGenerationTime
        {
            get { return _averageGenerationTime; }
           private set { _averageGenerationTime = value; OnPropertyChanged(); }
        }

        public string TimeToCompletion
        {
            get { return _timeToCompletion; }
            set { _timeToCompletion = value; OnPropertyChanged(); }
        }

        private void UpdateTimeStats()
        {
            TimeElapsed = DateTime.Now - TimeStarted;
            AverageGenerationTime = new TimeSpan(0, 0, 0, 0, CurrentGeneration == 0 ? 0 : (int)((float)TimeElapsed.TotalMilliseconds / CurrentGeneration));
            var msToCompletion = AverageGenerationTime.TotalMilliseconds * (NumberOfGenerations - CurrentGeneration);
            TimeToCompletion = TimeSpan.FromMilliseconds(msToCompletion).ToString();
        }

        private DrawingRenderer _drawingRenderer = new DrawingRenderer();
        private DateTime _timeStarted;
        private TimeSpan _timeElapsed;
        private TimeSpan _averageGenerationTime;
        private string _timeToCompletion;

        private void SetBestChromosome(DrawingChromosome c)
        {
            var bitmap = _drawingRenderer.GenBitmap(c, 200, 200);
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
