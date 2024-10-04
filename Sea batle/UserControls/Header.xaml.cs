using System.Windows;
using System.Windows.Controls;

namespace Sea_batle.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        public Header()
        {
            InitializeComponent();
        }

        private void CollapseBtn_Click(object sender, RoutedEventArgs e)
        {
           mainWindow.WindowState = WindowState.Minimized;
        }

        private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.WindowState = WindowState.Maximized;

            MaximizeBtn.Visibility = Visibility.Hidden;
            MinimazeBtn.Visibility = Visibility.Visible;     
        }
        private void MinimazeBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.WindowState = WindowState.Normal;

            MinimazeBtn.Visibility = Visibility.Hidden;
            MaximizeBtn.Visibility = Visibility.Visible;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
        }

        public void UpdateTitle(string title = "")
        {
            TitleTblock.Text = mainWindow.Title + title;
        }
    }
}
