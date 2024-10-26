using Sea_batle.Assistans;
using Sea_batle.Game.Map;
using Sea_batle.Game.Ship;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sea_batle.Pages
{
    /// <summary>
    /// Логика взаимодействия для PlacementPage.xaml
    /// </summary>
    public partial class PlacementPage : Page
    {
        private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
        private readonly Map _map = new Map();
        private readonly List<Ship> _fleet = new List<Ship>();

        private const int BattleShipCount = 1;
        private const int CruiserCount = 2;
        private const int DestroyerCount = 3;
        private const int SpeedboatCount = 4;

        private readonly Random _random = new Random();

        private double _cellSize;

        public PlacementPage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e) => _mainWindow.TopPanel.UpdateTitle(Title);

        private void BackBtn_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

        private double GetCellSizeMthd(Canvas field) => Math.Min(field.ActualWidth, field.ActualHeight) / _map.GetMapSize();

        private (int X, int Y) CalcCoordXY(Point position) =>
            ((int)(position.X / _cellSize), (int)(position.Y / _cellSize));

        private void FieldCanv_SizeChanged(object sender, SizeChangedEventArgs e) => RedrawFieldAndShips();

        private void RedrawFieldAndShips()
        {
            _cellSize = GetCellSizeMthd(FieldCanv);

            _map.DrawMap(FieldCanv, _cellSize);

            Ships.Children.Clear();

            CreateFleet();

            foreach (var ship in _fleet)
            {
                ship.UpdateSize(_cellSize);

                if (ship.IsPlaced) ship.UpdatePositionOnResize(FieldCanv);
                else ship.OutputShip();
            }
        }

        private void CreateFleet()
        {
            if (_fleet.Count == 0)
            {
                AddShipsToFleet(BattleShipCount, 4);
                AddShipsToFleet(CruiserCount, 3);
                AddShipsToFleet(DestroyerCount, 2);
                AddShipsToFleet(SpeedboatCount, 1);

                AddFleetToUI();
            }
        }

        private void AddShipsToFleet(int count, int shipLength)
        {
            for (int i = 0; i < count; i++)
                _fleet.Add(new Ship(Ships, _cellSize, shipLength, Orientation.Horizontal, _map, FindShipByVisual, FieldCanv, RedrawFieldAndShips));
        }

        private void AddFleetToUI()
        {
            foreach (var ship in _fleet.Where(s => !s.IsPlaced))
                ship.OutputShip();
        }

        private Ship FindShipByVisual(StackPanel shipVisual) =>
            _fleet.FirstOrDefault(ship => ship.ShipVisual == shipVisual);

        private void FieldCanv_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Serializable))
            {
                var droppedShip = e.Data.GetData(DataFormats.Serializable) as StackPanel;

                var droppedShipObj = FindShipByVisual(droppedShip);

                HandleShipDrop(droppedShipObj, e);

                RedrawFieldAndShips();
            }
        }

        private void HandleShipDrop(Ship droppedShipObj, DragEventArgs e)
        {
            if (droppedShipObj.IsPlaced) droppedShipObj.ClearShipFromMap();

            Panel oldParent = droppedShipObj.ShipVisual.Parent as Panel;

            oldParent?.Children.Remove(droppedShipObj.ShipVisual);

            var position = e.GetPosition(FieldCanv);

            var (x, y) = CalcCoordXY(position);

            if (droppedShipObj.IsValidPlacement(x, y))
            {
                droppedShipObj.UpdateCoordinates(x, y);

                droppedShipObj.PositionShipOnField(position);
            }
            else
                oldParent?.Children.Add(droppedShipObj.ShipVisual);
        }

        private void ClearField()
        {
            foreach (var ship in _fleet)
            {
                ship.ClearShipFromMap();

                ship.X = null;
                ship.Y = null;
            }
        }

        private void RandomBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearField();

            foreach (var ship in _fleet)
            {
                bool placed = false;

                while (!placed)
                {
                    ship.RotateShip(_random.Next(2) == 0 ? Orientation.Horizontal : Orientation.Vertical);

                    int x = _random.Next(_map.GetMapSize());
                    int y = _random.Next(_map.GetMapSize());

                    if (ship.IsValidPlacement(x, y))
                    {
                        ship.PositionShipOnField(new Point((x * _cellSize) + 1, (y * _cellSize) + 1));

                        placed = true;
                    }
                }
            }

            RedrawFieldAndShips();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e) 
        {
            foreach (var ship in _fleet)
                if (ship.X == null && ship.Y == null)
                {
                    MessageBoxCustom message = new MessageBoxCustom("Information", "Разместите все корабли.", "Ошибка", new Uri("pack://application:,,,/img/Icons/Error.png"));
                    message.ShowMessage();

                    return;
                }

            _mainWindow.OutputFrame.Navigate(new GamePage(GetCellSizeMthd, _fleet));
        }
    }
}