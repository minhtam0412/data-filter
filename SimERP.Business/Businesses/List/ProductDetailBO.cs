using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SimERP.Business.Interfaces.List;
using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Data;
using SimERP.Data.DBEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ProductDetail = SimERP.Business.Models.MasterData.ListDTO.ProductDetail;

namespace SimERP.Business.Businesses.List
{
    public class ProductDetailBO : Repository<ProductDetail>, IProductDetail
    {
        public List<ProductDetail> GetData(ReqListSearch reqListSearch)
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    string sqlWhere = string.Empty;
                    DynamicParameters param = new DynamicParameters();
                    List<string> listCondition = new List<string>();
                    listCondition.Add("(pd.IsDeleted = 0  OR pd.IsDeleted IS NULL)");
                    if (!string.IsNullOrEmpty(reqListSearch.SearchString))
                    {
                        listCondition.Add("(pd.SearchString Like @SearchString OR pd.ProductCode like @SearchString)");
                        param.Add("SearchString", "%" + reqListSearch.SearchString + "%");
                    }

                    if (listCondition.Count > 0)
                    {
                        sqlWhere = "WHERE " + listCondition.Join(" AND ");
                    }

                    string sqlQuery = @"SELECT COUNT(1) FROM cal.ProductDetail pd with(nolock) " + sqlWhere +
                                      @";SELECT
                                              id
                                             ,ProductId
                                             ,ProductCode
                                             ,ProductName
                                             ,ProductNamePlan
                                             ,GiaCIFNorth
                                             ,GiaCIF
                                             ,VATProduct
                                             ,ImportCode
                                             ,LoaiTien
                                             ,CPVeKhoLH
                                             ,CPVeKhoBN
                                             ,TgTonKhoTB
                                             ,IsDeleted
                                             ,pd.LoaiTienNorth
                                             ,pd.GiaMin
                                             ,pd.GiaMax
                                             ,pd.GiaTB
                                             ,cdMin.NameCus AS KhMinName
                                             ,cdMax.NameCus AS KhMaxName
                                            FROM cal.ProductDetail pd with(nolock)
                                            LEFT JOIN cal.CustomerDetail cdMin
                                              ON Id_KHMin = cdMin.CusId
                                            LEFT JOIN cal.CustomerDetail cdMax
                                              ON Id_KHMax = cdMax.CusId
                                          " + sqlWhere + " ORDER BY pd.CreatedDate DESC OFFSET " +
                                      reqListSearch.StartRow +
                                      " ROWS FETCH NEXT " + reqListSearch.MaxRow + " ROWS ONLY";

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        this.TotalRows = multiResult.Read<int>().Single();
                        return multiResult.Read<ProductDetail>().ToList();
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

        public bool Save(ProductDetail rowData, bool isNew)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (isNew)
                            {
                                if (CheckExistCode(rowData.ProductCode, rowData.ProductName))
                                {
                                    this.AddMessage("000005", "Mã code hoặc tên sản phẩm đã tồn tại, vui lòng chọn mã khác!");
                                    return false;
                                }
                                rowData.ProductId = db.ProductDetail.DefaultIfEmpty().Max(x => x.ProductId) + 1;
                                db.ProductDetail.Add(rowData);
                                db.SaveChanges();

                                if (rowData.Id > 0)
                                {
                                    foreach (var row in rowData.ListProductCIFPrice)
                                    {
                                        row.ProductId = rowData.ProductId;
                                        row.CreatedBy = rowData.CreatedBy ?? -1;
                                        row.CreatedDate = DateTime.Now;
                                        db.ProductCifprice.Add(row);
                                    }
                                }
                            }
                            else
                            {
                                db.Entry(rowData).State = EntityState.Modified;

                                foreach (var row in rowData.ListProductCIFPrice)
                                {
                                    if (row.Id < 1)
                                    {
                                        row.ProductId = rowData.ProductId;
                                        row.CreatedBy = rowData.CreatedBy ?? -1;
                                        row.CreatedDate = DateTime.Now;

                                        db.ProductCifprice.Add(row);
                                    }
                                    else
                                    {
                                        row.ModifyDate = DateTime.Now;
                                        row.ModifyBy = row.ModifyBy ?? -1;
                                        db.Entry(row).State = EntityState.Modified;
                                    }
                                }

                                if (rowData.ListProductCIFPricsDel != null && rowData.ListProductCIFPricsDel.Count > 0)
                                {
                                    foreach (var row in rowData.ListProductCIFPricsDel)
                                    {
                                        db.ProductCifprice.Remove(db.ProductCifprice.Find(row.Id));
                                    }
                                }
                            }

                            db.SaveChanges(true);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            this.AddMessage(MessageCode.MSGCODE_002, "Lưu không thành công: " + ex.Message);
                            Logger.Error(GetType(), ex);
                            transaction.Rollback();
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

            return false;
        }

        public bool DeleteProductDetail(int id)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    //TODO LIST: Kiểm tra sử dụng trước khi xóa
                    var productDetail = db.ProductDetail.Find(id);
                    if (productDetail != null)
                    {
                        productDetail.IsDeleted = 1;
                        db.Entry(productDetail).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    using (IDbConnection conn = IConnect.GetOpenConnection())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@ProductId", productDetail.ProductId);
                        string sqlQuery = @"UPDATE cal.ProductCIFPrice SET IsDelete = 1 WHERE ProductId = @ProductId;";
                        conn.Query(sqlQuery, param);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_003, "Delete ProductDetail unsucessful");
                Logger.Error(GetType(), ex);
                return false;
            }
        }

        public bool UpdateSortOrder(int upID, int downID)
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("UpID", upID);
                    param.Add("DownID", downID);

                    string sqlQuery =
                        @" UPDATE [cal].[AreaList] SET SortOrder = SortOrder - 1 WHERE id = @UpID;
                                         UPDATE [cal].[AreaList] SET SortOrder = SortOrder + 1 WHERE id = @DownID;";

                    conn.Query(sqlQuery, param);
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

        public Data.DBEntities.ProductDetail GetInfo(ReqListSearch reqListSearch)
        {
            Data.DBEntities.ProductDetail row = null;
            try
            {
                int id = Convert.ToInt32(reqListSearch.SearchString);
                using (var db = new DBEntities())
                {
                    row = db.ProductDetail.SingleOrDefault(x => x.Id == id);
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_001, ex.Message);
                Logger.Error(GetType(), ex);
                return null;
            }

            return row;
        }

        #region Private methods

        private bool CheckExistCode(string code, string name)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    int count = 0;
                    count = db.ProductDetail.Count(m =>
                        (m.ProductCode.Trim().ToLower() == code.Trim().ToLower() || m.ProductName.Trim().ToLower() == name.Trim().ToLower()) && m.IsDeleted != 1);
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

        #endregion Private methods
    }
}