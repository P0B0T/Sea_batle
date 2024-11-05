using Sea_batle.Game.Map;
using Sea_batle.Game.Ship;
using System.Windows;
using System.Windows.Controls;

namespace Sea_batle.Assistans
{
    public class FleetManager
    {
        private const int BattleShipCount = 1;
        private const int CruiserCount = 2;
        private const int DestroyerCount = 3;
        private const int SpeedboatCount = 4;

        private readonly Map _map;
        private readonly double _cellSize;
        private readonly Canvas _fieldCanvas;
        private readonly StackPanel _shipPanel;
        private readonly Action _redrawAction;
        private readonly Visibility _visible;

        public List<Ship> Fleet { get; private set; } = new List<Ship>();

        public FleetManager(Map map, double cellSize, Canvas fieldCanvas, StackPanel shipPanel, Action redrawAction, Visibility visibility = Visibility.Visible)
        {
            _map = map;
            _cellSize = cellSize;
            _fieldCanvas = fieldCanvas;
            _shipPanel = shipPanel;
            _redrawAction = redrawAction;
            _visible = visibility;

            CreateFleet();
        }

        private void CreateFleet()
        {
            AddShipsToFleet(BattleShipCount, 4);
            AddShipsToFleet(CruiserCount, 3);
            AddShipsToFleet(DestroyerCount, 2);
            AddShipsToFleet(SpeedboatCount, 1);

            AddFleetToUI();
        }

        private void AddShipsToFleet(int count, int shipLength)
        {
            for (int i = 0; i < count; i++)
            {
                var ship = new Ship(_shipPanel, _cellSize, shipLength, Orientation.Horizontal, _map, FindShipByVisual, _fieldCanvas, _redrawAction, _visible);

                Fleet.Add(ship);
            }
        }

        private void AddFleetToUI()
        {
            foreach (var ship in Fleet)
                if (!ship.IsPlaced)
                    ship.OutputShip();
        }

        public void ClearField()
        {
            foreach (var ship in Fleet)
                ship.ClearShipFromMap();
        }

        public Ship FindShipByVisual(StackPanel shipVisual) =>
            Fleet.Find(ship => ship.ShipVisual == shipVisual);

        public void RandomPlacement(double cellSize)
        {
            Random random = new Random();

            ClearField();

            foreach (var ship in Fleet)
            {
                bool placed = false;

                while (!placed)
                {
                    ship.RotateShip(random.Next(2) == 0 ? Orientation.Horizontal : Orientation.Vertical);

                    int x = random.Next(_map.GetMapSize());
                    int y = random.Next(_map.GetMapSize());

                    if (ship.IsValidPlacement(x, y))
                    {
                        ship.PositionShipOnField(new Point((x * cellSize) + 1, (y * cellSize) + 1));

                        placed = true;
                    }
                }
            }

            _redrawAction?.Invoke();
        }

        public bool IsFleetEmpty() => Fleet.All(ship => ship.Sunk);

        public void CheckAndShowSunkShips(int row, int col)
        {
            var ship = Fleet.FirstOrDefault(s => s.IsLocatedAt(row, col));

            if (ship.IsSunk())
                ship.ShowSunkShip();
        }
    }
}