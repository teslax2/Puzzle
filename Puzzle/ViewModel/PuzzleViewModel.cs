using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Specialized;
using System.Windows.Controls;
using Puzzle.Model;
using Puzzle.View;
using System.ComponentModel;

namespace Puzzle.ViewModel
{
    class PuzzleViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<FrameworkElement> _sprites = new ObservableCollection<FrameworkElement>();
        public INotifyCollectionChanged Sprites { get { return _sprites; } }
        public Dictionary<Puzzel, FrameworkElement> Puzzles = new Dictionary<Puzzel, FrameworkElement>();
        private bool _gameOver;
        public bool GameOver { get { return _gameOver; } set { _gameOver = value; OnPropertyChanged("GameOver"); } }
        public int CurrentTime { get; set; }

        private BitmapImage _loadedBitmap;
        private Image _loadedImage;
        private PuzzleModel puzzleModel;
        private Puzzel _currentPuzzel;

        public RelayCommand OpenImage { get { return new RelayCommand(OnOpenImage); } }
        public RelayCommand ShowHint { get { return new RelayCommand(OnShowHint); } }

        public PuzzleViewModel()
        {
            puzzleModel = new PuzzleModel();
            puzzleModel.PuzzelChanged += puzzleModel_PuzzelChanged;
            puzzleModel.GameOver += puzzleModel_GameOver;
            puzzleModel.ClockTick += puzzleModel_ClockTick;
        }

        void puzzleModel_ClockTick(object sender, TimeChangedEventsArgs e)
        {
            CurrentTime = e.Time;
            OnPropertyChanged("CurrentTime");
        }

        void puzzleModel_GameOver(object sender, EventArgs e)
        {
            GameOver = true;
        }

        void puzzleModel_PuzzelChanged(object sender, PuzzelChangedEventArgs e)
        {
            if (Puzzles.ContainsKey(e.Puzzel))
            {
                PuzzleHelper.SetLocation(Puzzles[e.Puzzel], e.Puzzel.Location.X, e.Puzzel.Location.Y);
            }
        }

        private void OnOpenImage()
        {
            Init();
            OpenFileDialog opn = new OpenFileDialog();
            bool? result = opn.ShowDialog();

            if (result == true)
            {
               _loadedBitmap = new BitmapImage();
               _loadedImage = new System.Windows.Controls.Image();

               _loadedBitmap.BeginInit();
               _loadedBitmap.UriSource = new Uri(opn.FileName);
               _loadedBitmap.DecodePixelHeight = 500;
               _loadedBitmap.EndInit();

               _loadedImage.Source = _loadedBitmap;
                CreateChunks();
            }

        }

        private void OnShowHint()
        {
           Puzzel puzzleToAnimate = puzzleModel.GetCorrectlyOrderedPuzzle();

           if (puzzleToAnimate != null)
           {
               PuzzleHelper.MoveElementOnCanvas(Puzzles[puzzleToAnimate], puzzleToAnimate.Matrix.Row * puzzleModel.PuzzleSize.Width, puzzleToAnimate.Matrix.Column * puzzleModel.PuzzleSize.Height);
               puzzleModel.Move(puzzleToAnimate, new Point(puzzleToAnimate.Matrix.Column * puzzleModel.PuzzleSize.Width, puzzleToAnimate.Matrix.Row * puzzleModel.PuzzleSize.Height));
           }
        }


        private void CreateChunks()
        {
            foreach(Puzzel puzzel in puzzleModel.Puzzles)
            {
                CroppedBitmap chunk = new CroppedBitmap(_loadedBitmap, new Int32Rect((puzzel.Matrix.Row * (int)puzzleModel.PuzzleSize.Width),
                                                                                    (puzzel.Matrix.Column * (int)puzzleModel.PuzzleSize.Height), 
                                                                                    (int)puzzleModel.PuzzleSize.Width, (int)puzzleModel.PuzzleSize.Height));
                Image chunkImage = new Image();
                chunkImage.Source = chunk;
                PuzzleHelper.SetLocation(chunkImage, puzzel.Location.X, puzzel.Location.Y);
                Puzzles.Add(puzzel, chunkImage);
                _sprites.Add(chunkImage);
            }
        }

        private void Init()
        {
            _sprites.Clear();
            Puzzles.Clear();
            GameOver = false;
        }


        internal void MovePuzzle(Point point)
        {
            if(_currentPuzzel != null )
            {
                Point location = new Point(point.X - puzzleModel.PuzzleSize.Width / 2, point.Y - puzzleModel.PuzzleSize.Height / 2);
                puzzleModel.UnStick(_currentPuzzel);
                puzzleModel.Move(_currentPuzzel,location);
            }
        }

        internal void MoveManyPuzzle(Point point)
        {
            if (_currentPuzzel != null)
            {
                Point location = new Point(point.X - puzzleModel.PuzzleSize.Width / 2, point.Y - puzzleModel.PuzzleSize.Height / 2);
                puzzleModel.MoveGroup(_currentPuzzel, location);
            }
        }

        internal void SelectPuzzle(Point point)
        {
            var PuzzleToMove = from Puzzle in puzzleModel.Puzzles
                               where (Puzzle.Location.X <= point.X && Puzzle.Location.X + Puzzle.Size.Width >= point.X &&
                                        Puzzle.Location.Y <= point.Y && Puzzle.Location.Y + Puzzle.Size.Height >= point.Y)
                               select Puzzle;
            if (PuzzleToMove.Count() >= 1)
            {
                _currentPuzzel = PuzzleToMove.FirstOrDefault();
            }
            else
                _currentPuzzel = null;
        }

        internal void StickPuzzles()
        {
            if (_currentPuzzel != null)
            {
                puzzleModel.Stick(_currentPuzzel);
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
