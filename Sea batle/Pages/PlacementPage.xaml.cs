using Sea_batle.Assistans;
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

        private FleetManager _fleet;

        private double _cellSize;

        public PlacementPage() => InitializeComponent();

        private void Page_Loaded(object sender, RoutedEventArgs e) => _mainWindow.TopPanel.UpdateTitle(Title);

        private void Page_Initialized(object sender, EventArgs e) => _fleet = new FleetManager(_map, _cellSize, FieldCanv, Ships, RedrawFieldAndShips);

        private void BackBtn_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

        private (int X, int Y) CalcCoordXY(Point position) =>
            ((int)(position.X / _cellSize), (int)(position.Y / _cellSize));

        private void FieldCanv_SizeChanged(object sender, SizeChangedEventArgs e) => RedrawFieldAndShips();

        private void RedrawFieldAndShips()
        {
            _cellSize = _map.GetCellSize(FieldCanv);

            _map.DrawMap(FieldCanv, _cellSize);

            Ships.Children.Clear();

            foreach (var ship in _fleet.Fleet)
            {
                ship.UpdateSize(_cellSize);

                if (ship.IsPlaced) ship.UpdatePositionOnResize(FieldCanv);
                else ship.OutputShip();
            }
        }

        private void FieldCanv_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Serializable))
            {
                var droppedShip = e.Data.GetData(DataFormats.Serializable) as StackPanel;

                var droppedShipObj = _fleet.FindShipByVisual(droppedShip);

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

        private void RandomBtn_Click(object sender, RoutedEventArgs e)
        {
            _fleet.RandomPlacement(_cellSize);

            RedrawFieldAndShips();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e) 
        {
            foreach (var ship in _fleet.Fleet)
                if (ship.X == null && ship.Y == null)
                {
                    MessageBoxCustom message = new MessageBoxCustom("Information", "Разместите все корабли.", "Ошибка", new Uri("pack://application:,,,/img/Icons/Error.png"));
                    message.ShowMessage();

                    return;
                }

            _mainWindow.OutputFrame.Navigate(new GamePage(_fleet));
        }
    }
}