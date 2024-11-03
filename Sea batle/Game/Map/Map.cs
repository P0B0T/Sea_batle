using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sea_batle.Game.Map
{
    public class Map
    {
        public Cell[,] Cells { get; private set; }

        private const int mapSize = 10;

        private readonly Action? _redrawAction;
        private readonly SolidColorBrush _hoverColor = new SolidColorBrush(Color.FromRgb(135, 194, 250));

        public Map(Action? redrawAction = null)
        {
            _redrawAction = redrawAction ?? _redrawAction;

            Cells = new Cell[mapSize, mapSize];

            for (int row = 0; row < mapSize; row++)
                for (int col = 0; col < mapSize; col++)
                    Cells[row, col] = new Cell();
        }

        public void DrawMap(Canvas field, double cellSize, bool clickable = false, GameManager? game = null)
        {
            field.Children.Clear();

            for (int row = 0; row < mapSize; row++)
                for (int col = 0; col < mapSize; col++)
                {
                    int currentRow = row;
                    int currentCol = col;

                    Rectangle rect = new Rectangle
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Stroke = Brushes.White,
                        Fill = Cells[row, col].IsHit ? new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/img/Icons/Hit.png"))) :
                               Cells[row, col].IsMiss ? new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/img/Icons/Miss.png"))) :
                               Brushes.Transparent
                    };

                    Canvas.SetLeft(rect, currentCol * cellSize);
                    Canvas.SetTop(rect, currentRow * cellSize);

                    if (clickable)
                    {
                        rect.MouseEnter += Cell_MouseEnter;
                        rect.MouseLeave += Cell_MouseLeave;

                        if (game != null)
                            rect.MouseLeftButtonDown += (s, e) => Cell_Click(game, currentRow, currentCol);
                    }

                    field.Children.Add(rect);
                }

        }

        private void Cell_Click(GameManager game, int row, int col)
        {
            game.PlayerMove(row, col);

            _redrawAction?.Invoke();
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Rectangle hoveredCell && hoveredCell.Fill == Brushes.Transparent)
                hoveredCell.Fill = _hoverColor;
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Rectangle hoveredCell && hoveredCell.Fill == _hoverColor)
                hoveredCell.Fill = Brushes.Transparent;
        }


        public int GetMapSize() => mapSize;

        public double GetCellSize(Canvas field) => Math.Min(field.ActualWidth, field.ActualHeight) / mapSize;
    }
}