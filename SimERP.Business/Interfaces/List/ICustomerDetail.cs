using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimERP.Data;
using SimERP.Data.DBEntities;

namespace SimERP.Business
{
    public interface ICustomerDetail : IRepository<CustomerDetail>
    {
        List<Models.MasterData.ListDTO.CustomerDetail> GetData(string searchString, int? areaId, int startRow, int maxRows);
        bool Save(Models.MasterData.ListDTO.CustomerDetail shippedprice, bool isNew);
        bool Delete(int id, int UserID);
        bool UpdateSortOrder(int upID, int downID);
        List<ProductDetail> GetProductCache();
    }
}