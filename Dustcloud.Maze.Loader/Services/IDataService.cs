using System.Collections.Generic;
using System.Threading.Tasks;
using Dustcloud.Maze.Model.Model;

namespace Dustcloud.Maze.Services.Services
{
    public interface IDataService
    {
        Task<IEnumerable<Tile>> LoadDataFileAsync(string filePath);
        IEnumerable<Tile> LoadDataFromString(string mazeEdit);
        Task SaveAsync(string filePath, string mazeEdit);
    }
}