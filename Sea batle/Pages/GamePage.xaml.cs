using Sea_batle.Assistans;
using Sea_batle.Game.Map;
using System.Windows;
using System.Windows.Controls;

namespace Sea_batle.Pages
{
    public partial class GamePage : Page
    {
        private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
        private readonly Map _mapPlayer = new Map();
        private readonly Map _mapBot = new Map(); 
        private readonly FleetManager _fleetPlayer;

        private FleetManager _fleetBot;
        private double _cellSize;

        public GamePage(FleetManager fleet)
        {
            _fleetPlayer = fleet;

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow.TopPanel.UpdateTitle(Title);

            _fleetBot.RandomPlacement(_cellSize);
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            _fleetBot = new FleetManager(_mapBot, _cellSize, FieldCanvBot, ShipsBot, RedrawFieldsAndShips);
        }

        private void FieldCanvBot_SizeChanged(object sender, SizeChangedEventArgs e) => RedrawFieldsAndShips();

        private void RedrawFieldsAndShips()
        {
            _cellSize = _mapPlayer.GetCellSize(FieldCanvPlayer);

            _mapPlayer.DrawMap(FieldCanvPlayer, _cellSize);
            _mapBot.DrawMap(FieldCanvBot, _cellSize);

            foreach (var ship in _fleetPlayer.Fleet)
            {
                ship.UpdateSize(_cellSize);
                ship.UpdatePositionOnResize(FieldCanvPlayer);
            }

            foreach (var ship in _fleetBot.Fleet)
            {
                ship.UpdateSize(_cellSize);

                if (ship.IsPlaced) ship.UpdatePositionOnResize(FieldCanvBot);
                else ship.OutputShip();

                ship.ShipVisual.Visibility = Visibility.Hidden;
            }
        }
    }
}