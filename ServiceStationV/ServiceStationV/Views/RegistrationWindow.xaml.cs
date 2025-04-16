using BCrypt.Net;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
using ServiceStationV.Models;
using ServiceStationV.Views;
namespace ServiceStationV
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        Views.LoginWindow loginWindow;
        private Views.LoginWindow loginWindow1;

        public RegistrationWindow( Views.LoginWindow lw)
        {
            InitializeComponent();
            loginWindow = lw;

        }


        private void GetBackBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loginWindow.Show();

        }

        private async void RegBTN_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordTB.Password != PasswordRepeatTB.Password)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка!", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                PasswordTB.Clear();
                PasswordRepeatTB.Clear();
                return;
            }
            User user = new()
            {
                FullName = FullNameTB.Text,
                PhoneNum = PhoneNumberTB.Text,
                Login = LoginTB.Text,
                Password = PasswordTB.Password
            };

            if (!user.ValidateUser()) return;
            using (SqlConnection con = new(App.conStr))
            {
                try
                {
                    await con.OpenAsync();
                    string query = @"SELECT COUNT(*) FROM Users 
                             WHERE Login = @Login OR PhoneNumber = @PhoneNumber";
                    int count;
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Login", LoginTB.Text);
                        cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumberTB.Text);
                        count = (int)await cmd.ExecuteScalarAsync();
                        if (count > 0)
                        {
                            MessageBox.Show("Пользователь с таким логином или номером телефона уже существует!",
                                            "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    if (UserRepository.AddUser(user, con))
                    {
                        MessageBox.Show("Вы успешно зарегистрировались!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка регистрации: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
