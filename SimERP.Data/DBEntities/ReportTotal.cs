using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class ReportTotal
    {
        public int id { get; set; }
        public string PhanLoai { get; set; }
        public int? NamNhap { get; set; }
        public DateTime? NgayNhap { get; set; }
        public string ChiCucHaiQuan { get; set; }
        public string CangXuatNhap { get; set; }
        public string TenLoHang { get; set; }
        public string MaDoanhNghiep { get; set; }
        public string DoanhNghiepXuatNhap { get; set; }
        public string HsCode { get; set; }
        public string ChungLoaiHangHoaXuatNhap { get; set; }
        public string DonViDoiTac { get; set; }
        public string NuocXuatXu { get; set; }
        public string Dvt { get; set; }
        public double? Luong { get; set; }
        public double? DonGia { get; set; }
        public double? TriGia { get; set; }
        public string NgoaiTeThanhToan { get; set; }
        public double? TyGiaVnd { get; set; }
        public double? TriGiaVnd { get; set; }
        public double? TyGiaUsd { get; set; }
        public double? TriGiaUsd { get; set; }
        public string DieuKienGiaoHang { get; set; }
        public string DieuKienThanhToan { get; set; }
        public string Tsxnk { get; set; }
        public double? ThueXnk { get; set; }
        public double? Tsttdb { get; set; }
        public double? ThueTtdb { get; set; }
        public double? Tsvat { get; set; }
        public double? ThueVat { get; set; }
        public double? PhuThu { get; set; }
        public double? MienThue { get; set; }
        public string PhuongTienVanTai { get; set; }
        public string TenPhuongTienVanTai { get; set; }
        public string NuocXuatKhau { get; set; }
        public string NuocNhapKhau { get; set; }
        public string CangNuocNgoai { get; set; }
        public string PhanLoaiTrangThai { get; set; }
        public string SoToKhai { get; set; }
        public string SearchString { get; set; }
        public string SearchDoanhNghiepXuatNhap { get; set; }
        public string SearchDonViDoiTac { get; set; }
        public string SearchNuocXuatXu { get; set; }
    }
}
