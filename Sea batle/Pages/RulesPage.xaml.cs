using System.Windows;
using System.Windows.Controls;

namespace Sea_batle.Pages
{
    /// <summary>
    /// Логика взаимодействия для RulesPage.xaml
    /// </summary>
    public partial class RulesPage : Page
    {
        public RulesPage()
        {
            InitializeComponent();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
