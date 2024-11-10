using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sea_batle.Assistans
{
    // импортированный класс
    public class MessageBoxCustom
    {
        private readonly string _view;
        private readonly string _messageText;

        private readonly Window _window;

        private readonly TextBlock _text;

        private readonly Button _okBtn;
        private readonly Button _canselBtn;
        private readonly Button _yesBtn;
        private readonly Button _noBtn;

        public MessageBoxResult Result { get; set; }

        public MessageBoxCustom(string view, string messageText, string title = null, Uri iconUri = null)
        {
            _view = view;
            _messageText = messageText;

            Style baseTextBlockStyle = (Style)Application.Current.FindResource("BaseTextBlock");
            Style baseButtonStyle = (Style)Application.Current.FindResource("BaseButton");

            Style buttonStyle = new Style(typeof(Button))
            {
                BasedOn = baseButtonStyle
            };

            buttonStyle.Setters.Add(new Setter(Button.VerticalAlignmentProperty, VerticalAlignment.Bottom));
            buttonStyle.Setters.Add(new Setter(Button.VisibilityProperty, Visibility.Collapsed));
            buttonStyle.Setters.Add(new Setter(Button.WidthProperty, 100.0));
            buttonStyle.Setters.Add(new Setter(Button.HeightProperty, 40.0));
            buttonStyle.Setters.Add(new Setter(Button.FontSizeProperty, 30.0));
            buttonStyle.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.DarkBlue));
            buttonStyle.Setters.Add(new Setter(Button.BorderBrushProperty, Brushes.DarkBlue));

            _window = new Window
            {
                Title = title ?? "",
                Width = 350,
                Height = 175,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Icon = iconUri != null ? BitmapFrame.Create(iconUri) : null,
                Background = new SolidColorBrush(Color.FromRgb(51, 97, 112))
            };

            _window.Loaded += Window_Loaded;

            Grid grid = new Grid();

            Grid gridText = new Grid
            {
                Height = 95,
                Margin = new Thickness(10, 10, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            _text = new TextBlock
            {
                Width = double.NaN,
                Height = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlignment = TextAlignment.Center,
                Style = baseTextBlockStyle
            };

            gridText.Children.Add(_text);

            _okBtn = new Button
            {
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Content = "ОК",
                Style = buttonStyle,
                IsDefault = true
            };

            _okBtn.Click += OK_Click;

            _canselBtn = new Button
            {
                Margin = new Thickness(0, 0, 10, 10),
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "Отмена",
                Style = buttonStyle
            };

            _canselBtn.Click += Cansel_Click;

            _yesBtn = new Button
            {
                Margin = new Thickness(10, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Left,
                Content = "Да",
                Style = buttonStyle
            };

            _yesBtn.Click += Yes_Click;

            _noBtn = new Button
            {
                Margin = new Thickness(0, 0, 10, 10),
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "Нет",
                Style = buttonStyle
            };

            _noBtn.Click += No_Click;

            grid.Children.Add(gridText);
            grid.Children.Add(_okBtn);
            grid.Children.Add(_canselBtn);
            grid.Children.Add(_yesBtn);
            grid.Children.Add(_noBtn);

            _window.Content = grid;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (_view)
            {
                case "Information":
                    _okBtn.Visibility = Visibility.Visible;
                    break;

                case "OK/CANSEL":
                    _okBtn.Visibility = Visibility.Visible;
                    _canselBtn.Visibility = Visibility.Visible;
                    break;

                case "YES/NO":
                    _yesBtn.Visibility = Visibility.Visible;
                    _noBtn.Visibility = Visibility.Visible;
                    break;
            }
        }

        public MessageBoxResult ShowMessage()
        {
            _text.Text = _messageText;

            _window.ShowDialog();

            return Result;
        }

        private void Cansel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;

            _window.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;

            _window.Close();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;

            _window.Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;

            _window.Close();
        }
    }
}