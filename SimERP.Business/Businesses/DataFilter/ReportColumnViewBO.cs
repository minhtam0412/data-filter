using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SimERP.Business.Interfaces.DataFilter;
using SimERP.Business.Interfaces.List;
using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Data;
using SimERP.Data.DBEntities;

namespace SimERP.Business.Businesses.DataFilter
{
    public class ReportColumnViewBO : Repository<ReportColumnView>, IReportColumnView
    {
        public List<ReportColumnView> GetData(ReqListSearch reqListSearch, int userId)
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    string sqlWhere = string.Empty;
                    DynamicParameters param = new DynamicParameters();
                    List<string> listCondition = new List<string>();

                    listCondition.Add("rcv.UserId = @UserId");
                    param.Add("UserId", userId);

                    if (!string.IsNullOrEmpty(reqListSearch.SearchString))
                    {
                        listCondition.Add("rcv.ViewType = @ViewType");
                        param.Add("ViewType", reqListSearch.SearchString);
                    }

                    if (listCondition.Count > 0)
                    {
                        sqlWhere = "WHERE " + listCondition.Join(" AND ");
                    }

                    string sqlQuery = @"SELECT count(1) FROM cal.ReportColumnView rcv with(nolock) " + sqlWhere +
                                      @";SELECT * FROM cal.ReportColumnView rcv with(nolock) " + sqlWhere;

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        this.TotalRows = multiResult.Read<int>().Single();
                        return multiResult.Read<ReportColumnView>().ToList();
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

        public bool Save(ReportColumnView[] rowData, int userId)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            List<string> lstColumnInsert = new List<string>();
                            this.DeleteReportColumnView(userId);
                            foreach (var columnView in rowData)
                            {
                                if (this.CheckExist(columnView, userId) || lstColumnInsert.Contains(columnView.ColumnCode))
                                    continue;
                                columnView.UserId = userId;
                                columnView.CreatedBy = userId;
                                columnView.CreatedDate = DateTimeOffset.Now;
                                db.ReportColumnView.Add(columnView);
                                lstColumnInsert.Add(columnView.ColumnCode);
                            }
                            db.SaveChanges();
                            trans.Commit();
                            return true;
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_002, "Lưu không thành công: " + ex.Message);
                Logger.Error(GetType(), ex);
                return false;
            }
        }

        public bool DeleteReportColumnView(int userId)
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("UserId", userId);

                    string sqlQuery = @"DELETE FROM cal.ReportColumnView WHERE UserId = @UserId;";

                    conn.Query(sqlQuery, param);
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_003, "Delete ReportColumnView unsucessful");
                Logger.Error(GetType(), ex);
                return false;
            }
        }

        private bool CheckExist(ReportColumnView rowData, int userId)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    int count = 0;
                    count = db.ReportColumnView.Count(x =>
                        string.Equals(rowData.ColumnCode, x.ColumnCode) && userId == x.UserId);
                    if (count > 0)
                        return true;
                    return false;
                }
            }
            catch (Exception ex)

            {
                this.AddMessage(MessageCode.MSGCODE_001, ex.Message);
                Logger.Error(GetType(), ex);
                return true;
            }
        }
    }
}