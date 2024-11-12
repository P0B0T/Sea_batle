using Sea_batle.Assistans;
using Sea_batle.Pages;
using Sea_batle.Windows;

namespace Sea_batle.Game
{
    public class GameManager
    {
        private Map.Map _playerMap;
        private Map.Map _botMap;
        private FleetManager _playerFleet;
        private FleetManager _botFleet;
        private GamePage? _gamePage;

        private bool _isPlayerTurn = true;
        private bool _isMissed = false;
        private bool _shotFired = false;

        private readonly Action _redrawAction;

        private List<(int, int)> hitPositions = new List<(int, int)>();
        private (int dx, int dy)? currentDirection = null;
        private Random _random = new Random();

        public GameManager(Map.Map playerMap, Map.Map botMap, FleetManager playerFleet, FleetManager botFleet, Action redrawAction, GamePage? gamePage = null)
        {
            _playerMap = playerMap;
            _botMap = botMap;
            _playerFleet = playerFleet;
            _botFleet = botFleet;
            _gamePage = gamePage ?? _gamePage;
            _redrawAction = redrawAction;
        }

        private ShootStatus Shoot(Map.Map map, int row, int col, FleetManager fleet)
        {
            var cell = map.Cells[row, col];

            if (cell.IsHit || cell.IsMiss) return ShootStatus.Error;

            if (cell.HasShip)
            {
                cell.IsHit = true;

                fleet.CheckAndShowSunkShips(row, col);

                return ShootStatus.Hit;
            }
            else
            {
                cell.IsMiss = true;

                return ShootStatus.Miss;
            }
        }

        public async void PlayerMove(int row, int col)
        {
            if (!_isPlayerTurn) return;

            ShootStatus hit = Shoot(_botMap, row, col, _botFleet);

            if (hit == ShootStatus.Error) return;

            _redrawAction.Invoke();

            if (IsGameOver())
            {
                EndGame();

                return;
            }

            if (hit == ShootStatus.Miss)
            {
                _isPlayerTurn = false;

                _gamePage?.RotateArrow(180);

                await Task.Delay(1500);

                BotMove();
            }
        }

        private void CheckMetrics(int row, int col)
        {
            var shootStatus = Shoot(_playerMap, row, col, _playerFleet);

            _shotFired = (shootStatus == ShootStatus.Hit || shootStatus == ShootStatus.Miss) ? true : false;

            _isMissed = shootStatus == ShootStatus.Miss ? true : false;
        }

        private async void BotMove()
        {
            _isMissed = false;

            while (!_isMissed)
            {
                _shotFired = false;

                if (hitPositions.Count > 0)
                {
                    var lastHitPosition = hitPositions.Last();

                    if (currentDirection.HasValue)
                    {
                        int nextRow = lastHitPosition.Item1 + currentDirection.Value.dy;
                        int nextCol = lastHitPosition.Item2 + currentDirection.Value.dx;

                        if (IsWithinBounds(nextRow, nextCol) && !_playerMap.Cells[nextRow, nextCol].IsHit && !_playerMap.Cells[nextRow, nextCol].IsMiss)
                        {
                            CheckMetrics(nextRow, nextCol);

                            _redrawAction.Invoke();

                            await Task.Delay(500);

                            if (_shotFired)
                            {
                                if (_playerMap.Cells[nextRow, nextCol].HasShip)
                                    hitPositions.Add((nextRow, nextCol));
                                else
                                    currentDirection = ReverseDirection(currentDirection.Value);
                            }
                        }
                        else
                            currentDirection = ReverseDirection(currentDirection.Value);
                    }

                    if (!_shotFired && hitPositions.Count > 1)
                    {
                        var firstHit = hitPositions.First();

                        int nextRow = firstHit.Item1 + currentDirection.Value.dy;
                        int nextCol = firstHit.Item2 + currentDirection.Value.dx;

                        if (IsWithinBounds(nextRow, nextCol) && !_playerMap.Cells[nextRow, nextCol].IsHit && !_playerMap.Cells[nextRow, nextCol].IsMiss)
                        {
                            CheckMetrics(nextRow, nextCol);

                            _redrawAction.Invoke();

                            await Task.Delay(500);

                            if (_shotFired && _playerMap.Cells[nextRow, nextCol].HasShip)
                                hitPositions.Insert(0, (nextRow, nextCol));
                        }

                        if (_shotFired)
                            HitsClean(lastHitPosition.Item1, lastHitPosition.Item2);
                    }

                    if (!_shotFired)
                    {
                        var neighbors = GetAvailableNeighbors(lastHitPosition.Item1, lastHitPosition.Item2);

                        foreach (var neighbor in neighbors)
                        {
                            int nextRow = neighbor.Item1;
                            int nextCol = neighbor.Item2;

                            CheckMetrics(nextRow, nextCol);

                            if (_shotFired)
                            {
                                if (_playerMap.Cells[nextRow, nextCol].HasShip)
                                {
                                    hitPositions.Add((nextRow, nextCol));

                                    currentDirection = DetermineDirection(lastHitPosition, (nextRow, nextCol));
                                }
                                break;
                            }

                            _redrawAction.Invoke();
                            await Task.Delay(500);
                        }
                    }

                    if (_shotFired)
                        HitsClean(lastHitPosition.Item1, lastHitPosition.Item2);
                }

                if (!_shotFired)
                {
                    _isMissed = ShootRandom();

                    _redrawAction.Invoke();

                    await Task.Delay(500);
                }

                if (IsGameOver())
                {
                    EndGame();

                    return;
                }
            }

            _gamePage?.RotateArrow(360);

            _isPlayerTurn = true;

            _redrawAction.Invoke();
        }

