using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Dustcloud.IOC.Attributes;
using Dustcloud.Maze.Model.Model;

namespace Dustcloud.Maze.Services.Services
{
    [DependencyConfiguration(typeof(IFindFinishService), typeof(FindFinishService), LifetimeManagerType.Singleton)]
    internal sealed class FindFinishService : IFindFinishService
    {
        private Subject<Route> _routeSubject = new();
        private Subject<bool> _resetSubject = new();
        private Subject<Route> _singleRouteSubject = new();

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

        /// <summary>
        /// One big function, fo sho.
        /// </summary>
        /// <param name="board">The Board</param>
        /// <param name="routes">The routes (the starting single route with just a StartTile in)</param>
        /// <param name="onlyFindQuickest">Self exp n'est-ce pas?</param>
        /// <returns>Doesn't matter</returns>
        public List<Route> FindAllRoutes(List<Tile> board, List<Route> routes, bool onlyFindQuickest = false)
        {
            List<Route> toBeAdded = new();

            foreach (var route in routes.Where(s => !s.IsFinished).ToList())
            {
                var lastTile = route.OrderedBreadCrumbs.Last();
                lastTile.HasBeenVisited = true;
                
                var neighbors = FindNonVisitedNeighbors(board, lastTile).ToList();
                
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
                
                foreach (var neighbor in neighbors.Where(s => s.TileType != TileType.Finish))
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

                //This could be optimized better-- 
                //Something to talk about
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

                    //need some sort of counter here to ask every 10k times whether or not to continue, otherwise will get OutOfMemoryException after around 400k records on 16gig system
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

        public IEnumerable<Tile> FindNonVisitedNeighbors(IEnumerable<Tile> board, Tile tile)
        {
            return FindAllNeighbors(board, tile).Where(s => !s.HasBeenVisited)
                                                .ToList();
        }

        private IEnumerable<Tile> FindAllNeighbors(IEnumerable<Tile> board, Tile tile)
        {
            return board.Where(s => s.CanBeSteppedOver &&
                                    ((s.X - 1 == tile.X && s.Y == tile.Y) ||
                                     (s.X + 1 == tile.X && s.Y == tile.Y) ||
                                     (s.X == tile.X && s.Y - 1 == tile.Y) ||
                                     (s.X == tile.X && s.Y + 1 == tile.Y)));
        }


        public bool CanMoveForward(Hero hero, IEnumerable<Tile> neighbors)
        {
            var (newPositionX, newPositionY) = CalculateForwardPosition(hero.X, hero.Y, hero.Direction);
            var newTile = neighbors.SingleOrDefault(s => s.X == newPositionX &&
                                                         s.Y == newPositionY &&
                                                         s.TileType != TileType.Wall);
            return newTile != null;
        }

        public IEnumerable<Tile> MoveForward(List<Tile> board, Hero hero)
        {
            var (newPositionX, newPositionY) = CalculateForwardPosition(hero.X, hero.Y, hero.Direction);
            hero.OccupiedTile.IsOccupied = false;
            var occupiedTile = board.Single(s => s.X == newPositionX && s.Y == newPositionY);
            occupiedTile.IsOccupied = true;
            hero.OccupiedTile = occupiedTile;

            return FindAllNeighbors(board, hero.OccupiedTile);
        }

        private (int, int) CalculateForwardPosition(int x, int y, Direction direction)
        {
            double newPositionX = x;
            double newPositionY = y;
            newPositionX += Math.Round(Math.Sin(Math.PI * Convert.ToDouble(direction) / 180.0), 0);
            newPositionY += -Math.Round(Math.Cos(Math.PI * Convert.ToDouble(direction) / 180.0), 0);
            var newX = Convert.ToInt32(newPositionX);
            var newY = Convert.ToInt32(newPositionY);
            return (newX, newY);
        }

    }
}
