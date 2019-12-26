using SimERP.Business.Models.RequestData;
using SimERP.Data.DBEntities;
using System;
using System.Collections.Generic;

namespace SimERP.Business.Models.MasterData.ListDTO
{
    #region Basic Param

    [Serializable]
    public class ReqListSearch
    {
        public AuthenParams AuthenParams { get; set; } = new AuthenParams();
        public string SearchString { get; set; }
        public bool? IsActive { get; set; } = null;
        public int StartRow { get; set; } = 0;
        public int MaxRow { get; set; } = 20;
        public Dictionary<string, dynamic> AddtionParams { get; set; }
    }

    public class ReqListAdd
    {
        public AuthenParams AuthenParams { get; set; } = new AuthenParams();
        public dynamic RowData { get; set; }
        public bool IsNew { get; set; }
    }

    public class ReqListDelete
    {
        public AuthenParams AuthenParams { get; set; } = new AuthenParams();
        public dynamic ID { get; set; }
    }

    public class ReqListUpdateSortOrder
    {
        public AuthenParams AuthenParams { get; set; } = new AuthenParams();
        public dynamic UpID { get; set; }
        public dynamic DownID { get; set; }
    }

    #endregion Basic Param

    public class Tax : Data.DBEntities.Tax
    {
        public string UserName { get; set; }
    }

    public class Unit : Data.DBEntities.Unit
    {
        public string UserName { get; set; }
    }

    public class PageList : Data.DBEntities.Page
    {
        public string ModuleName { get; set; }
        public List<Function> lstFunction { get; set; }
    }

    public class Function : Data.DBEntities.Function
    {
        public bool IsCheck { get; set; }
        public int PermissionID { get; set; }
        public bool IsRole { get; set; }
    }

    public class CustomerSale : Data.DBEntities.CustomerSale
    {
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }

    public class CustomerProduct : Data.DBEntities.CustomerProduct
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public string SaleName { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
    }

    public class Customer : Data.DBEntities.Customer
    {
        public List<AttachFile> ListAttachFile { get; set; }
        public List<AttachFile> ListAttachFileDelete { get; set; }
        public string CreateName { get; set; }
        public List<CustomerProduct> objProduct { get; set; }
        public List<CustomerSale> objSaler { get; set; }
        public List<CustomerCommission> objCommission { get; set; }
        public List<CustomerDelivery> objDelivery { get; set; }
    }

    public class Role : Data.DBEntities.Role
    {
        public string CreatedName { get; set; }
        public string LstPermission { get; set; }
    }

    public class RoleList
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool isCheck { get; set; }
    }

    public partial class Product : Data.DBEntities.Product
    {
        public List<AttachFile> ListAttachFile { get; set; }
        public List<AttachFile> ListAttachFileDelete { get; set; }
        public string UnitName { get; set; }
        public string PackageUnitName { get; set; }
        public string ProductCategoryName { get; set; }
        public string CountryName { get; set; }
        public string VendorName { get; set; }
    }

    public class AttachFile : Data.DBEntities.AttachFile
    {
        public bool IsChange { get; set; } = true; //default has change
        public bool IsNew { get; set; }
    }

    public class PermissionUser
    {
        public string UserName { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int PermissionId { get; set; }
        public int PageId { get; set; }
        public string PageName { get; set; }
        public string FunctionId { get; set; }
        public string ControllerName { get; set; }
        public int IsFromRole { get; set; }
    }

    public class VendorProduct : SimERP.Data.DBEntities.VendorProduct
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public string CountryName { get; set; }
        public string ProductCategoryName { get; set; }
    }

    public class ProductDetail : SimERP.Data.DBEntities.ProductDetail
    {
        public List<ProductCifprice> ListProductCIFPrice { get; set; }
        public List<ProductCifprice> ListProductCIFPricsDel { get; set; }
    }

    public class ProductCifprice : SimERP.Data.DBEntities.ProductCifprice
    {
        public decimal TyGia { get; set; }
        public decimal TyGiaNorth { get; set; }
        public string ProductCode { get; set; }
    }

    public class CustomerDetail : SimERP.Data.DBEntities.CustomerDetail
    {
        public List<CustomerDeliveryAddress> lstCustomerDeliveryAddress { get; set; }
        public List<Sales> lstSales { get; set; }
    }

    public class CustomerDeliveryAddress : SimERP.Data.DBEntities.CustomerDeliveryAddress
    {
        public string TenKhuVuc { get; set; }
    }

    public class Sales : SimERP.Data.DBEntities.Sales
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
    }

    public class ShippedPrice : SimERP.Data.DBEntities.ShippedPrice
    {
        public string StoreName { get; set; }
    }

    public class ReportTotal : SimERP.Data.DBEntities.ReportTotal
    {
        public decimal SoLuongQuyDoi { get; set; }
        public decimal DonGiaUSD { get; set; }
        public bool IsFilter { get; set; }
    }
}