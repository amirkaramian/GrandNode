using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Domain
{
    public class ParcelRecord
    {
        public Parcel parcel { get; set; }
    }
    public class Parcel
    {
        public int? to_service_point { get; set; }
        public string name { get; set; }
        public string company_name { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string address { get; set; }
        public string house_number { get; set; }
        public string address_2 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string postal_code { get; set; }
        public object country_state { get; set; }
        public string customs_invoice_nr { get; set; }
        public object customs_shipment_type { get; set; }
        public List<ParcelItem> parcel_items { get; set; }
        public string weight { get; set; }
        public string length { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string total_order_value { get; set; }
        public string total_order_value_currency { get; set; }
        public Shipment shipment { get; set; }
        public string shipping_method_checkout_name { get; set; }
        public int sender_address { get; set; }
        public int quantity { get; set; }
        public int total_insured_value { get; set; }
        public bool is_return { get; set; }
        public bool request_label { get; set; }
        public bool apply_shipping_rules { get; set; }
        public bool request_label_async { get; set; }
        public string to_post_number { get; set; }
        public int Id { get;  set; }
    }

    public class ParcelRecordItem
    {
        public string description { get; set; }
        public string hs_code { get; set; }
        public string origin_country { get; set; }
        public string product_id { get; set; }
        public Properties properties { get; set; }
        public int quantity { get; set; }
        public string sku { get; set; }
        public string value { get; set; }
        public double weight { get; set; }
    }

    public class Properties
    {
        public string color { get; set; }
        public string size { get; set; }
        public string internal_storage { get; set; }
    }

    public class Shipment
    {
        public int id { get; set; }
        public string name { get; set; }
    }

}
