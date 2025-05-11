using Microsoft.Data.SqlClient;
using ServiceStationV.Models;
using ServiceStationV.ViewsModels;
using ServiceStationV.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MessageBox = ServiceStationV.Views.MessageBox;
using System.Collections.ObjectModel;

namespace ServiceStationV.Views.Admin
{
    public partial class OrdersWindow : Window
    {
        private OrdersWindowViewModel _OWViewModel = new();

        public OrdersWindow()
        {
            try
            {
                InitializeComponent();
                DataContext = _OWViewModel;
                _OWViewModel.LoadAllOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void ApplySorting()
        {
            try
            {
                string selectedSort = (SortByDateCB.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Сортировка по дате";

                List<Order> sortedOrders = selectedSort switch
                {
                    "По возрастанию" or "By ascending" => _OWViewModel.Orders.OrderBy(o => o.OrderDate).ToList(),
                    "По убыванию" or "By descending" => _OWViewModel.Orders.OrderByDescending(o => o.OrderDate).ToList(),
                    _ => new List<Order>(_OWViewModel.Orders)
                };

                var observableResult = new ObservableCollection<Order>(sortedOrders);
                _OWViewModel.ReplaceOrders(observableResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сортировке: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SortByDateCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ApplySorting();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении сортировки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchText = SearchBox.Text?.Trim();

                _OWViewModel.Search(searchText);

                ApplySorting();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search_Click(sender, e);
        }

        private async void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_OWViewModel.Orders == null || !_OWViewModel.Orders.Any())
                {
                    MessageBox.Show("Нет активных заказов для сохранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foreach (var order in _OWViewModel.Orders)
                {
                    if (!string.IsNullOrWhiteSpace(order.UpdatedStatus) && order.UpdatedStatus != order.Status)
                    {
                        await OrderRepository.UpdateOrderStatusAsync(order.OrderId, order.UpdatedStatus);
                    }
                }

                _OWViewModel.LoadAllOrders();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении статусов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CompletedCB_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is CheckBox checkBox && checkBox.DataContext is Order order)
                {
                    order.Status = "COMPLETED";
                    await OrderRepository.UpdateOrderStatusAsync(order.OrderId, order.Status);
                    _OWViewModel.LoadAllOrders();
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении статуса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}