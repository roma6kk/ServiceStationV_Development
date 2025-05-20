using Microsoft.Data.SqlClient;
using ServiceStationV.ViewableData;
using ServiceStationV.Models;
using ServiceStationV.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views
{
    public partial class FavListWindow : Window
    {
        public FavListWindow()
        {
            try
            {
                InitializeComponent();
                LoadFavListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private async void LoadFavListAsync()
        {
            try
            {
                var favListIds = await GetFavListIdsAsync();
                var vm = new FavListWindowViewModels(favListIds);
                DataContext = vm;
                await vm.LoadFavListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке избранного: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<List<int>> GetFavListIdsAsync()
        {
            var favList = new List<int>();

            try
            {
                using (SqlConnection con = new SqlConnection(App.conStr))
                {
                    await con.OpenAsync();
                    string query = @"SELECT ServiceId FROM UserFavList WHERE Login = @Login";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Login", UserRepository.CurrentUser?.Login ?? throw new InvalidOperationException("Пользователь не авторизован"));
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                if (!reader.IsDBNull(0))
                                    favList.Add(reader.GetInt32(0));
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
                MessageBox.Show($"Ошибка при получении списка избранного: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return favList;
        }

        private async Task RemoveServiceFromFavList(int serviceId, string login)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(App.conStr))
                {
                    await con.OpenAsync();
                    string query = @"DELETE FROM UserFavList WHERE ServiceId = @ServiceId AND Login = @Login";
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
                MessageBox.Show($"Ошибка при удалении услуги из избранного: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async void RemoveBTN_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.DataContext is Service serviceToRemove)
                {
                    await RemoveServiceFromFavList(serviceToRemove.ServiceId, UserRepository.CurrentUser.Login);
                    LoadFavListAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении элемента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Service_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender is Border border && border.DataContext is Service selectedService)
                {
                    ServiceWindow serviceWindow = new ServiceWindow(selectedService);
                    serviceWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна услуги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}