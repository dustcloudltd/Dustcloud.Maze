using System;

namespace Dustcloud.Maze.Model.Model
{
    public abstract class Tile : ICloneable
    {
        private bool _isOccupied;
        public int X { get; }
        public int Y { get; }

        protected Tile(int x, int y, TileType tileType)
        {
            X = x;
            Y = y;
            TileType = tileType;
        }

        public bool IsOccupied
        {
            get => _isOccupied;
            set
            {
                _isOccupied = value;
                HasBeenVisited = true;
            }
        }

        public bool CanBeSteppedOver => TileType != TileType.Wall;

        public TileType TileType { get; }
        public bool HasBeenVisited { get; set; }

        public object Clone()
        {
            if (TileType == TileType.Empty)
            {
                return new EmptyTile(X, Y) { IsOccupied = this.IsOccupied, HasBeenVisited = this.HasBeenVisited };
            }
            else if (TileType == TileType.Wall)
            {
                return new Wall(X, Y) { IsOccupied = this.IsOccupied, HasBeenVisited = this.HasBeenVisited };
            }
            else if (TileType == TileType.Finish)
            {
                return new FinishTile(X, Y) { IsOccupied = this.IsOccupied, HasBeenVisited = this.HasBeenVisited };
            }
            else
            {
                return new StartTile(X, Y) { IsOccupied = this.IsOccupied, HasBeenVisited = this.HasBeenVisited };
            }

        }
    }
}