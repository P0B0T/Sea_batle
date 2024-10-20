using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sea_batle.Game.Map
{
    public class Map
    {
        public Cell[,] Cells { get; private set; }

        private const int MapSize = 10;

        public Map()
        {
            Cells = new Cell[MapSize, MapSize];

            for (int row = 0; row < MapSize; row++)
                for (int col = 0; col < MapSize; col++)
                    Cells[row, col] = new Cell();
        }

        public void DrawMap(Canvas field, double cellSize)
        {
            field.Children.Clear();

            for (int row = 0; row < MapSize; row++)
                for (int col = 0; col < MapSize; col++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Stroke = Brushes.White,
                        //Fill = Brushes.Transparent
                        Fill = Cells[row, col].HasShip ? Brushes.Gray : Brushes.Transparent
                    };

                    Canvas.SetLeft(rect, col * cellSize);
                    Canvas.SetTop(rect, row * cellSize);

                    field.Children.Add(rect);
                }
        }

        public int GetMapSize() => MapSize;
    }
}
