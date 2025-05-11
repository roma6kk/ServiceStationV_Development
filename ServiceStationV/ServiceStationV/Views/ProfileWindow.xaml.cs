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
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            try
            {
                InitializeComponent();
                this.DataContext = new ProfileWindowViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при инициализации окна профиля: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
        }

        private void ThemeToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ThemeManager.ToggleTheme();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при смене темы: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ChangePasswordBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var changePasswordWindow = new ChangePasswordWindow
                {
                    Owner = this
                };
                changePasswordWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при открытии окна смены пароля: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try
            {
                this.Topmost = true;
                this.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при активации окна: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void ChangeLanguageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newCulture = LocalizationManager.CurrentCulture.Name == "en-US"
                    ? new CultureInfo("ru-RU")
                    : new CultureInfo("en-US");
                LocalizationManager.SetLanguage(newCulture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при смене языка: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void MyOrdersBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MyOrdersWindow ordersWindow = new();
                ordersWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при открытии списка заказов: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            try
            {
                base.OnMouseLeftButtonDown(e);
                DragMove();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при перемещении окна: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void CartBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button)
                {
                    CartWindow cartWindow = new CartWindow();
                    cartWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при открытии корзины: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void GetBackBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при закрытии окна: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}