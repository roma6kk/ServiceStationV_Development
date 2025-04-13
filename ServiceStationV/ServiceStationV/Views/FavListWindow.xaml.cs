using Microsoft.Data.SqlClient;
using ServiceStationV.ViewsModels;
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

        private async Task<List<int>> GetFavListIdsAsync()
        {
            List<int> cart = new List<int>();
            using (SqlConnection con = new SqlConnection(App.conStr))
            {
                await con.OpenAsync();
                string query = "SELECT ServiceId FROM UserFavList";
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
    }
}
