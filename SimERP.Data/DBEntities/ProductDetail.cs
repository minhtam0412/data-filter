using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class ProductDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string Spktname { get; set; }
        public string ProductName { get; set; }
        public string ProductNamePlan { get; set; }
        public string ProductNameTkhq { get; set; }
        public string ProductNameFull { get; set; }
        public double? IdGroup { get; set; }
        public string Description { get; set; }
        public double? IdUnit { get; set; }
        public string Unit { get; set; }
        public double? Vatproduct { get; set; }
        public string SupplierCode { get; set; }
        public string Supplier { get; set; }
        public string Producer { get; set; }
        public string National { get; set; }
        public string Level { get; set; }
        public string Application { get; set; }
        public string Pack { get; set; }
        public decimal? ConvertKg { get; set; }
        public decimal? GiaCifnorth { get; set; }
        public decimal? GiaCif { get; set; }
        public string LoaiTien { get; set; }
        public string ImportCode { get; set; }
        public double? ImportTax { get; set; }
        public double? CpveKhoLh { get; set; }
        public double? CpveKhoBn { get; set; }
        public string Group { get; set; }
        public string Salesman { get; set; }
        public float? LuongTonB { get; set; }
        public float? LuongTon { get; set; }
        public int? Leadtime { get; set; }
        public string CtyPhuTrach { get; set; }
        public int? TgTonKhoTb { get; set; }
        public string GhiChuTonKho { get; set; }
        public int? ViewNo { get; set; }
        public short? ThutuxemB { get; set; }
        public short? Thutuxem { get; set; }
        public short? FullFcl { get; set; }
        public short? KgPallet { get; set; }
        public short? Etaetd { get; set; }
        public string CertNo { get; set; }
        public DateTime? CertDate { get; set; }
        public DateTime? CertEnd { get; set; }
        public string Licenses { get; set; }
        public string Remark { get; set; }
        public int? EmployeeId { get; set; }
        public string SearchString { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public int? IsDeleted { get; set; }
        public int? IsActive { get; set; }
        public double? GiaMin { get; set; }
        public int? IdKhmin { get; set; }
        public DateTimeOffset? NgayGiaMin { get; set; }
        public double? GiaMax { get; set; }
        public int? IdKhmax { get; set; }
        public DateTimeOffset? NgayGiaMax { get; set; }
        public decimal? GiaTb { get; set; }
        public string KhMinName { get; set; }
        public string KhMaxName { get; set; }
        public string LoaiTienNorth { get; set; }
    }
}
