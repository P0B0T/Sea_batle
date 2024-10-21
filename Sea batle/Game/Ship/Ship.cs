using Sea_batle.Assistans;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Sea_batle.Pages.PlacementPage;

namespace Sea_batle.Game.Ship
{
    public class Ship
    {
        private readonly StackPanel _port;
        private readonly Map.Map _map;
        private readonly Canvas _fieldCanvas;

        private double _cellSize;
        private Orientation _orientation;

        public int? X { get; private set; }
        public int? Y { get; private set; }
        public bool IsPlaced { get; set; } = false;
        public int Length { get; private set; }

        public StackPanel ShipVisual { get; private set; }

        private readonly FindShip _findShip;
        private readonly Action _redrawMap;

        public Ship(StackPanel port, double cellSize, int shipLength, Orientation orientation, Map.Map map, FindShip findShip, Canvas fieldCanvas, Action redrawMap, int? x = null, int? y = null)
        {
            _port = port;
            _cellSize = cellSize;
            Length = shipLength;
            _orientation = orientation;
            _map = map;
            _fieldCanvas = fieldCanvas;

            X = x;
            Y = y;

            _findShip = findShip;
            _redrawMap = redrawMap;

            ShipVisual = CreateShip();
        }

        private StackPanel CreateShip()
        {
            var shipVisual = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(30, 0, 0, 0),
                Background = Brushes.Transparent,
                Width = _cellSize * Length,
                Height = _cellSize
            };

            shipVisual.PreviewMouseDown += Ship_PreviewMouseDown;
            shipVisual.AllowDrop = true;

            shipVisual.Children.Add(CreatorImg.CreateImg("pack://application:,,,/img/Ship/Nose.png", _cellSize));

            for (int i = 0; i < Length - 2; i++)
                shipVisual.Children.Add(CreatorImg.CreateImg("pack://application:,,,/img/Ship/Deck.png", _cellSize));

            if (Length > 1)
                shipVisual.Children.Add(CreatorImg.CreateImg("pack://application:,,,/img/Ship/Stern.png", _cellSize));

            return shipVisual;
        }

        public void OutputShip()
        {
            int rowIndex = Length switch
            {
                4 => 0,
                3 => 1,
                2 => 2,
                1 => 3,
                _ => -1
            };

            if (rowIndex < 0) return;

            StackPanel row = _port.Children.Count > rowIndex && _port.Children[rowIndex] is StackPanel existingRow
                ? existingRow
                : new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 30, 0, 0)
                };

            if (_port.Children.Count <= rowIndex || _port.Children[rowIndex] != row)
                if (_port.Children.Count > rowIndex)
                    _port.Children.Insert(rowIndex, row);
                else
                    _port.Children.Add(row);

            if (ShipVisual.Parent is Panel oldParent)
                oldParent.Children.Remove(ShipVisual);

            row.Children.Add(ShipVisual);
        }


        private void Ship_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel shipVisual = sender as StackPanel;

            if (e.RightButton == MouseButtonState.Pressed)
            {
                Ship ship = _findShip(shipVisual);

                if (ship != null)
                {
                    ship.RotateShip();
                    e.Handled = true;
                }
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(shipVisual, new DataObject(DataFormats.Serializable, shipVisual), DragDropEffects.Move);
        }

        private void RotateShip()
        {
            if (IsPlaced)
                ClearShipFromMap();

            _orientation = _orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;

            ShipVisual.Orientation = _orientation;

            ShipVisual.Width = _orientation == Orientation.Horizontal ? _cellSize * Length : _cellSize;
            ShipVisual.Height = _orientation == Orientation.Vertical ? _cellSize * Length : _cellSize;

            foreach (UIElement element in ShipVisual.Children)
                if (element is Image part)
                {
                    part.RenderTransformOrigin = new Point(0.5, 0.5);

                    part.RenderTransform = _orientation == Orientation.Vertical ? new RotateTransform(90) : new RotateTransform(0);
                }

            if (X.HasValue && Y.HasValue)
                if (IsValidDropPosition(X.Value, Y.Value))
                    PositionShipOnField(new Point(X.Value * _cellSize, Y.Value * _cellSize));
                else
                    ClearShipFromMap();

            _redrawMap?.Invoke();
        }

        public void UpdateCoordinates(int x, int y)
        {
            X = x;
            Y = y;

            IsPlaced = true;
        }

        public void UpdateSize(double cellSize)
        {
            _cellSize = cellSize;

            ShipVisual.Width = ShipVisual.Orientation == Orientation.Horizontal ? _cellSize * Length : _cellSize;
            ShipVisual.Height = ShipVisual.Orientation == Orientation.Vertical ? _cellSize * Length : _cellSize;

            foreach (UIElement element in ShipVisual.Children)
                if (element is Image part)
                {
                    part.Width = _cellSize;
                    part.Height = _cellSize;
                }
        }

        public void ClearShipFromMap()
        {
            if (X.HasValue && Y.HasValue)
            {
                int x = X.Value;
                int y = Y.Value;

                int shipLength = Length;

                bool isHorizontal = _orientation == Orientation.Horizontal;

                for (int i = 0; i < shipLength; i++)
                    try
                    {
                        if (isHorizontal)
                            _map.Cells[y, x + i].HasShip = false;
                        else
                            _map.Cells[y + i, x].HasShip = false;
                    }
                    catch { }

                IsPlaced = false;
            }
        }

        public bool IsValidDropPosition(int x, int y)
        {
            bool isHorizontal = _orientation == Orientation.Horizontal;

            int mapSize = _map.GetMapSize();

            return isHorizontal
                ? x >= 0 && y >= 0 && y < mapSize && x + Length <= mapSize
                : x >= 0 && x < mapSize && y >= 0 && y + Length <= mapSize;
        }

        private (double Left, double Top) CalcCoordTopLeft(Point position)
        {
            var (x, y) = ((int)Math.Floor(position.X / _cellSize), (int)Math.Floor(position.Y / _cellSize));

            return (x * _cellSize, y * _cellSize);
        }

        public void PositionShipOnField(Point position)
        {
            var (left, top) = CalcCoordTopLeft(position);

            Canvas.SetLeft(ShipVisual, left);
            Canvas.SetTop(ShipVisual, top);

            X = (int)Math.Round(left / _cellSize);
            Y = (int)Math.Round(top / _cellSize);

            for (int i = 0; i < Length; i++)
                if (_orientation == Orientation.Horizontal)
                    _map.Cells[Y.Value, X.Value + i].HasShip = true;
                else
                    _map.Cells[Y.Value + i, X.Value].HasShip = true;

            IsPlaced = true;
        }

        public void UpdatePositionOnResize()
        {
            if (X.HasValue && Y.HasValue)
            {
                double left = X.Value * _cellSize - 30;
                double top = Y.Value * _cellSize;

                if (ShipVisual.Parent is Panel currentParent)
                    currentParent.Children.Remove(ShipVisual);

                Canvas.SetLeft(ShipVisual, left);
                Canvas.SetTop(ShipVisual, top);

                if (!_fieldCanvas.Children.Contains(ShipVisual))
                    _fieldCanvas.Children.Add(ShipVisual);
            }
        }
    }
}