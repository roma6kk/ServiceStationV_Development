using Microsoft.Data.SqlClient;
using ServiceStationV.Models;
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
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views.Admin
{
    /// <summary>
    /// Логика взаимодействия для OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        OrdersWindowViewModel _OWViewModel = new();
        
        public OrdersWindow()
        {
            InitializeComponent();
            DataContext = _OWViewModel;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Length > 0)
            {
                _OWViewModel.Search(SearchBox);
            }
            else
            {
                _OWViewModel.LoadAllOrders();
            }

        }

        private async void SaveBTN_Click(object sender, RoutedEventArgs e)
        {
            if (_OWViewModel.Orders == null || !_OWViewModel.Orders.Any())
            {
                MessageBox.Show("Нет активных заказов для сохранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var order in _OWViewModel.Orders)
            {
                if (!string.IsNullOrWhiteSpace(order.UpdatedStatus) && order.UpdatedStatus != order.Status)
                {
                    await OrderRepository.UpdateOrderStatusAsync(order.OrderId, order.UpdatedStatus);
                }
            }

            _OWViewModel.LoadAllOrders();
        }
        private async void CompletedCB_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is Order order)
            {
                order.Status = "COMPLETED";
                await OrderRepository.UpdateOrderStatusAsync(order.OrderId, order.Status);
                _OWViewModel.LoadAllOrders();
            }
        }
        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
