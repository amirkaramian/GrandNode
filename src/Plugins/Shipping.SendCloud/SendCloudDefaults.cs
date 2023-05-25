namespace Shipping.SendCloud
{
    public static class SendCloudDefaults
    {
        public static string ConsentCookieSystemName = "SendCloud";
        public const string ProviderSystemName = "Shipping.SendCloud";
        public const string FriendlyName = "Shipping.SendCloud.FriendlyName";
        public const string ConfigurationUrl = "../WidgetsSendCloud/Configure";

        public static string Label => "order_shipment_details_buttons"; 
        public static string Shiping => "order_details_shipping_top";
        public static string ShipingMethod => "checkout_completed_top";

    }
}
