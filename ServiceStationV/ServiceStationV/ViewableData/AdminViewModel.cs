using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ServiceStationV.Models;
using ServiceStationV.Repositories;
using MessageBox = ServiceStationV.Views.MessageBox;

namespace ServiceStationV.ViewableData
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICollectionView ViewServices { get; set; }

        private string _searchText;
        private string _selectedSort;
        private ServiceTypes? _selectedCategory = null;
        private ObservableCollection<SortOption> _sortOptions;

        public ObservableCollection<SortOption> SortOptions
        {
            get => _sortOptions;
            set
            {
                _sortOptions = value;
                OnPropertyChanged(nameof(SortOptions));
            }
        }

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

        public string SelectedSortOption
        {
            get => _selectedSort;
            set
            {
                if (_selectedSort != value)
                {
                    _selectedSort = value;
                    OnPropertyChanged(nameof(SelectedSortOption));
                    OnPropertyChanged(nameof(SortButtonVisibility));
                    ServiceSort();
                    ViewServices?.Refresh();
                    OnPropertyChanged(nameof(IsSortApplied));
                }
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
                    OnPropertyChanged(nameof(FilterButtonVisibility));
                    ViewServices?.Refresh();
                    OnPropertyChanged(nameof(IsFilterApplied));
                }
            }
        }

        public bool IsSortApplied => SelectedSortOption != null;
        public bool IsFilterApplied => SelectedServiceOption != null;

        public Visibility FilterButtonVisibility => SelectedServiceOption != null ? Visibility.Visible : Visibility.Collapsed;
        public Visibility SortButtonVisibility => SelectedSortOption != null ? Visibility.Visible : Visibility.Collapsed;

        public List<LocalizedServiceType> FilterServices { get; } = Enum.GetValues(typeof(ServiceTypes))
            .Cast<ServiceTypes>()
            .Select(t => new LocalizedServiceType { Type = t })
            .ToList();

        public AdminViewModel()
        {
            UpdateSortOptions();
            LocalizationManager.LanguageChanged += OnLanguageChanged;
            _ = InitializeServicesAsync();
        }


        public void UpdateSortOptions()
        {
            SortOptions = new ObservableCollection<SortOption>
            {
                new SortOption { Key = "PriceAsc", DisplayName = (string)Application.Current.Resources["Sort_Ascending"] },
                new SortOption { Key = "PriceDesc", DisplayName = (string)Application.Current.Resources["Sort_Descending"] },
                new SortOption { Key = "ByName", DisplayName = (string)Application.Current.Resources["Sort_ByName"] }
            };
            OnPropertyChanged(nameof(SortOptions));
        }

        private async Task InitializeServicesAsync()
        {
            await ServiceRepository.InitializeServicesAsync();
            ViewServices = CollectionViewSource.GetDefaultView(ServiceRepository.Services);
            OnPropertyChanged(nameof(ViewServices));
            ViewServices.Filter = ServiceFilter;
        }

        private async void OnLanguageChanged(object sender, EventArgs e)
        {
            UpdateSortOptions();
            await RefreshServicesAsync();
            ViewServices?.Refresh();
        }

        private bool ServiceFilter(object item)
        {
            if (item is not Service service)
                return false;

            return SelectedServiceOption == null || service.ServiceType == SelectedServiceOption.Value;
        }

        private void ServiceSort()
        {
            if (ViewServices == null) return;

            ViewServices.SortDescriptions.Clear();

            switch (SelectedSortOption)
            {
                case "PriceAsc":
                    ViewServices.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Ascending));
                    break;
                case "PriceDesc":
                    ViewServices.SortDescriptions.Add(new SortDescription("Price", ListSortDirection.Descending));
                    break;
                case "ByName":
                    ViewServices.SortDescriptions.Add(new SortDescription("ServiceName", ListSortDirection.Ascending));
                    break;
            }

            ViewServices.Refresh();
        }

        public async Task RefreshServicesAsync()
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

        public string GetLocalizedType(ServiceTypes type) =>
            Application.Current.Resources[type.ToString()]?.ToString() ?? type.ToString();

        public class SortOption
        {
            public string Key { get; set; }
            public string DisplayName { get; set; }
        }

        public class LocalizedServiceType
        {
            public ServiceTypes Type { get; set; }
            public string DisplayName => Application.Current.Resources[Type.ToString()]?.ToString() ?? Type.ToString();
        }
    }
}
