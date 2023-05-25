using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Models
{
    public class SenderAddressModel
    {
        public List<SenderAddress> sender_addresses { get; set; }
    }

    public class SenderAddress
    {
        public int id { get; set; }
        public string company_name { get; set; }
        public string contact_name { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string street { get; set; }
        public string house_number { get; set; }
        public string postal_box { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public object country_state { get; set; }
        public string vat_number { get; set; }
        public string eori_number { get; set; }
        public string label { get; set; }
        public int brand_id { get; set; }
        public string signature_full_name { get; set; }
        public string signature_initials { get; set; }
    }
}
