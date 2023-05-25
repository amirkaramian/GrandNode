using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Domain
{



    public class ServicePoints
    {
        public List<ServicePoint> ServicePointList { get; set; }
    }

    public class ServicePoint
    {
        public int id { get; set; }
        public string code { get; set; }
        public bool is_active { get; set; }
        public object shop_type { get; set; }
        public Extra_Data extra_data { get; set; }
        public string name { get; set; }
        public string street { get; set; }
        public string house_number { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string homepage { get; set; }
        public string carrier { get; set; }
        public string country { get; set; }
        public Formatted_Opening_Times formatted_opening_times { get; set; }
        public bool open_tomorrow { get; set; }
        public bool open_upcoming_week { get; set; }
    }

    public class Extra_Data
    {
        public object shop_type { get; set; }
    }

    public class Formatted_Opening_Times
    {
        public string[] _0 { get; set; }
        public string[] _1 { get; set; }
        public string[] _2 { get; set; }
        public string[] _3 { get; set; }
        public string[] _4 { get; set; }
        public string[] _5 { get; set; }
        public string[] _6 { get; set; }
    }


}
