using System.Collections.Generic;
using System.Threading.Tasks;
using Dustcloud.Maze.Model;

namespace Dustcloud.Maze.Loader
{
    public interface ILoaderService
    {
        Task<IEnumerable<Tile>> LoadDataFileAsync(string filePath);
        IEnumerable<Tile> LoadDataFromString(string mazeEdit);
    }
}