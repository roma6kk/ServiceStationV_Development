using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ServiceStationV.Models
{
    public enum ServiceTypesEN
    {
        Diagnostic,
        Engine,
        Suspension,
        Brakes,
        Wheels,
        Cooling,
        Tuning,
        Maintenance
    }
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
        public string ServiceName { get; set; }
        public string ServiceNameEN { get; set; }

        public string SmallDescription { get; set; }
        public string SmallDescriptionEN { get; set; }

        public string LargeDescription { get; set; }
        public string LargeDescriptionEN { get; set; }

        public decimal Price { get; set; }
        public string ImageSrc { get; set; }
        public ServiceTypes ServiceType { get; set; }
        public ServiceTypesEN ServiceTypeEN { get; set; }
        public string LocalizedPrice => string.Format(
      LocalizationManager.IsEnglish ? "Price: {0:C}" : "Цена: {0:C}", Price);
        public string LocalizedServiceType =>
    Application.Current.Resources[ServiceType.ToString()]?.ToString() ?? ServiceType.ToString();


    }
}
