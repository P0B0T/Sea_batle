using Sea_batle.Assistans;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sea_batle.Game.Ship
{
    public class ShipView
    {
        private static void CreateShip(StackPanel port, int length, double cellSize)
        {
            StackPanel ship = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(30, 0, 0, 0),
                Background = Brushes.Transparent,
                Width = cellSize * length,
                Height = cellSize
            };

            ship.PreviewMouseDown += Ship_PreviewMouseDown;
            ship.MouseRightButtonDown += Ship_MouseRightButtonDown;
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

        private static void Ship_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RoteteShip(sender);
        }

        private static void RoteteShip(object sender)
        {
            StackPanel ship = sender as StackPanel;

            (ship.Width, ship.Height) = (ship.Height, ship.Width);

            if (ship.Orientation == Orientation.Horizontal)
                ship.Orientation = Orientation.Vertical;
            else
                ship.Orientation = Orientation.Horizontal;

            foreach (UIElement element in ship.Children)
            {
                Image part = element as Image;

                part.RenderTransformOrigin = new Point(0.5, 0.5);
                part.RenderTransform = ship.Orientation == Orientation.Vertical ? new RotateTransform(90) : new RotateTransform(0);
            }
        }
    }
}