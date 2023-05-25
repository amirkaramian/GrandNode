using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Models
{
    public class PickupRequestModel
    {
        public string city { get; set; }
        public string company_name { get; set; }
        public string name { get; set; }
        public string country { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string address_2 { get; set; }
        public string pickup_from { get; set; }
        public string pickup_until { get; set; }
        public string postal_code { get; set; }
        public int quantity { get; set; }
        public string telephone { get; set; }
        public string total_weight { get; set; }
        public string carrier { get; set; }
    }
}
