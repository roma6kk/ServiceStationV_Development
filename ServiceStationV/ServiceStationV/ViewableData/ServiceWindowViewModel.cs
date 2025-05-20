using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStationV.Models;
using ServiceStationV.Views;
namespace ServiceStationV.ViewableData
{
    public class ServiceWindowViewModel
    {
        public int ServiceId { get; }
        public string ImagePath { get; }
        public string LargeDiscription { get; }

        public ServiceWindowViewModel()
        {
            ServiceId = ServiceWindow.LoadedService.ServiceId;
            ImagePath = ServiceWindow.LoadedService.ImageSrc;
            LargeDiscription = ServiceWindow.LoadedService.LargeDescription;
        }

    }
}
