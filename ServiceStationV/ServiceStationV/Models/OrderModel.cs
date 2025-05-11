using Azure.Core;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStationV.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string Login { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<string> Services { get; set; } = new();

        private string _updatedStatus;
        public string UpdatedStatus
        {
            get => _updatedStatus;
            set
            {
                _updatedStatus = value;
                OnPropertyChanged();
            }
        }
        private bool _isOrderHasFeedback;
        public bool IsOrderHasFeedback
        {
            get => _isOrderHasFeedback;
            set
            {
                _isOrderHasFeedback = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
