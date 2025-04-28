using Microsoft.Data.SqlClient;
using ServiceStationV.Models;
using ServiceStationV.Repositories;
using ServiceStationV.ViewsModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views
{
    public partial class CartWindow : Window
    {
        public List<int> CartServicesIds { get; private set; } = new List<int>();

        public CartWindow()
        {
            InitializeComponent();
            LoadCartAsync();
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
                MessageBox.Show($"Ошибка при загрузке данных корзины: {ex.Message}");
            }
        }
        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async Task<List<int>> GetCartAsync()
        {
            List<int> cart = new List<int>();
            using (SqlConnection con = new SqlConnection(App.conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT ServiceId FROM UserCart WHERE Login = @Login";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Login", UserRepository.CurrentUser.Login);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            cart.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return cart;
        }

        private async Task RemoveServiceFromCart(int serviceId, string login)
        {
            using (SqlConnection con = new SqlConnection(App.conStr))
            {
                await con.OpenAsync();
                string query = @"DELETE FROM UserCart WHERE ServiceId = @ServiceId AND Login = @Login";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private async void RemoveBTN_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                var serviceToRemove = btn.DataContext as Service;
                await RemoveServiceFromCart(serviceToRemove.ServiceId, UserRepository.CurrentUser.Login);
                LoadCartAsync();
            }
        }

        private async void OrderBTN_Click(object sender, EventArgs e)
        {
            if(CartServicesIds.Count < 1)
            {
                MessageBox.Show("Корзина пустая", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
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
                        orderId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }

                    string addServicesQuery = @"
                        INSERT INTO OrderServices (OrderId, ServiceName)
                        VALUES (@OrderId, @ServiceName);";

                    using (SqlCommand addServicesCmd = new SqlCommand(addServicesQuery, con))
                    {
                        foreach (var service in await ServiceRepository.GetServicesById(await GetCartAsync()))
                        {
                            addServicesCmd.Parameters.Clear(); 
                            addServicesCmd.Parameters.AddWithValue("@OrderId", orderId);
                            addServicesCmd.Parameters.AddWithValue("@ServiceName", LocalizationManager.IsEnglish ? service.ServiceName : service.ServiceNameEN);
                            await addServicesCmd.ExecuteNonQueryAsync();
                        }
                    }

                    await ClearCartAsync(UserRepository.CurrentUser.Login);
                    LoadCartAsync();
                    MessageBox.Show("Заказ успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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
                using (SqlConnection con = new(App.conStr))
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при очистке корзины: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}