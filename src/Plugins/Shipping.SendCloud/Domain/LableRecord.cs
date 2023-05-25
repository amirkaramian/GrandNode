using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Domain
{
    public class Lable
    {
        public int id { get; set; }
        public bool request_label { get; set; }
        public string name { get; set; }
        public Shipment shipment { get; set; }
    }
    public class LableRecord
    {
        [JsonPropertyName("parcel")]
        public Lable parcel { get; set; }
    }

}
