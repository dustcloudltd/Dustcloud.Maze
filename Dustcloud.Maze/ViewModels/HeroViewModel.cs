using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Dustcloud.IOC.Attributes;
using Dustcloud.Maze.Model;
using Prism.Commands;

namespace Dustcloud.Maze.ViewModels

{
    [DependencyConfiguration(typeof(IHeroViewModel), typeof(HeroViewModel), LifetimeManagerType.Transient)]
    internal class HeroViewModel : ViewModelBase, IHeroViewModel
    {
        public Hero Hero { get; set; }
        private ObservableCollection<string> _movementLogCollection = new();

        public HeroViewModel()
        {
            MovementLogCollection.CollectionChanged += MovementLogOnCollectionChanged;
        }

        private void MovementLogOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                OnPropertyChanged(nameof(MovementLog));
            }
        }

        public string MovementLog => string.Join(Environment.NewLine, MovementLogCollection);

        public int X
        {
            get
            {
                if (Hero != null)
                {
                    return Hero.X;
                }
                else return -1;
            }
        }

        public int Y
        {
            get
            {
                if (Hero != null)
                {
                    return Hero.Y;
                }
                else return -1;
            }
        }

        public Direction Direction
        {
            get
            {
                if (Hero != null)
                {
                    return Hero.Direction;
                }
                
                return Direction.North;
            }
            set
            {
                if (value == Hero.Direction) return;
                Hero.Direction = value;
                OnPropertyChanged(nameof(Direction));
            }
        }

        public void Initialize()
        {
            MovementLogCollection.Clear();
            MovementLogCollection.Add($"Hero lands on coordinates ({X}, {Y}) facing {Direction}");
            ResetXY();
        }

        public void Turn(int angle)
        {
            Hero.Turn(angle);
            OnPropertyChanged(nameof(Direction));
            var turns = "left";
            if (angle == 90)
            {
                turns = "right";
            }
            MovementLogCollection.Add($"Hero turns {turns} and is now facing {Direction}");
        }

        public void MoveForward()
        {
            ResetXY();
            MovementLogCollection.Add($"Hero moves onto coordinates ({X}, {Y}) facing {Direction}");
        }

        public void ResetXY()
        {
            OnPropertyChanged(nameof(X));
            OnPropertyChanged(nameof(Y));
        }

        public ObservableCollection<string> MovementLogCollection
        {
            get => _movementLogCollection;
            set
            {
                if (Equals(value, _movementLogCollection)) return;
                _movementLogCollection = value;
                OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            MovementLogCollection.CollectionChanged -= MovementLogOnCollectionChanged;
        }
    }
}
