using Grand.Business.Core.Interfaces.Cms;
using Grand.Business.Core.Interfaces.Common.Localization;

namespace Shipping.SendCloud
{
    public class SendCloudProvider : IWidgetProvider
    {
        private readonly SendCloudSettings _settings;
        private readonly ITranslationService _translationService;

        public SendCloudProvider(
            SendCloudSettings settings,
            ITranslationService translationService)
        {
            _settings = settings;
            _translationService = translationService;
        }

        public string ConfigurationUrl => SendCloudDefaults.ConfigurationUrl;

        public string SystemName => SendCloudDefaults.ProviderSystemName;

        public string FriendlyName => _translationService.GetResource(SendCloudDefaults.FriendlyName);

        public int Priority => _settings.DisplayOrder;

        public IList<string> LimitedToStores => new List<string>();

        public IList<string> LimitedToGroups => new List<string>();

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public async Task<IList<string>> GetWidgetZones()
        {
            return await Task.FromResult(new List<string>
            {
                SendCloudDefaults.Label,SendCloudDefaults.Shiping,SendCloudDefaults.ShipingMethod,
            });
        }

        public Task<string> GetPublicViewComponentName(string widgetZone)
        {
            return Task.FromResult("WidgetsSendCloud");
        }

    }
}
