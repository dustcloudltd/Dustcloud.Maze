using System.Linq;
using System.Windows;
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

            var assemblies = App.ResourceAssembly.GetReferencedAssemblies().Union(new[] { App.ResourceAssembly.GetName() }).ToArray();
            var resolver = Dustcloud.IOC.Initializer.DependencyInitializer.Initialize(assemblies);

            this.MainWindow = new MainWindow(){ DataContext = resolver.Resolve<IMainWindowViewModel>() };
            this.MainWindow.Show();
            base.OnStartup(e);

        }
    }
}
