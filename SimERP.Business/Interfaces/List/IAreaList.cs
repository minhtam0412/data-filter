using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Data;
using SimERP.Data.DBEntities;
using System.Collections.Generic;

namespace SimERP.Business.Interfaces.List
{
    public interface IAreaList : IRepository<AreaList>
    {
        List<AreaList> GetData(ReqListSearch reqListSearch);

        bool Save(AreaList rowData, bool isNew);

        bool DeleteAreaList(int id);

        bool UpdateSortOrder(int upID, int downID);
    }
}