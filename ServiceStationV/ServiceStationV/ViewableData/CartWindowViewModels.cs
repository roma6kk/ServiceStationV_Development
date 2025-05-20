using ServiceStationV.Models;
using ServiceStationV.Repositories;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ServiceStationV.ViewableData
{
    public class CartWindowViewModels : INotifyPropertyChanged
    {
        private List<Service> cartItems;
        public List<Service> CartItems
        {
            get => cartItems;
            set
            {
                cartItems = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public List<int> CartServicesIds { get; set; }
        public string ImagePath { get; set; }

        private decimal TotalPrice => CartItems?.Sum(x => x.Price) ?? 0;
        public string LocalizedTotalPriceString => string.Format(
      LocalizationManager.IsEnglish ? "Total: {0:C}" : "Итого: {0:C}", TotalPrice);
        public CartWindowViewModels(List<int> cartServicesIds)
        {
            CartServicesIds = cartServicesIds;
        }

        public async Task LoadCartItemsAsync()
        {
            CartItems = await ServiceRepository.GetServicesById(CartServicesIds);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
