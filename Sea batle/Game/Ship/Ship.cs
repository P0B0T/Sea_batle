﻿using Sea_batle.Assistans;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static Sea_batle.Assistans.Delegates;

namespace Sea_batle.Game.Ship
{
    public class Ship
    {
        private readonly StackPanel _port;
        private readonly Map.Map _map;
        private readonly Canvas _fieldCanvas;

        private double _cellSize;
        private Orientation _orientation;
        private StackPanel _sunkPanel;

        public int? X { get; set; }
        public int? Y { get; set; }
        public bool IsPlaced { get; set; } = false;
        public int Length { get; private set; }

        public StackPanel ShipVisual { get; private set; }

        public bool Sunk { get; private set; } = false;

        private readonly FindShip _findShip;
        private readonly Action _redrawMap;

        public Ship(StackPanel port, double cellSize, int shipLength, Orientation orientation, Map.Map map, FindShip findShip, Canvas fieldCanvas, Action redrawMap, Visibility visibility, int? x = null, int? y = null)
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

            ShipVisual = CreateShip(visibility);
        }

        private StackPanel CreateShip(Visibility visibility)
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

            shipVisual.Visibility = visibility == Visibility.Hidden ? Visibility.Hidden : Visibility.Visible;

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
                    var newOrientation = ship.ShipVisual.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;

                    ship.RotateShip(newOrientation);

