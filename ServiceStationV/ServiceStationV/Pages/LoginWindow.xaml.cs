﻿    using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServiceStationV.Models;
using ServiceStationV.Repositories;
using ServiceStationV.Views.Admin;
using System.Globalization;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LocalizationManager.LanguageChanged += OnLanguageChanged;

        }

        private void CloseBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            ChangeLanguageBtn.Content = LocalizationManager.GetString("LoginWindowChangeLanguage");
        }
        private void GoToRegBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            RegistrationWindow RegWindow = new(this);
            RegWindow.Show();
        }

        private void ChangeLanguageBtn_Click(object sender, RoutedEventArgs e)
        {
            var newCulture = LocalizationManager.CurrentCulture.Name == "en-US"
                ? new CultureInfo("ru-RU")
                : new CultureInfo("en-US");

            LocalizationManager.SetLanguage(newCulture);
        }
        private async void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTB.Text) || string.IsNullOrWhiteSpace(PasswordTB.Password))
            {
                MessageBox.Show("Логин и пароль не могут быть пустыми!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            User LoggedUser = new();
            try
            {
                using (SqlConnection con = new(App.conStr))
                {
                    await con.OpenAsync();
                    string query = "SELECT Login, FullName, PhoneNumber, Password FROM Users Where Login = @Login";
                    using (SqlCommand cmd = new(query,con))
                    {
                        cmd.Parameters.Add("@Login", SqlDbType.VarChar, 15).Value = LoginTB.Text;
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) 
                            {
                                LoggedUser = new User
                                {
                                    Login = reader["Login"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    PhoneNum = reader["PhoneNumber"].ToString(),
                                    Password = reader["Password"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка LoginBTN_Click: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                if (BCrypt.Net.BCrypt.Verify(PasswordTB.Password, LoggedUser.Password))
                {
                    UserRepository.CurrentUser = LoggedUser;
                    if (LoggedUser.Login == "admin")
                    {
                        AdminWindow adminWindow = new AdminWindow();
                        adminWindow.Show();
                        this.Close();
                        return;
                    }
                    MainMenuWindow MMWindow = new MainMenuWindow();
                    Application.Current.MainWindow = MMWindow;
                    MMWindow.Show();
                    this.Close();
                    return;
                }
                MessageBox.Show("Неверный логин и/или пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                PasswordTB.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка входа:" + ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                PasswordTB.Clear();
            }
        }
    }
}