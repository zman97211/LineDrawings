using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
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
        private string _inputFilename;
        private string _outputFilename;
        private int _numberOfGenerations;
        private int _numberOfLines;
        private double _mutationProbability;
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

        public double MutationProbability
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

            _ga.GenerationRan += OnGenerationComplete;

            _gaThread = new Thread(() => _ga.Start());
            _gaThread.Start();
        }

        private void OnGenerationComplete(object sender, EventArgs e)
        {
            CurrentGeneration = _ga.GenerationsNumber;
            Progress = (double)CurrentGeneration / NumberOfGenerations;
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
