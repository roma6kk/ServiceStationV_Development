using Microsoft.Data.SqlClient;
using ServiceStationV.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ServiceStationV.ViewableData
{

    public partial class MyOrdersWindowViewModel
    {
        public ObservableCollection<Order> ActualOrders { get; set; }
        public ObservableCollection<Order> CompletedOrders { get; set; }
        public bool isOrderHasFeedback { get; set; }

        public MyOrdersWindowViewModel()
        {
            ActualOrders = new ObservableCollection<Order>();
            CompletedOrders = new ObservableCollection<Order>();
        }
        private static async Task<bool> IsOrderHasFeedbackAsync(int orderId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(App.conStr))
                {
                    if (string.IsNullOrEmpty(con.ConnectionString))
                    {
                        MessageBox.Show("Строка подключения не задана.");
                        return true; 
                    }

                    await con.OpenAsync();

                    string query = "SELECT COUNT(*) FROM Feedbacks WHERE OrderId = @OrderId ";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);

                        int count = Convert.ToInt32(await cmd.ExecuteScalarAsync() ?? 0);
                        return count == 0; 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке отзыва: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return true; 
            }
        }
public async Task LoadFeedbacksAsync()
{
    foreach (var order in CompletedOrders.ToList())
    {
        bool hasFeedback = await IsOrderHasFeedbackAsync(order.OrderId);
        order.IsOrderHasFeedback = hasFeedback;
    }
}
        public async Task LoadOrdersAsync()
        {
            var actualOrders = await OrderRepository.GetInProgressOrdersAsync();
            var completedOrders = await OrderRepository.GetCompletedOrdersAsync();

            ActualOrders.Clear();
            foreach (var order in actualOrders)
            {
                ActualOrders.Add(order);
            }

            CompletedOrders.Clear();
            foreach (var order in completedOrders)
            {
                CompletedOrders.Add(order);
            }
        }
        public void RefreshOrders()
        {
            foreach (var order in CompletedOrders.ToList())
            {
                var copy = new Order
                {
                    OrderId = order.OrderId,
                    Login = order.Login,
                    Status = order.Status,
                    OrderDate = order.OrderDate,
                    Services = order.Services,
                    UpdatedStatus = order.UpdatedStatus,
                    IsOrderHasFeedback = order.IsOrderHasFeedback
                };

                int index = CompletedOrders.IndexOf(order);
                if (index >= 0)
                {
                    CompletedOrders.RemoveAt(index);
                    CompletedOrders.Insert(index, copy);
                }
            }
        }
    }
}
