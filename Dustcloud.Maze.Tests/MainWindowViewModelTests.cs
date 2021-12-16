using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Dustcloud.Maze.Model.Model;
using Dustcloud.Maze.Services.Services;
using Dustcloud.Maze.ViewModels;
using Moq;
using NUnit.Framework;
using Microsoft.Reactive.Testing;

namespace Dustcloud.Maze.Tests
{
    [TestFixture]
    public class MainWindowViewModelTests : ReactiveTest
    {
        [Test]
        public void GivenAStream_RoutesMustBePopulated()
        {
            var findFinishService = new Mock<IFindFinishService>();
            findFinishService.Setup(n =>
                    n.FindAllRoutes(It.IsAny<List<Tile>>(), It.IsAny<List<Route>>(), It.IsAny<bool>()))
                .Returns(new List<Route>());
            var testScheduler = new TestScheduler();

            var schedulerFactory = new Mock<ISchedulerFactory>();
            schedulerFactory.SetupGet(s => s.Dispatcher).Returns(testScheduler);
            schedulerFactory.SetupGet(s => s.DefaultScheduler).Returns(testScheduler);

            var input = testScheduler.CreateColdObservable(OnNext(100, new Route(new List<Tile>())),
                                                           OnNext(200, new Route(new List<Tile> { new StartTile(0, 0), new EmptyTile(0, 1) })),
                                                           OnNext(250, new Route(new List<Tile> { new StartTile(0, 0), new EmptyTile(0, 1), new EmptyTile(1, 1) })),
                                                           OnNext(300, new Route(new List<Tile>
                                                                                { new StartTile(0, 0), new EmptyTile(0, 1), new EmptyTile(1, 1), new FinishTile(1, 2) })));
            
            findFinishService.Setup(s => s.ObserveRoutes())
                .Returns(input);
            findFinishService.Setup(s => s.ObserveSingleRoute())
                .Returns(Observable.Empty<Route>());

            findFinishService.Setup(s => s.ObserveReset())
                .Returns(testScheduler.CreateColdObservable(OnNext(450, true)));

            var vm = new MainWindowViewModelBuilder().With(schedulerFactory.Object)
                                                                     .With(findFinishService.Object)
                                                                     .Build();
            
            testScheduler.AdvanceTo(250);
            Assert.AreEqual(2, vm.Routes.Count);
            Assert.IsFalse(vm.IsCalculating);
            Assert.AreEqual(2, vm.TotalRoutesFound);

            testScheduler.AdvanceTo(500);
            Assert.IsNotEmpty(vm.Routes);
            Assert.AreEqual(4, vm.Routes.Count);
            Assert.IsFalse(vm.IsCalculating);
            Assert.AreEqual(4, vm.TotalRoutesFound);
        }
    }

    internal class MainWindowViewModelBuilder
    {
        private IDataService _dataService;
        private IFindFinishService _findFinishService;
        private IHeroViewModel _heroViewModel;
        private ISchedulerFactory _schedulerFactory;

        public MainWindowViewModel Build()
        {
            return new MainWindowViewModel(_dataService ?? Mock.Of<IDataService>(),
                _findFinishService ?? Mock.Of<FindFinishService>(),
                _schedulerFactory ?? Mock.Of<ISchedulerFactory>(),
                _heroViewModel ?? Mock.Of<IHeroViewModel>());

        }
        public MainWindowViewModelBuilder With(IHeroViewModel value)
        {
            _heroViewModel = value;
            return this;
        }
        public MainWindowViewModelBuilder With(ISchedulerFactory value)
        {
            _schedulerFactory = value;
            return this;
        }

        public MainWindowViewModelBuilder With(IFindFinishService value)
        {
            _findFinishService = value;
            return this;
        }

        public MainWindowViewModelBuilder With(IDataService value)
        {
            _dataService = value;
            return this;
        }
    }

}
