using Microsoft.Data.SqlClient;
using ServiceStationV.Models;
using ServiceStationV.Repositories;
using ServiceStationV.ViewsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views
{
    public partial class CartWindow : Window
    {
        public List<int> CartServicesIds { get; private set; } = new();

        public CartWindow()
        {
            try
            {
                InitializeComponent();
                LoadCartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private async void LoadCartAsync()
        {
            try
            {
                CartServicesIds = await GetCartAsync();
                var vm = new CartWindowViewModels(CartServicesIds);
                await vm.LoadCartItemsAsync();
                DataContext = vm;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке корзины: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async Task<List<int>> GetCartAsync()
        {
            var cart = new List<int>();

            try
            {
                using (SqlConnection con = new SqlConnection(App.conStr))
                {
                    await con.OpenAsync();
                    string query = @"SELECT ServiceId FROM UserCart WHERE Login = @Login";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Login", UserRepository.CurrentUser?.Login ?? throw new InvalidOperationException("Пользователь не авторизован"));
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (!reader.IsDBNull(0))
                                    cart.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {sqlEx.Message}", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении данных из корзины: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return cart;
        }

        private async Task RemoveServiceFromCart(int serviceId, string login)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(App.conStr))
                {
                    await con.OpenAsync();
                    string query = @"DELETE FROM UserCart WHERE ServiceId = @ServiceId AND Login = @Login";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                        cmd.Parameters.AddWithValue("@Login", login);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении услуги из корзины: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RemoveBTN_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is Service serviceToRemove)
                {
                    await RemoveServiceFromCart(serviceToRemove.ServiceId, UserRepository.CurrentUser.Login);
                    LoadCartAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении элемента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OrderBTN_Click(object sender, EventArgs e)
        {
            try
            {
                if (CartServicesIds.Count == 0)
                {
                    MessageBox.Show("Корзина пустая", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(App.conStr))
                {
                    await con.OpenAsync();

                    string createOrderQuery = @"
                        INSERT INTO Orders (Login, Status, OrderDate)
                        VALUES (@Login, 'ACTUAL', @OrderDate);
                        SELECT SCOPE_IDENTITY();";

                    int orderId;
                    using (SqlCommand cmd = new SqlCommand(createOrderQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@Login", UserRepository.CurrentUser.Login);
                        cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        var result = await cmd.ExecuteScalarAsync();
                        orderId = Convert.ToInt32(result);
                    }

                    string addServicesQuery = @"INSERT INTO OrderServices (OrderId, ServiceName) VALUES (@OrderId, @ServiceName);";

                    using (SqlCommand addServicesCmd = new SqlCommand(addServicesQuery, con))
                    {
                        foreach (var service in await ServiceRepository.GetServicesById(await GetCartAsync()))
                        {
                            addServicesCmd.Parameters.Clear();
                            addServicesCmd.Parameters.AddWithValue("@OrderId", orderId);
                            var serviceName = service.ServiceName == null ? service.ServiceNameEN : service.ServiceName;
                            addServicesCmd.Parameters.Add("@ServiceName", SqlDbType.NVarChar, 4000).Value = serviceName ?? (object)DBNull.Value;
                            await addServicesCmd.ExecuteNonQueryAsync();
                        }
                    }

                    await ClearCartAsync(UserRepository.CurrentUser.Login);
                    LoadCartAsync();
                    MessageBox.Show("Заказ успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException fEx)
            {
                MessageBox.Show($"Ошибка формата данных: {fEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ClearCartAsync(string login)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(App.conStr))
                {
                    await con.OpenAsync();
                    string query = @"DELETE FROM UserCart WHERE Login = @Login";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Login", login);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при очистке корзины: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
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