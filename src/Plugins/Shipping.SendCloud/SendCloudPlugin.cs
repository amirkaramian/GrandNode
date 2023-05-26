using Grand.Business.Core.Extensions;
using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Infrastructure.Plugins;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shipping.SendCloud
{
    public class SendCloudPlugin : BasePlugin, IPlugin
    {
        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;

        public SendCloudPlugin(
            ISettingService settingService,
            ITranslationService translationService,
            ILanguageService languageService)
        {
            _settingService = settingService;
            _translationService = translationService;
            _languageService = languageService;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string ConfigurationUrl()
        {
            return SendCloudDefaults.ConfigurationUrl;
        }

        public override async Task Install()
        {
            var settings = new SendCloudSettings {
                ClientId = "",
                ClientSecret = "",
                SendCloudUrl = "https://panel.sendcloud.sc/api/v2/",
                ServicePointCloudUrl = "https://servicepoints.sendcloud.sc/api/v2/",
                EnableServicePoint = false,
                ServiceName = "Shipping.SendCloud",
                CarrierList = new List<SelectListItem>() {
                     new SelectListItem(){Text="dhl",Value="1"},
                     new SelectListItem(){Text="postat",Value="2"}
                },
                Carrier = "postat",

            };
            await _settingService.SaveSetting(settings);

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.FriendlyName", "Send Cloud");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.ClientId", "Client Id");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.ClientId.Hint", "Get or set Client Id");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.ClientSecret", "Client Secret");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.ClientSecret.Hint", "Get or set Client secret");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.SendCloudUrl", "Send Cloud Url APi");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.SendCloudUrl.Hint", "Get or set Send Cloud Url api");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.EnablePickup", "Enable Pickup");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.EnablePickup.Hint", "Get or set Send EnablePickup");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.ConsentDefaultState", "Consent default state");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.ConsentDefaultState.Hint", "Get or set the value to consent default state");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.ServiceName", "Service name");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.ServiceName.Hint", "Get or set the value to consent cookie name");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.Carrier", "Consent Carrier description");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.Carrier.Hint", "Get or set the value to consent Carrier");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.ServicePointCloudUrl", "Service Point Cloud Url");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.ServicePointCloudUrl.Hint", "Service Point Cloud Url");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.EnableServicePoint", "Enable Service Point");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.EnableServicePoint.Hint", "Get or set Enable Service Point");
            await base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task Uninstall()
        {
            //settings
            await _settingService.DeleteSetting<SendCloudSettings>();

            //locales
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.ClientId");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.ClientId.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.ClientSecret");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.ClientSecret.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.SendCloudUrl");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.SendCloudUrl.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.EnablePickup");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.EnablePickup.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.AllowToDisableConsentCookie");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.AllowToDisableConsentCookie.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.ConsentDefaultState");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.ConsentDefaultState.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.ServiceName");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.ServiceName.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.Carrier");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.Carrier.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.ServicePointCloudUrl");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.ServicePointCloudUrl.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.EnableServicePoint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Shipping.SendCloud.EnableServicePoint.Hint");
            await base.Uninstall();
        }


    }
}
