using Sea_batle.Assistans;

namespace Sea_batle.Game
{
    public class GameManager
    {
        private Map.Map _playerMap;
        private Map.Map _botMap;
        private FleetManager _playerFleet;
        private FleetManager _botFleet;

        public GameManager(Map.Map playerMap, Map.Map botMap, FleetManager playerFleet, FleetManager botFleet)
        {
            _playerMap = playerMap;
            _botMap = botMap;
            _playerFleet = playerFleet;
            _botFleet = botFleet;
        }

        public void PlayerMove(int row, int col)
        {
            var cell = _botMap.Cells[row, col];

            if (cell.IsHit || cell.IsMiss) return;

            if (cell.HasShip)
            {
                cell.IsHit = true;

                _botFleet.CheckAndShowSunkShips(row, col);
            }
            else
            {
                cell.IsMiss = true;
            }

            if (IsGameOver())
                EndGame();
            else
                BotMove();
        }

        private void BotMove()
        {
            //// Логика для хода бота: выбираем случайную или стратегическую клетку для выстрела
            //int row = /* Вычисление строки для выстрела */;
            //int col = /* Вычисление столбца для выстрела */;

            //var cell = _playerMap.Cells[row, col];

            //if (cell.HasShip)
            //{
            //    cell.IsHit = true;

            //    //if (IsShipSunk(_playerFleet, row, col))
            //    //{
            //    //    // Обработка потопленного корабля игрока
            //    //}
            //}
            //else
            //{
            //    cell.IsMiss = true;
            //}

            // Проверяем, окончена ли игра
            if (IsGameOver())
                EndGame();
        }

        private bool IsGameOver() => _playerFleet.IsFleetEmpty() || _botFleet.IsFleetEmpty();

        private void EndGame()
        {
            // Логика окончания игры: объявляем победителя, показываем сообщение и т.д.
        }
    }
}
