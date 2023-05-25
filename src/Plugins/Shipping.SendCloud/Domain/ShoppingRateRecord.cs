using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Domain
{
    public class ShoppingRateRecord
    {
        public string ShoppingMethodId { get; set; }
        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
        public int Weight { get; set; }
        public string Weightunit { get; set; }
    }
}
