using Grand.Domain.Orders;
using Grand.Web.Common.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Stores;
using Shipping.SendCloud.Domain;
using Shipping.SendCloud.Services;
using Grand.Domain.Shipping;
using Grand.Business.Core.Interfaces.Common.Pdf;
using SharpCompress.Common;
using Grand.Business.Core.Interfaces.Checkout.Shipping;

namespace Shipping.SendCloud.Controllers
{
    public class WidgetsSendCloudInfoController : BasePaymentController
    {
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IShippingSendCloudService _shippingSendCloudService;
        private readonly IShipmentService _shipmentService;
        public WidgetsSendCloudInfoController(ISettingService settingService,
            IStoreService storeService,
            IShippingSendCloudService shippingSendCloudService,
            IShipmentService shipmentService)
        {
            _settingService = settingService;
            _storeService = storeService;
            _shippingSendCloudService = shippingSendCloudService;
            _shipmentService = shipmentService;
        }
        public async Task<(bool success, Dictionary<string, string> values)> GetSendCloudInfo()
        {
            var stores = await _storeService.GetAllStores();
            var settings = _settingService.LoadSetting<SendCloudSettings>(stores.FirstOrDefault()!.Id);
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                { "Carrier", settings.Carrier },
                { "ClientId", settings.ClientId },
                { "ClientSecret", settings.ClientSecret },
                { "EnablePickup", settings.EnablePickup.ToString() },
                { "SendCloudUrl", settings.SendCloudUrl },
                { "ServiceName", settings.ServiceName },
            };
            return (true, values);
        }
        [HttpGet]
        public async Task<IActionResult> GetLabel(string id, string shipmentId)
        {
            var fileName = await _shippingSendCloudService.GetLabel(id);
            var bytes = await System.IO.File.ReadAllBytesAsync(fileName);
            var parcel = await _shippingSendCloudService.GetParcel(id);
            var shipment = await _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                return RedirectToAction("List");
            shipment.TrackingNumber = parcel.parcel.tracking_number;
            await _shipmentService.UpdateShipment(shipment);
            return File(bytes, "application/pdf", fileName);
        }
    }
}
