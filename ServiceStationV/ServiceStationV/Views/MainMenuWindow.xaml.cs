using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using ServiceStationV.Models;
using ServiceStationV.ViewsModels;
using ServiceStationV.Repositories;
using System.Windows.Media;
using ServiceStationV.Views;

namespace ServiceStationV
{
    public partial class MainMenuWindow : Window
    {
        private readonly MainMenuViewModel _viewModel = new();

        public MainMenuWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void ServiceBTN_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { DataContext: Service selectedService })
            {
                ServiceWindow serviceWindow = new ServiceWindow(selectedService);
                serviceWindow.ShowDialog();
            }
        }

        private void CartBTN_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                CartWindow cartWindow = new CartWindow();
                cartWindow.ShowDialog();
            }
        }

        private void FavListBTN_Click(Object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                FavListWindow favListWindow = new FavListWindow();
                favListWindow.ShowDialog();
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedServiceOption = null;
        }

        private void ClearSortButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectedSortOption = null;
        }

        private void ProfileBTN_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow profileWindow = new ProfileWindow();
            profileWindow.ShowDialog();
        }
    }
}
