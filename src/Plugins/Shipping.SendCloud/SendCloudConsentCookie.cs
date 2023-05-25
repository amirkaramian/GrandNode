using Grand.Business.Core.Interfaces.Cms;

namespace Shipping.SendCloud
{
    public class SendCloudConsentCookie : IConsentCookie
    {
        private readonly SendCloudSettings _settings;

        public SendCloudConsentCookie(SendCloudSettings settings)
        {
            _settings = settings;
        }

        public string SystemName => SendCloudDefaults.ConsentCookieSystemName;

        public bool AllowToDisable => _settings.EnablePickup;

        public bool? DefaultState => _settings.EnablePickup;

        public int DisplayOrder => 10;

        public Task<string> FullDescription()
        {
            return Task.FromResult(_settings.ServiceName);
        }

        public Task<string> Name()
        {
            return Task.FromResult(_settings.ServiceName);
        }
    }
}
