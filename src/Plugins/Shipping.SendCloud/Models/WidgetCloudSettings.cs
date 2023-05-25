using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Models
{
    public class WidgetCloudSettings
    {
        public string Carrier { get;  set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string SendCloudUrl { get; set; }
        public string ServiceName { get; set; }
        public bool EnablePickup { get; set; }
    }
}
