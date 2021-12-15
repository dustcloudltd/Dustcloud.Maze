using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Prism.Commands;

namespace Dustcloud.Maze.ViewModels;

public interface IMainWindowViewModel : INotifyPropertyChanged, IDisposable
{
    IHeroViewModel HeroViewModel { get; }
    ICommand ItemClickCommand { get; }
    ObservableCollection<TileViewModel> TileCollection { get; }
    ICommand LoadDataCommand { get; }
    DelegateCommand<bool?> CheckCoordsCommand { get; }
    DelegateCommand ValidateAndParseDataCommand { get; }
    DelegateCommand PlaceHeroCommand { get; }
    string FilePath { get; set; }
    int NumberOfWalls { get; }
    int NumberOfEmpties { get; }
    string CoordExplanation { get; set; }
    int CoordY { get; }
    int CoordX { get; }
}