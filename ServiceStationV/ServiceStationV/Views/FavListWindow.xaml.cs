using Microsoft.Data.SqlClient;
using ServiceStationV.ViewsModels;
using ServiceStationV.Models;
using ServiceStationV.Repositories;
using System;
using System.Collections.Generic;
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
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views
{

    public partial class FavListWindow : Window
    {
        public FavListWindow()
        {
            InitializeComponent();
            LoadFavListAsync();
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
                MessageBox.Show($"Ошибка при загрузке данных избранного: {ex.Message}");
            }
        }
        private async Task RemoveServiceFromFavList(int serviceId, string login)
        {
            using (SqlConnection con = new SqlConnection(App.conStr)) {
                await con.OpenAsync();
                string query = @"DELETE FROM UserFavList WHERE ServiceId = @ServiceId AND Login = @Login";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    cmd.Parameters.AddWithValue("@Login", login);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private async Task<List<int>> GetFavListIdsAsync()
        {
            List<int> cart = new List<int>();
            using (SqlConnection con = new SqlConnection(App.conStr))
            {
                await con.OpenAsync();
                string query = @"SELECT ServiceId FROM UserFavList WHERE Login = @Login";
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
        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void RemoveBTN_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                var serviceToRemove = btn.DataContext as Service;
                await RemoveServiceFromFavList(serviceToRemove.ServiceId, UserRepository.CurrentUser.Login);
                LoadFavListAsync();
            }
        }
        private void Service_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Service selectedService)
            {
                ServiceWindow serviceWindow = new ServiceWindow(selectedService);
                serviceWindow.ShowDialog();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
