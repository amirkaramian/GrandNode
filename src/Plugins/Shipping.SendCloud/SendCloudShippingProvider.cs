using Grand.Business.Core.Interfaces.Catalog.Prices;
using Grand.Business.Core.Interfaces.Catalog.Products;
using Grand.Business.Core.Enums.Checkout;
using Grand.Business.Core.Interfaces.Checkout.CheckoutAttributes;
using Grand.Business.Core.Interfaces.Checkout.Shipping;
using Grand.Business.Core.Utilities.Checkout;
using Grand.Business.Core.Extensions;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Domain.Catalog;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Domain.Orders;
using Grand.Domain.Shipping;
using Grand.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shipping.SendCloud.Services;
using Grand.Domain.Directory;
using Grand.Domain.Data;
using Shipping.SendCloud.Domain;
using Shipping.SendCloud.Models;
using DotLiquid.Util;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Stores;

namespace Shipping.SendCloud
{
    public class SendCloudShippingCalcPlugin : IShippingRateCalculationProvider
    {
        #region Fields

        private readonly IShippingMethodService _shippingMethodService;
        private readonly IWorkContext _workContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITranslationService _translationService;
        private readonly IProductService _productService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICurrencyService _currencyService;
        private readonly SendCloudShippingSettings _SendCloudShippingSettings;
        private readonly IShippingSendCloudService _shippingSendCloudService;
        private readonly WidgetCloudSettings _cloudSettings;
        private readonly HttpClient _httpClient;
        private readonly ICountryService _countryService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        #endregion

        #region Ctor
        public SendCloudShippingCalcPlugin(
            IShippingMethodService shippingMethodService,
            IWorkContext workContext,
            ITranslationService translationService,
            IProductService productService,
            IServiceProvider serviceProvider,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICurrencyService currencyService,
            SendCloudShippingSettings SendCloudShippingSettings,
            IShippingSendCloudService shippingSendCloudService,
           ISettingService settingService, IStoreService storeService,
            ICountryService countryService, IProductAttributeParser productAttributeParser)
        {
            _shippingMethodService = shippingMethodService;
            _workContext = workContext;
            _translationService = translationService;
            _productService = productService;
            _serviceProvider = serviceProvider;
            _checkoutAttributeParser = checkoutAttributeParser;
            _currencyService = currencyService;
            _SendCloudShippingSettings = SendCloudShippingSettings;
            _shippingSendCloudService = shippingSendCloudService;
            _settingService = settingService;
            _storeService = storeService;
            _cloudSettings = GetSetting();
            _countryService = countryService;
            _productAttributeParser = productAttributeParser;
        }
        #endregion

        #region Utilities

