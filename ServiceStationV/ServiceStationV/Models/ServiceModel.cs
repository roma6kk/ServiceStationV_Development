using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static ServiceStationV.ValidationAttributes.ServiceValidationAttributes;
using static ServiceStationV.ValidationAttributes.ServiceValidationAttributes.LargeDescriptionENAttribute;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.Models
{
    public enum ServiceTypes
    {
        Диагностика,
        Двигатель,
        Подвеска,
        Тормоза,
        Колеса,
        Охлаждение,
        Тюнинг,
        Обслуживание
    }
    public class Service 
    {
        public int ServiceId { get; set; }
        [ServiceName]
        public string ServiceName { get; set; }
        [ServiceNameEN]
        public string ServiceNameEN { get; set; }
        [SmallDescription]
        public string SmallDescription { get; set; }
        [SmallDescriptionEN]
        public string SmallDescriptionEN { get; set; }
        [LargeDescription]
        public string LargeDescription { get; set; }
        [LargeDescriptionEN]
        public string LargeDescriptionEN { get; set; }
        [PriceAttribute]
        public decimal Price { get; set; }
        public string ImageSrc { get; set; }
        public ServiceTypes ServiceType { get; set; }
        public string LocalizedPrice => string.Format(
            LocalizationManager.IsEnglish ? "Price: {0:C}" : "Цена: {0:C}", Price);
        public string LocalizedServiceType =>
            Application.Current.Resources[ServiceType.ToString()]?.ToString() ?? ServiceType.ToString();
        private bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(ServiceName) &&
                   string.IsNullOrWhiteSpace(ServiceNameEN) &&
                   string.IsNullOrWhiteSpace(SmallDescription) &&
                   string.IsNullOrWhiteSpace(SmallDescriptionEN) &&
                   string.IsNullOrWhiteSpace(LargeDescription) &&
                   string.IsNullOrWhiteSpace(LargeDescriptionEN) &&
                   Price <= 0 &&
                   string.IsNullOrWhiteSpace(ImageSrc);

        }
        public bool ValidateService()
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
