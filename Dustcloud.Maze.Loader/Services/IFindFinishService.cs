using System;
using System.Collections.Generic;
using Dustcloud.Maze.Model.Model;

namespace Dustcloud.Maze.Services.Services
{
    /// <summary>
    /// When implemented, serves as the automatic route searching service.
    /// </summary>
    public interface IFindFinishService
    {
        IObservable<Route> ObserveRoutes();
        List<Route> FindAllRoutes(List<Tile> board, List<Route> routes, bool onlyFindQuickest = false);
        IObservable<Route> ObserveSingleRoute();
        IObservable<bool> ObserveReset();
        IEnumerable<Tile> FindNonVisitedNeighbors(IEnumerable<Tile> board, Tile tile);
        bool CanMoveForward(Hero hero, IEnumerable<Tile> neighbors);
        IEnumerable<Tile> MoveForward(List<Tile> board, Hero hero);
    }
}