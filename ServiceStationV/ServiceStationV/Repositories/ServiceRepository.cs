using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public static bool AddService(Service service)
        {
            try
            {
                using (SqlConnection con = new(App.conStr))
                {
                    con.Open();
                    string addServiceQuery = "INSERT INTO Services (ServiceName, SmallDescription, LargeDescription, Price, ImageSrc, ServiceType)" +
                                " VALUES(@ServiceName, @SmallDescription, @LargeDescription, @Price, @ImageSrc, @ServiceType)";

                    using (SqlCommand cmd = new SqlCommand(addServiceQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@ServiceName", service.ServiceName);
                        cmd.Parameters.AddWithValue("@SmallDescription", service.SmallDescription);
                        cmd.Parameters.AddWithValue("@LargeDescription", service.LargeDescription);
                        cmd.Parameters.AddWithValue("@Price", service.Price);
                        cmd.Parameters.AddWithValue("@ImageSrc", service.ImageSrc);
                        cmd.Parameters.AddWithValue("@ServiceType", service.ServiceType.ToString());

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
        public static async Task<ObservableCollection<Service>> GetAllServicesAsync()
        {
            var services = new ObservableCollection<Service>();

            try
            {
                await using var con = new SqlConnection(App.conStr);
                await con.OpenAsync();

                var getServicesQuery = "SELECT * FROM Services";
                await using var cmd = new SqlCommand(getServicesQuery, con);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    services.Add(new Service()
                    {
                        ServiceId = reader.GetInt32(0),
                        ServiceName = reader.GetString(1),
                        SmallDescription = reader.GetString(2),
                        LargeDescription = reader.GetString(3),
                        Price = reader.GetDecimal(4),
                        ImageSrc = reader.GetString(5),
                        ServiceType = (ServiceTypes)Enum.Parse(typeof(ServiceTypes), reader.GetString(6)),
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
                    string searchQuery = "SELECT * FROM Services WHERE ServiceName LIKE @Servicename";
                    using (SqlCommand cmd = new SqlCommand(searchQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@Servicename", $"%{searchString}%");
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                Service service = new Service
                                {
                                    ServiceId = reader.GetInt32(0),
                                    ServiceName = reader.GetString(1),
                                    SmallDescription = reader.GetString(2),
                                    LargeDescription = reader.GetString(3),
                                    Price = reader.GetDecimal(4),
                                    ImageSrc = reader.GetString(5),
                                    ServiceType = (ServiceTypes)Enum.Parse(typeof(ServiceTypes), reader.GetString(6)),
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

                string query = "SELECT * FROM Services WHERE ServiceId IN (" + string.Join(",", ids) + ")";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Service service = new Service()
                            {
                                ServiceId = reader.GetInt32(0),
                                ServiceName = reader.GetString(1),
                                SmallDescription = reader.GetString(2),
                                LargeDescription = reader.GetString(3),
                                Price = reader.GetDecimal(4),
                                ImageSrc = reader.GetString(5),
                                ServiceType = (ServiceTypes)Enum.Parse(typeof(ServiceTypes), reader.GetString(6)),
                            };
                            services.Add(service);
                        }
                    }
                }
            }
            return services;
        }
    }

}
