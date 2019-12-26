using SimERP.Data;
using SimERP.Data.DBEntities;
using System.Collections.Generic;
using SimERP.Business.Models.MasterData.ListDTO;

namespace SimERP.Business.Interfaces.List
{
    public interface IStore : IRepository<Store>
    {
        List<Store> GetData(ReqListSearch reqListSearch);

        bool Save(Store rowData, bool isNew);

        bool DeleteStore(int id);

        bool UpdateSortOrder(int upID, int downID);
    }
}