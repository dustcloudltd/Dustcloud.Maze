namespace Dustcloud.Maze.Model.Model
{
    public class Hero
    {
        public Hero(Tile tile)
        {
            OccupiedTile = tile;
            OccupiedTile.IsOccupied = true;
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