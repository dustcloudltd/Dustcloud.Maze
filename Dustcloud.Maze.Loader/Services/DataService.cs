using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dustcloud.IOC.Attributes;
using Dustcloud.Maze.Model.Model;

namespace Dustcloud.Maze.Services.Services
{
    [DependencyConfiguration(typeof(IDataService), typeof(DataService), LifetimeManagerType.Singleton)]
    internal sealed class DataService : IDataService
    {
        public async Task<IEnumerable<Tile>> LoadDataFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
            
            var mazeData = await File.ReadAllLinesAsync(filePath);
            return LoadData(mazeData);
        }

        private IEnumerable<Tile> LoadData(string[] mazeData)
        {
            var tileList = new List<Tile>();
            var yIndex = 0;
            foreach (var line in mazeData)
            {
                var xIndex = 0;

                foreach (var c in line)
                {
                    switch (c)
                    {
                        case ' ':
                            tileList.Add(new EmptyTile(xIndex, yIndex));
                            break;
                        case 'X':
                            tileList.Add(new Wall(xIndex, yIndex));
                            break;
                        case 'S':
                            tileList.Add(new StartTile(xIndex, yIndex));
                            break;
                        case 'F':
                            tileList.Add(new FinishTile(xIndex, yIndex));
                            break;
                        default:
                            break;
                    }

                    xIndex++;
                }

                yIndex++;
            }

            var validationErrors = ValidateTileList(tileList);
            if (!validationErrors.Any())
            {
                return tileList;
            }
            else
            {
                throw new InvalidDataException(string.Join(Environment.NewLine, validationErrors));
            }
        }

        private List<string> ValidateTileList(List<Tile> tileList)
        {
            var validationErrors = new List<string>();
            if (tileList.Count(s => s.TileType == TileType.Start) != 1)
            {
                validationErrors.Add($"Invalid number of start tiles.");
            }
            if (tileList.Count(s => s.TileType == TileType.Finish) != 1)
            {
                validationErrors.Add($"Invalid number of finish tiles.");
            }

            if (tileList.All(s => s.TileType != TileType.Empty))
            {
                validationErrors.Add($"Invalid number of empty tiles.");
            }
            if (tileList.All(s => s.TileType != TileType.Wall))
            {
                validationErrors.Add($"No wall tiles used.");
            }

            var maxX = tileList.Max(x => x.X);
            var maxY = tileList.Max(y => y.Y);
            if (tileList.Count(s => s.X == maxX) -1 != maxY||
            tileList.Count(s => s.Y == maxY) -1 != maxX)
            {
                validationErrors.Add($"Data not formatted properly");
            }

            return validationErrors;
        }

        public IEnumerable<Tile> LoadDataFromString(string mazeEdit)
        {
            return LoadData(mazeEdit.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
        }

        public async Task SaveAsync(string filePath, string mazeEdit)
        {
            if (File.Exists(filePath))
            {
                //too bad
            }

            await File.WriteAllTextAsync(filePath, mazeEdit);
        }
    }
}
