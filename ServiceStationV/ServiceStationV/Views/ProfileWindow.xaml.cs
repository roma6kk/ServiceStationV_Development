using ServiceStationV.Models;
using ServiceStationV.Repositories;
using ServiceStationV.Views;
using ServiceStationV.ViewsModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace ServiceStationV{
    /// <summary>
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
            this.DataContext = new ProfileWindowViewModel();
        }

        private void ThemeToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ToggleTheme();
        }
        private void ChangePasswordBTN_Click(object sender, RoutedEventArgs e)
        {
            var changePasswordWindow = new ChangePasswordWindow
            {
                Owner = this
            };
            changePasswordWindow.ShowDialog();
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Activate();
        }
        private async void ChangeLanguageBtn_Click(object sender, RoutedEventArgs e)
        {
            var newCulture = LocalizationManager.CurrentCulture.Name == "en-US"
                ? new CultureInfo("ru-RU")
                : new CultureInfo("en-US");
            LocalizationManager.SetLanguage(newCulture);

        }

        private void MyOrdersBTN_Click(Object sender, RoutedEventArgs e)
        {

        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void GetBackBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
