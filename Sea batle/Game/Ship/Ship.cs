using Sea_batle.Assistans;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Sea_batle.Game.Ship
{
    public class Ship
    {
        private readonly StackPanel _port;
        private readonly double _cellSize;
        private readonly Orientation _orientation;

        private int _X;
        private int _Y;

        public int? X { get; private set; }
        public int? Y { get; private set; }
        public bool IsPlaced { get; set; } = false;
        public int Lenght { get; private set; }

        public Ship(StackPanel port, double cellSize, int shipLength, Orientation orientation, int? x = null, int? y = null)
        {
            _port = port;
            _cellSize = cellSize;
            Lenght = shipLength;
            _orientation = orientation;

            X = x;
            Y = y;
        }

        private StackPanel CreateShip()
        {
            StackPanel ship = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(30, 0, 0, 0),
                Background = Brushes.Transparent,
                Width = _cellSize * Lenght,
                Height = _cellSize
            };

            ship.PreviewMouseDown += Ship_PreviewMouseDown;
            ship.MouseRightButtonDown += Ship_MouseRightButtonDown;
            ship.AllowDrop = true;

            ship.Children.Add(CreatorImg.CreateImg("pack://application:,,,/img/Ship/Nose.png", _cellSize));

            for (int i = 0; i < Lenght - 2; i++)
                ship.Children.Add(CreatorImg.CreateImg("pack://application:,,,/img/Ship/Deck.png", _cellSize));

            if (Lenght > 1)
                ship.Children.Add(CreatorImg.CreateImg("pack://application:,,,/img/Ship/Stern.png", _cellSize));

            return ship;
        }

        public void OutputShip()
        {
            int rowIndex = Lenght switch
            {
                4 => 0,
                3 => 1,
                2 => 2,
                1 => 3
            };

            if (rowIndex >= 0)
            {
                if (_port.Children.Count <= rowIndex || !(_port.Children[rowIndex] is StackPanel row))
                {
                    row = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 30, 0, 0)
                    };
                    _port.Children.Insert(rowIndex, row);
                }

                StackPanel ship = CreateShip();
                ((StackPanel)_port.Children[rowIndex]).Children.Add(ship);
            }
        }

        private static void Ship_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragDrop.DoDragDrop(sender as StackPanel, new DataObject(DataFormats.Serializable, sender as StackPanel), DragDropEffects.Move);
        }

        private void Ship_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RotateShip(sender);
        }

        private void RotateShip(object sender)
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

        public void UpdateCoordinates(int x, int y)
        {
            X = x;
            Y = y;
            IsPlaced = true;
        }
    }
}