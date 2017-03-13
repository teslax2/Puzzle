using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Puzzle.Model
{
    class PuzzelChangedEventArgs
    {
        public Point Location { get; private set; }
        public Puzzel Puzzel { get; private set; }

        public PuzzelChangedEventArgs(Puzzel Puzzel, Point Location)
        {
            this.Location = Location;
            this.Puzzel = Puzzel;
        }
    }
}
