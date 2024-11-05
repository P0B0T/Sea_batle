using Sea_batle.Assistans;
using Sea_batle.Pages;

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

        private readonly Action _redrawAction;

        public GameManager(Map.Map playerMap, Map.Map botMap, FleetManager playerFleet, FleetManager botFleet, Action redrawAction, GamePage? gamePage = null)
        {
            _playerMap = playerMap;
            _botMap = botMap;
            _playerFleet = playerFleet;
            _botFleet = botFleet;
            _gamePage = gamePage ?? _gamePage;
            _redrawAction = redrawAction;
        }

        private void Shoot(Map.Map map, int row, int col, FleetManager fleet)
        {
            var cell = map.Cells[row, col];

            if (cell.IsHit || cell.IsMiss) return;

            if (cell.HasShip)
            {
                cell.IsHit = true;

                fleet.CheckAndShowSunkShips(row, col);
            }
            else
                cell.IsMiss = true;
        }

        public async void PlayerMove(int row, int col)
        {
            if (!_isPlayerTurn) return;

            _isPlayerTurn = false;

            Shoot(_botMap, row, col, _botFleet);

            _gamePage?.RotateArrow(180);
            _redrawAction.Invoke();

            if (IsGameOver())
                EndGame();
            else
            {
                await Task.Delay(1500);

                BotMove();

                _isPlayerTurn = true;
            }
        }

        private void BotMove()
        {
            int row = 0; // получать рандомно или отталкиваясь от попадания
            int col = 0; // получать рандомно или отталкиваясь от попадания

            Shoot(_playerMap, row, col, _playerFleet);

            _gamePage?.RotateArrow(360);
            _redrawAction.Invoke();

            if (IsGameOver())
                EndGame();
        }

        private bool IsGameOver() => _playerFleet.IsFleetEmpty() || _botFleet.IsFleetEmpty();

        private void EndGame()
        {
            var winner = _playerFleet.IsFleetEmpty() ? "Вы проиграли (" : "Вы победили!";

            MessageBoxCustom message = new MessageBoxCustom("Information", winner, "Победа!", new Uri("pack://application:,,,/img/Icons/Information.png"));
            message.ShowMessage();
        }
    }
}
