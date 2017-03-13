using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Puzzle.View
{
    /// <summary>
    /// Interaction logic for Puzzle.xaml
    /// </summary>
    public partial class Puzzle : Window
    {
        ViewModel.PuzzleViewModel viewModel;

        public Puzzle()
        {
            InitializeComponent();
            viewModel = FindResource("viewModel") as ViewModel.PuzzleViewModel;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource is System.Windows.Controls.Image)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    viewModel.MoveManyPuzzle(e.GetPosition(null));
                }
                else if (e.LeftButton == MouseButtonState.Pressed)
                {
                    viewModel.MovePuzzle(e.GetPosition(null));
                }
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is System.Windows.Controls.Image)
            {
                viewModel.SelectPuzzle(e.GetPosition(null));
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.StickPuzzles();
        }

    }
}
