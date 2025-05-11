using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServiceStationV.ValidationAttributes
{
    class UserValidationAttributes
    {
        

        public class FullNameAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                {
                    return new ValidationResult("Поле с вашим именем не может быть пустым!");
                }

                string fullName = value.ToString();
                string pattern = @"^[A-Za-zА-Яа-я]+ [A-Za-zА-Яа-я]+( [A-Za-zА-Яа-я]+)?$";

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
                if (!Regex.IsMatch(pn, pattern))
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
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
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

        
    }
}
