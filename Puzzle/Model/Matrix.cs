using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Model
{
    class Matrix
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public Matrix(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
