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
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views.Admin
{
    public partial class AdminWindow : Window
    {
        private readonly AdminViewModel _viewModel = new();

        public AdminWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            ThemeManager.LoadTheme("RS");
            LocalizationManager.LanguageChanged += OnLanguageChanged;
        }

        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void StatisticBTN_Click(object sender, RoutedEventArgs e)
        {
            StatisticWindow sw = new StatisticWindow();
            sw.ShowDialog();
        }


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
                EditServiceWindow editServiceWindow = new EditServiceWindow(selectedService, _viewModel);
                editServiceWindow.ShowDialog();
            }
        }
        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Service selectedService)
            {
                var serviceWindow = new ServiceWindow(selectedService);
                serviceWindow.ShowDialog();
            }
        }
        private async void DeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: Service selectedService })
            {
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
        private void AddServiceBTN_Click(object sender, RoutedEventArgs e)
        {
            AddServiceWindow addServiceWindow = new(_viewModel);
            addServiceWindow.ShowDialog();
            
        }

        private void OrdersBTN_Click(object sender, RoutedEventArgs e)
        {
            OrdersWindow orderServiceWindow = new();
            orderServiceWindow.ShowDialog();
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
