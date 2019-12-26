using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimERP.Data;
using SimERP.Data.DBEntities;

namespace SimERP.Business
{
    public interface IShippedPrice : IRepository<ShippedPrice>
    {
        List<Models.MasterData.ListDTO.ShippedPrice> GetData(string searchString, int startRow, int maxRows);
        bool Save(Models.MasterData.ListDTO.ShippedPrice shippedprice, bool isNew);
        bool Delete(int id);
        bool UpdateSortOrder(int upID, int downID);
    }
}