using Microsoft.Data.SqlClient;
using ServiceStationV.Models;
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
using ServiceStationV.Repositories;
using System.ComponentModel.DataAnnotations;

namespace ServiceStationV.Views
{
    /// <summary>
    /// Логика взаимодействия для ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }
        private void CancelBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private async void ChangePasswordBTN_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                if(!BCrypt.Net.BCrypt.Verify(CurrentPasswordBox.Password, UserRepository.CurrentUser.Password))
                {
                    MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    CurrentPasswordBox.Password = "";
                    ConfirmPasswordBox.Password = "";
                    NewPasswordBox.Password = "";
                    return;
                }
                if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show("Пароли не свопадают", "Ошибка", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    CurrentPasswordBox.Password = "";
                    ConfirmPasswordBox.Password = "";
                    NewPasswordBox.Password = "";
                    return;
                }
                User user = new()
                {
                    FullName = UserRepository.CurrentUser.FullName,
                    PhoneNum = UserRepository.CurrentUser.PhoneNum,
                    Login = UserRepository.CurrentUser.Login,
                    Password = ConfirmPasswordBox.Password
                };
                if (!user.ValidateUser())
                {
                    CurrentPasswordBox.Password = "";
                    ConfirmPasswordBox.Password = "";
                    NewPasswordBox.Password = "";
                    return;
                }
                using (SqlConnection con = new(App.conStr))
                {
                    await   con.OpenAsync();
                    string query = @"UPDATE Users SET Password = @Password WHERE Login = @Login";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(ConfirmPasswordBox.Password));
                        cmd.Parameters.AddWithValue("@Login", UserRepository.CurrentUser.Login);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Пароль успешно изменён!", "Успех", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
