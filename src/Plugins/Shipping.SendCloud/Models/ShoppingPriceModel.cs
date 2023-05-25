using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Models
{
    public class ShoppingPriceModel
    {
        public double? Price { get; set; }
        public string Currency { get; set; }
        public string To_country { get; set; }
        public Breakdown[] Breakdown { get; set; }
    }
    public class Breakdown
    {
        public string Type { get; set; }
        public string Label { get; set; }
        public double Value { get; set; }
    }
}
