﻿using System.Windows;
using System.Windows.Controls;

namespace Sea_batle.Pages
{
    /// <summary>
    /// Логика взаимодействия для RulesPage.xaml
    /// </summary>
    public partial class RulesPage : Page
    {
        private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;

        public RulesPage() => InitializeComponent();

        private void BackBtn_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

        private void Page_Loaded(object sender, RoutedEventArgs e) => _mainWindow.TopPanel.UpdateTitle(this.Title);
    }
}