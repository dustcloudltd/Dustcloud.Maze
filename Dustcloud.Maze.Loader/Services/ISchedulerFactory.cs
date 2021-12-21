using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace Dustcloud.Maze.Services.Services
{
    public interface ISchedulerFactory
    {
        IScheduler DefaultScheduler { get; }
        IScheduler Dispatcher { get; }
    }
}