        private bool ShootRandom()
        {
            int row, col;

            do
            {
                row = _random.Next(0, _playerMap.GetMapSize());
                col = _random.Next(0, _playerMap.GetMapSize());
            } 
            while (_playerMap.Cells[row, col].IsHit || _playerMap.Cells[row, col].IsMiss);

            ShootStatus shotFired = Shoot(_playerMap, row, col, _playerFleet);

            if (shotFired == ShootStatus.Hit)
            {
                if (_playerMap.Cells[row, col].HasShip)
                    hitPositions.Add((row, col));

                HitsClean(row, col);
            }

            if (shotFired == ShootStatus.Miss) return true;

            return false;
        }

        private void HitsClean(int row, int col)
        {
            var ship = _playerFleet.Fleet.FirstOrDefault(s => s.IsLocatedAt(row, col));

            if (ship != null && ship.Sunk)
                hitPositions.Clear();
        }

        private (int dx, int dy) ReverseDirection((int dx, int dy) direction) => (-direction.dx, -direction.dy);

        private (int dx, int dy)? DetermineDirection((int, int) firstHit, (int, int) secondHit)
        {
            int dx = secondHit.Item2 - firstHit.Item2;
            int dy = secondHit.Item1 - firstHit.Item1;

            return dx switch
            {
                0 => (0, dy > 0 ? 1 : -1),
                _ when dy == 0 => (dx > 0 ? 1 : -1, 0),
                _ => null
            };
        }

        private bool IsWithinBounds(int row, int col) => row >= 0 && row < _playerMap.GetMapSize() && col >= 0 && col < _playerMap.GetMapSize();

        private readonly (int row, int col)[] directions = { (-1, 0), (1, 0), (0, -1), (0, 1) };

        private List<(int, int)> GetAvailableNeighbors(int row, int col)
        {
            var neighbors = new List<(int, int)>();

            foreach (var (dx, dy) in directions)
            {
                int newRow = row + dx, newCol = col + dy;
                if (IsWithinBounds(newRow, newCol) && !_playerMap.Cells[newRow, newCol].IsHit)
                    neighbors.Add((newRow, newCol));
            }

            return neighbors;
        }

        private bool IsGameOver() => _playerFleet.IsFleetEmpty() || _botFleet.IsFleetEmpty();

        private void EndGame()
        {
            var winner = _playerFleet.IsFleetEmpty() ? "Вы проиграли (" : "Вы победили!";

            MessageBoxCustom message = new MessageBoxCustom("Information", winner, "Победа!", new Uri("pack://application:,,,/img/Icons/Information.png"));
            message.ShowMessage();

            RestartWindow restart = new RestartWindow();
            restart.ShowDialog();
        }
    }
}