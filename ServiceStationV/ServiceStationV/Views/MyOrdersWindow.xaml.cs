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
        }
        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
