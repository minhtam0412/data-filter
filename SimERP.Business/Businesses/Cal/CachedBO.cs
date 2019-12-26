using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SimERP.Business.Interfaces.Cal;
using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Data;
using SimERP.Data.DBEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SimERP.Business.Models.Cal;

namespace SimERP.Business.Businesses.Cal
{
    public class CachedBO : Repository<object>, ICachedData
    {
        public List<PriceSpreadsheets> GetData(int userId)
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    string sqlWhere = string.Empty;
                    DynamicParameters param = new DynamicParameters();

                    string sqlQuery = @"SELECT t.* FROM [cal].[PriceSpreadsheets] t with(nolock) WHERE t.IsDelete = 0 AND t.CreatedBy = " + userId + " ORDER BY t.CreatedDate DESC";

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        return multiResult.Read<PriceSpreadsheets>().ToList();
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

        public bool Save(PriceSpreadsheets rowData, bool isNew)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    if (isNew)
                    {
                        rowData.Id = 0;
                        db.PriceSpreadsheets.Add(rowData);
                    }
                    else
                    {
                        db.Entry(rowData).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_002, "Lưu không thành công: " + ex.Message);
                Logger.Error(GetType(), ex);
                return false;
            }
        }

        public bool Delete(int Id)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    PriceSpreadsheets item = db.PriceSpreadsheets.Find(Id);
                    item.IsActive = false;
                    item.IsDelete = true;

                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_003, "Delete PriceSpreadsheets unsucessful");
                Logger.Error(GetType(), ex);
                return false;
            }
        }
        public List<Models.MasterData.ListDTO.ProductDetail> GetProductCache()
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    List<Models.MasterData.ListDTO.ProductDetail> dataResult = new List<Models.MasterData.ListDTO.ProductDetail>();

                    DynamicParameters param = new DynamicParameters();

                    string sqlQuery = @"SELECT t.* FROM cal.ProductDetail t WHERE t.IsActive = 1 AND t.IsDeleted = 0 ORDER BY t.CreatedDate, t.ProductName";

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        dataResult = multiResult.Read<Models.MasterData.ListDTO.ProductDetail>().ToList();
                    }
                    if (dataResult.Count > 0)
                    {
                        foreach (Models.MasterData.ListDTO.ProductDetail item in dataResult)
                        {
                            sqlQuery = @";SELECT pc.id
                                                 ,pc.ProductId
                                                 ,pc.CIFNorth
                                                 ,pc.CIFSouth
                                                 ,pc.LoaiTien
                                                 ,pc.LoaiTienNorth
                                                 ,pc.IsDefault
                                                 ,pc.IsDefaultNorth
                                                 ,pc.SortOrder
                                                 ,pc.EffectiveDate AT TIME ZONE 'UTC' AS EffectiveDate
                                                 ,pc.EffectiveDateNorth AT TIME ZONE 'UTC' AS EffectiveDateNorth
                                                 ,ac.ChiPhi TyGia
                                                 ,ac1.ChiPhi TyGiaNorth
                                        FROM cal.ProductCIFPrice pc
                                        LEFT JOIN cal.AggregateCosts ac ON pc.LoaiTien = ac.LoaiCP
                                        LEFT JOIN cal.AggregateCosts ac1 ON pc.LoaiTienNorth = ac1.LoaiCP
                                        WHERE pc.ProductId = " + item.ProductId + " ORDER BY pc.IsDefault DESC, pc.SortOrder ASC";

                            using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                            {
                                item.ListProductCIFPrice = multiResult.Read<Models.MasterData.ListDTO.ProductCifprice>().ToList();
                            }
                        }
                    }
                    return dataResult;
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_001, ex.Message);
                Logger.Error(GetType(), ex);
                return null;
            }
        }

        public List<Models.MasterData.ListDTO.CustomerDetail> GetCustomerCache()
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    List<Models.MasterData.ListDTO.CustomerDetail> dataResult = new List<Models.MasterData.ListDTO.CustomerDetail>();
                    DynamicParameters param = new DynamicParameters();

                    string sqlQuery = @"SELECT t.* FROM cal.CustomerDetail t WHERE t.IsActive = 1 AND t.IsDelete = 0 ORDER BY t.NameCus";

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        dataResult =  multiResult.Read<Models.MasterData.ListDTO.CustomerDetail>().ToList();
                    }

                    if (dataResult.Count > 0)
                    {
                        foreach (Models.MasterData.ListDTO.CustomerDetail item in dataResult)
                        {
                            sqlQuery = @"SELECT s.*
                                        FROM cal.Sales s
                                        WHERE s.IsActive = 1 AND s.CusId = " + item.CusId;

                            using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                            {
                                item.lstSales = multiResult.Read<Models.MasterData.ListDTO.Sales>().ToList();
                            }
                        }
                    }
                    
                    return dataResult;
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_001, ex.Message);
                Logger.Error(GetType(), ex);
                return null;
            }
        }

        public List<AmountDefine> GetAmountDefine()
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    DynamicParameters param = new DynamicParameters();

                    string sqlQuery = @"SELECT t.* FROM cal.AmountDefine t";

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        return multiResult.Read<AmountDefine>().ToList();
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

        public List<Models.MasterData.ListDTO.CustomerDeliveryAddress> GetAreaListCache(int CusId)
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    DynamicParameters param = new DynamicParameters();

                    string sqlQuery = @"SELECT t.*, l.TenKhuVuc FROM cal.CustomerDeliveryAddress t 
                                        LEFT JOIN cal.AreaList l ON l.id = t.IdKhuVuc WHERE t.IsActive = 1 AND t.IsDelete = 0 AND t.CusId = " + CusId;

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        return multiResult.Read<Models.MasterData.ListDTO.CustomerDeliveryAddress>().ToList();
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