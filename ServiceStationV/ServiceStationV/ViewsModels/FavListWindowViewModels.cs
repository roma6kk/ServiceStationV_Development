using ServiceStationV.Models;
using ServiceStationV.Repositories;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace ServiceStationV.ViewsModels
{
    public class FavListWindowViewModels : INotifyPropertyChanged
    {
        private List<Service> _favList;
        public List<Service> FavList
        {
            get => _favList;
            set
            {
                _favList = value;
                OnPropertyChanged();
            }
        }

        public List<int> FavListIds { get; set; }

        public FavListWindowViewModels(List<int> favListIds)
        {
            FavListIds = favListIds;
            FavList = new List<Service>(); // Инициализация пустого списка
        }

        public async Task LoadFavListAsync()
        {
            try
            {
                var services = await ServiceRepository.GetServicesById(FavListIds);
                FavList = new List<Service>(services);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки избранного: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}