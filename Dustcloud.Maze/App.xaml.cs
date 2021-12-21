using System.Linq;
using System.Windows;
using Dustcloud.IOC.Initializer;
using Dustcloud.Maze.ViewModels;

namespace Dustcloud.Maze
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var resolver = DependencyInitializer.Initialize();

            this.MainWindow = new MainWindow(){ DataContext = resolver.Resolve<IMainWindowViewModel>() };
            this.MainWindow.Show();
            base.OnStartup(e);

        }
    }
}
