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
        private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
        private readonly Map _map = new Map();
        private readonly List<Ship> _fleet = new List<Ship>();

        private const int CruiserCount = 2;
        private const int DestroyerCount = 3;
        private const int SpeedboatCount = 4;

        public delegate Ship FindShip(StackPanel shipVisual);

        public PlacementPage()
        {
            InitializeComponent();
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
            _map.DrawMap(FieldCanv, GetCellSize()); // убрать потом

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

                double left = ship.X.Value * cellSize - 30;
                double top = ship.Y.Value * cellSize;

                if (ship.ShipVisual.Orientation == Orientation.Horizontal)
                {
                    Canvas.SetLeft(ship.ShipVisual, left);
                    Canvas.SetTop(ship.ShipVisual, top);
                }
                else
                {
                    Canvas.SetLeft(ship.ShipVisual, left);
                    Canvas.SetTop(ship.ShipVisual, top);
                }

                if (!FieldCanv.Children.Contains(ship.ShipVisual))
                    FieldCanv.Children.Add(ship.ShipVisual);
            }
        }

        private Ship FindShipByVisual(StackPanel shipVisual)
        {
            return _fleet.FirstOrDefault(ship => ship.ShipVisual == shipVisual);
        }

        private void CreateFleet()
        {
            _fleet.Add(new Ship(Ships, GetCellSize(), 4, Orientation.Horizontal, _map, new FindShip(FindShipByVisual)));

            AddShipsToFleet(CruiserCount, 3);
            AddShipsToFleet(DestroyerCount, 2);
            AddShipsToFleet(SpeedboatCount, 1);

            AddFleetToUI();
        }

        private void AddShipsToFleet(int count, int shipLength)
        {
            for (int i = 0; i < count; i++)
                _fleet.Add(new Ship(Ships, GetCellSize(), shipLength, Orientation.Horizontal, _map, new FindShip(FindShipByVisual)));
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
                droppedShipObj.ClearShipFromMap();

            Panel oldParent = droppedShip.Parent as Panel;

            oldParent?.Children.Remove(droppedShip);

            var position = e.GetPosition(FieldCanv);

            var (x, y) = CalcCoordXY(position);

            int shipLength = droppedShip.Children.Count;

            if (droppedShipObj.IsValidDropPosition(x, y))
            {
                droppedShipObj.UpdateCoordinates(x, y);

                PositionShipOnField(droppedShip, position, x, y, shipLength);

                droppedShipObj.IsPlaced = true;
            }
            else
                oldParent?.Children.Add(droppedShip);
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
    }
}