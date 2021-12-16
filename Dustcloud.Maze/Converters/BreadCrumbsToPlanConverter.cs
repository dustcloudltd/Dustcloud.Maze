using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using Dustcloud.Maze.Model;
using Dustcloud.Maze.Model.Model;

namespace Dustcloud.Maze.Converters
{
    public class BreadCrumbsToPlanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not List<Tile> route)
            {
                return Binding.DoNothing;
            }

            var plan = new StringBuilder();
            var index = 0;
            foreach (var tile in route)
            {
                plan.AppendLine($"{++index}. Move to coordinates: ({tile.X}, {tile.Y}).");
            }

            return plan.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
