using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Dustcloud.IOC.Attributes;
using Dustcloud.Maze.Model.Model;
using Dustcloud.Maze.Services.Services;
using Prism.Commands;

namespace Dustcloud.Maze.ViewModels
{
    /// <summary>
    /// This is where all the fun happens.
    /// The DependencyConfiguration attribute is of my devise - Dustcloud.IOC.Standard nuget package 
    /// </summary>
    [DependencyConfiguration(typeof(IMainWindowViewModel), typeof(MainWindowViewModel), LifetimeManagerType.Singleton)]
    internal sealed class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        private readonly IDataService _dataService;
        private readonly IFindFinishService _findFinishService;
        private readonly ISchedulerFactory _schedulerFactory;

        private ICommand _loadDataCommand;
        private ICommand _itemClickCommand;
        private DelegateCommand<bool?> _checkCoordsCommand;
        private DelegateCommand _validateAndParseDataCommand;
        private DelegateCommand _executePlaceExplorer;
        private DelegateCommand<string> _findAutomaticRouteCommand;
        private ICommand _newFileCommand;
        private IEnumerable<Tile> _neighbors;

        private int _coordX;
        private int _coordY;
        private int _columns;
        private int _rows;
        private bool _isCalculating;
        private string _coordExplanation;
        private string _filePath;
        private string _mazeEdit;

        private DelegateCommand _moveForwardCommand;
        private DelegateCommand<string> _turnCommand;
        private Route _selectedRoute;
        private bool _isFileLoaded;
        private bool _isNewFile;
        private ICommand _saveAsDataCommand;
        private int _totalRoutesFound;

        public ObservableCollection<TileViewModel> TileCollection { get; } = new();
        public ObservableCollection<Route> Routes { get; } = new();

        public ICollectionView RoutesCollectionView { get; }

        public IHeroViewModel HeroViewModel { get; }


        public MainWindowViewModel(IDataService dataService,
                                   IFindFinishService findFinishService,
                                   ISchedulerFactory schedulerFactory,
                                   IHeroViewModel heroViewModel)
        {
            _dataService = dataService;
            _findFinishService = findFinishService;
            _schedulerFactory = schedulerFactory;
            HeroViewModel = heroViewModel;
            
            PropertyChanged += OnPropertyChanged;
            TileCollection.CollectionChanged += TileCollectionOnCollectionChanged;
            
            RoutesCollectionView = new ListCollectionView(Routes);
            RoutesCollectionView.SortDescriptions
                              .Add(new SortDescription(nameof(Route.Length), ListSortDirection.Ascending));

            SubscribeObservables();
        }

