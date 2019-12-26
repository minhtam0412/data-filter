using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Data;
using SimERP.Data.DBEntities;
using System.Collections.Generic;
using System.Data;
using ProductCifprice = SimERP.Business.Models.MasterData.ListDTO.ProductCifprice;

namespace SimERP.Business.Interfaces.List
{
    public interface IProductCifprice : IRepository<ProductCifprice>
    {
        List<ProductCifprice> GetData(ReqListSearch reqListSearch);
        List<ProductCifprice> CompareData(DataTable dtbNew, ref string messageText);
        bool CompareAllData(ref DataTable rdtbNew, ref string messageText);
        int UpdateData(List<ProductCifprice> lstCifprices, int currentUserId, ref string messageText);
    }
}