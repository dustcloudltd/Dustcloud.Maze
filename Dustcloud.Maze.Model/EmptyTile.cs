namespace Dustcloud.Maze.Model;

public class EmptyTile : Tile
{
    public EmptyTile(int x, int y) : base(x, y, Model.TileType.Empty)
    {

    }
}