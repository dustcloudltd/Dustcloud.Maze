namespace Dustcloud.Maze.Model;

public class Wall : Tile
{
    public Wall(int x, int y) : base(x, y, Model.TileType.Wall)
    {

    }
}