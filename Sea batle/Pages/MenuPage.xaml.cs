using System.Windows;
using System.Windows.Controls;

namespace Sea_batle.Pages
{
    /// <summary>
    /// Логика взаимодействия для Menu.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;

        public MenuPage() => InitializeComponent();

        private void StartBtn_Click(object sender, RoutedEventArgs e) => _mainWindow.OutputFrame.Navigate(new PlacementPage());

        private void RulesBtn_Click(object sender, RoutedEventArgs e) => _mainWindow.OutputFrame.Navigate(new RulesPage());

        private void Page_Loaded(object sender, RoutedEventArgs e) => _mainWindow.TopPanel.UpdateTitle(this.Title);
    }
}
