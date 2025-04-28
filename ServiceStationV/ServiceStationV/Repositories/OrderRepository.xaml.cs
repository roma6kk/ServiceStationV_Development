using System.Collections.Generic;
using System.Configuration;
using ServiceStationV.Models;
using ServiceStationV;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using MessageBox = ServiceStationV.Views.MessageBox;

public static class OrderRepository
{

    public static async Task<ObservableCollection<Order>> GetInProgressOrdersAsync()
    {
        var orders = new ObservableCollection<Order>();

        using (SqlConnection con = new SqlConnection(App.conStr))
        {
            var command = new SqlCommand(@"
                SELECT o.OrderId, o.Status, o.OrderDate, os.ServiceName
                FROM Orders o
                LEFT JOIN OrderServices os ON o.OrderId = os.OrderId
                WHERE o.Status != 'COMPLETED'
                ORDER BY o.OrderDate DESC", con);

            await con.OpenAsync();
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int orderId = reader.GetInt32(0);
                    var existingOrder = orders.FirstOrDefault(o => o.OrderId == orderId);

                    if (existingOrder == null)
                    {
                        existingOrder = new Order
                        {
                            OrderId = orderId,
                            Status = reader.GetString(1),
                            OrderDate = reader.GetDateTime(2)
                        };
                        orders.Add(existingOrder);
                    }

                    if (!reader.IsDBNull(3))
                    {
                        existingOrder.Services.Add(reader.GetString(3));
                    }
                }
            }
        }

        return orders;
    }

    public static async Task<ObservableCollection<Order>> GetCompletedOrdersAsync()
    {
        var orders = new ObservableCollection<Order>();

        using (SqlConnection con = new SqlConnection(App.conStr))
        {
            SqlCommand command = new SqlCommand(@"
                SELECT o.OrderId, o.Status, o.OrderDate, os.ServiceName
                FROM Orders o
                LEFT JOIN OrderServices os ON o.OrderId = os.OrderId
                WHERE o.Status = 'COMPLETED'
                ORDER BY o.OrderDate DESC", con);

            await con.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int orderId = reader.GetInt32(0);
                    var existingOrder = orders.FirstOrDefault(o => o.OrderId == orderId);

                    if (existingOrder == null)
                    {
                        existingOrder = new Order
                        {
                            OrderId = orderId,
                            Status = reader.GetString(1),
                            OrderDate = reader.GetDateTime(2)
                        };
                        orders.Add(existingOrder);
                    }

                    if (!reader.IsDBNull(3))
                    {
                        existingOrder.Services.Add(reader.GetString(3));
                    }
                }
            }
        }

        return orders;
    }

    public static async Task<ObservableCollection<Order>> GetAllOrdersAsync()
    {
        ObservableCollection<Order> orders = new();
        using (SqlConnection con = new(App.conStr))
        {
            await con.OpenAsync();
            string query = "SELECT * FROM Orders WHERE Status != 'COMPLETED'";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    Order order = new Order
                    {
                        OrderId = reader.GetInt32(0),
                        Login = reader.GetString(1),
                        OrderDate = reader.GetDateTime(2),
                        Status = reader.GetString(3),
                        Services = await LoadServiceNames(reader.GetInt32(0))
                    };
                    orders.Add(order);
                }
                return orders;
            }
        }
    }
    public static async Task<List<string>> LoadServiceNames(int orderId)
    {
        using (SqlConnection con = new(App.conStr))
        {
            List<string> serviceNames = new();
            await con.OpenAsync();
            string query = @"SELECT ServiceName From OrderServices WHERE OrderId = @orderId";
            using (SqlCommand cmd = new(query, con))
            {
                cmd.Parameters.AddWithValue("@orderId", orderId);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    serviceNames.Add(reader.GetString(0));
                }
            }
            return serviceNames;
        }
    }

    public static async Task UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        try
        {
            using (SqlConnection con = new(App.conStr))
            {
                await con.OpenAsync();
                string query = @"UPDATE Orders SET Status = @NewStatus WHERE OrderId = @OrderId";

                using (SqlCommand cmd = new(query, con))
                {
                    cmd.Parameters.AddWithValue("@NewStatus", newStatus);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    await cmd.ExecuteNonQueryAsync();

                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при обновлении статуса заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}