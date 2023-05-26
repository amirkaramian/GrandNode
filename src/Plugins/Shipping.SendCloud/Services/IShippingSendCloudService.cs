using Grand.Domain;
using Grand.Domain.Shipping;
using Shipping.SendCloud.Domain;
using Shipping.SendCloud.Models;

namespace Shipping.SendCloud.Services
{
    public interface IShippingSendCloudService
    {
        Task DeleteShippingSendCloudRecord(ShippingSendCloudRecord shippingSendCloudRecord);
        Task<IPagedList<ShippingSendCloudRecord>> GetAll(int pageIndex = 0, int pageSize = int.MaxValue);
        Task<ShippingSendCloudRecord> FindRecord(string shippingMethodId,
            string storeId, string warehouseId,
            string countryId, string stateProvinceId, string zip, double weight);
        Task<ShippingSendCloudRecord> GetById(string shippingSendCloudRecordId);
        Task InsertShippingSendCloudRecord(ShippingSendCloudRecord shippingSendCloudRecord);
        Task UpdateShippingSendCloudRecord(ShippingSendCloudRecord shippingSendCloudRecord);
        Task<ShoppingPriceModel> GetRate(ShoppingRateRecord shoppingRate);
        Task<SenderAddressModel> GetSenderAddress();
        Task<ShippingMethodList> GetShippingMethods(ShippingMethodModel model);
        Task<ParcelModelRoot> CreateParcel(ParcelRecord model);
        Task<ParcelModelRoot> CreateLable(LableRecord model);
        Task<PickupRecord> CreatePickUpRequest(PickupRequestModel model);
        Task<ShippingSendCloudRecord> GetOrSet(int methodId, string methodName);
        Task<List<ServicePoint>> GetServicePoint(string countrCode, string carrier);
        Task<string> GetLabel(string lableId);
        Task<ParcelModelRoot> GetParcel(string id);
    }

}
