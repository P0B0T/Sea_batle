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
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        Map map = new Map();

        List<Ship> fleet = new List<Ship>();

        const int cruiserCount = 2;
        const int destroyerCount = 3;
        const int speedboatCount = 4;

        public PlacementPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow.TopPanel.UpdateTitle(this.Title);
        }

        private double GetCellSize()
        {
            return Math.Min(FieldCanv.ActualWidth / map.GetMapSize(), FieldCanv.ActualHeight / map.GetMapSize());
        }

        private Dictionary<char, int> CalcCoordXY(Point position)
        {
            return new Dictionary<char, int>()
            {
                { 'X', (int)Math.Floor(position.X / GetCellSize())},
                { 'Y', (int)Math.Floor(position.Y / GetCellSize())}
            };
        }

        private Dictionary<char, double> CalcCoordTopLeft(Point position)
        {
            return new Dictionary<char, double>()
            {
                { 'L', CalcCoordXY(position)['X'] * GetCellSize() - 30},
                { 'T', CalcCoordXY(position)['Y'] * GetCellSize()}
            };
        }

        private void FieldCanv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            map.DrawMap(FieldCanv, GetCellSize());

            Ships.Children.Clear();

            CreateFleet();

            AddFleet();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void CreateFleet()
        {
            fleet.Add(new Ship(Ships, GetCellSize(), 4, Orientation.Horizontal));

            for (int i = 0; i < cruiserCount; i++)
                fleet.Add(new Ship(Ships, GetCellSize(), 3, Orientation.Horizontal));

            for (int i = 0; i < destroyerCount; i++)
                fleet.Add(new Ship(Ships, GetCellSize(), 2, Orientation.Horizontal));

            for (int i = 0; i < speedboatCount; i++)
                fleet.Add(new Ship(Ships, GetCellSize(), 1, Orientation.Horizontal));
        }

        private void AddFleet()
        {
            foreach (var ship in fleet)
                ship.OutputShip();
        }

        private void FieldCanv_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Serializable))
            {
                StackPanel droppedShip = e.Data.GetData(DataFormats.Serializable) as StackPanel;

                Ship droppedShipObj = fleet.FirstOrDefault(s => !s.IsPlaced);

                if (droppedShip != null && droppedShipObj != null)
                {
                    Panel oldParent = droppedShip.Parent as Panel;

                    if (oldParent != null)
                        oldParent.Children.Remove(droppedShip);

                    var position = e.GetPosition(FieldCanv);

                    int x = CalcCoordXY(position)['X'];
                    int y = CalcCoordXY(position)['Y'];

                    int shipLength = droppedShip.Children.Count;

                    if (droppedShip.Orientation == Orientation.Horizontal
                        ? x + shipLength <= map.GetMapSize() && y >= 0 && y < map.GetMapSize()
                        : y + shipLength <= map.GetMapSize() && x >= 0 && x < map.GetMapSize())
                    {
                        droppedShipObj.UpdateCoordinates(x, y);

                        Canvas.SetLeft(droppedShip, CalcCoordTopLeft(position)['L']);
                        Canvas.SetTop(droppedShip, CalcCoordTopLeft(position)['T']);

                        FieldCanv.Children.Add(droppedShip);

                        for (int i = 0; i < shipLength; i++)
                            if (droppedShip.Orientation == Orientation.Horizontal)
                                map.Cells[y, x + i].HasShip = true;
                            else
                                map.Cells[y + i, x].HasShip = true;

                        droppedShipObj.IsPlaced = true;

                        //map.DrawMap(FieldCanv, GetCellSize());
                    }
                    else
                        oldParent.Children.Add(droppedShip);
                }
            }
        }
    }
}