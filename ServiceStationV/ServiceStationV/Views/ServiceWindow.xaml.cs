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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using ServiceStationV.Models;
using ServiceStationV.Repositories;
using ServiceStationV.ViewsModels;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views
{
    public partial class ServiceWindow : Window
    {
        public static Service LoadedService { get; private set; } = null;

        public ServiceWindow(Service service)
        {
            InitializeComponent();
            LoadedService = service;
            Loaded += async (s, e) =>
            {
                this.Opacity = 0;
                this.BeginAnimation(OpacityProperty,
                    new DoubleAnimation(1, TimeSpan.FromSeconds(0.3)));
                await SetCartButtonBackground();
                await SetFavListButtonBackground();
                string path;
                switch (service.ServiceType)
                {
                    case ServiceTypes.Двигатель:
                        ServiceVideo.Height = 200;
                        ServiceVideo.Width = 200;

                        path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "EngineChange.mp4");
                        ServiceVideo.Source = new Uri(path, UriKind.Absolute);
                        ServiceVideo.Play();
                        break;
                    case ServiceTypes.Диагностика:
                        ServiceVideo.Height = 400;
                        ServiceVideo.Width = 200;

                        path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "Diagnostic.mp4");
                        ServiceVideo.Source = new Uri(path, UriKind.Absolute);
                        ServiceVideo.Play();
                        break;
                    case ServiceTypes.Обслуживание:
                        ServiceVideo.Height = 400;
                        ServiceVideo.Width = 200;

                        path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "Maintenance.mp4");
                        ServiceVideo.Source = new Uri(path, UriKind.Absolute);
                        ServiceVideo.Play();
                        break;
                    case ServiceTypes.Тюнинг:
                        ServiceVideo.Height = 400;
                        ServiceVideo.Width = 200;

                        path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", "Tuning.mp4");
                        ServiceVideo.Source = new Uri(path, UriKind.Absolute);
                        ServiceVideo.Play();
                        break;
                }
            };

            DataContext = new ServiceWindowViewModel();
        }

        private async Task SetCartButtonBackground()
        {
            var viewModel = this.DataContext as ServiceWindowViewModel;
            if (await IsServiceInCart(UserRepository.CurrentUser.Login, viewModel.ServiceId))
            {
                CartBTN.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/Close.png")));
            }
            else
            {
                CartBTN.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/ProfileImages/ShoppingCartBlack.jpg")));
            }
        }
        private async Task SetFavListButtonBackground()
        {
            var viewModel = this.DataContext as ServiceWindowViewModel;
            if (await IsServiceInFavList(UserRepository.CurrentUser.Login, viewModel.ServiceId))
            {
                FavListBTN.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/icons/StarAdded.png")));
            }
            else
            {
                FavListBTN.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/icons/StarAdd.png")));
            }
        }
        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Activate();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
        private async void FavListBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var FavListBTN = sender as Button;
                ImageBrush AddedBrush = new ImageBrush();
                AddedBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/icons/StarAdded.png", UriKind.Absolute));
                ImageBrush DefaultBrush = new ImageBrush();
                DefaultBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/icons/StarAdd.png", UriKind.Absolute));
                var viewModel = this.DataContext as ServiceWindowViewModel;
                if (!await IsServiceInFavList(UserRepository.CurrentUser.Login, viewModel.ServiceId))
                {
                    using (SqlConnection con = new(App.conStr))
                    {
                        await con.OpenAsync();
                        string query = "INSERT INTO UserFavList (Login, ServiceId) VALUES (@Login, @ServiceId)";
                        using (SqlCommand com = new(query, con))
                        {
                            com.Parameters.AddWithValue("@Login", UserRepository.CurrentUser.Login);
                            com.Parameters.AddWithValue("@ServiceId", viewModel.ServiceId);
                            await com.ExecuteNonQueryAsync();
                        }
                    }
                    FavListBTN.Background = AddedBrush;
                }
                else
                {
                    using (SqlConnection con = new(App.conStr))
                    {
                        await con.OpenAsync();
                        string query = "DELETE FROM UserFavList WHERE ServiceId = @ServiceId";
                        using (SqlCommand com = new(query, con))
                        {
                            com.Parameters.AddWithValue("@ServiceId", viewModel.ServiceId);
                            await com.ExecuteNonQueryAsync();
                        }
                        FavListBTN.Background = DefaultBrush;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void CartBTN_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                var CartBTN = sender as Button;
                ImageBrush CloseBrush = new ImageBrush();
                CloseBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/Close.png", UriKind.Absolute));
                ImageBrush DefaultBrush = new ImageBrush();
                DefaultBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/ProfileImages/ShoppingCartBlack.jpg", UriKind.Absolute));
                var viewModel = this.DataContext as ServiceWindowViewModel;
                if (!await IsServiceInCart(UserRepository.CurrentUser.Login, viewModel.ServiceId))
                {
                    using (SqlConnection con = new(App.conStr))
                    {
                        await con.OpenAsync();
                        string query = "INSERT INTO UserCart (Login, ServiceId) VALUES (@Login, @ServiceId)";
                        using (SqlCommand com = new(query, con))
                        {
                            com.Parameters.AddWithValue("@Login", UserRepository.CurrentUser.Login);
                            com.Parameters.AddWithValue("@ServiceId", viewModel.ServiceId);
                            await com.ExecuteNonQueryAsync();
                        }
                    }
                    CartBTN.Background = CloseBrush;
                }
                else
                {
                    using (SqlConnection con = new(App.conStr))
                    {
                        await con.OpenAsync();
                        string query = "DELETE FROM UserCart WHERE ServiceId = @ServiceId";
                        using (SqlCommand com = new(query, con))
                        {
                            com.Parameters.AddWithValue("@ServiceId", viewModel.ServiceId); 
                            await com.ExecuteNonQueryAsync();
                        }
                        CartBTN.Background = DefaultBrush;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task<bool> IsServiceInFavList(string login, int ServiceId)
        {
            using (SqlConnection con = new(App.conStr))
            {
                await con.OpenAsync();
                string checkQuery = "SELECT COUNT(*) FROM UserFavList WHERE Login = @Login AND ServiceId = @ServiceId";
                using (SqlCommand checkCmd = new(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@Login", login);
                    checkCmd.Parameters.AddWithValue("@ServiceId", ServiceId);
                    int count = (int)await checkCmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }
        private async Task<bool> IsServiceInCart(string login, int ServiceId)
        {
            using (SqlConnection con = new(App.conStr))
            {
                await con.OpenAsync();
                string checkQuery = "SELECT COUNT(*) FROM UserCart WHERE Login = @Login AND ServiceId = @ServiceId";
                using (SqlCommand checkCmd = new(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@Login", login);
                    checkCmd.Parameters.AddWithValue("@ServiceId", ServiceId);
                    int count = (int)await checkCmd.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        } 
    }
}
