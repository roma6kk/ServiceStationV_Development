using Microsoft.Data.SqlClient;
using ServiceStationV.Models;
using ServiceStationV.Repositories;
using ServiceStationV.ViewableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public partial class MyOrdersWindow : Window
    {
        MyOrdersWindowViewModel viewModel = new();
        public MyOrdersWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += MyOrdersWindow_Loaded;
        }

        private async void MyOrdersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadOrdersAsync();
            await viewModel.LoadFeedbacksAsync();
            viewModel.RefreshOrders();
        }

        private async void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button == null) return;

            if (button.DataContext is not Order order)
            {
                MessageBox.Show("Не удалось получить данные заказа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int orderId = order.OrderId;

            string feedback = button.Tag?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(feedback))
            {
                MessageBox.Show("Введите отзыв перед сохранением", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(App.conStr))
                {
                    await con.OpenAsync();

                    string query = "INSERT INTO Feedbacks(OrderId, Login, Feedback) VALUES(@OrderId, @Login, @Feedback)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.Parameters.AddWithValue("@Login", UserRepository.CurrentUser.Login);
                        cmd.Parameters.AddWithValue("@Feedback", feedback);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                order.IsOrderHasFeedback = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке отзыва: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
