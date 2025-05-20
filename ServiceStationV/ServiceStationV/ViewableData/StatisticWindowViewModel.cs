using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ServiceStationV.ViewableData
{
    public class StatisticWindowViewModel : INotifyPropertyChanged
    {
        private static readonly string _connectionString = App.conStr;

        private ObservableCollection<Feedback> _recentFeedbacks = new();
        public ObservableCollection<Feedback> RecentFeedbacks
        {
            get => _recentFeedbacks;
            set
            {
                _recentFeedbacks = value;
                OnPropertyChanged();
            }
        }

        public async Task GetRecentFeedbacksAsync()
        {
            int count = 10;
            var feedbacks = new List<Feedback>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = $@"SELECT TOP({count}) * FROM Feedbacks ORDER BY OrderDateTime DESC";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            feedbacks.Add(new Feedback
                            {
                                OrderId = reader.GetInt32(0),
                                Login = reader.GetString(1),
                                FeedbackText = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                OrderDateTime = reader.GetDateTime(3)
                            });
                        }
                    }
                }
            }

            RecentFeedbacks = new ObservableCollection<Feedback>(feedbacks);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public class Feedback
        {
            public int OrderId { get; set; }
            public string Login { get; set; }
            public string FeedbackText { get; set; }
            public DateTime OrderDateTime { get; set; }
        }
    }
}