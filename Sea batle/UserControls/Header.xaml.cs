using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sea_batle.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();
        }

        private void CollapseBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }

        private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Maximized;

            MaximizeBtn.Visibility = Visibility.Hidden;
            MinimazeBtn.Visibility = Visibility.Visible;     
        }
        private void MinimazeBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Normal;

            MinimazeBtn.Visibility = Visibility.Hidden;
            MaximizeBtn.Visibility = Visibility.Visible;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        private void Header_Loaded(object sender, RoutedEventArgs e)
        {
            TitleTblock.Text = Window.GetWindow(this).Title;
        }
    }
}
