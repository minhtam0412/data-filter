using System.Collections.Generic;
using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Data;
using SimERP.Data.DBEntities;

namespace SimERP.Business.Interfaces.DataFilter
{
    public interface IReportColumnView : IRepository<ReportColumnView>
    {
        List<ReportColumnView> GetData(ReqListSearch reqListSearch, int userId);

        bool Save(ReportColumnView[] rowData, int userId);

        bool DeleteReportColumnView(int userId);

    }
}