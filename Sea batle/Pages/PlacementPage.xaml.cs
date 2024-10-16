using Sea_batle.Game.Map;
using Sea_batle.Game.Ship;
using System.Windows;
using System.Windows.Controls;

namespace Sea_batle.Pages
{
    /// <summary>
    /// Логика взаимодействия для PlacementPage.xaml
    /// </summary>
    public partial class PlacementPage : Page
    {
        private readonly MainWindow _mainWindow;
        private readonly Map _map;
        private readonly List<Ship> _fleet;

        private const int CruiserCount = 2;
        private const int DestroyerCount = 3;
        private const int SpeedboatCount = 4;

        public PlacementPage()
        {
            InitializeComponent();
            _mainWindow = (MainWindow)Application.Current.MainWindow;
            _map = new Map();
            _fleet = new List<Ship>();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow.TopPanel.UpdateTitle(this.Title);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private double GetCellSize() =>
            Math.Min(FieldCanv.ActualWidth / _map.GetMapSize(), FieldCanv.ActualHeight / _map.GetMapSize());

        private (int X, int Y) CalcCoordXY(Point position) =>
            ((int)Math.Floor(position.X / GetCellSize()), (int)Math.Floor(position.Y / GetCellSize()));

        private (double Left, double Top) CalcCoordTopLeft(Point position)
        {
            var (x, y) = CalcCoordXY(position);

            return (x * GetCellSize() - 30, y * GetCellSize());
        }

        private void FieldCanv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedrawFieldAndShips();
        }

        private void RedrawFieldAndShips()
        {
            _map.DrawMap(FieldCanv, GetCellSize());

            Ships.Children.Clear();

            if (_fleet.Count == 0)
                CreateFleet();

            foreach (var ship in _fleet)
            {
                ship.UpdateSize(GetCellSize());

                if (ship.IsPlaced)
                    UpdateShipPositionOnResize(ship);
                else
                    ship.OutputShip();
            }
        }

        private void UpdateShipPositionOnResize(Ship ship)
        {
            if (ship.X.HasValue && ship.Y.HasValue)
            {
                double cellSize = GetCellSize();

                double left = ship.X.Value * cellSize;
                double top = ship.Y.Value * cellSize;

                if (ship.ShipVisual.Orientation == Orientation.Horizontal)
                {
                    Canvas.SetLeft(ship.ShipVisual, left - 30);
                    Canvas.SetTop(ship.ShipVisual, top);
                }
                else
                {
                    Canvas.SetLeft(ship.ShipVisual, left - 30);
                    Canvas.SetTop(ship.ShipVisual, top);
                }

                if (!FieldCanv.Children.Contains(ship.ShipVisual))
                    FieldCanv.Children.Add(ship.ShipVisual);
            }
        }

        private void CreateFleet()
        {
            _fleet.Add(new Ship(Ships, GetCellSize(), 4, Orientation.Horizontal));

            AddShipsToFleet(CruiserCount, 3);
            AddShipsToFleet(DestroyerCount, 2);
            AddShipsToFleet(SpeedboatCount, 1);

            AddFleetToUI();
        }

        private void AddShipsToFleet(int count, int shipLength)
        {
            for (int i = 0; i < count; i++)
                _fleet.Add(new Ship(Ships, GetCellSize(), shipLength, Orientation.Horizontal));
        }

        private void AddFleetToUI()
        {
            foreach (var ship in _fleet)
                if (!ship.IsPlaced)
                    ship.OutputShip();
        }

        private void FieldCanv_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Serializable))
            {
                StackPanel droppedShip = e.Data.GetData(DataFormats.Serializable) as StackPanel;

                Ship droppedShipObj = _fleet.FirstOrDefault(s => s.ShipVisual == droppedShip);

                if (droppedShip != null && droppedShipObj != null)
                    HandleShipDrop(droppedShip, droppedShipObj, e);

                RedrawFieldAndShips();
            }
        }

        private void HandleShipDrop(StackPanel droppedShip, Ship droppedShipObj, DragEventArgs e)
        {
            if (droppedShipObj.IsPlaced)
                ClearShipFromMap(droppedShipObj);

            Panel oldParent = droppedShip.Parent as Panel;

            oldParent?.Children.Remove(droppedShip);

            var position = e.GetPosition(FieldCanv);

            var (x, y) = CalcCoordXY(position);

            int shipLength = droppedShip.Children.Count;

            if (IsValidDropPosition(droppedShip, x, y, shipLength))
            {
                droppedShipObj.UpdateCoordinates(x, y);

                PositionShipOnField(droppedShip, position, x, y, shipLength);

                droppedShipObj.IsPlaced = true;
            }
            else
                oldParent?.Children.Add(droppedShip);
        }

        private bool IsValidDropPosition(StackPanel droppedShip, int x, int y, int shipLength)
        {
            bool isHorizontal = droppedShip.Orientation == Orientation.Horizontal;

            int mapSize = _map.GetMapSize();

            return isHorizontal
                ? x + shipLength <= mapSize && y >= 0 && y < mapSize
                : y + shipLength <= mapSize && x >= 0 && x < mapSize;
        }

        private void PositionShipOnField(StackPanel droppedShip, Point position, int x, int y, int shipLength)
        {
            var (left, top) = CalcCoordTopLeft(position);

            Canvas.SetLeft(droppedShip, left);
            Canvas.SetTop(droppedShip, top);

            FieldCanv.Children.Add(droppedShip);

            for (int i = 0; i < shipLength; i++)
                if (droppedShip.Orientation == Orientation.Horizontal)
                    _map.Cells[y, x + i].HasShip = true;
                else
                    _map.Cells[y + i, x].HasShip = true;
        }

        private void ClearShipFromMap(Ship ship)
        {
            int x = ship.X.Value;
            int y = ship.Y.Value;

            int shipLength = ship.Length;

            bool isHorizontal = ship.ShipVisual.Orientation == Orientation.Horizontal;

            for (int i = 0; i < shipLength; i++)
                if (isHorizontal)
                    _map.Cells[y, x + i].HasShip = false;
                else
                    _map.Cells[y + i, x].HasShip = false;

            ship.IsPlaced = false;
        }
    }
}