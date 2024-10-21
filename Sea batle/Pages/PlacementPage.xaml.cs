using Sea_batle.Game.Map;
using Sea_batle.Game.Ship;
using System.Linq;
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

        public PlacementPage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e) => _mainWindow.TopPanel.UpdateTitle(Title);

        private void BackBtn_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

        private double GetCellSize() => Math.Min(FieldCanv.ActualWidth, FieldCanv.ActualHeight) / _map.GetMapSize();

        private (int X, int Y) CalcCoordXY(Point position) =>
            ((int)(position.X / GetCellSize()), (int)(position.Y / GetCellSize()));

        private void FieldCanv_SizeChanged(object sender, SizeChangedEventArgs e) => RedrawFieldAndShips();

        private void RedrawFieldAndShips()
        {
            _map.DrawMap(FieldCanv, GetCellSize());

            Ships.Children.Clear();

            CreateFleet();

            foreach (var ship in _fleet)
            {
                ship.UpdateSize(GetCellSize());

                if (ship.IsPlaced) ship.UpdatePositionOnResize();
                else ship.OutputShip();
            }
        }

        private void CreateFleet()
        {
            if (_fleet.Count == 0)
            {
                _fleet.Add(new Ship(Ships, GetCellSize(), 4, Orientation.Horizontal, _map, FindShipByVisual, FieldCanv, RedrawFieldAndShips));

                AddShipsToFleet(CruiserCount, 3);
                AddShipsToFleet(DestroyerCount, 2);
                AddShipsToFleet(SpeedboatCount, 1);

                AddFleetToUI();
            }
        }

        private void AddShipsToFleet(int count, int shipLength)
        {
            for (int i = 0; i < count; i++)
                _fleet.Add(new Ship(Ships, GetCellSize(), shipLength, Orientation.Horizontal, _map, FindShipByVisual, FieldCanv, RedrawFieldAndShips));
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

                if (droppedShip != null && droppedShipObj != null)
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

            if (droppedShipObj.IsValidDropPosition(x, y))
            {
                droppedShipObj.UpdateCoordinates(x, y);

                droppedShipObj.PositionShipOnField(position);
            }
            else
                oldParent?.Children.Add(droppedShipObj.ShipVisual);
        }
    }
}