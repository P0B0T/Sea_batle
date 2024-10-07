using Sea_batle.Game.Map;
using Sea_batle.Game.Ship;
using System.Runtime.CompilerServices;
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

        private void FieldCanv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            

            map.DrawMap(FieldCanv, GetCellSize());

            Ships.Children.Clear();

            ShipView.AddShip(Ships, 4, 1, GetCellSize());
            ShipView.AddShip(Ships, 3, 2, GetCellSize());
            ShipView.AddShip(Ships, 2, 3, GetCellSize());
            ShipView.AddShip(Ships, 1, 4, GetCellSize());
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void FieldCanv_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void FieldCanv_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Serializable))
            {
                StackPanel droppedShip = e.Data.GetData(DataFormats.Serializable) as StackPanel;
                Panel oldParent = droppedShip.Parent as Panel;

                oldParent.Children.Remove(droppedShip);

                var position = e.GetPosition(FieldCanv);

                double left = Math.Round(position.X / GetCellSize()) * GetCellSize();
                double top = Math.Round(position.Y / GetCellSize()) * GetCellSize();

                Canvas.SetLeft(droppedShip, left);
                Canvas.SetTop(droppedShip, top);

                FieldCanv.Children.Add(droppedShip);
            }
        }
    }
}