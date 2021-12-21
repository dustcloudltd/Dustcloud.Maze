using System.Reactive.Concurrency;
using System.Windows.Threading;
using Dustcloud.IOC.Attributes;

namespace Dustcloud.Maze.Services.Services
{
    [DependencyConfiguration(typeof(ISchedulerFactory), typeof(SchedulerFactory), LifetimeManagerType.Singleton)]
    internal sealed class SchedulerFactory : ISchedulerFactory
    {
        private IScheduler _dispatcher;
        public IScheduler DefaultScheduler { get; } = Scheduler.Default;

        public IScheduler Dispatcher
        {
            get
            {
                if (_dispatcher == null)
                {
                    _dispatcher = new SynchronizationContextScheduler(new DispatcherSynchronizationContext(System.Windows.Threading.Dispatcher.CurrentDispatcher));
                }

                return _dispatcher;
            }
        } 
    }
}