using System;
using Dustcloud.Maze.Model.Services;

namespace Dustcloud.Maze.Model
{
    public class Hero
    {
        public Hero(Tile tile)
        {
            OccupiedTile = tile;
        }

        public int X
        {
            get
            {
                if (OccupiedTile != null)
                {
                    return OccupiedTile.X;
                }

                return -1;
            }
        }

        public int Y
        {
            get
            {
                if (OccupiedTile != null)
                {
                    return OccupiedTile.Y;
                }

                return -1;
            }
        }

        public Direction Direction { get; set; }

        public Tile OccupiedTile { get; set; }

        public void Turn(int angle)
        {
            Direction = Direction + angle;
            if ((int)Direction >= 360)
            {
                Direction = Direction.North;
            }
            else if ((int)Direction < 0)
            {
                Direction = Direction.West;
            }
        }
    }
}