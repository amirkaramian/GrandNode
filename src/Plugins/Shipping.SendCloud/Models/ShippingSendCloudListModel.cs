using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Shipping.SendCloud.Models
{
    public class ShippingSendCloudListModel : BaseModel
    {
        [GrandResourceDisplayName("Plugins.Shipping.SendCloud.Fields.LimitMethodsToCreated")]
        public bool LimitMethodsToCreated { get; set; }

        [GrandResourceDisplayName("Plugins.Shipping.SendCloud.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}