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
    public class CustomerDetailBO : Repository<CustomerDetail>, ICustomerDetail
    {
        #region Public methods

        public List<Models.MasterData.ListDTO.CustomerDetail> GetData(string searchString, int? areaId, int startRow,
            int maxRows)
        {
            try
            {
                List<Models.MasterData.ListDTO.CustomerDetail> dataResult = new List<Models.MasterData.ListDTO.CustomerDetail>();

                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    string sqlWhere = string.Empty;
                    DynamicParameters param = new DynamicParameters();

                    //if (!string.IsNullOrEmpty(searchString) || areaId != null)
                    //    sqlWhere += " WHERE ";

                    if (areaId != null)
                    {
                        sqlWhere += " AND t.IdKhuVuc = @IdKhuVuc ";
                        param.Add("IdKhuVuc", areaId);
                    }

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        sqlWhere += " AND t.SearchString Like @SearchString ";
                        param.Add("SearchString", "%" + searchString + "%");
                    }

                    string sqlQuery = @" SELECT Count(1) FROM  [cal].[CustomerDetail] t with(nolock) WHERE t.IsDelete = 0 " + sqlWhere +
                                      @";SELECT t.* FROM [cal].[CustomerDetail] t with(nolock) WHERE t.IsDelete = 0 " + sqlWhere + " ORDER BY t.CreatedDate DESC OFFSET " + startRow +
                                      " ROWS FETCH NEXT " + maxRows + " ROWS ONLY";

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        this.TotalRows = multiResult.Read<int>().Single();
                        dataResult = multiResult.Read<Models.MasterData.ListDTO.CustomerDetail>().ToList();
                    }

                    foreach (Models.MasterData.ListDTO.CustomerDetail item in dataResult)
                    {
                        sqlQuery = @"SELECT t.*, l.TenKhuVuc FROM [cal].[CustomerDeliveryAddress] t with(nolock) 
                                    LEFT JOIN [cal].[AreaList] l with(nolock) ON l.id  = t.IdKhuVuc  WHERE t.IsDelete = 0 AND t.CusId = " + item.CusId;
                        var lsttem_address = conn.QueryMultiple(sqlQuery);
                        List<Models.MasterData.ListDTO.CustomerDeliveryAddress> lstAddress = lsttem_address.Read<Models.MasterData.ListDTO.CustomerDeliveryAddress>().ToList();

                        if (lstAddress.Count > 0)
                            item.lstCustomerDeliveryAddress = lstAddress;
                        else
                            item.lstCustomerDeliveryAddress = new List<Models.MasterData.ListDTO.CustomerDeliveryAddress>();

                        //---------------------------------------------------
                        sqlQuery = @"SELECT t.*, l.ProductCode, l.ProductName
                                    FROM [cal].[Sales] t with(nolock) 
                                    LEFT JOIN [cal].[ProductDetail] l with(nolock) ON l.ProductId  = t.ProductId  WHERE l.IsActive = 1 AND t.IsDelete = 0 AND t.CusId = " + item.CusId  +
                                    " ORDER BY t.ThoiGianBan";
                        var lsttem_sales = conn.QueryMultiple(sqlQuery);
                        List<Models.MasterData.ListDTO.Sales> lstSales = lsttem_sales.Read<Models.MasterData.ListDTO.Sales>().ToList();

                        if (lstSales.Count > 0)
                            item.lstSales = lstSales;
                        else
                            item.lstSales = new List<Models.MasterData.ListDTO.Sales>();
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

        public bool Save(Models.MasterData.ListDTO.CustomerDetail customerDetail, bool isNew)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            CustomerDeliveryAddress itemdefault = customerDetail.lstCustomerDeliveryAddress.Find(x => x.IsDefault == true);
                            if (itemdefault != null)
                            {
                                customerDetail.DeliveryAdr = itemdefault.DeliveryAdr;
                                customerDetail.IdKhuVuc = itemdefault.IdKhuVuc;
                                customerDetail.MaKhuVuc = itemdefault.MaKhuVuc;
                            }
                            else
                            {
                                customerDetail.DeliveryAdr = null;
                                customerDetail.IdKhuVuc = null;
                                customerDetail.MaKhuVuc = null;
                            }

                            if (isNew)
                            {
                                if (CheckExistCode(customerDetail.CustomerCode))
                                {
                                    this.AddMessage("000004", "Mã Khách Hàng đã tồn tại, vui lòng chọn mã khác!");
                                    return false;
                                }

                                customerDetail.SortOrder = db.CustomerDetail.Max(u => (int?)u.SortOrder) != null
                                    ? db.CustomerDetail.Max(u => (int?)u.SortOrder) + 1
                                    : 1;
                                db.CustomerDetail.Add(customerDetail);
                            }
                            else
                            {
                                db.Entry(customerDetail).State = EntityState.Modified;
                            }

                            db.SaveChanges();

                            if (isNew)
                            {
                                if (customerDetail.lstCustomerDeliveryAddress.Count > 0)
                                {
                                    foreach (CustomerDeliveryAddress item in customerDetail.lstCustomerDeliveryAddress)
                                    {
                                        item.CusId = customerDetail.CusId;
                                        SaveCustomerDeliveryAddress(item, true);

                                    }

                                    foreach (Sales item in customerDetail.lstSales)
                                    {
                                        item.CusId = customerDetail.CusId;
                                        SaveCustomerSales(item, true);
                                    }
                                }
                            }
                            else
                            {
                                using (IDbConnection conn = IConnect.GetOpenConnection())
                                {
                                    string sqlQuery = @"SELECT t.* FROM [cal].[CustomerDeliveryAddress] t with(nolock) WHERE t.CusId = " + customerDetail.CusId;
                                    var lsttem = conn.QueryMultiple(sqlQuery);
                                    List<Models.MasterData.ListDTO.CustomerDeliveryAddress> lstCustomerDeliveryAddress = lsttem.Read<Models.MasterData.ListDTO.CustomerDeliveryAddress>().ToList();

                                    if (customerDetail.lstCustomerDeliveryAddress.Count <= 0)
                                    {
                                        foreach (CustomerDeliveryAddress item in lstCustomerDeliveryAddress)
                                        {
                                            DeleteCustomerDeliveryAddress(item.Id);
                                        }
                                    }
                                    else
                                    {
                                        foreach (CustomerDeliveryAddress item in lstCustomerDeliveryAddress)
                                        {
                                            if (!CheckIssueItem(item.Id, customerDetail.lstCustomerDeliveryAddress))
                                                DeleteCustomerDeliveryAddress(item.Id);

                                        }
                                        foreach (CustomerDeliveryAddress item in customerDetail.lstCustomerDeliveryAddress)
                                        {
                                            if (!CheckIssueItem(item.Id, lstCustomerDeliveryAddress))
                                            {
                                                item.CusId = customerDetail.CusId;
                                                SaveCustomerDeliveryAddress(item, true);
                                            }
                                            else
                                                SaveCustomerDeliveryAddress(item, false);
                                        }
                                    }

                                    sqlQuery = @"SELECT t.* FROM [cal].[Sales] t with(nolock) WHERE t.IsActive = 1 AND t.CusId = " + customerDetail.CusId;
                                    var lsttem_sale = conn.QueryMultiple(sqlQuery);
                                    List<Models.MasterData.ListDTO.Sales> lstCustomerSales = lsttem_sale.Read<Models.MasterData.ListDTO.Sales>().ToList();

                                    if (customerDetail.lstSales.Count <= 0)
                                    {
                                        foreach (Sales item in lstCustomerSales)
                                        {
                                            DeleteCustomerSales(item.Id);
                                        }
                                    }
                                    else
                                    {
                                        foreach (Sales item in lstCustomerSales)
                                        {
                                            if (!CheckIssueItemSales(item.Id, customerDetail.lstSales))
                                                DeleteCustomerSales(item.Id);

                                        }
                                        foreach (Sales item in customerDetail.lstSales)
                                        {
                                            if (!CheckIssueItemSales(item.Id, lstCustomerSales))
                                            {
                                                item.CusId = customerDetail.CusId;
                                                SaveCustomerSales(item, true);
                                            }
                                            else
                                                SaveCustomerSales(item, false);
                                        }
                                    }
                                }
                            }
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            this.AddMessage(MessageCode.MSGCODE_002, "Lưu không thành công: " + ex.Message);
                            Logger.Error(GetType(), ex);

                            transaction.Rollback();
                            return false;
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


        public bool SaveCustomerDeliveryAddress(CustomerDeliveryAddress customerDeliveryAddress, bool isNew)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    if (isNew)
                    {
                        db.CustomerDeliveryAddress.Add(customerDeliveryAddress);
                    }
                    else
                    {
                        db.Entry(customerDeliveryAddress).State = EntityState.Modified;
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

        public bool SaveCustomerSales(Sales sales, bool isNew)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    if (sales.PayType == 0)
                    {
                        sales.GiaNgoaiTe = null;
                        sales.LoaiNgoaiTe = null;
                    }
                    else
                    {
                        sales.GiaDong = null;
                    }

                    if (isNew)
                    {
                        
                        sales.GiaId = getStrNumber(sales.CusId) + "_" + getStrNumber(sales.ProductId);
                        db.Sales.Add(sales);
                    }
                    else
                    {
                        db.Entry(sales).State = EntityState.Modified;
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

        public bool Delete(int id, int UserID)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //TODO LIST: Kiểm tra sử dụng trước khi xóa
                            CustomerDetail item = db.CustomerDetail.Find(id);
                            item.IsActive = false;
                            item.IsDelete = true;
                            item.ModifyDate = DateTimeOffset.Now;
                            item.ModifyBy = UserID;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();

                            using (IDbConnection conn = IConnect.GetOpenConnection())
                            {
                                string sqlQuery = @"SELECT t.* FROM [cal].[CustomerDeliveryAddress] t with(nolock) WHERE t.IsDelete = 0 AND t.CusId = " + id;
                                var lsttem = conn.QueryMultiple(sqlQuery);
                                List<CustomerDeliveryAddress> lstCustomerDeliveryAddress = lsttem.Read<CustomerDeliveryAddress>().ToList();

                                if (lstCustomerDeliveryAddress.Count > 0)
                                {
                                    foreach (CustomerDeliveryAddress subitem in lstCustomerDeliveryAddress)
                                    {
                                        CustomerDeliveryAddress customerDeliveryAddress = db.CustomerDeliveryAddress.Find(subitem.Id);
                                        customerDeliveryAddress.IsActive = false;
                                        customerDeliveryAddress.IsDelete = true;
                                        customerDeliveryAddress.ModifyDate = DateTimeOffset.Now;
                                        customerDeliveryAddress.ModifyBy = UserID;
                                        db.Entry(customerDeliveryAddress).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            this.AddMessage(MessageCode.MSGCODE_002, "Xóa không thành công: " + ex.Message);
                            Logger.Error(GetType(), ex);

                            transaction.Rollback();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_003, "Delete CustomerDetail unsucessfull");
                Logger.Error(GetType(), ex);
                return false;
            }
        }

        public bool DeleteCustomerDeliveryAddress(int id)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    //TODO LIST: Kiểm tra sử dụng trước khi xóa
                    db.CustomerDeliveryAddress.Remove(db.CustomerDeliveryAddress.Find(id));
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_003, "Delete CustomerDeliveryAddress unsucessfull");
                Logger.Error(GetType(), ex);
                return false;
            }
        }

        public bool DeleteCustomerSales(int id)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    //TODO LIST: Kiểm tra sử dụng trước khi xóa
                    db.Sales.Remove(db.Sales.Find(id));
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_003, "Delete Sales unsucessfull");
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

                    string sqlQuery = @" UPDATE [cal].[CustomerDetail] SET SortOrder = SortOrder - 1 WHERE CusId = @UpID;
                                         UPDATE [cal].[CustomerDetail] SET SortOrder = SortOrder + 1 WHERE CusId = @DownID;";

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

        public List<ProductDetail> GetProductCache()
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    string sqlQuery = @"SELECT
                                           pd.id
                                          ,pd.ProductId
                                          ,pd.ProductCode
                                         ,pd.ProductName
                                         ,pd.ProductNamePlan
                                         ,pd.ProductNameFull
                                        FROM cal.ProductDetail pd WHERE pd.IsActive = 1";

                    using (var multiResult = conn.QueryMultiple(sqlQuery))
                    {
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
        #endregion

        #region Private methods

        private bool CheckExistCode(string CustomerCode)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    int count = 0;
                    count = db.CustomerDetail.Where(m => m.CustomerCode == CustomerCode && m.IsDelete == false).Count();
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

        private bool CheckIssueItem(int Id, List<Models.MasterData.ListDTO.CustomerDeliveryAddress> customerDeliveryAddresses)
        {
            try
            {
                foreach (CustomerDeliveryAddress item in customerDeliveryAddresses)
                {
                    if (item.Id == Id)
                        return true;
                }
                return false;
            }
            catch (Exception ex)

            {
                this.AddMessage(MessageCode.MSGCODE_001, ex.Message);
                Logger.Error(GetType(), ex);
                return false;
            }
        }

        private bool CheckIssueItemSales(int Id, List<Models.MasterData.ListDTO.Sales> customerSales)
        {
            try
            {
                foreach (Sales item in customerSales)
                {
                    if (item.Id == Id)
                        return true;
                }
                return false;
            }
            catch (Exception ex)

            {
                this.AddMessage(MessageCode.MSGCODE_001, ex.Message);
                Logger.Error(GetType(), ex);
                return false;
            }
        }

        private string getStrNumber(int Id)
        {
            try
            {
                string str = Id.ToString();
                string strNumber = "";
                for(int i = 4 - str.Length; i > 0; --i)
                {
                    strNumber += "0";
                }
                return strNumber + str;
            }
            catch (Exception ex)

            {
                this.AddMessage(MessageCode.MSGCODE_001, ex.Message);
                Logger.Error(GetType(), ex);
                return "";
            }
        }
        #endregion
    }
}