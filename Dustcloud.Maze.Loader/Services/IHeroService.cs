using System.Collections.Generic;
using Dustcloud.Maze.Model.Model;

namespace Dustcloud.Maze.Services.Services;

public interface IHeroService
{
    IEnumerable<Tile> FindAllNeighbors(IEnumerable<Tile> board, Tile tile);
    IEnumerable<Tile> FindNonVisitedNeighbors(IEnumerable<Tile> board, Tile tile);
    bool CanMoveForward(Hero hero, IEnumerable<Tile> neighbors);
    IEnumerable<Tile> MoveForward(List<Tile> board, Hero hero);
}