using Dustcloud.Maze.Annotations;
using Dustcloud.Maze.Model;

namespace Dustcloud.Maze.ViewModels
{
    public class TileViewModel : ViewModelBase
    {
        private bool _isLitUp;

        [NotNull]
        public Tile Tile { get; }

        public int X => Tile.X;
        public int Y => Tile.Y;

        public TileViewModel(Tile tile)
        {
            Tile = tile;
        }

        public TileType TileType => Tile.TileType;

        public bool IsOccupied
        {
            get { return Tile.IsOccupied; }
        }

        public bool IsLitUp
        {
            get => _isLitUp;
            set
            {
                if (value == _isLitUp) return;
                _isLitUp = value;
                OnPropertyChanged();
            }
        }

        public void ResetOccupancy()
        {
            OnPropertyChanged(nameof(IsOccupied));
        }
    }
}
