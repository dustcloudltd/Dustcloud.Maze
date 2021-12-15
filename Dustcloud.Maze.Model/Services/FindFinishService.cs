using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Dustcloud.IOC.Attributes;

namespace Dustcloud.Maze.Model.Services
{
    [DependencyConfiguration(typeof(IFindFinishService), typeof(FindFinishService), LifetimeManagerType.Singleton)]
    internal class FindFinishService : IFindFinishService
    {
        private readonly IBoardService _boardService;

        private Subject<Route> _routeSubject = new Subject<Route>();
        public FindFinishService(IBoardService boardService)
        {
            _boardService = boardService;
        }

        public IObservable<Route> ObserveRoutes()
        {
            return _routeSubject;
        }
        
        public List<Route> GetRoutes(List<Tile> board, List<Route> routes)
        {
            List<Route> toBeAdded = new();

            foreach (var route in routes.Where(s => !s.IsFinished).ToList())
            {
                var lastTile = route.OrderedBreadCrumbs.Last();
                lastTile.HasBeenVisited = true;
                var neighbors = _boardService.FindNonVisitedNeighbors(board, lastTile).ToList();
                foreach (var neighbor in neighbors.ToList())
                {
                    if (route.OrderedBreadCrumbs.Any(s => s.X == neighbor.X && s.Y == neighbor.Y))
                    {
                        neighbors.Remove(neighbor);
                    }
                }
                

                if (!neighbors.Any())
                {
                    routes.Remove(route);
                    continue;
                }

                var finishTile = neighbors.SingleOrDefault(s => s.TileType == TileType.Finish);
                if (finishTile != null)
                {
                    route.OrderedBreadCrumbs.Add(finishTile);
                    _routeSubject.OnNext(route);
                    routes.Remove(route);
                    continue;
                }

                foreach (var neighbor in neighbors)
                {
                    var newList = new List<Tile> (route.OrderedBreadCrumbs);
                    var neighborClone = neighbor.Clone() as Tile;
                    neighborClone.HasBeenVisited = true;
                    var newBoard = new List<Tile>(board);
                    var index = newBoard.IndexOf(neighbor);
                    newBoard.Remove(neighbor);
                    newBoard.Insert(index, neighborClone);
                    newList.Add(neighborClone);
                    
                    var newRoute = new Route(newList);
                    toBeAdded.Add(newRoute);
                }
                routes.Remove(route);

            }
            
            var routesDictionary = routes.ToDictionary(s => s.KeyChain);
            foreach (var add in toBeAdded)
            {
                if (!routesDictionary.ContainsKey(add.KeyChain))
                {
                    routes.Add(add);
                    routesDictionary.Add(add.KeyChain, add);
                }
            }

            if (routes.Any())
            {
                return GetRoutes(board, routes);
            }
            else
            {
                _routeSubject.OnCompleted();
                return routes;
            }
        }

        public void Redo()
        {
            _routeSubject = new Subject<Route>();
        }
    }
}
