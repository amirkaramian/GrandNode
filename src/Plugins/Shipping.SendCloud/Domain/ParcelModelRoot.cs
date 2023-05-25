using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shipping.SendCloud.Domain
{
    public class AddressDivided
    {
        public string street { get; set; }
        public string house_number { get; set; }
    }

    public class Carrier
    {
        public string code { get; set; }
    }

    public class ParcelCountry
    {
        public string iso_2 { get; set; }
        public string iso_3 { get; set; }
        public string name { get; set; }
    }

    public class CustomsDeclaration
    {
        public string type { get; set; }
        public string size { get; set; }
        public string link { get; set; }
    }

    public class Data
    {
    }

    public class Label
    {
        public List<string> normal_printer { get; set; }
        public string label_printer { get; set; }
    }

    public class ParcelModel
    {
        public int id { get; set; }
        public string address { get; set; }
        public string address_2 { get; set; }
        public AddressDivided address_divided { get; set; }
        public string city { get; set; }
        public string company_name { get; set; }
        [JsonPropertyName("country")]
        public ParcelCountry country { get; set; }
        public Data data { get; set; }
        public string date_created { get; set; }
        public object date_announced { get; set; }
        public string date_updated { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string postal_code { get; set; }
        public string reference { get; set; }
        [JsonPropertyName("shipment")]
        public ShipmentParcel shipment { get; set; }
        public Status status { get; set; }
        public object to_service_point { get; set; }
        public string telephone { get; set; }
        public string tracking_number { get; set; }
        public string weight { get; set; }
        public Label label { get; set; }
        public CustomsDeclaration customs_declaration { get; set; }
        public string order_number { get; set; }
        public int insured_value { get; set; }
        public int total_insured_value { get; set; }
        public object to_state { get; set; }
        public string customs_invoice_nr { get; set; }
        public object customs_shipment_type { get; set; }
        public List<ParcelItem> parcel_items { get; set; }
        public List<object> documents { get; set; }
        public object type { get; set; }
        public string shipment_uuid { get; set; }
        public int shipping_method { get; set; }
        public string external_order_id { get; set; }
        public string external_shipment_id { get; set; }
        public object external_reference { get; set; }
        public bool is_return { get; set; }
        public string note { get; set; }
        public string to_post_number { get; set; }
        public string total_order_value { get; set; }
        public string total_order_value_currency { get; set; }
        public int quantity { get; set; }
        public string colli_tracking_number { get; set; }
        public string colli_uuid { get; set; }
        public int collo_nr { get; set; }
        public int collo_count { get; set; }
        public object awb_tracking_number { get; set; }
        public object box_number { get; set; }
        public string length { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string shipping_method_checkout_name { get; set; }
        public object contract { get; set; }
        public Carrier carrier { get; set; }
    }

    public class ParcelItem
    {
        public string description { get; set; }
        public int quantity { get; set; }
        public double weight { get; set; }
        public string value { get; set; }
        public string hs_code { get; set; }
        public string origin_country { get; set; }
        public string product_id { get; set; }
        public object item_id { get; set; }
        public Properties properties { get; set; }
        public string sku { get; set; }
        public object return_reason { get; set; }
        public object return_message { get; set; }
    }

    //public class Properties
    //{
    //    public string size { get; set; }
    //    public string color { get; set; }
    //    public string internal_storage { get; set; }
    //}

    public class ParcelModelRoot
    {
        public ParcelModel parcel { get; set; }
    }


    public class Status
    {
        public int id { get; set; }
        public string message { get; set; }
    }
    public class ShipmentParcel
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
