using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Puzzle.View
{
    static class PuzzleHelper
    {
        public static void SetLocation(FrameworkElement element, double x, double y)
        {
            Canvas.SetTop(element, y);
            Canvas.SetLeft(element, x);
        }

        public static void ResizeElement(FrameworkElement element, double width, double height)
        {
            element.Width = width;
            element.Height = height;
        }

        public static void MoveElementOnCanvas(UIElement element, double toX, double toY)
        {
            double fromX = Canvas.GetLeft(element);
            double fromY = Canvas.GetTop(element);

            Storyboard storyboard = new Storyboard();
            DoubleAnimation animationX = CreateAnimation(element,fromX,toX, new PropertyPath(Canvas.LeftProperty));
            DoubleAnimation animationY = CreateAnimation(element, fromY, toY, new PropertyPath(Canvas.TopProperty));

            storyboard.Children.Add(animationX);
            storyboard.Children.Add(animationY);
            storyboard.Begin();
        }

        private static DoubleAnimation CreateAnimation(UIElement element, double from, double to, PropertyPath propertyToAnimate)
        {
            DoubleAnimation animation = new DoubleAnimation();
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, propertyToAnimate);
            animation.From = from;
            animation.To = to;
            animation.Duration = TimeSpan.FromMilliseconds(1500);
            return animation;
        }
    }
}
