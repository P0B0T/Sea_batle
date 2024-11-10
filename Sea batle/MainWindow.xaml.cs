using Sea_batle.Assistans;
using Sea_batle.Pages;
using Sea_batle.Windows;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Sea_batle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            }
            catch { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => OutputFrame.Navigate(new MenuPage());
    }
}