        private void TileCollectionOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            FindAutomaticRouteCommand.RaiseCanExecuteChanged();
            PlaceHeroCommand.RaiseCanExecuteChanged();
            CheckCoordsCommand.RaiseCanExecuteChanged();
        }

        private void SubscribeObservables()
        {
            Disposables.Add(_findFinishService.ObserveRoutes()
                .SubscribeOn(_schedulerFactory.DefaultScheduler)
                .ObserveOn(_schedulerFactory.Dispatcher)
                .Subscribe(AddRoute, OnError));

            Disposables.Add(_findFinishService.ObserveSingleRoute()
                .Take(2)
                .SubscribeOn(_schedulerFactory.DefaultScheduler)
                .ObserveOn(_schedulerFactory.Dispatcher)
                .Subscribe(onNext: AddRoute, OnError));

            Disposables.Add(_findFinishService.ObserveReset()
                .SubscribeOn(_schedulerFactory.DefaultScheduler)
                .ObserveOn(_schedulerFactory.Dispatcher)
                .Subscribe(OnResetReceived));

        }

        private void OnError(Exception exception)
        {
            IsCalculating = false;
            FindAutomaticRouteCommand.RaiseCanExecuteChanged();
        }

        private void AddRoute(Route route)
        {
            Routes.Add(route);
            TotalRoutesFound++;
        }

        private void OnResetReceived(bool isReset)
        {
            if (isReset)
            {
                IsCalculating = false;
                FindAutomaticRouteCommand.RaiseCanExecuteChanged();
                return;
            }
        }

        public int TotalRoutesFound
        {
            get => _totalRoutesFound;
            set
            {
                if (value == _totalRoutesFound) return;
                _totalRoutesFound = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfWalls
        {
            get { return TileCollection.Count(s => s.TileType == TileType.Wall); }
        }

        public int NumberOfEmpties
        {
            get { return TileCollection.Count(s => s.TileType == TileType.Empty); }
        }

        public DelegateCommand<bool?> CheckCoordsCommand
        {
            get { return _checkCoordsCommand ??= new DelegateCommand<bool?>(ExecuteCheckCoords, CanExecuteCheckCoords); }
        }

        private bool CanExecuteCheckCoords(bool? arg)
        {
            return TileCollection.Any() && CoordY < Rows && CoordX < Columns && CoordY >= 0 && CoordX >=0;
        }

        public DelegateCommand ValidateAndParseDataCommand
        {
            get { return _validateAndParseDataCommand ??= new DelegateCommand(ExecuteValidateAndParseData, CanExecuteValidateAndParseData); }
        }

        public DelegateCommand PlaceHeroCommand
        {
            get { return _executePlaceExplorer ??= new DelegateCommand(ExecutePlaceExplorer, CanExecutePlaceExplorer); }
        }

        public ICommand ItemClickCommand
        {
            get { return _itemClickCommand ??= new DelegateCommand<TileViewModel>(ExecuteItemClick); }
        }
        public ICommand LoadDataCommand
        {
            get { return _loadDataCommand ??= new DelegateCommand(ExecuteLoadDataAsync); }
        }

        public ICommand SaveAsDataCommand
        {
            get 
            {
                return _saveAsDataCommand ??= new DelegateCommand(ExecuteSaveData, () => IsNewFile && 
                                                                                                        !string.IsNullOrEmpty(MazeEdit) &&
                                                                                                        TileCollection.Any());
            }
        }

        public ICommand NewFileCommand
        {
            get { return _newFileCommand ??= new DelegateCommand(ExecuteNewFile); }
        }


        public DelegateCommand<string> FindAutomaticRouteCommand
        {
            get
            {
                return _findAutomaticRouteCommand ??= new DelegateCommand<string>(ExecuteFindAutomaticRoute, CanExecuteFindAutomaticRoute);
            }
        }

        private bool CanExecuteFindAutomaticRoute(string _)
        {
            return TileCollection.Any() && !IsCalculating;
        }

        public DelegateCommand MoveForwardCommand
        {
            get { return _moveForwardCommand ??= new DelegateCommand(ExecuteMoveForward, CanExecuteMoveForward); }
        }

        public DelegateCommand<string> TurnCommand
        {
            get { return _turnCommand ??= new DelegateCommand<string>(ExecuteTurn, CanExecuteTurn); }
        }

        private bool CanExecuteTurn(string arg)
        {
            return HeroViewModel.Hero != null;
        }

        public bool IsCalculating
        {
            get => _isCalculating;
            set
            {
                if (value == _isCalculating) return;
                _isCalculating = value;
                OnPropertyChanged();
            }
        }

        public Route SelectedRoute
        {
            get => _selectedRoute;
            set
            {
                if (Equals(value, _selectedRoute)) return;
                _selectedRoute = value;
                OnPropertyChanged(nameof(SelectedRoute));
            }
        }

        public string MazeEdit
        {
            get => _mazeEdit;
            set
            {
                if (value == _mazeEdit) return;
                _mazeEdit = value;
                OnPropertyChanged(nameof(MazeEdit));
                ValidateAndParseDataCommand.RaiseCanExecuteChanged();
            }
        }

        public int Rows
        {
            get => _rows;
            set
            {
                if (value == _rows) return;
                _rows = value;
                OnPropertyChanged(nameof(Rows));
            }
        }

        public int Columns
        {
            get => _columns;
            set
            {
                if (value == _columns) return;
                _columns = value;
                OnPropertyChanged(nameof(Columns));
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (value == _filePath) return;
                _filePath = value;
                OnPropertyChanged();
            }
        }

        public string CoordExplanation
        {
            get => _coordExplanation;
            set
            {
                if (value == _coordExplanation) return;
                _coordExplanation = value;
                OnPropertyChanged();
            }
        }

        public int CoordY
        {
            get => _coordY;
            set
            {
                if (value == _coordY) return;
                _coordY = value;
                OnPropertyChanged(nameof(CoordY));
                CheckCoordsCommand.RaiseCanExecuteChanged();
            }
        }

        public int CoordX
        {
            get => _coordX;
            set
            {
                if (value == _coordX) return;
                _coordX = value;
                OnPropertyChanged(nameof(CoordX));
                CheckCoordsCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsFileLoaded
        {
            get => _isFileLoaded;
            set
            {
                if (value == _isFileLoaded) return;
                _isFileLoaded = value;
                OnPropertyChanged(nameof(IsFileLoaded));
            }
        }

        public bool IsNewFile
        {
            get => _isNewFile;
            set
            {
                if (value == _isNewFile) return;
                _isNewFile = value;
                OnPropertyChanged(nameof(IsNewFile));
            }
        }

        public ICommand ClosingCommand
        {
            get { return new DelegateCommand(Dispose); }
        }

        private async void ExecuteSaveData()
        {
            await _dataService.SaveAsync(FilePath, MazeEdit);
        }

        private void ExecuteNewFile()
        {
            TileCollection.Clear();
            HeroViewModel.Hero = null;
            FilePath = string.Empty;
            IsFileLoaded = false;
            IsNewFile = true;
        }


        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedRoute))
            {
                foreach (var tile in TileCollection)
                {
                    tile.IsLitUp = false;
                }

                if (SelectedRoute != null)
                {
                    foreach (var tile in SelectedRoute.OrderedBreadCrumbs)
                    {
                        TileCollection.Single(s => s.X == tile.X && s.Y == tile.Y).IsLitUp = true;
                    }
                }
            }
        }

        private void ExecutePlaceExplorer()
        {
            ResetBoard();
            HeroViewModel.Hero = new Hero(TileCollection.Single(s => s.TileType == TileType.Start).Tile);
            HeroViewModel.Initialize();
            _neighbors = _findFinishService.FindNonVisitedNeighbors(TileCollection.Select(s => s.Tile), HeroViewModel.Hero.OccupiedTile);
            TurnCommand.RaiseCanExecuteChanged();
            MoveForwardCommand.RaiseCanExecuteChanged();
        }

        private void ResetBoard()
        {
            foreach (var tile in TileCollection.Select(s => s.Tile))
            {
                tile.HasBeenVisited = false;
            }
        }

        private void ExecuteItemClick(TileViewModel tile)
        {
            if (tile.X == CoordX &&
                tile.Y == CoordY)
            {
                tile.IsLitUp = false;
                ExecuteCheckCoords(true);
                return;
            }

            CoordX = tile.X;
            CoordY = tile.Y;

            ExecuteCheckCoords(false);
        }

        private void ExecuteTurn(string angle)
        {
            HeroViewModel.Turn(Convert.ToInt32(angle));
            HeroViewModel.ResetXY();
            MoveForwardCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteMoveForward()
        {
            return HeroViewModel.Hero != null &&
                   _findFinishService.CanMoveForward(HeroViewModel.Hero, _neighbors);
        }


        private void ExecuteMoveForward()
        {
            _neighbors = _findFinishService.MoveForward(TileCollection.Select(s => s.Tile).ToList(), HeroViewModel.Hero);
            
            var possibleMoves = string.Join(';', _neighbors.Select(s => $"({s.X}, {s.Y})"));
            HeroViewModel.MovementLogCollection.Add($"Possible moves: {possibleMoves}");
            var isFinished = HeroViewModel.CheckHeroState();
            foreach (var tileVm in TileCollection)
            {
                tileVm.ResetOccupancy();
            }
            MoveForwardCommand.RaiseCanExecuteChanged();
            if (isFinished)
            {
                foreach (var tileVm in TileCollection.Where(s => s.Tile.HasBeenVisited))
                {
                    tileVm.IsLitUp = true;
                }
            }
        }

        private void ExecuteCheckCoords(bool? toggleBack)
        {
            foreach (var tileViewModel in TileCollection)
            {
                tileViewModel.IsLitUp = false;
            }
            if (toggleBack.HasValue && 
                toggleBack == true)
            {   
                CoordExplanation = string.Empty;
            }
            else
            {
                var tile = TileCollection.SingleOrDefault(s => s.X == CoordX && s.Y == CoordY);
                if (tile != null)
                {
                    tile.IsLitUp = true;
                    CoordExplanation = $"The selected tile is: {tile.TileType}";
                }
            }
        }

        private async void ExecuteLoadDataAsync()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                return;
            }

            TileCollection.Clear();
            HeroViewModel.Hero = null;

            var tiles = await _dataService.LoadDataFileAsync(FilePath);
           
            CreateTileCollection(tiles);
            IsNewFile = false;
            IsFileLoaded = true;
        }

        private void CreateTileCollection(IEnumerable<Tile> tiles)
        {
            TileCollection.Clear();
            foreach (var tile in tiles)
            {
                TileCollection.Add(new TileViewModel(tile));
            }

            Columns = TileCollection.Max(s => s.X) + 1;
            Rows = TileCollection.Max(s => s.Y) + 1;
            OnPropertyChanged(nameof(NumberOfEmpties));
            OnPropertyChanged(nameof(NumberOfWalls));
        }


        private void ExecuteFindAutomaticRoute(string parameter)
        {
            var findQuickest = false;
            
            if (!string.IsNullOrEmpty(parameter))
            {
                bool.TryParse(parameter, out findQuickest);
            }
            var startTile = TileCollection.Single(s => s.TileType == TileType.Start).Tile;

            ResetBoard();
            HeroViewModel.Hero = new Hero(startTile);

            TotalRoutesFound = 0;
            startTile.IsOccupied = true;
            Routes.Clear();
            IsCalculating = true;
            FindAutomaticRouteCommand.RaiseCanExecuteChanged();
            Task.Run(() =>
            {
                _findFinishService.FindAllRoutes(TileCollection.Select(s => s.Tile).ToList(),
                    new List<Route>() { new Route(new List<Tile>() { startTile }) }, findQuickest);
            });
        }

        private bool CanExecutePlaceExplorer()
        {
            return TileCollection.Any() &&
                   HeroViewModel.Hero == null;
        }

        private void ExecuteValidateAndParseData()
        {
            var tiles = _dataService.LoadDataFromString(MazeEdit);
            CreateTileCollection(tiles);
        }

        private bool CanExecuteValidateAndParseData()
        {
            return !string.IsNullOrEmpty(MazeEdit);
        }

        private void ReleaseUnmanagedResources()
        {
            foreach (var disposable in Disposables)
            {
                disposable.Dispose();
            }
            PropertyChanged -= OnPropertyChanged;
            HeroViewModel.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~MainWindowViewModel()
        {
            ReleaseUnmanagedResources();
        }
    }
}
