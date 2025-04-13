using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string ServiceName { get; set; }
        public string SmallDescription { get; set; }
        public string LargeDescription { get; set; }
        public decimal Price { get; set; }
        public string ImageSrc { get; set; }
        public ServiceTypes ServiceType { get; set; }

    }
}
