using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Navigation;

namespace ServiceStationV
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string UserListFilePath = "./ServiceStationV/UserList.json";
    }

    public static class UserRepository
    {
        public static User CurrentUser = new User();
        public static List<User> Users = new List<User>();
        static UserRepository()
        {
            RefreshUserList();
        }
        public static void AddUser(User NewUser)
        {
            RefreshUserList();
            Users.Add(NewUser);
            string UsersToJson = JsonSerializer.Serialize<List<User>>(Users, new JsonSerializerOptions {WriteIndented = true });
            File.WriteAllText(App.UserListFilePath, UsersToJson);

        }
        private static void RefreshUserList()
        {
            if (!File.Exists(App.UserListFilePath))
            {
                File.WriteAllText(App.UserListFilePath, "[]");
            }

            string json = File.ReadAllText(App.UserListFilePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                File.WriteAllText(App.UserListFilePath, "[]");
                json = "[]";
            }

            try
            {
                List<User> DeserializedUsers = JsonSerializer.Deserialize<List<User>>(json);
                ValidateUserList(DeserializedUsers);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Ошибка десериализации: {ex.Message}");
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
                            // Логирование ошибок валидации
                            Console.WriteLine($"Ошибка валидации данных из UserListJson: {validationResult.ErrorMessage}");
                        }
                    }
                }
            }
        }
    }

    public class User
    {
        [FullName]
        public string FullName { get; set; }
        [PhoneNumber]
        public string PhoneNum { get; set; }
        [Login]
        public string Login { get; set; }
        [Password]
        public string Password { get; set; }
        public User(string fn, string pn, string l, string p)
        { 
            this.FullName = fn;
            this.PhoneNum = pn;
            this.Login = l; 
            this.Password = p;
        }
        public User()
        {
            this.FullName = "";
            this.PhoneNum = "";
            this.Login = "";
            this.Password = "";
        }
        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(FullName) &&
                   string.IsNullOrWhiteSpace(PhoneNum) &&
                   string.IsNullOrWhiteSpace(Login) &&
                   string.IsNullOrWhiteSpace(Password);
        }
        public bool ValidateUser()
        {

            if (!this.IsEmpty())
            {
                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(this);

                bool isValid = Validator.TryValidateObject(this, validationContext, validationResults, true);

                if (isValid)
                {
                    return true;
                }
                else
                {
                    string errors = "";
                    foreach (var validationResult in validationResults)
                    {
                        errors += validationResult.ErrorMessage;
                        errors += "\n";
                    }
                    MessageBox.Show(errors, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните поля!");
                return false;
            }
        }

    }

    // Валидационные атрибуты

    public class FullNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Поле с вашим именем не может быть пустым!");
            }

            string fullName = value.ToString();
            string pattern = @"^[A-Za-z]+ [A-Za-z]+( [A-Za-z]+)?$";

            if (!Regex.IsMatch(fullName, pattern))
            {
                return new ValidationResult("Нам необходимо ваше полное имя! ;)");
            }

            return ValidationResult.Success;
        }
    }
    public class PhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Поле с вашим номером не может быть пустым");
            }
            string pn = value.ToString();
            string pattern = @"^375[0-9]{9}$";
            if(!Regex.IsMatch(pn, pattern))
            {
                return new ValidationResult("Номер телефона введен некорректно! Вот вам пример: 375xxXXXXXXX");
            }
            return ValidationResult.Success;
        }
    }
    public class LoginAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Поле с вашим логином не может быть пустым!");
            }
            string login = value.ToString();
            string pattern = @"^[A-Za-z][A-Za-z0-9]{4,14}$";
            if (!Regex.IsMatch(login, pattern))
            {
                return new ValidationResult("Логин должен начинаться с буквы и содержать от 5 до 15 символов. Пример: VladeletsAudi66");
            }
            return ValidationResult.Success;
        }
    }
    public class PasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Поле с вашим паролем не может быть пустым!");
            }
            string password = value.ToString();
            string pattern = @"^[A-Z][A-Za-z0-9]{5,19}$";
            if (!Regex.IsMatch(password, pattern))
            {
                return new ValidationResult("Пароль должен начинаться с заглавной буквы и содержать от 6 до 20 символов");
            }

            return ValidationResult.Success;
        }
    }

    //-------------------------------

}
