using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dustcloud.Maze.Model.Model;
using Dustcloud.Maze.Services.Services;
using Dustcloud.Maze.Services.Tests;
using Moq;
using NUnit.Framework;

namespace Dustcloud.Maze.Services.Tests
{
    public class HeroServiceTests
    {
        [TestCase(2, 1, Direction.East, ExpectedResult = true)]
        [TestCase(2, 1, Direction.North, ExpectedResult = false)]
        public bool GivenAHeroAndNeighbors_CheckIfCanMoveForward(int x, int y, Direction direction)
        {
            var board = DataMocker.GetDataBoard();

            var hero = new Hero(board.Single(s => s.X == x && s.Y == y)) { Direction = direction };

            var service = new FindFinishService();
            var neighbors = service.FindNonVisitedNeighbors(board, hero.OccupiedTile);
            var canMoveForward = service.CanMoveForward(hero, neighbors);

            return canMoveForward;
        }

        [Test]
        public void GivenAHeroAndBoard_MoveForwardFindAllNeighbors()
        {
            var board = DataMocker.GetDataBoard();

            var hero = new Hero(board.Single(s => s.TileType == TileType.Start)){Direction = Direction.East};

            var service = new FindFinishService();
            var neighbors = service.MoveForward(board.ToList(), hero).ToList();

            Assert.NotNull(neighbors);
            Assert.IsNotEmpty(neighbors);
            Assert.AreEqual(3, neighbors.Count);
            Assert.AreSame(neighbors.Single(x => x.X == 2 && x.Y == 1), board.Single(x => x.X == 2 && x.Y == 1));
            Assert.AreSame(neighbors.Single(x => x.X == 1 && x.Y == 2), board.Single(x => x.X == 1 && x.Y == 2));
            Assert.AreSame(neighbors.Single(x => x.X == 0 && x.Y == 1), board.Single(x => x.X == 0 && x.Y == 1));


        }

        [Test]
        public void GivenAHeroOnTheBoard_FindNonVisitedNeighbors()
        {
            var board = DataMocker.GetDataBoard();
            board.Single(x => x.X == 1 && x.Y == 1).HasBeenVisited = true;
            
            var occupiedTile = board.Single(x => x.X == 2 && x.Y == 1);
            occupiedTile.IsOccupied = true;
            var service = new FindFinishService();
            var neighbors = service.FindNonVisitedNeighbors(board.ToList(), occupiedTile).ToList();

            Assert.NotNull(neighbors);
            Assert.IsNotEmpty(neighbors);
            Assert.AreEqual(1, neighbors.Count);
            Assert.AreSame(neighbors.Single(x => x.X == 3 && x.Y == 1), board.Single(x => x.X == 3 && x.Y == 1));

        }
        [Test]
        public void GivenAMockDataBoard_FindBothRoutes()
        {
            var board = DataMocker.GetDataBoard();
            var startTile = board.Single(s => s.TileType == TileType.Start);
            var originalRouteTileList = new List<Tile>();
            originalRouteTileList.Add(startTile);
            var originalRoute = new Route(originalRouteTileList);
            var service = new FindFinishService();

            List<Route> routes = new();

            service.ObserveRoutes().Subscribe(r => routes.Add(r));
            service.FindAllRoutes(board.ToList(), new List<Route> { originalRoute }, false);

            Assert.IsNotEmpty(routes);
            Assert.AreEqual(2, routes.Count);
        }
    }


    public class DataMocker
    {

        public static string GetDataString()
        {
            return $@"XXXFX
S   X
X X X
X   X
XXXXX";
        }

        public static IEnumerable<Tile> GetDataBoard()
        {
            return new List<Tile>()
            {
                new Wall(0, 0),
                new Wall(1, 0),
                new Wall(2, 0),
                new FinishTile(3, 0),
                new Wall(4, 0),
                //XXXFX
                
                new StartTile(0, 1),
                new EmptyTile(1, 1),
                new EmptyTile(2, 1),
                new EmptyTile(3, 1),
                new Wall(4, 1),
                //S   X

                new Wall(0, 2),
                new EmptyTile(1, 2),
                new Wall(2, 2),
                new EmptyTile(3, 2),
                new Wall(4, 2),
                //X X X

                new Wall(0, 3),
                new EmptyTile(1, 3),
                new EmptyTile(2, 3),
                new EmptyTile(3, 3),
                new Wall(4, 3),
                //X   X

                new Wall(0, 4),
                new Wall(1, 4),
                new Wall(2, 4),
                new Wall(3, 4),
                new Wall(4, 4),
                //XXXXX
            };
        }
    }
}