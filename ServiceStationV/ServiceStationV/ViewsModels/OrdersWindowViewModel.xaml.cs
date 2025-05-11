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
    public partial class OrdersWindowViewModel : INotifyCollectionChanged
    {
        private ObservableCollection<Order> _allOrders = new();
        public ObservableCollection<Order> AllOrders
        {
            get => _allOrders;
            private set
            {
                _allOrders = value;
                OnPropertyChanged(nameof(AllOrders));
            }
        }

        private ObservableCollection<Order> _orders = new();
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            private set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
        public OrdersWindowViewModel()
        {
            LoadAllOrders();
        }

        public async void LoadAllOrders()
        {
            try
            {
                var orders = await OrderRepository.GetInProgressOrdersAsync();
                AllOrders.Clear();
                foreach (var order in orders)
                {
                    AllOrders.Add(order);
                }

                ReplaceOrders(new ObservableCollection<Order>(AllOrders));
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заказов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ReplaceOrders(AllOrders);
                return;
            }

            int orderId;
            if (int.TryParse(searchText, out orderId))
            {
                var result = AllOrders.FirstOrDefault(o => o.OrderId == orderId);
                if (result != null)
                {
                    ReplaceOrders(new ObservableCollection<Order> { result });
                }
                else
                {
                    ReplaceOrders(new ObservableCollection<Order>()); 
                }
            }
            else
            {
                MessageBox.Show("Введите корректный ID заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ReplaceOrders(ObservableCollection<Order> newOrders)
        {
            Orders.Clear();
            foreach (var order in newOrders)
            {
                Orders.Add(order);
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


        



        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
