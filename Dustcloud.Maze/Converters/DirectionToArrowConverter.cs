using System;
using System.Globalization;
using System.Windows.Data;
using Dustcloud.Maze.Model;

namespace Dustcloud.Maze.Converters
{
    public class DirectionToArrowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Direction direction)
            {
                switch (direction)
                {
                    case Direction.North:
                        return "↑";
                    case Direction.South:
                        return "↓";
                    case Direction.East:
                        return "→";
                    case Direction.West:
                        return "←";
                }
            }

            return "X";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
