using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sea_batle.Game.Map
{
    public class Map
    {
        private Cell[,] Cells { get; set; }
        private const int mapSize = 10;

        public Map()
        {
            Cells = new Cell[mapSize, mapSize];

            for (int row = 0; row < mapSize; row++)
                for (int col = 0; col < mapSize; col++)
                    Cells[row, col] = new Cell();
        }

        public void DrawMap(Canvas field, double cellSize)
        {
            field.Children.Clear();

            for (int row = 0; row < mapSize; row++)
                for (int col = 0; col < mapSize; col++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Stroke = Brushes.White
                    };

                    Canvas.SetLeft(rect, col * cellSize);
                    Canvas.SetTop(rect, row * cellSize);

                    field.Children.Add(rect);
                }
        }

        public int GetMapSize()
        {
            return mapSize;
        }
    }
}
