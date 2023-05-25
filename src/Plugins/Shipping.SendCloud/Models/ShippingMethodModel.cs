using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Models
{
    public class ShippingMethodModel
    {
        public int SenderAddress { get; set; }
        public string ToCountry { get; set;}
        public string FromPostal_code { get; set; }
        public string ToPostal_code { get; set; }
    }
}
