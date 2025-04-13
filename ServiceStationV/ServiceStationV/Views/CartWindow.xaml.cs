using Microsoft.Data.SqlClient;
using ServiceStationV.ViewsModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

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
                await vm.LoadCartItemsAsync(); // ← сюда загрузку
                DataContext = vm;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных корзины: {ex.Message}");
            }
        }


        private async Task<List<int>> GetCartAsync()
        {
            List<int> cart = new List<int>();
            using (SqlConnection con = new SqlConnection(App.conStr))
            {
                await con.OpenAsync();
                string query = "SELECT ServiceId FROM UserCart";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
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
    
    private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}