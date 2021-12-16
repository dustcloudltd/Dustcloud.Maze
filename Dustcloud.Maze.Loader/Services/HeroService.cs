using System;
using System.Collections.Generic;
using System.Linq;
using Dustcloud.IOC.Attributes;
using Dustcloud.Maze.Model.Model;

namespace Dustcloud.Maze.Services.Services
{
    [DependencyConfiguration(typeof(IHeroService), typeof(HeroService), LifetimeManagerType.Singleton)]
    internal class HeroService : IHeroService
    {
        public IEnumerable<Tile> FindNonVisitedNeighbors(IEnumerable<Tile> board, Tile tile)
        {
            return board.Where(s => s.CanBeSteppedOver &&
                                    ((s.X - 1 == tile.X && s.Y == tile.Y) ||
                                     (s.X + 1 == tile.X && s.Y == tile.Y) ||
                                     (s.X == tile.X && s.Y - 1 == tile.Y) ||
                                     (s.X == tile.X && s.Y + 1 == tile.Y)) &&
                                     !s.HasBeenVisited)
                .ToList();
        }

        public IEnumerable<Tile> FindAllNeighbors(IEnumerable<Tile> board, Tile tile)
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
