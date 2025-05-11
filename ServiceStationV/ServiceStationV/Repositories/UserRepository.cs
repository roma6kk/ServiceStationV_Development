using Microsoft.Data.SqlClient;
using ServiceStationV.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Repositories
{
    public static class UserRepository
    {
        public static User CurrentUser = new User();
        public static List<User> Users = new List<User>();
        public static bool AddUser(User user, SqlConnection con)
        {
            try
            {
                string addUserQuery = "INSERT INTO Users (Login, FullName, PhoneNumber, Password)" +
                            " VALUES(@Login, @FullName, @PhoneNumber, @Password)";

                using (SqlCommand cmd = new(addUserQuery, con))
                {
                    cmd.Parameters.AddWithValue("@Login", user.Login);
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNum);
                    cmd.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(user.Password));
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении пользователя в БД: " + ex.Message, "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

        }

        private static void ValidateUserList(List<User> userlist)
        {
            if (userlist != null)
            {
                foreach (var user in userlist)
                {
                    var validationResults = new List<ValidationResult>();
                    var validationContext = new ValidationContext(user);

                    bool isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

                    if (isValid)
                    {
                        Users.Add(user);
                    }
                    else
                    {
                        foreach (var validationResult in validationResults)
                        {
                            Console.WriteLine($"Ошибка валидации данных: {validationResult.ErrorMessage}");
                        }
                    }
                }
            }
        }
    }

}
