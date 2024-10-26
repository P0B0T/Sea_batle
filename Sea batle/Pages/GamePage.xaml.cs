using Sea_batle.Game.Map;
using Sea_batle.Game.Ship;
using System.Windows;
using System.Windows.Controls;
using static Sea_batle.Assistans.Delegates;

namespace Sea_batle.Pages
{
    /// <summary>
    /// Логика взаимодействия для GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
        private readonly Map _mapPlayer = new Map();
        private readonly Map _mapBot = new Map();
        private readonly List<Ship> _fleetPlayer = new List<Ship>();

        private double _cellSize;

        public GamePage(GetCellSize cellSize, List<Ship> fleet)
        {
            InitializeComponent();

            _cellSize = cellSize(FieldCanvPlayer);
            _fleetPlayer = fleet;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => _mainWindow.TopPanel.UpdateTitle(Title);

        private void FieldCanvBot_SizeChanged(object sender, SizeChangedEventArgs e) => RedrawFieldsAndShips();

        private void RedrawFieldsAndShips()
        {
            _mapPlayer.DrawMap(FieldCanvPlayer, _cellSize);
            _mapBot.DrawMap(FieldCanvBot, _cellSize);

            //foreach (var ship in _fleetPlayer)
            //{
            //    ship.UpdateSize(_cellSize);

            //    ship.UpdatePositionOnResize(FieldCanvPlayer);
            //}
        }
    }
}