using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Puzzle.Model
{
    class PuzzleModel
    {
        public List<Puzzel> Puzzles { get; private set; }

        private readonly static int _rows = 5;
        private readonly static int _columns = 5;
        private readonly static Size _puzzleSize = new Size(100, 100);
        private readonly static Size _boardSize = new Size(_puzzleSize.Width*_rows + _puzzleSize.Width, _puzzleSize.Height*_columns + _puzzleSize.Height);
        private readonly Random _random = new Random();
        private int _group = 0;
        private int _currentTime;

        public Size PuzzleSize { get { return _puzzleSize; } }
        public Size BoardSize { get { return _boardSize; } }
        public int Group { get { return ++_group; } }
        public DispatcherTimer timeToEnd;
        public int CurrentTime { get { return _currentTime; } set { _currentTime = value; } }

        public event EventHandler<PuzzelChangedEventArgs> PuzzelChanged;
        public event EventHandler GameOver;
        public event EventHandler<TimeChangedEventsArgs> ClockTick;

        public PuzzleModel()
        {
            Puzzles = new List<Puzzel>();
            timeToEnd = new DispatcherTimer();
            timeToEnd.Interval = TimeSpan.FromMilliseconds(1000);
            timeToEnd.Tick += timeToEnd_Tick;
            timeToEnd.Start();
            _currentTime = 30;
            Init();
        }

        void timeToEnd_Tick(object sender, EventArgs e)
        {
            if (CurrentTime > 0)
            {
                CurrentTime--;
                OnTimeChanged();
            }
                
        }

        private void OnPuzzelChanged(Puzzel puzzel, Point location)
        {
            EventHandler<PuzzelChangedEventArgs> puzzelChanged = PuzzelChanged;
            if (puzzelChanged != null)
            {
                puzzelChanged(this, new PuzzelChangedEventArgs(puzzel, location));
            }
        }

        private void OnGameOver()
        {
            EventHandler gameOver = GameOver;
            if (gameOver != null)
            {
                gameOver(this, new EventArgs());
            }
        }

        private void OnTimeChanged()
        {
            EventHandler<TimeChangedEventsArgs> clockTick = ClockTick;
            if (clockTick != null)
                clockTick(this, new TimeChangedEventsArgs(CurrentTime));
        }

        public void Init()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int ii = 0; ii < _columns; ii++)
                {
                    Puzzles.Add(new Puzzel(_puzzleSize,
                                            new Point(_random.Next(450),_random.Next(450)),
                                            new Matrix(i,ii),
                                            i*10+ii));
                }
            }
        }

        public void Move(Puzzel puzzel, Point location)
        {
            puzzel.Move(location);
            OnPuzzelChanged(puzzel, puzzel.Location);

            if (CheckMatrix() == true)
                OnGameOver();
        }

        public void MoveGroup(Puzzel puzzel, Point location)
        {
            List<Puzzel> groupped = GetGroup(puzzel);
            Point savedLocation = new Point(puzzel.Location.X, puzzel.Location.Y);

            for (int i = 0; i < groupped.Count; i++)
            {
                if (groupped.Count > 1)
                    System.Diagnostics.Debug.WriteLine(".");
                Double offsetX = (savedLocation.X - groupped[i].Location.X) - (savedLocation.X - groupped[i].Location.X)%100;
                Double offsetY = (savedLocation.Y - groupped[i].Location.Y) - (savedLocation.Y - groupped[i].Location.Y)%100;
                Move(groupped[i], new Point(location.X - offsetX, location.Y - offsetY));
            }
        }

        private List<Puzzel> GetGroup(Puzzel puzzel)
        {
                var groupped = (from puzzles in Puzzles
                               where puzzles.Group == puzzel.Group && puzzles.Group>=0
                               select puzzles).ToList();
                System.Diagnostics.Debug.WriteLine(groupped.Count);

                return groupped;

        }

        public void Stick(Puzzel puzzel)
        {

            var toStick = (from puzzleToStick in Puzzles
                           where puzzleToStick != puzzel && checkLocation(puzzel.Location, puzzleToStick.Location)
                           orderby Math.Abs(puzzleToStick.Location.X - puzzel.Location.X), Math.Abs(puzzleToStick.Location.Y - puzzel.Location.Y)
                           select puzzleToStick).ToList();

            if (toStick.Count >0) 
            {
                    Align(toStick[0],puzzel);
                    OnPuzzelChanged(puzzel, puzzel.Location);          
            }
                //align to border
            else if (puzzel.Location.X < _puzzleSize.Width && puzzel.Location.Y < _puzzleSize.Height)
            {
                Align(puzzel, puzzel);
                OnPuzzelChanged(puzzel, puzzel.Location);
            }
        }

        private void Align(Puzzel basePuzzel, Puzzel puzzelToAlign)
        {
            double horizontal = basePuzzel.Location.X - puzzelToAlign.Location.X;
            double vertical = basePuzzel.Location.Y - puzzelToAlign.Location.Y;
            double span = _puzzleSize.Width;

            //frame
            if (puzzelToAlign.Location.X < _puzzleSize.Width)
                Move(puzzelToAlign, new Point(0, puzzelToAlign.Location.Y));
            if (puzzelToAlign.Location.Y < _puzzleSize.Height)
                Move(puzzelToAlign, new Point(puzzelToAlign.Location.X, 0));
            //left
            if(puzzelToAlign.Location.X > basePuzzel.Location.X - _puzzleSize.Width - span && puzzelToAlign.Location.X < basePuzzel.Location.X - _puzzleSize.Width + span &&
                puzzelToAlign.Location.Y > basePuzzel.Location.Y - span && puzzelToAlign.Location.Y < basePuzzel.Location.Y + span)
            {
                Move(puzzelToAlign, new Point(basePuzzel.Location.X - _puzzleSize.Width, basePuzzel.Location.Y));
                ChangeGroup(puzzelToAlign, basePuzzel);
            }
            //right
            else if (puzzelToAlign.Location.X > basePuzzel.Location.X + _puzzleSize.Width - span && puzzelToAlign.Location.X < basePuzzel.Location.X + _puzzleSize.Width + span &&
                puzzelToAlign.Location.Y > basePuzzel.Location.Y - span && puzzelToAlign.Location.Y < basePuzzel.Location.Y + span)
            {
                Move(puzzelToAlign, new Point(basePuzzel.Location.X + _puzzleSize.Width, basePuzzel.Location.Y));
                ChangeGroup(puzzelToAlign, basePuzzel);
            }

            //top
            else if (puzzelToAlign.Location.X > basePuzzel.Location.X - span && puzzelToAlign.Location.X < basePuzzel.Location.X + span &&
                puzzelToAlign.Location.Y > basePuzzel.Location.Y - _puzzleSize.Height - span && puzzelToAlign.Location.Y < basePuzzel.Location.Y - _puzzleSize.Height + span)
            {
                Move(puzzelToAlign, new Point(basePuzzel.Location.X, basePuzzel.Location.Y - _puzzleSize.Height));
                ChangeGroup(puzzelToAlign, basePuzzel);
            }
            //bottom
            else if (puzzelToAlign.Location.X > basePuzzel.Location.X - span && puzzelToAlign.Location.X < basePuzzel.Location.X + span &&
                puzzelToAlign.Location.Y > basePuzzel.Location.Y + _puzzleSize.Height - span && puzzelToAlign.Location.Y < basePuzzel.Location.Y + _puzzleSize.Height + span)
            {
                Move(puzzelToAlign, new Point(basePuzzel.Location.X, basePuzzel.Location.Y + _puzzleSize.Height));
                ChangeGroup(puzzelToAlign, basePuzzel);
            }
            else
            {
                puzzelToAlign.AssignToGroup(-1);
            }

        }

        private void ChangeGroup(Puzzel fromGroup, Puzzel toGroup)
        {
            List<Puzzel> group = GetGroup(fromGroup);

            for (int i = 0; i < group.Count; i++)
            {
                group[i].AssignToGroup(toGroup.Group);
            }
        }

        public void UnStick(Puzzel puzzel)
        {
            var maxGroup = (from item in Puzzles
                          orderby item.Group descending
                          select item.Group).FirstOrDefault();

            puzzel.AssignToGroup(maxGroup+1);
        }

        private bool checkLocation(Point box1, Point box2)
        {
            double widht = 0;
            double height = 0;
            double span = 20;
            if (box1.X > box2.X)
                widht = box1.X - box2.X;
            else
                widht = box2.X - box1.X;

            if (box1.Y > box2.Y)
                height = box1.Y - box2.Y;
            else
                height = box2.Y - box1.Y;

            if ((widht>=_puzzleSize.Width && widht<_puzzleSize.Width+span && height<=span) || (height>=_puzzleSize.Height && height<_puzzleSize.Height+span && widht<=span))
                return true;
            else
                return false;

        }

        public void Shuffle()
        {
            foreach (Puzzel puzzle in Puzzles)
            {
                puzzle.Move(new Point(
                    _random.Next((int)_boardSize.Width - (int)_puzzleSize.Width), 
                    _random.Next((int)_boardSize.Height - (int)_puzzleSize.Height)));
            }
        }

        public bool CheckMatrix()
        {
            Puzzel puzzel = GetCorrectlyOrderedPuzzle();
            if (puzzel == null)
                return true;
            else
                return false;
        }

        public Puzzel GetCorrectlyOrderedPuzzle()
        {
            var puzzlesOrderedByMatrix = (from puzzle in Puzzles
                                          orderby puzzle.Matrix.Column, puzzle.Matrix.Row
                                          select puzzle).ToList();

            foreach(Puzzel puzzle in puzzlesOrderedByMatrix)
            {
                if (puzzle.Location.X == (puzzle.Matrix.Column * PuzzleSize.Height) && puzzle.Location.Y == (puzzle.Matrix.Row * PuzzleSize.Width))
                    continue;
                else
                    return puzzle;
            }
            return null;
        }

    }
}
