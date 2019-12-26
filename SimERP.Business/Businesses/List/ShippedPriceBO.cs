using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SimERP.Data;
using SimERP.Data.DBEntities;

namespace SimERP.Business
{
    public class ShippedPriceBO : Repository<ShippedPrice>, IShippedPrice
    {
        #region Public methods

        public List<Models.MasterData.ListDTO.ShippedPrice> GetData(string searchString, int startRow, int maxRows)
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    string sqlWhere = string.Empty;
                    DynamicParameters param = new DynamicParameters();

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        sqlWhere += " AND t.SearchString Like @SearchString ";
                        param.Add("SearchString", "%" + searchString + "%");
                    }

                    string sqlQuery = @" SELECT Count(1) FROM  [cal].[ShippedPrice] t with(nolock) WHERE t.IsDelete = 0" + sqlWhere +
                                      @";SELECT
                                          t.*
                                         ,u.FullName AS UserName, s.StoreName
                                        FROM [cal].[ShippedPrice] t WITH (NOLOCK)
                                        LEFT JOIN acc.[User] u WITH (NOLOCK)
                                          ON u.UserID = t.CreatedBy
                                          LEFT JOIN cal.Store s ON t.StoreId = s.StoreId
                                        WHERE t.IsDelete = 0 " + sqlWhere + " ORDER BY t.CreatedDate DESC OFFSET " + startRow +
                                      " ROWS FETCH NEXT " + maxRows + " ROWS ONLY";

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        this.TotalRows = multiResult.Read<int>().Single();
                        return multiResult.Read<Models.MasterData.ListDTO.ShippedPrice>().ToList();
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

        public bool Save(Models.MasterData.ListDTO.ShippedPrice shippedprice, bool isNew)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    if (isNew)
                    {
                        if (CheckExistCode(shippedprice.MaDoGiaVc, shippedprice.StoreId))
                        {
                            this.AddMessage("000004", "Mã giá đã tồn tại, vui lòng chọn mã khác!");
                            return false;
                        }

                        shippedprice.SortOrder = db.ShippedPrice.Max(u => (int?)u.SortOrder) != null
                            ? db.ShippedPrice.Max(u => (int?)u.SortOrder) + 1
                            : 1;
                        db.ShippedPrice.Add(shippedprice);
                    }
                    else
                    {
                        db.Entry(shippedprice).State = EntityState.Modified;
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

        public bool Delete(int id)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    //TODO LIST: Kiểm tra sử dụng trước khi xóa
                    ShippedPrice item = db.ShippedPrice.Find(id);
                    item.IsActive = false;
                    item.IsDelete = true;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_003, "Delete ShippedPrice unsucessfull");
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

                    string sqlQuery = @" UPDATE [cal].[ShippedPrice] SET SortOrder = SortOrder - 1 WHERE id = @UpID;
                                         UPDATE [cal].[ShippedPrice] SET SortOrder = SortOrder + 1 WHERE id = @DownID;";

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

        #endregion

        #region Private methods

        private bool CheckExistCode(string MaDoGiaVc, int? storeId)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    int count = 0;
                    count = db.ShippedPrice.Where(m => m.MaDoGiaVc == MaDoGiaVc && storeId == m.StoreId && m.IsDelete == false).Count();
                    if (count > 0)
                        return true;
                    return false;
                }
            }
            catch (Exception ex)

            {
                this.AddMessage(MessageCode.MSGCODE_001, ex.Message);
                Logger.Error(GetType(), ex);
                return false;
            }
        }

        #endregion
    }
}