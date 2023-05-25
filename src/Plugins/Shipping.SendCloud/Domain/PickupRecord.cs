using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Domain
{
    public class PickupRecord
    {
        public int id { get; set; }
        public DateTime pickup_from { get; set; }
        public DateTime pickup_until { get; set; }
        public string country { get; set; }
        public string carrier { get; set; }
        public string tracking_number { get; set; }
        public string pickup_status { get; set; }
        public DateTime created_at { get; set; }
        public object cancelled_at { get; set; }
        public object contract { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string postal_code { get; set; }
        public int quantity { get; set; }
        public string company_name { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string address_2 { get; set; }
        public string reference { get; set; }
        public string special_instructions { get; set; }
        public string telephone { get; set; }
        public string total_weight { get; set; }
    }
}
