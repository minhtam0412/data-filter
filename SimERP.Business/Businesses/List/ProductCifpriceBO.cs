using Dapper;
using Microsoft.EntityFrameworkCore.Internal;
using SimERP.Business.Interfaces.List;
using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Data;
using SimERP.Data.DBEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using FastMember;
using Microsoft.EntityFrameworkCore;
using ProductCifprice = SimERP.Business.Models.MasterData.ListDTO.ProductCifprice;

namespace SimERP.Business.Businesses.List
{
    public class ProductCifpriceBO : Repository<ProductCifprice>, IProductCifprice
    {
        public List<ProductCifprice> GetData(ReqListSearch reqListSearch)
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
                        listCondition.Add("pc.ProductId = @ProductId ");
                        param.Add("ProductId", reqListSearch.SearchString);
                    }

                    if (listCondition.Count > 0)
                    {
                        sqlWhere = "WHERE " + listCondition.Join(" AND ");
                    }

                    string sqlQuery =
                        @"SELECT count(1) FROM cal.ProductCIFPrice pc LEFT JOIN cal.AggregateCosts ac ON pc.LoaiTien = ac.LoaiCP " +
                        sqlWhere +
                        @";SELECT   pc.id
                                                 ,pc.ProductId
                                                 ,pc.CIFNorth
                                                 ,pc.CIFSouth
                                                 ,pc.LoaiTien
                                                 ,pc.IsDefault
                                                 ,pc.SortOrder
                                                 ,pc.EffectiveDate AT TIME ZONE 'UTC' AS EffectiveDate
                                                 ,ac.ChiPhi TyGia
                                                 ,pc.LoaiTienNorth
                                                 ,pc.EffectiveDateNorth AT TIME ZONE 'UTC' AS EffectiveDateNorth
                                                 ,pc.CreatedDate
                                                 ,pc.CreatedBy
                                                 ,pc.IsDefaultNorth
                                        FROM cal.ProductCIFPrice pc
                                        LEFT JOIN cal.AggregateCosts ac ON pc.LoaiTien = ac.LoaiCP
                                          " + sqlWhere + " ORDER BY pc.IsDefault DESC, pc.IsDefaultNorth DESC, pc.SortOrder ASC";

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        this.TotalRows = multiResult.Read<int>().Single();
                        return multiResult.Read<ProductCifprice>().ToList();
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

        public DataTable GetCurrentCIFPriceAllProduct()
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    DynamicParameters param = new DynamicParameters();
                    string sqlQuery = @"SELECT
                                          pd.ProductCode
                                         ,pd.ProductId
                                         ,pc.CIFSouth
                                         ,pc.CIFNorth
                                         ,pc.IsDefault
                                         ,pc.IsDefaultNorth
                                        FROM cal.ProductDetail pd
                                        LEFT JOIN cal.ProductCIFPrice pc
                                          ON pc.ProductId = pd.ProductId
                                          AND (pc.IsDefault = 1
                                          OR pc.IsDefaultNorth = 1)
                                        WHERE pd.IsDeleted = 0";

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        IEnumerable<ProductCifprice> data = multiResult.Read<ProductCifprice>().ToList();
                        DataTable table = new DataTable();
                        using (var reader = ObjectReader.Create(data))
                        {
                            table.Load(reader);
                        }

                        return table;
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

