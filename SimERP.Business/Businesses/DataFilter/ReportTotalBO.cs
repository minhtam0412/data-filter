using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.EntityFrameworkCore.Internal;
using SimERP.Business.Interfaces.DataFilter;
using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Data;
using SimERP.Data.DBEntities;
using ReportTotal = SimERP.Business.Models.MasterData.ListDTO.ReportTotal;

namespace SimERP.Business.Businesses.DataFilter
{
    public class ReportTotalBO : Repository<ReportTotal>, IReportTotal
    {
        public List<ReportTotal> GetData(ReqListSearch reqListSearch)
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    string sqlWhere = string.Empty;
                    DynamicParameters param = new DynamicParameters();
                    List<string> listCondition = new List<string>();

                    if (!string.IsNullOrEmpty(reqListSearch.SearchString))
                    {
                        listCondition.Add("rt.SearchString Like @SearchString OR rt.SearchDoanhNghiepXuatNhap Like @SearchString OR rt.SearchDoanhNghiepXuatNhap Like @SearchString" +
                                          " OR rt.SearchDoanhNghiepXuatNhap Like @SearchString");
                        param.Add("SearchString", "%" + reqListSearch.SearchString + "%");
                    }

                    if (listCondition.Count > 0)
                    {
                        sqlWhere = "WHERE " + listCondition.Join(" AND ");
                    }

                    string sqlQuery = @"SELECT  count(1) FROM cal.ReportTotal rt with(nolock) " + sqlWhere +
                                      @";SELECT  * FROM cal.ReportTotal rt with(nolock)
                                          " + sqlWhere + " ORDER BY rt.DonViDoiTac ASC, rt.NgayNhap DESC ";

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        this.TotalRows = multiResult.Read<int>().Single();
                        return multiResult.Read<ReportTotal>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_001, ex.Message);
                Logger.Error(GetType(), ex);
                return null;
            }
        }
    }
}