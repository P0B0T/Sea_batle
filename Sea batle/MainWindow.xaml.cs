using System.Windows;
using System.Windows.Input;
using Sea_batle.Pages;

namespace Sea_batle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MenuPage menu = new MenuPage();

            OutputFrame.Navigate(menu);

            this.Title += menu.Title;
        }
    }
}