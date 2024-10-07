using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Sea_batle.Assistans
{
    public class CreatorImg
    {
        public static Image CreateImg(string path, double cellSize)
        {
            return new Image()
            {
                Source = new BitmapImage(new Uri(path, UriKind.Absolute)),
                Stretch = System.Windows.Media.Stretch.Fill,
                Width = cellSize,
                Height = cellSize
            };
        }
    }
}
