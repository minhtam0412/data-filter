using SimERP.Business.Models.Cal;
using System.Collections.Generic;
using SimERP.Data;
using SimERP.Data.DBEntities;

namespace SimERP.Business.Interfaces.Cal
{
    public interface ICachedData : IRepository<object>
    {
        List<Models.MasterData.ListDTO.ProductDetail> GetProductCache();
        List<Models.MasterData.ListDTO.CustomerDetail> GetCustomerCache();
        List<AmountDefine> GetAmountDefine();
        List<Models.MasterData.ListDTO.CustomerDeliveryAddress> GetAreaListCache(int CusId);
        List<PriceSpreadsheets> GetData(int userId);
        bool Save(PriceSpreadsheets priceSpreadsheets, bool isNew);
        bool Delete(int Id);
    }
}