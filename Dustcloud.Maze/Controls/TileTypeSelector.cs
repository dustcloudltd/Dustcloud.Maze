using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Dustcloud.Maze.Model;
using Dustcloud.Maze.ViewModels;

namespace Dustcloud.Maze.Controls
{
    public class TileTypeSelector : DataTemplateSelector
    {
        public DataTemplate EmptyTileTemplate { get; set; }
        public DataTemplate WallTemplate { get; set; }
        public DataTemplate FinishTemplate { get; set; }
        public DataTemplate StartTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is TileViewModel tvm)
            {
                switch (tvm.TileType)
                {
                    case TileType.Finish:
                        return FinishTemplate;
                    case TileType.Empty:
                        return EmptyTileTemplate;
                    case TileType.Start:
                        return StartTemplate;
                    case TileType.Wall:
                        return WallTemplate;

                }
                
            }
           
            return null;

        }
    }
}
