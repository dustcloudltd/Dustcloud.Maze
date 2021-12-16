using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Dustcloud.IOC.Attributes;
using Dustcloud.Maze.Model.Model;

namespace Dustcloud.Maze.Services.Services
{
    [DependencyConfiguration(typeof(IFindFinishService), typeof(FindFinishService), LifetimeManagerType.Singleton)]
    internal class FindFinishService : IFindFinishService
    {
        private readonly IHeroService _heroService;
        private Subject<Route> _routeSubject = new();
        private Subject<bool> _resetSubject = new();
        private Subject<Route> _singleRouteSubject = new();

        public FindFinishService(IHeroService heroService)
        {
            _heroService = heroService;
        }

        public IObservable<Route> ObserveRoutes()
        {
            return _routeSubject;
        }

        public IObservable<Route> ObserveSingleRoute()
        {
            return _singleRouteSubject;
        }

        public IObservable<bool> ObserveReset()
        {
            return _resetSubject;
        }

        public List<Route> FindAllRoutes(List<Tile> board, List<Route> routes, bool onlyFindQuickest = false)
        {
            List<Route> toBeAdded = new();

            foreach (var route in routes.Where(s => !s.IsFinished).ToList())
            {
                var lastTile = route.OrderedBreadCrumbs.Last();
                lastTile.HasBeenVisited = true;
                
                var neighbors = _heroService.FindNonVisitedNeighbors(board, lastTile).ToList();
                
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

                    if (onlyFindQuickest)
                    {
                        _singleRouteSubject.OnNext(route);
                        _resetSubject.OnNext(true);
                        routes.Clear();
                        return routes;
                    }

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
                return FindAllRoutes(board, routes, onlyFindQuickest);
            }
            else
            {
                _resetSubject.OnNext(true);
                return routes;
            }
        }
    }
}
