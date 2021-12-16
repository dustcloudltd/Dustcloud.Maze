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

namespace Dustcloud.Maze.Tests
{
    [TestFixture]
    public class FindFinishServiceTests
    {
        [Test]
        public void GivenAMockDataBoard_FindBothRoutes()
        {
            var board = DataMocker.GetDataBoard();
            var startTile = board.Single(s => s.TileType == TileType.Start);
            var originalRouteTileList = new List<Tile>();
            originalRouteTileList.Add(startTile);
            var originalRoute = new Route(originalRouteTileList);
            var heroService= new HeroService();
            var service = new FindFinishService(heroService);

            List<Route> routes = new();

            service.ObserveRoutes().Subscribe(r => routes.Add(r));
            service.FindAllRoutes(board.ToList(), new List<Route> { originalRoute }, false);

            Assert.IsNotEmpty(routes);
            Assert.AreEqual(2, routes.Count);
        }
    }
}
