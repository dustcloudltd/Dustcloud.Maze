using System;
using System.Collections.Generic;

namespace Dustcloud.Maze.Model.Services;

public interface IFindFinishService
{
    IObservable<Route> ObserveRoutes();
    List<Route> GetRoutes(List<Tile> board, List<Route> routes);
    void Redo();
}