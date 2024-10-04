using System.Windows;
using System.Windows.Controls;

namespace Sea_batle.Pages
{
    /// <summary>
    /// Логика взаимодействия для Menu.xaml
    /// </summary>
    public partial class MenuPage : Page
    {
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public MenuPage()
        {
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void RulesBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.OutputFrame.Navigate(new RulesPage());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow.TopPanel.UpdateTitle(this.Title);
        }
    }
}
