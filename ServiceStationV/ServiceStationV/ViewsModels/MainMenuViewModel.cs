using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using ServiceStationV.Models;
using ServiceStationV.Repositories;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System;
using System.DirectoryServices;
namespace ServiceStationV.ViewsModels
{
    public class MainMenuViewModel : INotifyPropertyChanged
    {
        public ICollectionView ViewServices { get; private set; }
        private string _searchText;
        private ServiceTypes? _selectedCategory = null;
        public List<ServiceTypes> FilterServices { get; } = new List<ServiceTypes>(Enum.GetValues(typeof(ServiceTypes)).Cast<ServiceTypes>()).ToList();
        public List<string> SortOptions { get; } = new List<string>
{
    "Цена (по возрастанию)",
    "Цена (по убыванию)",
    "Название"
};
        private string _selectedSort;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                _ = RefreshServicesAsync();
                ViewServices?.Refresh();
            }
        }
        public ServiceTypes? SelectedServiceOption
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedServiceOption));
                    OnPropertyChanged(nameof(IsFilterApplied));
                    ViewServices?.Refresh();
                }
            }
        }
        public string SelectedSortOption
        {
            get => _selectedSort;
            set
            {
                if (_selectedSort != value)
                {
                    _selectedSort = value;
                    OnPropertyChanged(nameof(SelectedSortOption));
                    OnPropertyChanged(nameof(IsSortApplied));
                    ServiceSort();
                    ViewServices?.Refresh();
                }
            }
        }
        public bool IsSortApplied => SelectedSortOption != null;
        public bool IsFilterApplied => SelectedServiceOption != null;

        public event PropertyChangedEventHandler? PropertyChanged;



        public MainMenuViewModel()
        {
            _ = InitializeServicesAsync();
        }

        private async Task InitializeServicesAsync()
        {
            await ServiceRepository.InitializeServicesAsync();
            ViewServices = CollectionViewSource.GetDefaultView(ServiceRepository.Services);
            OnPropertyChanged(nameof(ViewServices));
            ViewServices.Filter = ServiceFilter;

        }


        private bool ServiceFilter(object item)
        {
            if (item is not Service service)
                return false;

            return SelectedServiceOption == null || service.ServiceType == SelectedServiceOption.Value;

        }
        private void ServiceSort()
        {
            if (ViewServices != null)
            {
                ViewServices.SortDescriptions.Clear();
                switch (SelectedSortOption)
                {
                    case "Цена (по возрастанию)":
                        ViewServices.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Ascending));
                        break;
                    case "Цена (по убыванию)":
                        ViewServices.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Descending));
                        break;
                    case "Название":
                        ViewServices.SortDescriptions.Add(new SortDescription("ServiceName", ListSortDirection.Ascending));
                        break;
                }
                ViewServices.Refresh();
            }
        }
        private async Task RefreshServicesAsync()
        {

            ObservableCollection<Service> services;

            if (string.IsNullOrWhiteSpace(SearchText) && SelectedServiceOption == null)
            {
                services = await ServiceRepository.GetAllServicesAsync();
            }
            else
            {
                services = await ServiceRepository.SearchServicesAsync(SearchText);
            }
            ServiceRepository.Services.Clear();
            foreach (var service in services)
            {
                ServiceRepository.Services.Add(service);
            }

            ViewServices?.Refresh();
        }
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



    }
}
