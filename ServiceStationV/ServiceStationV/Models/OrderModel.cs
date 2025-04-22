using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
