using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStationV.ValidationAttributes.UserValidationAttributes;
using System.Windows;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Models
{
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
        private bool IsEmpty()
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
                MessageBox.Show("Пожалуйста, заполните поля!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

    }
}
