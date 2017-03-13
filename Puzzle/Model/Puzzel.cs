using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Puzzle.Model
{
    class Puzzel
    {
        public Point Location { get; private set; }
        public Size Size { get; private set; }
        public Matrix Matrix { get; private set; }
        public List<Puzzel> Neighbours { get; private set; }
        public int Group { get; private set; }

        public Puzzel(Size size, Point point, Matrix matrix, int group)
        {
            Location = point;
            Size = size;
            Matrix = matrix;
            Neighbours = new List<Puzzel>();
            Group = group;
        }

        public void Move(Point location)
        {
            Location = location;
        }

        public void AddNeighbour(Puzzel neighbour)
        {
            if(!Neighbours.Contains(neighbour))
                Neighbours.Add(neighbour);
        }

        public void RemoveNeighbour(Puzzel neighbour)
        {
            if(Neighbours.Contains(neighbour))
                Neighbours.Remove(neighbour);
        }

        public void AssignToGroup(int group)
        {
            Group = group;
        }
    }
}
