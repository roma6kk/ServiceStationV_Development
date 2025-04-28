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

namespace ServiceStationV.ViewsModels
{

    public partial class MyOrdersWindowViewModel
    {
        public ObservableCollection<Order> ActualOrders { get; set; }
        public ObservableCollection<Order> CompletedOrders { get; set; }

        public MyOrdersWindowViewModel()
        {
            ActualOrders = new ObservableCollection<Order>();
            CompletedOrders = new ObservableCollection<Order>();
            LoadOrders();
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

        private async void LoadOrders()
        {
            await LoadOrdersAsync();
        }
    }
}
