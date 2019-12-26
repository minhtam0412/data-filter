using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class PriceSpreadsheets
    {
        public int Id { get; set; }
        public string MaHh { get; set; }
        public string TenHh { get; set; }
        public double? ThueNk { get; set; }
        public double? GiaCif { get; set; }
        public string LoaiTien { get; set; }
        public double? TyGia { get; set; }
        public double? CpveKho { get; set; }
        public double? CpluuKho { get; set; }
        public double? CpbocXep { get; set; }
        public double? TgluuKhoTb { get; set; }
        public double? GiaVonTaiKho { get; set; }
        public string MaKh { get; set; }
        public string TenKh { get; set; }
        public string MaKhuVuc { get; set; }
        public string DiaChiGh { get; set; }
        public string KhuVucGh { get; set; }
        public double? SoLuong { get; set; }
        public double? Cpgiao { get; set; }
        public double? GiaBanDuKien { get; set; }
        public double? CpquanLyTt { get; set; }
        public double? CpquanLyGt { get; set; }
        public double? CptaiChinh { get; set; }
        public double? GiaVonGiaoToiKh { get; set; }
        public double? HanThanhToan { get; set; }
        public double? LoiNhuanDuKien { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? ModifyBy { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public int IdColorGiaCif { get; set; }
        public int IdColorTyGia { get; set; }
        public int IdColorCpveKho { get; set; }
        public int IdColorCpluuKho { get; set; }
        public int IdColorCpbocXep { get; set; }
        public int IdColorTgluuKhoTb { get; set; }
        public int IdColorSoLuong { get; set; }
        public int IdColorCpgiao { get; set; }
        public int IdColorCpquanLyTt { get; set; }
        public int IdColorCpquanLyGt { get; set; }
        public int IdColorCptaiChinh { get; set; }
        public int IdColorCpquanLyGtTotal { get; set; }
        public int IdColorGiaBanDuKien { get; set; }
        public double? ChenhLechTyGia { get; set; }
        public double? CpluuKhoTotal { get; set; }
        public double? GiaBanHienHanh { get; set; }
        public double? CpquanLyTtTotal { get; set; }
        public double? CpquanLyGtTotal { get; set; }
        public double? CptaiChinhTotal { get; set; }
        public int StoreId { get; set; }
    }
}
