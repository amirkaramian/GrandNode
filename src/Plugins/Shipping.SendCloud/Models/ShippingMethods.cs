using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Models
{
    public class ShippingMethods
    {
        public List<ShippingMethod> shipping_methods { get; set; }
    }
    public class Country
    {
        public int id { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public string iso_2 { get; set; }
        public string iso_3 { get; set; }
        public int? lead_time_hours { get; set; }
        public int from_id { get; set; }
        public string from_name { get; set; }
        public string from_iso_2 { get; set; }
        public string from_iso_3 { get; set; }
        public List<PriceBreakdown> price_breakdown { get; set; }
    }

    public class PriceBreakdown
    {
        public string type { get; set; }
        public string label { get; set; }
        public double value { get; set; }
    }

    public class ShippingMethod
    {
        public int id { get; set; }
        public string name { get; set; }
        public string carrier { get; set; }
        public double min_weight { get; set; }
        public double max_weight { get; set; }
        public string service_point_input { get; set; }
        public int price { get; set; }
        public List<Country> countries { get; set; }
    }
}
