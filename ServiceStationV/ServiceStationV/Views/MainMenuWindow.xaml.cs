using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ServiceStationV.Models;
using ServiceStationV.Views;
using ServiceStationV.ViewsModels;
using MessageBox = System.Windows.MessageBox;

namespace ServiceStationV
{
    public partial class MainMenuWindow : Window
    {
        private readonly MainMenuViewModel _viewModel = new();

        public MainMenuWindow()
        {
            try
            {
                LocalizationManager.LanguageChanged += OnLanguageChanged;
                InitializeComponent();
                ThemeManager.LoadTheme("RS");
                DataContext = _viewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при инициализации окна: {ex.Message}\nПопробуйте перезапустить приложение.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Close(); 
            }
        }

        private void CloseBTN_Click( object sender, RoutedEventArgs e )
        {
            this.Close();
        }
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            try
            {
                _viewModel.UpdateSortOptions();
                _viewModel.ViewServices?.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при смене языка: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        private void ServiceBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button { DataContext: Service selectedService })
                {
                    ServiceWindow serviceWindow = new ServiceWindow(selectedService);
                    serviceWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show(
                        "Не удалось определить выбранную услугу.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при открытии услуги: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void CartBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CartWindow cartWindow = new CartWindow();
                cartWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при открытии корзины: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void FavListBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FavListWindow favListWindow = new FavListWindow();
                favListWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при открытии избранного: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.SelectedServiceOption = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при сбросе фильтра: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        private void ClearSortButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.SelectedSortOption = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при сбросе сортировки: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        private void ProfileBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProfileWindow profileWindow = new ProfileWindow();
                profileWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при открытии профиля: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}