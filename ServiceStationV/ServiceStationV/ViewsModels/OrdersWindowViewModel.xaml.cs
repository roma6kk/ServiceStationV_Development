using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ServiceStationV.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.ComponentModel;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.ViewsModels
{
    /// <summary>
    /// Логика взаимодействия для OrdersWindowViewModel.xaml
    /// </summary>
    public partial class OrdersWindowViewModel : INotifyCollectionChanged
    {
        public ObservableCollection<Order> Orders { get; private set; } = new();
        public OrdersWindowViewModel()
        {
            LoadAllOrders();
        }
        public async void LoadAllOrders()
        {
            try
            {
                var orders = await OrderRepository.GetInProgressOrdersAsync();
                Orders.Clear();
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task<Order?> LoadOrderByIdAsync(int orderId)
        {
            using (SqlConnection con = new(App.conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT * FROM Orders WHERE OrderId = @OrderId";
                using (SqlCommand cmd = new(query, con))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Order
                            {
                                OrderId = reader.GetInt32(0),
                                Login = reader.GetString(1),
                                OrderDate = reader.GetDateTime(2),
                                Status = reader.GetString(3),
                                Services = await OrderRepository.LoadServiceNames(reader.GetInt32(0))
                            };
                        }
                    }
                }
            }
            return null;
        }


        public async void Search(TextBox searchBox)
        {
            int orderId;
            if (int.TryParse(searchBox.Text, out orderId))
            {
                ObservableCollection<Order> orders = new();
                Order? order = await LoadOrderByIdAsync(orderId);
                if (order != null)
                {
                    Orders.Clear();
                    Orders.Add(order);
                }
                else
                {
                    searchBox.Clear();
                    MessageBox.Show("Указанный OrderId не найден. Возможно, заказ уже выполнен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Ошибка ввода.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
