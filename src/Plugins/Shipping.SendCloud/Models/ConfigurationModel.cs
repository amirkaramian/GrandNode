using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shipping.SendCloud.Models
{
    public class ConfigurationModel : BaseModel
    {
        public string StoreScope { get; set; }

        [GrandResourceDisplayName("Shipping.SendCloud.ClientId")]
        public string ClientId { get; set; }

        [GrandResourceDisplayName("Shipping.SendCloud.ClientSecret")]
        public string ClientSecret { get; set; }

        [GrandResourceDisplayName("Shipping.SendCloud.SendCloudUrl")]
        public string SendCloudUrl { get; set; }

        [GrandResourceDisplayName("Shipping.SendCloud.ServiceName")]
        public string ServiceName { get; set; }

        [GrandResourceDisplayName("Shipping.SendCloud.EnablePickup")]
        public bool EnablePickup { get; set; }

        [GrandResourceDisplayName("Shipping.SendCloud.Carrier")]
        public string Carrier { get; set; }
        [GrandResourceDisplayName("Shipping.SendCloud.CarrierList")]
        public IList<SelectListItem> CarrierList { get; set; }




    }
}