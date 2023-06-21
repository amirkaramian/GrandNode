using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Cms;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using Shipping.SendCloud.Models;
using Grand.Business.Core.Utilities.Checkout;
using Shipping.SendCloud.Domain;
using Grand.Business.Core.Interfaces.Checkout.Shipping;
using Shipping.SendCloud.Services;
using Grand.Business.Core.Interfaces.Catalog.Products;
using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Stores;
using Grand.Domain.Customers;
using Grand.Domain.Orders;
using Grand.Web.Common;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Domain.Directory;
using Grand.Domain.Shipping;
using Grand.Domain.Common;
using StackExchange.Redis;
using Grand.Business.Core.Interfaces.Catalog.Directory;
using Microsoft.Extensions.DependencyInjection;
using System;
using Grand.Business.Core.Interfaces.Common.Logging;
using Grand.Business.Core.Extensions;

namespace Shipping.SendCloud.Components
{
    [ViewComponent(Name = "WidgetsSendCloud")]
    public class WidgetsSendCloudViewComponent : ViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly ICookiePreference _cookiePreference;
        private readonly SendCloudSettings _settings;
        private readonly IShippingSendCloudService _shippingSendCloudService;
        private readonly IProductService _productService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly WidgetCloudSettings _cloudSettings;
        private readonly ICountryService _countryService;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly ILogger _logger;
        public WidgetsSendCloudViewComponent(
            IWorkContext workContext,
            IOrderService orderService,
            ICookiePreference cookiePreference,
            SendCloudSettings settings,
             IShippingSendCloudService shippingSendCloudService,
             IProductService productService,
             ISettingService settingService,
             IStoreService storeService,
             ICountryService countryService,
             IMeasureService measureService,
             MeasureSettings measureSettings,
             ILogger logger)
        {
            _workContext = workContext;
            _orderService = orderService;
            _cookiePreference = cookiePreference;
            _settings = settings;
            _shippingSendCloudService = shippingSendCloudService;
            _productService = productService;
            _settingService = settingService;
            _storeService = storeService;
            _countryService = countryService;
            _cloudSettings = GetSetting();
            _measureService = measureService;
            _measureSettings = measureSettings;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            try
            {
                await _logger.Information("InvokeAsync SendCloud");
                if (_settings.EnablePickup)
                {
                    var enabled = await _cookiePreference.IsEnable(_workContext.CurrentCustomer, _workContext.CurrentStore, SendCloudDefaults.ConsentCookieSystemName);
                    if ((enabled.HasValue && !enabled.Value) || (!enabled.HasValue && !_settings.EnablePickup))
                        return Content("");
                }
                //label
                if (widgetZone == SendCloudDefaults.Label)
                {
                    var model = JsonConvert.DeserializeObject<ShipmentModel>(JsonConvert.SerializeObject(additionalData));
                    if (model != null)
                    {
                        return View("Default", await GetLabelPrint(model));
                    }
                }
                if (widgetZone == SendCloudDefaults.Shiping)
                {
                    var model = JsonConvert.DeserializeObject<ShipmentModel>(JsonConvert.SerializeObject(additionalData));
                    if (model != null)
                    {
                        return View("Default", "");
                    }
                }
                await _logger.Information("before widgetZone == SendCloudDefaults.ShipingMethod");
                if (widgetZone == SendCloudDefaults.ShipingMethod)
                {
                    var orderId = additionalData as string;
                    if (!string.IsNullOrEmpty(orderId))
                    {
                        await _logger.Information("before ProcessOrder");
                        return View("Default", await ProcessOrder(orderId));
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.Error(ex.Message, ex);
                return Content(ex.Message);
            }
            return Content("");
        }

        private async Task<string> GetLabelPrint(ShipmentModel model)
        {
            var order = await _orderService.GetOrderById(model.OrderId);
            if (order.UserFields.Count > 0)
            {
                var modelObj = order.UserFields.FirstOrDefault(x => x.Key == "Parcel").Value;
                var parcel = JsonConvert.DeserializeObject<ParcelModelRoot>(modelObj);
                var trackingScript = "<div class=\"btn-group btn-group-devided\">\r\n            " +
               $"                <a href=\"/Plugins/WidgetsSendCloudInfo/GetLabel?id={parcel.parcel.id}&shipmentId={model.Id}\" onClick=\"location.reload();\" class=\"btn blue\">\r\n          " +
               "                      <i class=\"fa fa-file-pdf-o\"></i> Print Lable\r\n                            </a>\r\n           ";
                return trackingScript;
            }
            return "<div class=\"btn-group btn-group-devided\">\r\n            " +
               $"                <a href=\"#\" class=\"btn blue\">\r\n          " +
               "                      <i class=\"fa fa-file-pdf-o\"></i> Print Lable\r\n                            </a>\r\n           ";
        }
        private async Task<string> ProcessOrder(string orderId)
        {

            var order = await _orderService.GetOrderById(orderId);
            if (order != null && order.ShippingAddress != null)
            {
                var country = await _countryService.GetCountryById(order.ShippingAddress.CountryId);// _workContext.CurrentCustomer.ShippingAddress.CountryId);
                string countryLetter = country.TwoLetterIsoCode;
                var method = await _shippingSendCloudService.GetOrSet(0, order.ShippingMethod);
                if (method == null || method.Id == "0")
                    return "";
                await _logger.Information("before CreateParcel");
                var parcel = await CreateParcel(order, countryLetter, int.Parse(method.Id));
                await _logger.Information($"parcel {JsonConvert.SerializeObject(parcel.parcel)}");
                var lable = await CreateLable(order, parcel.parcel.id, int.Parse(method.Id), order.ShippingMethod);
                await _logger.Information($"lable {JsonConvert.SerializeObject(lable.parcel)}");
                if (_cloudSettings.EnablePickup)
                {
                    var pickUp = CreatePickUpRequest(order, country.TwoLetterIsoCode);
                }
            }
            return "";
        }
        public async Task<ParcelModelRoot> CreateParcel(Grand.Domain.Orders.Order order, string countryLetter, int shippingMethodId)
        {

            var senderAddresses = await _shippingSendCloudService.GetSenderAddress();
            var points = await _shippingSendCloudService.GetServicePoint(countryLetter, _cloudSettings.Carrier);
            var parcel = new Parcel() {
                name = _workContext.CurrentCustomer.GetFullName(),
                company_name = _workContext.CurrentCustomer.ShippingAddress.Company ?? "",
                email = _workContext.CurrentCustomer.Email,
                telephone = _workContext.CurrentCustomer.ShippingAddress.PhoneNumber ?? "",
                address = _workContext.CurrentCustomer.ShippingAddress.Address1 ?? "",
                address_2 = string.IsNullOrEmpty(_workContext.CurrentCustomer.ShippingAddress.Address2) ? string.Empty : _workContext.CurrentCustomer.ShippingAddress.Address2,
                house_number = "-",
                city = _workContext.CurrentCustomer.ShippingAddress.City ?? "",
                country = countryLetter,
                postal_code = _workContext.CurrentCustomer.ShippingAddress.ZipPostalCode,
                to_post_number = _workContext.CurrentCustomer.ShippingAddress.ZipPostalCode,
                customs_invoice_nr = order.OrderNumber.ToString(),
                customs_shipment_type = null,
                country_state = null,
                parcel_items = new List<Domain.ParcelItem>(),
                sender_address = senderAddresses.sender_addresses.FirstOrDefault().id,
                shipment = new Domain.Shipment() { id = shippingMethodId, name = order.ShippingMethod },
                quantity = order.OrderItems.Sum(x => x.Quantity),
                total_insured_value = 0,
                is_return = false,
                request_label = false,
                apply_shipping_rules = false,
                request_label_async = false
            };
            if (points != null && _cloudSettings.EnableServicePoint)
                parcel.to_service_point = points.FirstOrDefault(x => x.city == _workContext.CurrentCustomer.ShippingAddress.City)?.id;
            var weight = await GetWeight();
            double conversion = weight == "lb(s)" ? 0.453592 : weight == "gram(s)" ? 0.001 : 1;
            await _logger.Information(string.Format("s1 parcel {0}", JsonConvert.SerializeObject(parcel)));

            foreach (var item in order.OrderItems)
            {
                var procuct = await _productService.GetProductById(item.ProductId);
                parcel.parcel_items.Add(new ParcelItem() {
                    quantity = item.Quantity,
                    weight = Math.Round(procuct.Weight * conversion, 3),
                    description = !string.IsNullOrEmpty(procuct.Name) && procuct.Name.Length > 200 ? procuct.Name.Substring(0, 200) : procuct.Name,
                    value = procuct.Price.ToString("F2", CultureInfo.InvariantCulture),
                    sku = procuct.Sku ?? "not set",
                    product_id = procuct.Id,
                    properties = new Properties() {
                        size = (procuct.Width * procuct.Height).ToString("F2", CultureInfo.InvariantCulture),
                    },
                    hs_code = ""
                });
            }
            parcel.weight = Math.Round(parcel.parcel_items.Sum(x => x.weight * x.quantity), 3).ToString("F2", CultureInfo.InvariantCulture);
            await _logger.Information(string.Format("s2 parcel {0}", JsonConvert.SerializeObject(parcel)));
            var resp = await _shippingSendCloudService.CreateParcel(new ParcelRecord() { parcel = parcel });
            await _logger.Information(string.Format("s2 order.UserFields == null {0}", order.UserFields == null));
            if (order.UserFields == null)
            {
                order.UserFields = new List<UserField>();
            }
            order.UserFields.Add(new UserField() {
                Key = "Parcel",
                StoreId = order.StoreId,
                Value = JsonConvert.SerializeObject(resp)
            });

            await _orderService.UpdateOrder(order);
            return resp;
        }

        public async Task<ParcelModelRoot> CreateLable(Grand.Domain.Orders.Order order, int parcelId, int shipmentId, string shipmentName)
        {
            var lable = new Lable() {
                id = parcelId,
                request_label = true,
                name = _workContext.CurrentCustomer.GetFullName(),
                shipment = new Domain.Shipment() { name = shipmentName, id = shipmentId }
            };
            var resp = await _shippingSendCloudService.CreateLable(new LableRecord() { parcel = lable });
            if (order.UserFields == null)
            {
                order.UserFields = new List<UserField>();
            }
            if (resp != null)
                order.UserFields.Add(new UserField() {
                    Key = "Parcel",
                    StoreId = order.StoreId,
                    Value = JsonConvert.SerializeObject(resp)
                });

            return resp;
        }
        public async Task<PickupRecord> CreatePickUpRequest(Grand.Domain.Orders.Order order, string countrycode)
        {

            var senderAddresses = await _shippingSendCloudService.GetSenderAddress();
            var request = new PickupRequestModel() {
                name = _workContext.CurrentCustomer.GetFullName(),
                company_name = senderAddresses.sender_addresses.FirstOrDefault().company_name,
                email = _workContext.CurrentCustomer.Email,
                telephone = _workContext.CurrentCustomer.ShippingAddress.PhoneNumber,
                address = _workContext.CurrentCustomer.ShippingAddress.Address1,
                address_2 = string.IsNullOrEmpty(_workContext.CurrentCustomer.ShippingAddress.Address2) ? string.Empty : _workContext.CurrentCustomer.ShippingAddress.Address2,
                city = _workContext.CurrentCustomer.ShippingAddress.City,
                country = countrycode,
                postal_code = _workContext.CurrentCustomer.ShippingAddress.ZipPostalCode,
                quantity = order.OrderItems.Sum(x => x.Quantity),
                pickup_from = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.AddDays(1).Day, 8, 0, 0).ToString("s"),
                pickup_until = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.AddDays(1).Day, 10, 0, 0).ToString("s"),
                carrier = _cloudSettings.Carrier

            };
            var resp = await _shippingSendCloudService.CreatePickUpRequest(request);
            foreach (var item in order.OrderItems)
            {
                var procuct = await _productService.GetProductById(item.ProductId);
                request.total_weight += procuct.Weight;
            }
            order.UserFields.Add(new UserField() {
                Key = "Pickup",
                StoreId = order.StoreId,
                Value = JsonConvert.SerializeObject(resp)
            });
            return resp;
        }
        private WidgetCloudSettings GetSetting()
        {
            var stores = _storeService.GetAllStores().GetAwaiter().GetResult();
            var settings = _settingService.LoadSetting<SendCloudSettings>(stores.FirstOrDefault()!.Id);
            return new WidgetCloudSettings() {
                ClientId = settings.ClientId,
                ClientSecret = settings.ClientSecret,
                SendCloudUrl = settings.SendCloudUrl,
                ServicePointCloudUrl = settings.ServicePointCloudUrl,
                Carrier = settings.Carrier,
                EnablePickup = settings.EnablePickup,
                EnableServicePoint = settings.EnableServicePoint,
            };
        }

        private async Task<string> GetWeight()
        {
            return (await _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId)).Name;
        }

    }
}