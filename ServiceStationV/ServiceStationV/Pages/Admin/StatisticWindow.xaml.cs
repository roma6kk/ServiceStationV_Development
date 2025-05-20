using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microsoft.Data.SqlClient;
using ServiceStationV.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ServiceStationV.ViewableData;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views.Admin
{
    public partial class StatisticWindow : Window
    {
        private readonly StatisticWindowViewModel _viewModel;

        public StatisticWindow()
        {
            try
            {
                InitializeComponent();
                _viewModel = new StatisticWindowViewModel();
                DataContext = _viewModel;
                Loaded += LoadStatisticsAsync; 
                Loaded += async (s, e) =>
                {   
                    await _viewModel.GetRecentFeedbacksAsync(); 
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при закрытии окна: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static async Task<Dictionary<string, int>> GetServiceTypeCountsAsync()
        {
            var stats = new Dictionary<string, int>();

            try
            {
                using (SqlConnection con = new SqlConnection(App.conStr))
                {
                    string query = @"SELECT s.ServiceType, COUNT(*) AS Count
                                     FROM Orders o
                                     JOIN OrderServices os ON o.OrderId = os.OrderId
                                     JOIN Services s ON os.ServiceName = s.ServiceName OR os.ServiceName = s.ServiceNameEN
                                     GROUP BY s.ServiceType";

                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader.IsDBNull(0)) continue;

                                string serviceType = reader.GetString(0);
                                int count = reader.GetInt32(1);

                                stats[serviceType] = count;
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении статистики по типам услуг: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return stats;
        }

        private async void LoadStatisticsAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                var orders = await OrderRepository.GetAllOrdersAsync();

                if (orders == null)
                {
                    throw new InvalidOperationException("Не удалось загрузить список заказов.");
                }

                TotalOrdersTB.Text = orders.Count.ToString();
                ActiveOrdersTB.Text = orders.Count(o => o.Status != "COMPLETED").ToString();
                CompletedOrdersTB.Text = orders.Count(o => o.Status == "COMPLETED").ToString();

                var stats = await GetServiceTypeCountsAsync();

                PieChartControl.Series = new ObservableCollection<ISeries>(
                    stats.Select(s => new PieSeries<int>
                    {
                        Values = new[] { s.Value },
                        Name = s.Key,
                        DataLabelsSize = 14,
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
                    }));
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException ioEx)
            {
                MessageBox.Show(ioEx.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}