using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Model
{
    class TimeChangedEventsArgs
    {
        public int Time { get; set; }
        public TimeChangedEventsArgs(int time)
        {
            Time = time;
        }
    }
}
