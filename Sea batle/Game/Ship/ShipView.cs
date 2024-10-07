using Sea_batle.Assistans;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sea_batle.Game.Ship
{
    public class ShipView
    {
        private static void CreateShip(StackPanel port, int length, double cellSize)
        {
            StackPanel ship = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(30, 0, 0, 0)
            };

            ship.PreviewMouseDown += Ship_PreviewMouseDown;
            ship.AllowDrop = true;

            ship.Children.Add(CreatorImg.CreateImg("pack://application:,,,/img/Ship/Nose.png", cellSize));

            for (int i = 0; i < length - 2; i++)
                ship.Children.Add(CreatorImg.CreateImg("pack://application:,,,/img/Ship/Deck.png", cellSize));

            if (length > 1)
                ship.Children.Add(CreatorImg.CreateImg("pack://application:,,,/img/Ship/Stern.png", cellSize));
            port.Children.Add(ship);
        }

        public static void AddShip(StackPanel port, int shipLenght, int shipCount, double cellSize)
        {
            StackPanel row = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 30, 0, 0)
            };

            for (int i = 0; i < shipCount; i++)
                CreateShip(row, shipLenght, cellSize);

            port.Children.Add(row);
        }

        private static void Ship_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragDrop.DoDragDrop(sender as StackPanel, new DataObject(DataFormats.Serializable, sender as StackPanel), DragDropEffects.Move);
        }
    }
}
