using Grand.Business.Core.Interfaces.Common.Configuration;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Common.Stores;
using Grand.Business.Core.Utilities.Common.Security;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Authorization;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Shipping.SendCloud.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using Newtonsoft.Json;
using Wangkanai.Extensions;

namespace Shipping.SendCloud.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin]
    [PermissionAuthorize(PermissionSystemName.Widgets)]
    public class WidgetsSendCloudController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;
        private HttpClient _httpClient;
        public WidgetsSendCloudController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            ITranslationService translationService,
            HttpClient httpClient)
        {
            _workContext = workContext;
            _storeService = storeService;
            _settingService = settingService;
            _translationService = translationService;
            _httpClient = httpClient;

        }
        protected virtual async Task<string> GetActiveStore(IStoreService storeService, IWorkContext workContext)
        {
            var stores = await storeService.GetAllStores();
            if (stores.Count < 2)
                return stores.FirstOrDefault()!.Id;

            var storeId = workContext.CurrentCustomer.GetUserFieldFromEntity<string>(SystemCustomerFieldNames.AdminAreaStoreScopeConfiguration);
            var store = await storeService.GetStoreById(storeId);

            return store != null ? store.Id : "";
        }
        [AuthorizeAdmin]
        public async Task<IActionResult> Configure()
        {

            //load settings for a chosen store scope
            var storeScope = await GetActiveStore(_storeService, _workContext);
            var settings = _settingService.LoadSetting<SendCloudSettings>(storeScope);

            var model = new ConfigurationModel {
                ClientId = settings.ClientId,
                ClientSecret = settings.ClientSecret,
                ServiceName = settings.ServiceName,
                SendCloudUrl = settings.SendCloudUrl,
                EnablePickup = settings.EnablePickup,
                Carrier = settings.Carrier,
            };
            //if (settings != null && !string.IsNullOrEmpty(settings.ClientId))
            //{
            //    var items = await GetCarrier(settings.ClientId, settings.ClientSecret, settings.SendCloudUrl);

            //    foreach (var item in items)
            //    {
            //        if (!model.CarrierList.Any(x => x.Value == item))
            //        {
            //            model.CarrierList.Add(new SelectListItem() { Text = item, Value = item, Selected = settings == settings });
            //        }
            //    }
            //}
            //else
            model.CarrierList = new List<SelectListItem>() {//just DHL_express, postat, dpd, dpd_at
                     new SelectListItem(){Text="dhl_express",Value="dhl_express", Selected =settings.Carrier=="dhl_express" },
                 new SelectListItem(){Text="postat",Value="postat", Selected =settings.Carrier=="postat" },
                 new SelectListItem(){Text="dpd",Value="dpd", Selected =settings.Carrier=="dpd" },
                 new SelectListItem(){Text="dpd_at",Value="dpd_at", Selected =settings.Carrier=="dpd_at" }};
            return View(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var storeScope = await GetActiveStore(_storeService, _workContext);
            var settings = _settingService.LoadSetting<SendCloudSettings>(storeScope);
            settings.ClientId = model.ClientId;
            settings.ClientSecret = model.ClientSecret;
            settings.ServiceName = model.ServiceName;
            settings.SendCloudUrl = model.SendCloudUrl;
            settings.EnablePickup = model.EnablePickup;
            settings.Carrier = model.Carrier;

            await _settingService.SaveSetting(settings, storeScope);

            //now clear settings cache
            await _settingService.ClearCache();
            Success(_translationService.GetResource("Admin.Plugins.Saved"));
            return await Configure();
        }
        public virtual async Task<List<string>> GetCarrier(string clientId, string clientSecret, string sendCloudUrl)
        {
            var authenticationString = $"{clientId}:{clientSecret}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(authenticationString));
            _httpClient.BaseAddress = new Uri(sendCloudUrl);
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            var response = await _httpClient.GetAsync($"carriers");
            try
            {
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<string>>(json);
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}