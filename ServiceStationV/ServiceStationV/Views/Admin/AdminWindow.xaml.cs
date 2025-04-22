using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using ServiceStationV.Models;
using ServiceStationV.ViewsModels;
using ServiceStationV.Repositories;
using System.Windows.Media;
using ServiceStationV.Views;
using Microsoft.Data.SqlClient;

namespace ServiceStationV.Views.Admin
{
    public partial class AdminWindow : Window
    {
        private readonly AdminViewModel _viewModel = new();

        public AdminWindow()
        {
            // ЭТО ТОЖЕ 
            LocalizationManager.LanguageChanged += OnLanguageChanged;
            InitializeComponent();
            ThemeManager.LoadTheme("RS");
            DataContext = _viewModel;
        }
        // Я ХЗ ПОЧЕМУ НО ЕСЛИ ЭТО УБРАТЬ ТИПЫ СЕРВИСОВ В КОМБОБОКСЕ НЕ ПЕРЕВОДЯТСЯ НАДО БДУЕТ ФИКСИТЬ 
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            _viewModel.UpdateSortOptions();
            _viewModel.ViewServices?.Refresh();
        }
        private void ServiceBTN_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: Service selectedService })
            {
                ServiceWindow serviceWindow = new ServiceWindow(selectedService);
                serviceWindow.ShowDialog();
            }
        }

        private void EditService_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: Service selectedService })
            {
                // Открываем окно редактирования с передачей выбранной услуги
                EditServiceWindow editServiceWindow = new EditServiceWindow(selectedService, _viewModel);
                editServiceWindow.ShowDialog();

                // После закрытия окна обновляем данные в ViewModel
                //_viewModel.LoadServices();
            }
        }
        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Service selectedService)
            {
                // Открываем окно с подробной информацией об услуге
                var serviceWindow = new ServiceWindow(selectedService);
                serviceWindow.ShowDialog();
            }
        }
        private async void DeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: Service selectedService })
            {
                // Подтверждение удаления
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить услугу '{selectedService.ServiceName}'?",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                try
                {
                    if (result == MessageBoxResult.Yes)
                    {
                        using (SqlConnection con = new(App.conStr))
                        {
                            await con.OpenAsync();
                            string query = @"DELETE FROM UserCart WHERE ServiceId = @ServiceId;
                                             DELETE FROM UserFavList WHERE ServiceId = @ServiceId
                                             DELETE FROM OrderServices WHERE ServiceId = @ServiceId;
                                             DELETE FROM Services WHERE ServiceId = @ServiceId;
                                             ";
                            using (SqlCommand cmd = new(query, con))
                            {
                                cmd.Parameters.AddWithValue("@ServiceId", selectedService.ServiceId);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    await _viewModel.RefreshServicesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении услуги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private async void AddServiceBTN_Click(object sender, RoutedEventArgs e)
        {
            AddServiceWindow addServiceWindow = new(_viewModel);
            addServiceWindow.ShowDialog();
            
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedServiceOption = null;
        }

        private void ClearSortButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedSortOption = null;
        }


    }
}