        private async Task<double?> GetRate(double weight, Domain.ShippingMethod shippingMethod, string countryFrom, string countryTo)
        {

            var shippingSendCloudService = _serviceProvider.GetRequiredService<IShippingSendCloudService>();
            var shippingSendCloudSettings = _serviceProvider.GetRequiredService<SendCloudShippingSettings>();
            var request = new ShoppingRateRecord() {
                FromCountry = countryFrom,
                ToCountry = countryTo,
                Weight = (int)weight,
                ShoppingMethodId = shippingMethod.id.ToString(),
                Weightunit = "kilogram",

            };
            var shippingSendCloudRecord = await shippingSendCloudService.GetRate(request);
            if (shippingSendCloudRecord == null)
            {
                if (shippingSendCloudSettings.LimitMethodsToCreated)
                    return null;

                return 0;
            }


            double shippingTotal = 0.0;
            //charge amount per weight unit
            if (shippingSendCloudRecord.Price == null)
                return null;
            if (shippingSendCloudRecord.Price > 0)
            {
                var weightRate = weight - shippingMethod.min_weight;
                weightRate = weightRate < 0 ? 0 : weight;
                shippingTotal = (double)(shippingSendCloudRecord.Price * weightRate);
            }

            //percentage rate of subtotal
            //if (shippingSendCloudRecord.PercentageRateOfSubtotal > 0)
            //{
            //    shippingTotal += Math.Round((float)subTotal * (float)shippingSendCloudRecord.PercentageRateOfSubtotal / 100f, 2);
            //}

            if (shippingTotal < 0)
                shippingTotal = 0;
            return shippingTotal;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets shopping cart item weight (of one item)
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>Shopping cart item weight</returns>
        private async Task<double> GetShoppingCartItemWeight(ShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            var product = await _productService.GetProductById(shoppingCartItem.ProductId);
            if (product == null)
                return 0;

            //attribute weight
            double attributesTotalWeight = 0;
            if (shoppingCartItem.Attributes != null && shoppingCartItem.Attributes.Any())
            {
                var attributeValues = _productAttributeParser.ParseProductAttributeValues(product, shoppingCartItem.Attributes);
                foreach (var attributeValue in attributeValues)
                {
                    switch (attributeValue.AttributeValueTypeId)
                    {
                        case AttributeValueType.Simple:
                            {
                                //simple attribute
                                attributesTotalWeight += attributeValue.WeightAdjustment;
                            }
                            break;
                        case AttributeValueType.AssociatedToProduct:
                            {
                                //bundled product
                                var associatedProduct = await _productService.GetProductById(attributeValue.AssociatedProductId);
                                if (associatedProduct is { IsShipEnabled: true })
                                {
                                    attributesTotalWeight += associatedProduct.Weight * attributeValue.Quantity;
                                }
                            }
                            break;
                    }
                }
            }
            var weight = product.Weight + attributesTotalWeight;
            return weight;
        }
        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="includeCheckoutAttributes">A value indicating whether we should calculate weights of selected checkout attributes</param>
        /// <returns>Total weight</returns>
        private async Task<double> GetTotalWeight(GetShippingOptionRequest request, bool includeCheckoutAttributes = true)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            Customer customer = request.Customer;

            double totalWeight = 0;
            //shopping cart items
            foreach (var packageItem in request.Items)
                totalWeight += await GetShoppingCartItemWeight(packageItem.ShoppingCartItem) * packageItem.GetQuantity();

            //checkout attributes
            if (customer != null && includeCheckoutAttributes)
            {
                var checkoutAttributes = customer.GetUserFieldFromEntity<List<CustomAttribute>>(SystemCustomerFieldNames.CheckoutAttributes, request.StoreId);
                if (checkoutAttributes.Any())
                {
                    var attributeValues = await _checkoutAttributeParser.ParseCheckoutAttributeValues(checkoutAttributes);
                    foreach (var attributeValue in attributeValues)
                        totalWeight += attributeValue.WeightAdjustment;
                }
            }
            return totalWeight;
        }

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public async Task<GetShippingOptionResponse> GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null || getShippingOptionRequest.Items.Count == 0)
            {
                response.AddError("No shipment items");
                return response;
            }
            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError("Shipping address is not set");
                return response;
            }

            var storeId = getShippingOptionRequest.StoreId;
            if (string.IsNullOrEmpty(storeId))
                storeId = _workContext.CurrentStore.Id;


            var stateProvinceId = getShippingOptionRequest.ShippingAddress.StateProvinceId;

            //string warehouseId = getShippingOptionRequest.WarehouseFrom != null ? getShippingOptionRequest.WarehouseFrom.Id : "";

            var zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
            double subTotal = 0;
            var priceCalculationService = _serviceProvider.GetRequiredService<IPricingService>();

            foreach (var packageItem in getShippingOptionRequest.Items)
            {
                if (packageItem.ShoppingCartItem.IsFreeShipping)
                    continue;

                var product = await _productService.GetProductById(packageItem.ShoppingCartItem.ProductId);
                if (product != null)
                    subTotal += (await priceCalculationService.GetSubTotal(packageItem.ShoppingCartItem, product)).subTotal;
            }

            var weight = await GetTotalWeight(getShippingOptionRequest);
            var senderAddresses = await _shippingSendCloudService.GetSenderAddress();
            var sender = senderAddresses.sender_addresses.FirstOrDefault();
            if (sender == null)
                throw new Exception("invalid sender address");
            var country = await _countryService.GetCountryById(getShippingOptionRequest.ShippingAddress.CountryId);
            if (country == null)
                throw new Exception("invalid country");

            var methods = await _shippingSendCloudService.GetShippingMethods(new Models.ShippingMethodModel() {
                SenderAddress = sender.id,
                FromPostal_code = sender.postal_code,
                ToCountry = country.TwoLetterIsoCode,
                ToPostal_code = getShippingOptionRequest.Customer.Addresses.FirstOrDefault()?.ZipPostalCode
            });
            //var countryId = getShippingOptionRequest.CountryFrom.Id;
            //var shippingMethods = await _shippingMethodService.GetAllShippingMethods(countryId, _workContext.CurrentCustomer);
            foreach (var shippingMethod in methods.shipping_methods)
            {
                double? rate = null;
                foreach (var item in getShippingOptionRequest.Items.GroupBy(x => x.ShoppingCartItem.WarehouseId).Select(x => x.Key))
                {

                    var _rate = await GetRate(weight, shippingMethod, sender.country, country.TwoLetterIsoCode);
                    if (_rate.HasValue)
                    {
                        rate ??= 0;

                        rate += _rate.Value;
                        await _shippingSendCloudService.GetOrSet(shippingMethod.id, shippingMethod.name);
                    }
                }

                if (rate is { })
                {
                    var shippingOption = new ShippingOption {
                        //  Id = shippingMethod.id,
                        Name = shippingMethod.name,
                        Description = shippingMethod.carrier,
                        Rate = await _currencyService.ConvertFromPrimaryStoreCurrency(rate.Value, _workContext.WorkingCurrency)
                    };
                    response.ShippingOptions.Add(shippingOption);
                }
            }


            return response;
        }

        /// <summary>
        /// Gets fixed shipping rate (if Shipping rate  method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public async Task<double?> GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return await Task.FromResult(default(double?));
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

        private WidgetCloudSettings GetSetting()
        {
            var stores = _storeService.GetAllStores().GetAwaiter().GetResult();
            var settings = _settingService.LoadSetting<SendCloudSettings>(stores.FirstOrDefault()!.Id);
            return new WidgetCloudSettings() {
                ClientId = settings.ClientId,
                ClientSecret = settings.ClientSecret,
                SendCloudUrl = settings.SendCloudUrl,
                Carrier = settings.Carrier,
                EnablePickup = settings.EnablePickup,
            };
          

        }
        #endregion

        #region Properties


        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker => null;

        public ShippingRateCalculationType ShippingRateCalculationType => ShippingRateCalculationType.Off;

        public string ConfigurationUrl => SendCloudShippingDefaults.ConfigurationUrl;

        public string SystemName => SendCloudShippingDefaults.ProviderSystemName;

        public string FriendlyName => _translationService.GetResource(SendCloudShippingDefaults.FriendlyName);

        public int Priority => _SendCloudShippingSettings.DisplayOrder;

        public IList<string> LimitedToStores => new List<string>();

        public IList<string> LimitedToGroups => new List<string>();

        public async Task<IList<string>> ValidateShippingForm(string shippingOption, IDictionary<string, string> data)
        {
            //you can implement here any validation logic
            return await Task.FromResult(new List<string>());
        }

        public async Task<string> GetControllerRouteName()
        {
            return await Task.FromResult("");
        }

        public async Task<IList<string>> ValidateShippingForm(IFormCollection form)
        {

            return await Task.FromResult(new List<string>());
        }

        public async Task<string> GetPublicViewComponentName()
        {
            return await Task.FromResult("");
        }

        #endregion
    }

}
