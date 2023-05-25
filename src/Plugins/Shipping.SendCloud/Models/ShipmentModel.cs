using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Models
{
    public class ShipmentModel
    {
        public string Id { get; set; }
        public Guid OrderGUId { get; set; }
        public string OrderId { get; set; }
    }
}