                    e.Handled = true;
                }
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(shipVisual, new DataObject(DataFormats.Serializable, shipVisual), DragDropEffects.Move);
        }

        public void RotateShip(Orientation newOrientation)
        {
            if (IsPlaced)
                ClearShipFromMap();

            _orientation = newOrientation;

            ShipVisual.Orientation = _orientation;

            ShipVisual.Width = _orientation == Orientation.Horizontal ? _cellSize * Length : _cellSize;
            ShipVisual.Height = _orientation == Orientation.Vertical ? _cellSize * Length : _cellSize;

            foreach (UIElement element in ShipVisual.Children)
            {
                if (element is Image part)
                {
                    part.RenderTransformOrigin = new Point(0.5, 0.5);

                    part.RenderTransform = _orientation == Orientation.Vertical ? new RotateTransform(90) : new RotateTransform(0);
                }
            }

            if (X.HasValue && Y.HasValue)
            {
                if (IsValidPlacement(X.Value, Y.Value))
                    PositionShipOnField(new Point((X.Value * _cellSize) + 1, (Y.Value * _cellSize) + 1));
                else
                    ClearShipFromMap();
            }

            _redrawMap?.Invoke();
        }

        public void UpdateCoordinates(int x, int y)
        {
            X = x;
            Y = y;
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

                X = null;
                Y = null;

                IsPlaced = false;
            }
        }

        private (double Left, double Top) CalcCoordTopLeft(Point position)
        {
            var (x, y) = ((int)Math.Floor(position.X / _cellSize), (int)Math.Floor(position.Y / _cellSize));

            return (x * _cellSize, y * _cellSize);
        }

        public bool IsValidPlacement(int x, int y)
        {
            bool isHorizontal = _orientation == Orientation.Horizontal;

            int mapSize = _map.GetMapSize();

            if (isHorizontal)
            {
                if (x < 0 || y < 0 || x + Length > mapSize || y >= mapSize)
                    return false;
            }
            else
                if (x < 0 || y < 0 || x >= mapSize || y + Length > mapSize)
                    return false;
 
            for (int i = 0; i < Length; i++)
            {
                int targetX = isHorizontal ? x + i : x;
                int targetY = isHorizontal ? y : y + i;

                if (targetX < 0 || targetX >= mapSize || targetY < 0 || targetY >= mapSize)
                    return false;

                if (_map.Cells[targetY, targetX].HasShip || !IsSurroundingCellsFree(targetX, targetY))
                    return false;
            }

            return true;
        }

        private bool IsSurroundingCellsFree(int x, int y)
        {
            int mapSize = _map.GetMapSize();

            for (int offsetY = -1; offsetY <= 1; offsetY++)
                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    int checkX = x + offsetX;
                    int checkY = y + offsetY;

                    if (checkX >= 0 && checkX < mapSize && checkY >= 0 && checkY < mapSize)
                        if (_map.Cells[checkY, checkX].HasShip)
                            return false;
                }

            return true;
        }


        public void PositionShipOnField(Point position)
        {
            var (left, top) = CalcCoordTopLeft(position);

            int x = (int)Math.Round(left / _cellSize);
            int y = (int)Math.Round(top / _cellSize);

            Canvas.SetLeft(ShipVisual, left);
            Canvas.SetTop(ShipVisual, top);

            X = x;
            Y = y;

            for (int i = 0; i < Length; i++)
            {
                if (_orientation == Orientation.Horizontal)
                    _map.Cells[Y.Value, X.Value + i].HasShip = true;
                else
                    _map.Cells[Y.Value + i, X.Value].HasShip = true;
            }

            IsPlaced = true;
        }

        public void UpdatePositionOnResize(Canvas field)
        {
            if (X.HasValue && Y.HasValue)
            {
                double left = X.Value * _cellSize - 30;
                double top = Y.Value * _cellSize;

                UpdatePanel(ShipVisual, field, left, top);
            }
        }

        private void UpdatePanel(StackPanel visualPanel, Canvas field, double left, double top)
        {
            if (visualPanel != null)
            {
                if (visualPanel.Parent is Panel currentParent)
                    currentParent.Children.Remove(visualPanel);

                Canvas.SetLeft(visualPanel, left);
                Canvas.SetTop(visualPanel, top);

                if (!field.Children.Contains(visualPanel))
                    field.Children.Add(visualPanel);
            }
        }

        public bool IsLocatedAt(int row, int col)
        {
            for (int i = 0; i < Length; i++)
            {
                int checkX = _orientation == Orientation.Horizontal ? X.Value + i : X.Value;
                int checkY = _orientation == Orientation.Horizontal ? Y.Value : Y.Value + i;

                if (checkX == col && checkY == row)
                    return true;
            }

            return false;
        }

        public bool IsSunk()
        {
            for (int i = 0; i < Length; i++)
            {
                int checkX = _orientation == Orientation.Horizontal ? X.Value + i : X.Value;
                int checkY = _orientation == Orientation.Horizontal ? Y.Value : Y.Value + i;

                if (!_map.Cells[checkY, checkX].IsHit)
                    return false;
            }

            Sunk = true;

            return true;
        }

        public void ShowSunkShip()
        {
            ShipVisual.Visibility = Visibility.Visible;

            Panel.SetZIndex(ShipVisual, 1);

            _sunkPanel = new StackPanel
            {
                Orientation = _orientation,
                Margin = new Thickness(30, 0, 0, 0),
                Width = ShipVisual.Width,
                Height = ShipVisual.Height,
                Background = Brushes.Transparent
            };

            for (int i = 0; i < Length; i++)
            {
                Rectangle overlay = new Rectangle
                {
                    Width = _cellSize,
                    Height = _cellSize,
                    Fill = Brushes.Red,
                    Opacity = 0.2
                };

                _sunkPanel.Children.Add(overlay);
            }

            if (ShipVisual.Parent is Canvas parentCanvas)
            {
                parentCanvas.Children.Add(_sunkPanel);

                Canvas.SetLeft(_sunkPanel, Canvas.GetLeft(ShipVisual));
                Canvas.SetTop(_sunkPanel, Canvas.GetTop(ShipVisual));
            }

            for (int i = 0; i < Length; i++)
            {
                int shipX = _orientation == Orientation.Horizontal ? X.Value + i : X.Value;
                int shipY = _orientation == Orientation.Horizontal ? Y.Value : Y.Value + i;

                MarkSurroundingCellsAsMiss(shipX, shipY);
            }
        }

        public void UpdateSunkPanelPosition(Canvas field) => UpdatePanel(_sunkPanel, field, Canvas.GetLeft(ShipVisual), Canvas.GetTop(ShipVisual));

        private void MarkSurroundingCellsAsMiss(int x, int y)
        {
            int mapSize = _map.GetMapSize();

            for (int offsetY = -1; offsetY <= 1; offsetY++)
                for (int offsetX = -1; offsetX <= 1; offsetX++)
                {
                    int checkX = x + offsetX;
                    int checkY = y + offsetY;

                    if (checkX >= 0 && checkX < mapSize && checkY >= 0 && checkY < mapSize)
                    {
                        var cell = _map.Cells[checkY, checkX];

                        if (!cell.HasShip)
                            cell.IsMiss = true;
                    }
                }
        }
    }
}