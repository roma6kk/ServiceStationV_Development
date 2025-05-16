using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = ServiceStationV.Views.MessageBox;
using Microsoft.Data.SqlClient;
using ServiceStationV.Models;

namespace ServiceStationV.Repositories
{

    public static class ServiceRepository
    {

        private static bool _isInitialized = false;
        public static ObservableCollection<Service> Services { get; } = new();

        public static async Task InitializeServicesAsync()
        {
            if (_isInitialized) return;

            var services = await GetAllServicesAsync();
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Services.Clear();
                foreach (var service in services)
                {
                    Services.Add(service);

                }
            });
            _isInitialized = true;
        }
        public static async Task<bool> AddService(Service service)
        {
            try
            {
                using (SqlConnection con = new(App.conStr))
                {
                    con.Open();
                    string addServiceQuery = "INSERT INTO Services (ServiceName, SmallDescription, LargeDescription, Price, ImageSrc, ServiceType, ServiceNameEN, SmallDescriptionEN, LargeDescriptionEN)" +
                                " VALUES(@ServiceName, @SmallDescription, @LargeDescription, @Price, @ImageSrc, @ServiceType, @ServiceNameEN, @SmallDescriptionEN, @LargeDescriptionEN)";

                    using (SqlCommand cmd = new SqlCommand(addServiceQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                        cmd.Parameters.AddWithValue("@ServiceNameEN", service.ServiceNameEN);
                        cmd.Parameters.AddWithValue("@SmallDescription", service.SmallDescription);
                        cmd.Parameters.AddWithValue("@SmallDescriptionEN", service.SmallDescriptionEN);
                        cmd.Parameters.AddWithValue("@LargeDescription", service.LargeDescription);
                        cmd.Parameters.AddWithValue("@LargeDescriptionEN", service.LargeDescriptionEN);
                        cmd.Parameters.AddWithValue("@Price", service.Price);
                        cmd.Parameters.AddWithValue("@ImageSrc", service.ImageSrc);
                        cmd.Parameters.AddWithValue("@ServiceType", service.ServiceType.ToString());
                        cmd.Parameters.AddWithValue("@ServiceId", service.ServiceId);

                        cmd.ExecuteNonQuery();
                    }
                }
                Services.Add(service);

                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении услуги в БД: " + ex.Message, "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

        }
        public static async Task<bool> UpdateService(Service service)
        {
            try
            {
                using (SqlConnection con = new(App.conStr))
                {
                    await con.OpenAsync();
                    string addServiceQuery = "UPDATE Services SET ServiceName = @ServiceName, ServiceNameEN = @ServiceNameEN, " +
                        "SmallDescription = @SmallDescription, SmallDescriptionEN = @SmallDescriptionEN, LargeDescription = @LargeDescription, " +
                        "LargeDescriptionEN = @LargeDescriptionEN, Price = @Price, ImageSrc = @ImageSrc, ServiceType = @ServiceType " +
                        "WHERE ServiceId = @ServiceId; ";

                    using (SqlCommand cmd = new SqlCommand(addServiceQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                        cmd.Parameters.AddWithValue("@ServiceNameEN", service.ServiceNameEN);
                        cmd.Parameters.AddWithValue("@SmallDescription", service.SmallDescription);
                        cmd.Parameters.AddWithValue("@SmallDescriptionEN", service.SmallDescriptionEN);
                        cmd.Parameters.AddWithValue("@LargeDescription", service.LargeDescription);
                        cmd.Parameters.AddWithValue("@LargeDescriptionEN", service.LargeDescriptionEN);
                        cmd.Parameters.AddWithValue("@Price", service.Price);
                        cmd.Parameters.AddWithValue("@ImageSrc", service.ImageSrc);
                        cmd.Parameters.AddWithValue("@ServiceType", service.ServiceType.ToString());
                        cmd.Parameters.AddWithValue("@ServiceId", service.ServiceId);

                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при добавлении услуги в БД: " + ex.Message, "Ошибка SQL", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

        }
        public static async Task<ObservableCollection<Service>> GetAllServicesAsync()
        {
            var services = new ObservableCollection<Service>();

            try
            {
                await using var con = new SqlConnection(App.conStr);
                await con.OpenAsync();

                var getServicesQuery = @"SELECT ServiceId, ServiceName, 
                           SmallDescription, LargeDescription, 
                           Price, ImageSrc, ServiceType, ServiceNameEN, SmallDescriptionEN, LargeDescriptionEN
                           FROM Services";
                await using var cmd = new SqlCommand(getServicesQuery, con);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    services.Add(new Service()
                    {
                        ServiceId = reader.GetInt32(0),
                        ServiceName = LocalizationManager.IsEnglish ? reader.GetString(7) : reader.GetString(1),
                        SmallDescription = LocalizationManager.IsEnglish ? reader.GetString(8) : reader.GetString(2),
                        LargeDescription = LocalizationManager.IsEnglish ? reader.GetString(9) : reader.GetString(3),
                        Price = reader.GetDecimal(4),
                        ImageSrc = reader.GetString(5),
                        ServiceType = (Enum.TryParse<ServiceTypes>(reader.GetString(6), out var stRu) ? stRu : ServiceTypes.Обслуживание)
                    });
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при извлечении списка услуг из БД: " + ex.Message,
                              "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return services;
        }
        public static async Task<ObservableCollection<Service>> SearchServicesAsync(string searchString)
        {
            ObservableCollection<Service> services = new ObservableCollection<Service>();
            try
            {
                using (SqlConnection con = new(App.conStr))
                {
                    await con.OpenAsync();
                    string searchQuery = "SELECT * FROM Services WHERE ServiceName LIKE @SearchString OR ServiceNameEN LIKE @SearchString";
                    using (SqlCommand cmd = new SqlCommand(searchQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@SearchString", $"%{searchString}%");

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Service service = new Service
                                {
                                    ServiceId = reader.GetInt32(0),
                                    ServiceName = LocalizationManager.IsEnglish ? reader.GetString(7) : reader.GetString(1),
                                    SmallDescription = LocalizationManager.IsEnglish ? reader.GetString(8) : reader.GetString(2),
                                    LargeDescription = LocalizationManager.IsEnglish ? reader.GetString(9) : reader.GetString(3),
                                    Price = reader.GetDecimal(4),
                                    ImageSrc = reader.GetString(5),
                                    ServiceType =(Enum.TryParse<ServiceTypes>(reader.GetString(6), out var stRu) ? stRu : ServiceTypes.Обслуживание)
                                };
                                services.Add(service);
                            }
                        }
                    }
                }
                return services;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске услуг: " + ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return services;
            }
        }


        public static async Task<List<Service>> GetServicesById(List<int> ids)
        {
            List<Service> services = new List<Service>();

            if (ids == null || ids.Count == 0)
                return services;

            using (SqlConnection con = new SqlConnection(App.conStr))
            {
                await con.OpenAsync();

                string query = @"SELECT ServiceId, ServiceName, 
                           SmallDescription, LargeDescription, 
                           Price, ImageSrc, ServiceType, 
                           ServiceNameEN, SmallDescriptionEN, LargeDescriptionEN
                         FROM Services 
                         WHERE ServiceId IN (" + string.Join(",", ids) + ")";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            services.Add(new Service
                            {
                                ServiceId = reader.GetInt32(0),
                                ServiceName = LocalizationManager.IsEnglish ? reader.GetString(7) : reader.GetString(1),
                                SmallDescription = LocalizationManager.IsEnglish ? reader.GetString(8) : reader.GetString(2),
                                LargeDescription = LocalizationManager.IsEnglish ? reader.GetString(9) : reader.GetString(3),
                                Price = reader.GetDecimal(4),
                                ImageSrc = reader.GetString(5),
                                ServiceType = (Enum.TryParse<ServiceTypes>(reader.GetString(6), out var stRu) ? stRu : ServiceTypes.Обслуживание)
                            });
                        }
                    }
                }
            }

            return services;
        }
        public static async Task<Service> GetFullServiceById(int id)
        {
            Service service = new();
            try
            {
                using (SqlConnection con = new SqlConnection(App.conStr))
                {
                    await con.OpenAsync();

                    string query = @"SELECT ServiceId, ServiceName, 
                           SmallDescription, LargeDescription, 
                           Price, ImageSrc, ServiceType, 
                           ServiceNameEN, SmallDescriptionEN, LargeDescriptionEN
                         FROM Services 
                         WHERE ServiceId = @ServiceId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ServiceId", id);
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                service = new Service
                                {
                                    ServiceId = reader.GetInt32(0),
                                    ServiceName = reader.GetString(1),
                                    SmallDescription = reader.GetString(2),
                                    LargeDescription = reader.GetString(3),
                                    Price = reader.GetDecimal(4),
                                    ImageSrc = reader.GetString(5),
                                    ServiceType = (Enum.TryParse<ServiceTypes>(reader.GetString(6), out var stRu) ? stRu : ServiceTypes.Обслуживание),
                                    ServiceNameEN = reader.GetString(7),
                                    SmallDescriptionEN = reader.GetString(8),
                                    LargeDescriptionEN = reader.GetString(9),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка получения услуги по id: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return service;
        }

    }

}
