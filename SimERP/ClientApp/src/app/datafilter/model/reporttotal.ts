export class ReportTotal {
  id = 0;
  PhanLoai: string;
  NamNhap: number;
  NgayNhap: Date;
  ChiCucHaiQuan: string;
  CangXuatNhap: string;
  TenLoHang: string;
  MaDoanhNghiep: string;
  DoanhNghiepXuatNhap: string;
  HsCode: string;
  ChungLoaiHangHoaXuatNhap: string;
  DonViDoiTac: string;
  NuocXuatXu: string;
  Dvt: string;
  Luong: number;
  DonGia: number;
  TriGia: number;
  NgoaiTeThanhToan: string;
  TyGiaVnd: number;
  TriGiaVnd: number;
  TyGiaUsd: number;
  TriGiaUsd: number;
  DieuKienGiaoHang: string;
  DieuKienThanhToan: string;
  Tsxnk: string;
  ThueXnk: number;
  Tsttdb: number;
  ThueTtdb: number;
  Tsvat: number;
  ThueVat: number;
  PhuThu: number;
  MienThue: number;
  PhuongTienVanTai: string;
  TenPhuongTienVanTai: string;
  NuocXuatKhau: string;
  NuocNhapKhau: string;
  CangNuocNgoai: string;
  PhanLoaiTrangThai: string;
  SoToKhai: string;
  Selected = true;
  SearchString: string;
  SearchDoanhNghiepXuatNhap: string;
  SearchDonViDoiTac: string;
  SearchNuocXuatXu: string;


  // custom properties
  SoLuongQuyDoi?: number;
  DonGiaUSD?: number;
  IsFilter = 0; // dùng để tô màu khi filter
}
