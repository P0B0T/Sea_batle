using Sea_batle.Pages;
using System.Windows;
using System.Windows.Controls;

namespace Sea_batle.Windows
{
    /// <summary>
    /// Логика взаимодействия для RestartWindow.xaml
    /// </summary>
    public partial class RestartWindow : Window
    {
        private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;

        public RestartWindow() => InitializeComponent();

        private void restBtn_Click(object sender, RoutedEventArgs e) => IsNavigate(new PlacementPage());

        private void menuBtn_Click(object sender, RoutedEventArgs e) => IsNavigate(new MenuPage());

        private void IsNavigate(Page page)
        {
            Close();

            _mainWindow.OutputFrame.Navigate(page);
        }
    }
}
