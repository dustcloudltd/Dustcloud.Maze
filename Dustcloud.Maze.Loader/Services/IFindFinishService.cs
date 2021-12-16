using System;
using System.Collections.Generic;
using Dustcloud.Maze.Model.Model;

namespace Dustcloud.Maze.Services.Services;

public interface IFindFinishService
{
    IObservable<Route> ObserveRoutes();
    List<Route> FindAllRoutes(List<Tile> board, List<Route> routes, bool onlyFindQuickest = false);
    IObservable<Route> ObserveSingleRoute();
    IObservable<bool> ObserveReset();
}