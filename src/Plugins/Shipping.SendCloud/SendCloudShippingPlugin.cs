using Grand.Business.Core.Extensions;
using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Domain.Orders;
using Grand.Infrastructure.Plugins;

namespace Shipping.SendCloud
{
    public class SendCloudShippingPlugin : BasePlugin, IPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;
        #endregion

        #region Ctor
        public SendCloudShippingPlugin(
            ISettingService settingService,
            ITranslationService translationService,
            ILanguageService languageService)
        {
            _settingService = settingService;
            _translationService = translationService;
            _languageService = languageService;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task Install()
        {
            //settings
            var settings = new SendCloudShippingSettings {
                LimitMethodsToCreated = false
            };
            await _settingService.SaveSetting(settings);

            //locales
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Shipping.SendCloud.FriendlyName", "Shipping by Weight");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.Store", "Store");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.Warehouse", "Warehouse");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.Country", "Country");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.StateProvince", "State / province");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.Zip", "Zip");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.ShippingMethod", "Shipping method");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.From", "Order weight from");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.To", "Order weight to");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.AdditionalFixedCost", "Additional fixed cost");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.LowerWeightLimit", "Lower weight limit");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.PercentageRateOfSubtotal", "Charge percentage (of subtotal)");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.RatePerWeightUnit", "Rate per weight unit");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.LimitMethodsToCreated", "Limit shipping methods to configured ones");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.DataHtml", "Data");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Fields.DisplayOrder", "DisplayOrder");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.AddRecord", "Add record");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Formula", "Formula to calculate rates");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Shipping.SendCloud.Formula.Value", "[additional fixed cost] + ([order total weight] - [lower weight limit]) * [rate per weight unit] + [order subtotal] * [charge percentage]");

            await base.Install();
        }

        /// <summary>
        /// Returns a value indicating whether shipping methods should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public async Task<bool> HideShipmentMethods(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this shipping methods if all products in the cart are downloadable
            //or hide this shipping methods if current customer is from certain country
            return await Task.FromResult(false);
        }

        #endregion

    }

}
