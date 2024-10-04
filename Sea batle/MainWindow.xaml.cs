using Sea_batle.Pages;
using Sea_batle.UserControls;
using System.Windows;
using System.Windows.Input;

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
            OutputFrame.Navigate(new MenuPage());
        }
    }
}