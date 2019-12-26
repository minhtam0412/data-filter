using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Data;
using SimERP.Data.DBEntities;
using System.Collections.Generic;
using ReportTotal = SimERP.Business.Models.MasterData.ListDTO.ReportTotal;

namespace SimERP.Business.Interfaces.DataFilter
{
    public interface IReportTotal : IRepository<ReportTotal>
    {
        List<ReportTotal> GetData(ReqListSearch reqListSearch);
    }
}