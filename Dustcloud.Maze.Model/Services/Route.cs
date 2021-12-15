using System.Collections.Generic;
using System.Linq;

namespace Dustcloud.Maze.Model.Services;

public class Route
{
    public Route(List<Tile> originalRoute)
    {
        OrderedBreadCrumbs = originalRoute;
        KeyChain = string.Join(';', OrderedBreadCrumbs.Select(s => $"({s.X},{s.Y})"));
    }

    public string KeyChain { get; }

    public List<Tile> OrderedBreadCrumbs { get; } = new();

    public bool IsFinished => OrderedBreadCrumbs.Any(s => s.TileType == TileType.Finish);

    public int Length => OrderedBreadCrumbs.Count;

}