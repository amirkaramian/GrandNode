using Grand.Domain.Configuration;

namespace Shipping.SendCloud
{
    public class SendCloudShippingSettings : ISettings
    {
        public bool LimitMethodsToCreated { get; set; }
        public int DisplayOrder { get; set; }

    }
}