        public bool CompareAllData(ref DataTable dtbNew, ref string messageText)
        {
            try
            {
                if (dtbNew.Rows.Count == 0) return true;

                //Distinct lại danh sách tránh bị lặp dữ liệu từ file Access
                var dtbNewDistinct = dtbNew.DefaultView.ToTable(true, new string[] { "Product_ID", "GiaCIFMN", "LoaiTienMN", "NgayMN", "GiaCIFMB", "LoaiTienMB", "NgayMB", "IsExisted", "IsDefault", "IsDefaultNorth", "IsUpdateCurrentCIFSouth", "IsUpdateCurrentCIFNorth" });

                DataTable dtbDistinctId = dtbNewDistinct.DefaultView.ToTable(true, "Product_Id");

                if (dtbDistinctId.Rows.Count > 0)
                {
                    //duyệt qua từng product trong file template
                    foreach (DataRow row in dtbDistinctId.Rows)
                    {
                        int productId = 0;
                        int.TryParse(Convert.ToString(row["Product_Id"]), out productId);
                        if (productId == 0) continue;

                        //lấy thông tin giá CIF trong DB
                        DataTable dtbCIFPriceDB = this.GetCIFPrice(productId);

                        //lấy thông tin giá CIF từ file của a Minh
                        var arrCIFPriceImport = dtbNewDistinct.Select("Product_Id = " + productId);
                        if (arrCIFPriceImport.Length > 0)
                        {
                            int indexSouth = -1;
                            DateTime dtSouthMax = this.GetEffectiveDate(arrCIFPriceImport[0], "NgayMN");
                            int indexNorth = -1;
                            DateTime dtNorthMax = this.GetEffectiveDate(arrCIFPriceImport[0], "NgayMB");

                            //lấy ra chỉ số dòng đang có Ngày tờ khai lớn nhất
                            for (int i = 0; i < arrCIFPriceImport.Length; i++)
                            {
                                CheckDataChanged(arrCIFPriceImport[i], dtbCIFPriceDB);
                                DateTime dtSouth = this.GetEffectiveDate(arrCIFPriceImport[i], "NgayMN");
                                DateTime dtNorth = this.GetEffectiveDate(arrCIFPriceImport[i], "NgayMB");

                                if (dtSouth != new DateTime() && dtSouth >= dtSouthMax)
                                {
                                    dtSouthMax = dtSouth;
                                    indexSouth = i;
                                }

                                if (dtNorth != new DateTime() && dtNorth >= dtNorthMax)
                                {
                                    dtNorthMax = dtNorth;
                                    indexNorth = i;
                                }
                            }

                            //lấy ra dữ liệu giá CIF Miền Nam hiện tại trong DB
                            var drCurrentCIFPriceSouth = dtbCIFPriceDB.Select("IsDefault = 1");
                            if (drCurrentCIFPriceSouth.Length > 0)
                            {
                                if (indexSouth > -1)
                                {
                                    //lấy thông tin hiện tại trong DB
                                    string LoaiTienSouthDB = this.GetLoaiTien(drCurrentCIFPriceSouth[0], "LoaiTien");
                                    DateTime EffectiveDateSouthDB = this.GetEffectiveDateDB(drCurrentCIFPriceSouth[0], "EffectiveDate");
                                    decimal CIFPriceSouthDB = this.GetCIFPrice(drCurrentCIFPriceSouth[0], "CIFSouth");

                                    //lấy thông tin row có ngày tờ khai max theo index đã tìm ở trên
                                    string LoaiTienSouthImport = this.GetLoaiTien(arrCIFPriceImport[indexSouth], "LoaiTienMN");
                                    DateTime EffectiveDateSouthImport = this.GetEffectiveDate(arrCIFPriceImport[indexSouth], "NgayMN");
                                    decimal CIFPriceSouthImport = this.GetCIFPrice(arrCIFPriceImport[indexSouth], "GiaCIFMN");

                                    //thực hiện so sánh 2 dữ liệu trên
                                    if (String.Compare(LoaiTienSouthImport, LoaiTienSouthDB, StringComparison.CurrentCulture) != 0 ||
                                        (EffectiveDateSouthImport.Year != EffectiveDateSouthDB.Year || EffectiveDateSouthImport.Month != EffectiveDateSouthDB.Month
                                                                                                    || EffectiveDateSouthImport.Day != EffectiveDateSouthDB.Day)
                                        || CIFPriceSouthImport != CIFPriceSouthDB)
                                    {
                                        arrCIFPriceImport[indexSouth]["IsDefault"] = 1;
                                        arrCIFPriceImport[indexSouth]["IsUpdateCurrentCIFSouth"] = 1;
                                    }
                                }
                            }
                            //trong DB chưa có dòng giá CIF hiện hành nào
                            else
                            {
                                //thực hiện đánh dấu dòng dữ liệu có ngày Max từ file import làm giá hiện hành
                                if (indexSouth > -1)
                                    arrCIFPriceImport[indexSouth]["IsDefault"] = 1;
                            }


                            //----------------------------------------------------------------------------------
                            //lấy ra dữ liệu giá CIF Miền Bắc hiện tại trong DB
                            var drCurrentCIFPriceNorth = dtbCIFPriceDB.Select("IsDefaultNorth = 1");
                            if (drCurrentCIFPriceNorth.Length > 0)
                            {
                                if (indexNorth > -1)
                                {
                                    //lấy thông tin hiện tại trong DB
                                    string LoaiTienNorthDB = this.GetLoaiTien(drCurrentCIFPriceNorth[0], "LoaiTienNorth");
                                    DateTime EffectiveDateNorthDB = this.GetEffectiveDateDB(drCurrentCIFPriceNorth[0], "EffectiveDateNorth");
                                    decimal CIFPriceNorthDB = this.GetCIFPrice(drCurrentCIFPriceNorth[0], "CIFNorth");

                                    //lấy thông tin row có ngày tờ khai max theo index đã tìm ở trên
                                    string LoaiTienNorthImport = this.GetLoaiTien(arrCIFPriceImport[indexNorth], "LoaiTienMB");
                                    DateTime EffectiveDateNorthImport = this.GetEffectiveDate(arrCIFPriceImport[indexNorth], "NgayMB");
                                    decimal CIFPriceNorthImport = this.GetCIFPrice(arrCIFPriceImport[indexNorth], "GiaCIFMB");

                                    //thực hiện so sánh 2 dữ liệu trên
                                    if (String.Compare(LoaiTienNorthImport, LoaiTienNorthDB, StringComparison.CurrentCulture) != 0 ||
                                        (EffectiveDateNorthImport.Year != EffectiveDateNorthDB.Year || EffectiveDateNorthImport.Month != EffectiveDateNorthDB.Month
                                                                                                    || EffectiveDateNorthImport.Day != EffectiveDateNorthDB.Day)
                                        || CIFPriceNorthImport != CIFPriceNorthDB)
                                    {
                                        arrCIFPriceImport[indexSouth]["IsDefaultNorth"] = 1;
                                        arrCIFPriceImport[indexSouth]["IsUpdateCurrentCIFNorth"] = 1;
                                    }
                                }
                            }
                            //trong DB chưa có dòng giá CIF hiện hành nào
                            else
                            {
                                //thực hiện đánh dấu dòng dữ liệu có ngày Max từ file import làm giá hiện hành
                                if (indexNorth > -1)
                                    arrCIFPriceImport[indexNorth]["IsDefaultNorth"] = 1;
                            }
                        }

                        var arrImportFinal = dtbNewDistinct.Select("IsExisted = 0 AND Product_Id = " + productId);
                        if (arrImportFinal.Length > 0)
                        {
                            using (var db = new DBEntities())
                            {
                                using (var trans = db.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        foreach (DataRow drImport in arrImportFinal)
                                        {
                                            Data.DBEntities.ProductCifprice productCifprice = this.GetProductCIFPrice(drImport);
                                            var SortOrder = db.ProductCifprice.Where(y => y.ProductId == productCifprice.ProductId).DefaultIfEmpty().Max(x => x.SortOrder);
                                            productCifprice.SortOrder = SortOrder != null ? SortOrder + 1 : 1;
                                            db.ProductCifprice.Add(productCifprice);
                                            if (drImport["IsUpdateCurrentCIFSouth"] != DBNull.Value && Convert.ToInt32(drImport["IsUpdateCurrentCIFSouth"]) == 1)
                                            {
                                                using (IDbConnection conn = IConnect.GetOpenConnection())
                                                {
                                                    DynamicParameters param = new DynamicParameters();
                                                    param.Add("@ProductId", productCifprice.ProductId);

                                                    string sqlQuery =
                                                        @"UPDATE cal.ProductCIFPrice SET IsDefault = 0 WHERE ProductId = @ProductId;";

                                                    conn.Query(sqlQuery, param);
                                                }
                                            }
                                            if (drImport["IsUpdateCurrentCIFNorth"] != DBNull.Value && Convert.ToInt32(drImport["IsUpdateCurrentCIFNorth"]) == 1)
                                            {
                                                using (IDbConnection conn = IConnect.GetOpenConnection())
                                                {
                                                    DynamicParameters param = new DynamicParameters();
                                                    param.Add("@ProductId", productCifprice.ProductId);

                                                    string sqlQuery =
                                                        @"UPDATE cal.ProductCIFPrice SET IsDefaultNorth = 0 WHERE ProductId = @ProductId;";

                                                    conn.Query(sqlQuery, param);
                                                }
                                            }

                                            if (drImport["IsDefault"] != DBNull.Value &&
                                                Convert.ToInt32(drImport["IsDefault"]) == 1)
                                            {
                                                // lấy ra thông tin của sản phẩm để update thông tin show ra màn hình danh sách
                                                var productDetail = db.ProductDetail.FirstOrDefault(x => x.ProductId == productCifprice.ProductId);
                                                if (productDetail != null)
                                                {
                                                    productDetail.GiaCif = productCifprice.Cifsouth;
                                                    productDetail.LoaiTien = productCifprice.LoaiTien;
                                                    db.Entry(productDetail).State = EntityState.Modified;
                                                }
                                            }

                                            if (drImport["IsDefaultNorth"] != DBNull.Value &&
                                                Convert.ToInt32(drImport["IsDefaultNorth"]) == 1)
                                            {
                                                // lấy ra thông tin của sản phẩm để update thông tin show ra màn hình danh sách
                                                var productDetail = db.ProductDetail.FirstOrDefault(x => x.ProductId == productCifprice.ProductId);
                                                if (productDetail != null)
                                                {
                                                    productDetail.GiaCifnorth = productCifprice.Cifnorth;
                                                    productDetail.LoaiTienNorth = productCifprice.LoaiTienNorth;
                                                    db.Entry(productDetail).State = EntityState.Modified;
                                                }
                                            }
                                        }

                                        db.SaveChanges();
                                        trans.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        messageText = ex.Message;
                                        trans.Rollback();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                messageText = e.Message;
                return false;
            }

            return true;
        }

        private Data.DBEntities.ProductCifprice GetProductCIFPrice(DataRow drImport)
        {
            Data.DBEntities.ProductCifprice productCifprice = new Data.DBEntities.ProductCifprice();
            DateTime dtNow = DateTime.Now;
            productCifprice.ProductId = Convert.ToInt32(drImport["Product_ID"]);
            productCifprice.Cifsouth = drImport["GiaCIFMN"] != DBNull.Value ? Convert.ToDecimal(drImport["GiaCIFMN"]) : 0;
            productCifprice.LoaiTien = !string.IsNullOrEmpty(Convert.ToString(drImport["LoaiTienMN"]))
                ? Convert.ToString(drImport["LoaiTienMN"]).ToUpper() : null;
            string ngayMN = Convert.ToString(drImport["NgayMN"]);
            DateTime dtMN;
            if (!string.IsNullOrEmpty(ngayMN))
            {
                DateTime.TryParseExact(ngayMN, "dd/MM/yy", CultureInfo.CurrentCulture, DateTimeStyles.None,
                    out dtMN);
                var dtRsl = new DateTime(dtMN.Year, dtMN.Month, dtMN.Day, dtNow.Hour, dtNow.Month, dtNow.Second);
                productCifprice.EffectiveDate = dtRsl;
            }


            productCifprice.IsDefault = drImport["IsDefault"] != DBNull.Value ? Convert.ToInt32(drImport["IsDefault"]) : 0;

            productCifprice.Cifnorth = drImport["GiaCIFMB"] != DBNull.Value ? Convert.ToDecimal(drImport["GiaCIFMB"]) : 0;
            productCifprice.LoaiTienNorth = !string.IsNullOrEmpty(Convert.ToString(drImport["LoaiTienMB"]))
                ? Convert.ToString(drImport["LoaiTienMB"]).ToUpper() : null;
            string ngayMB = Convert.ToString(drImport["NgayMB"]);
            DateTime dtMB;
            if (!string.IsNullOrEmpty(ngayMB))
            {
                DateTime.TryParseExact(ngayMB, "dd/MM/yy", CultureInfo.CurrentCulture, DateTimeStyles.None,
                    out dtMB);
                var dtRsl = new DateTime(dtMB.Year, dtMB.Month, dtMB.Day, dtNow.Hour, dtNow.Month, dtNow.Second);
                productCifprice.EffectiveDateNorth = dtRsl;
            }
            productCifprice.IsDefaultNorth = drImport["IsDefaultNorth"] != DBNull.Value ? Convert.ToInt32(drImport["IsDefaultNorth"]) : 0;

            productCifprice.IsActive = 1;
            productCifprice.IsImport = true;
            productCifprice.CreatedDate = DateTimeOffset.Now;

            return productCifprice;
        }

        private void CheckDataChanged(DataRow drImport, DataTable dtbCifPriceDb)
        {
            string LoaiTienSouthImport = this.GetLoaiTien(drImport, "LoaiTienMN");
            DateTime EffectiveDateSouthImport = this.GetEffectiveDate(drImport, "NgayMN");
            decimal CIFPriceSouthImport = this.GetCIFPrice(drImport, "GiaCIFMN");

            string LoaiTienNorthImport = this.GetLoaiTien(drImport, "LoaiTienMB");
            DateTime EffectiveDateNorthImport = this.GetEffectiveDate(drImport, "NgayMB");
            decimal CIFPriceNorthImport = this.GetCIFPrice(drImport, "GiaCIFMB");

            bool bolIsExisted = false;
            foreach (DataRow drDB in dtbCifPriceDb.Rows)
            {
                string LoaiTienSouthDB = this.GetLoaiTien(drDB, "LoaiTien");
                DateTime EffectiveDateSouthDB = this.GetEffectiveDateDB(drDB, "EffectiveDate");
                decimal CIFPriceSouthDB = this.GetCIFPrice(drDB, "CIFSouth");

                string LoaiTienNorthDB = this.GetLoaiTien(drDB, "LoaiTienNorth");
                DateTime EffectiveDateNorthDB = this.GetEffectiveDateDB(drDB, "EffectiveDateNorth");
                decimal CIFPriceNorthDB = this.GetCIFPrice(drDB, "CIFNorth");

                if (String.Compare(LoaiTienSouthImport, LoaiTienSouthDB, StringComparison.CurrentCulture) == 0 &&
                    String.Compare(LoaiTienNorthImport, LoaiTienNorthDB, StringComparison.CurrentCulture) == 0 &&
                    (EffectiveDateSouthImport.Year == EffectiveDateSouthDB.Year && EffectiveDateSouthImport.Month == EffectiveDateSouthDB.Month
                                                                                && EffectiveDateSouthImport.Day == EffectiveDateSouthDB.Day) &&
                    (EffectiveDateNorthImport.Year == EffectiveDateNorthDB.Year && EffectiveDateNorthImport.Month == EffectiveDateNorthDB.Month
                                                                                && EffectiveDateNorthImport.Day == EffectiveDateNorthDB.Day)
                    && CIFPriceSouthImport == CIFPriceSouthDB && CIFPriceNorthImport == CIFPriceNorthDB)
                {
                    bolIsExisted = true;
                    break;
                }
            }

            drImport["IsExisted"] = bolIsExisted ? 1 : 0;
        }

        private string GetLoaiTien(DataRow row, string colName)
        {
            string LoaiTien = !string.IsNullOrEmpty(Convert.ToString(row[colName]))
                ? Convert.ToString(row[colName]).ToUpper()
                : null;
            return LoaiTien;
        }

        private DateTime GetEffectiveDate(DataRow row, string colName)
        {
            string strDate = Convert.ToString(row[colName]);
            DateTime dtRsl = new DateTime();
            if (!string.IsNullOrEmpty(strDate))
            {
                DateTime.TryParseExact(strDate, "dd/MM/yy", CultureInfo.CurrentCulture, DateTimeStyles.None,
                    out dtRsl);
            }
            return dtRsl;
        }

        private DateTime GetEffectiveDateDB(DataRow row, string colName)
        {
            string strDate = Convert.ToString(row[colName]);
            DateTime dtRsl = new DateTime();
            if (!string.IsNullOrEmpty(strDate))
            {
                DateTime.TryParse(strDate, CultureInfo.CurrentCulture, DateTimeStyles.None, out dtRsl);
            }
            return dtRsl;
        }

        private decimal GetCIFPrice(DataRow row, string colName)
        {
            decimal CIFPrice = 0;
            if (row[colName] != DBNull.Value)
            {
                CIFPrice = Convert.ToDecimal(row[colName]);
            }

            return CIFPrice;
        }


        private DataTable GetCIFPrice(int productId)
        {
            try
            {
                using (IDbConnection conn = IConnect.GetOpenConnection())
                {
                    DynamicParameters param = new DynamicParameters();
                    string sqlQuery = @"SELECT
                                          pd.ProductCode
                                         ,pd.ProductId
                                         ,pc.CIFSouth
                                         ,pc.CIFNorth
                                         ,pc.IsDefault
                                         ,pc.IsDefaultNorth
                                         ,pc.EffectiveDate AT TIME ZONE 'UTC' AS EffectiveDate
                                         ,pc.EffectiveDateNorth AT TIME ZONE 'UTC' AS EffectiveDateNorth
                                         ,pc.LoaiTien
                                         ,pc.LoaiTienNorth
                                        FROM cal.ProductDetail pd
                                        LEFT JOIN cal.ProductCIFPrice pc
                                          ON pc.ProductId = pd.ProductId
                                        WHERE pd.IsDeleted = 0
                                        AND pd.ProductId = " + productId;

                    using (var multiResult = conn.QueryMultiple(sqlQuery, param))
                    {
                        IEnumerable<ProductCifprice> data = multiResult.Read<ProductCifprice>().ToList();
                        DataTable table = new DataTable();
                        using (var reader = ObjectReader.Create(data))
                        {
                            table.Load(reader);
                        }

                        return table;
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


        /// <summary>
        /// Thực hiện compare dữ liệu từ file excel & dữ liệu hiện tại trong DB
        /// </summary>
        /// <param name="dtbNew">Dữ liệu từ file excel</param>
        /// <returns>Danh sách các product có thay đổi giá CIF</returns>
        public List<ProductCifprice> CompareData(DataTable dtbNew, ref string messageText)
        {
            List<ProductCifprice> lstRsl = new List<ProductCifprice>();
            try
            {
                DataTable dtbDataCurrent = this.GetCurrentCIFPriceAllProduct();
                foreach (DataRow rowNew in dtbNew.Rows)
                {
                    string productId = Convert.ToString(rowNew["Product_Id"]);
                    if (string.IsNullOrEmpty(productId))
                    {
                        continue;
                    }
                    string LoaiTienSouth = !string.IsNullOrEmpty(Convert.ToString(rowNew["LoaiTienMN"]))
                        ? Convert.ToString(rowNew["LoaiTienMN"]).ToUpper()
                        : null;
                    string LoaiTienNorth = !string.IsNullOrEmpty(Convert.ToString(rowNew["LoaiTienMB"]))
                        ? Convert.ToString(rowNew["LoaiTienMB"]).ToUpper()
                        : null;

                    string ngayMN = Convert.ToString(rowNew["NgayMN"]);
                    DateTime dtMN = new DateTime();
                    if (!string.IsNullOrEmpty(ngayMN))
                    {
                        DateTime.TryParseExact(ngayMN, "dd/MM/yy", CultureInfo.CurrentCulture, DateTimeStyles.None,
                            out dtMN);
                    }


                    string ngayMB = Convert.ToString(rowNew["NgayMB"]);
                    DateTime dtMB = new DateTime();
                    if (!string.IsNullOrEmpty(ngayMB))
                    {
                        DateTime.TryParseExact(ngayMB, "dd/MM/yy", CultureInfo.CurrentCulture, DateTimeStyles.None,
                            out dtMB);
                    }

                    var drCurrent = dtbDataCurrent.Select("ProductId = " + productId);
                    if (drCurrent.Length > 0)
                    {
                        var drCurrentSouth = dtbDataCurrent.Select("ProductId = " + productId + "AND IsDefault = 1");
                        var drCurrentNorth = dtbDataCurrent.Select("ProductId = " + productId + "AND IsDefaultNorth = 1");
                        decimal curentCIFPriceSouth = 0;
                        if (drCurrentSouth.Length > 0)
                        {
                            curentCIFPriceSouth = drCurrentSouth[0]["CIFSouth"] != DBNull.Value
                                ? Convert.ToDecimal(drCurrentSouth[0]["CIFSouth"])
                                : 0;
                        }

                        decimal curentCIFPriceNorth = 0;
                        if (drCurrentNorth.Length > 0)
                        {
                            curentCIFPriceNorth = drCurrentNorth[0]["CIFNorth"] != DBNull.Value
                                ? Convert.ToDecimal(drCurrentNorth[0]["CIFNorth"])
                                : 0;
                        }

                        int ProductId = Convert.ToInt32(drCurrent[0]["ProductId"]);

                        decimal newCIFPriceSouth = 0;
                        if (rowNew["GiaCIFMN"] != DBNull.Value)
                        {
                            newCIFPriceSouth = Convert.ToDecimal(rowNew["GiaCIFMN"]);
                        }

                        decimal newCIFPriceNorth = 0;
                        if (rowNew["GiaCIFMB"] != DBNull.Value)
                        {
                            newCIFPriceNorth = Convert.ToDecimal(rowNew["GiaCIFMB"]);
                        }

                        if ((newCIFPriceSouth != 0 && newCIFPriceSouth != curentCIFPriceSouth) ||
                            (newCIFPriceNorth != 0 && curentCIFPriceNorth != newCIFPriceNorth))
                        {
                            //kiểm tra trong danh sách đã có hay chưa, nếu chưa thì mới add vào danh sách để thêm mới
                            bool bolIsExistedSouth = false;

                            //nếu giá CIF miền Nam có thay đổi
                            if (newCIFPriceSouth != curentCIFPriceSouth)
                            {
                                if (lstRsl.Exists(x => x.Cifsouth == newCIFPriceSouth))
                                {
                                    bolIsExistedSouth = true;
                                }
                            }

                            //nếu giá CIF miền Bắc có thay đổi
                            bool bolIsExistedNorth = false;
                            if (newCIFPriceNorth != curentCIFPriceSouth)
                            {
                                if (lstRsl.Exists(x => x.Cifnorth == newCIFPriceNorth))
                                {
                                    bolIsExistedNorth = true;
                                }
                            }

                            // đã có Product trong table ProductDetail & chưa tồn tại trong danh sách chuẩn bị để thêm mới
                            if ((!bolIsExistedSouth || !bolIsExistedNorth) && ProductId > 0)
                            {
                                ProductCifprice cifprice = new ProductCifprice();
                                cifprice.ProductId = ProductId;
                                cifprice.IsDefault = (newCIFPriceSouth != 0 && newCIFPriceSouth != curentCIFPriceSouth)
                                    ? 1
                                    : 0;
                                cifprice.IsDefaultNorth = (newCIFPriceNorth != 0 && curentCIFPriceNorth != newCIFPriceNorth)
                                    ? 1
                                    : 0;

                                cifprice.Cifsouth = newCIFPriceSouth;
                                cifprice.LoaiTien = LoaiTienSouth;
                                if (!string.IsNullOrEmpty(ngayMN))
                                    cifprice.EffectiveDate = this.ChangeHour(dtMN);

                                cifprice.Cifnorth = newCIFPriceNorth;
                                cifprice.LoaiTienNorth = LoaiTienNorth;
                                if (!string.IsNullOrEmpty(ngayMB))
                                    cifprice.EffectiveDateNorth = this.ChangeHour(dtMB);
                                lstRsl.Add(cifprice);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                messageText = e.Message;
            }

            return lstRsl;
        }

        private DateTime ChangeHour(DateTime dt)
        {
            DateTime rsl = new DateTime(dt.Year, dt.Month, dt.Day, DateTime.Now.Hour, DateTime.Now.Minute,
                DateTime.Now.Second);
            return rsl;
        }

        public int UpdateData(List<ProductCifprice> lstCifprices, int currentUserId, ref string messageText)
        {
            int count = 0;
            try
            {
                using (var db = new DBEntities())
                {
                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (ProductCifprice cifpriceNew in lstCifprices)
                            {
                                // lấy ra danh sách giá CIF hiện tại của sản phẩm
                                var lstCIFPrice = db.ProductCifprice.Where(x => x.ProductId == cifpriceNew.ProductId);
                                bool bolHasChangeDefaultSouth = lstCifprices.Any(x =>
                                    x.ProductId == cifpriceNew.ProductId && x.IsDefault == 1);
                                bool bolHasChangeDefaultNorth = lstCifprices.Any(x =>
                                    x.ProductId == cifpriceNew.ProductId && x.IsDefaultNorth == 1);
                                // xử lý bỏ check là giá hiện hành
                                foreach (Data.DBEntities.ProductCifprice cifprice in lstCIFPrice)
                                {
                                    if (cifprice.IsDefault == 1 && bolHasChangeDefaultSouth)
                                    {
                                        cifprice.IsDefault = 0;
                                    }

                                    if (cifprice.IsDefaultNorth == 1 && bolHasChangeDefaultNorth)
                                    {
                                        cifprice.IsDefaultNorth = 0;
                                    }

                                    cifprice.ModifyDate = DateTimeOffset.Now;
                                    db.Entry(cifprice).State = EntityState.Modified;
                                }

                                // lấy ra thông tin của sản phẩm
                                var productDetail =
                                    db.ProductDetail.FirstOrDefault(x => x.ProductId == cifpriceNew.ProductId);
                                if (productDetail != null)
                                {
                                    var currentSouth = lstCifprices.FirstOrDefault(x => x.IsDefault == 1 && x.ProductId == cifpriceNew.ProductId);
                                    if (cifpriceNew.Cifsouth != 0 && currentSouth != null && currentSouth.ProductId > 0)
                                    {
                                        productDetail.GiaCif = cifpriceNew.Cifsouth;
                                        productDetail.LoaiTien = cifpriceNew.LoaiTien;
                                    }

                                    var currentNorth = lstCifprices.Single(x => x.IsDefaultNorth == 1 && x.ProductId == cifpriceNew.ProductId);
                                    if (cifpriceNew.Cifnorth != 0 && currentNorth != null && currentNorth.ProductId > 0)
                                    {
                                        productDetail.GiaCifnorth = cifpriceNew.Cifnorth;
                                        productDetail.LoaiTienNorth = cifpriceNew.LoaiTienNorth;
                                    }

                                    db.Entry(productDetail).State = EntityState.Modified;
                                }

                                cifpriceNew.CreatedBy = currentUserId;
                                cifpriceNew.IsImport = true;
                                cifpriceNew.CreatedDate = DateTimeOffset.Now;
                                cifpriceNew.EffectiveDate = cifpriceNew.EffectiveDate;
                                cifpriceNew.EffectiveDateNorth = cifpriceNew.EffectiveDateNorth;
                                cifpriceNew.SortOrder = db.ProductCifprice.Where(y => y.ProductId == cifpriceNew.ProductId).DefaultIfEmpty().Max(x => x.SortOrder) + 1;
                                db.ProductCifprice.Add(cifpriceNew);

                                count++;
                            }

                            db.SaveChanges();
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            messageText = ex.Message;
                            trans.Rollback();
                            return -1;
                        }
                    }

                    return count;
                }
            }
            catch (Exception ex)
            {
                this.AddMessage(MessageCode.MSGCODE_001, ex.Message);
                Logger.Error(GetType(), ex);
                return -1;
            }
        }
    }
}