using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Dustcloud.Maze.Model;

namespace Dustcloud.Maze.ViewModels
{
    public interface IHeroViewModel : IDisposable
    {
        public Hero Hero { get; set; }
        string MovementLog { get; }
        int X { get; }
        int Y { get; }

        Direction Direction { get; set; }

        //DelegateCommand MoveForwardCommand { get; }
        //DelegateCommand<string> TurnCommand { get; }
        ObservableCollection<string> MovementLogCollection { get; set; }

        //event EventHandler CheckIfCanMoveForward;
        void Initialize();
        event PropertyChangedEventHandler? PropertyChanged;
        void Turn(int angle);
        void MoveForward();
        void ResetXY();
    }
}