export class PriceSpreadsheets {
    Id: number;
    MaHh: string;
    TenHh: string;
    ThueNk?: number;
    GiaCif?: number;
    LoaiTien: string;
    ChenhLechTyGia: number;
    TyGia?: number;
    CpveKho?: number;
    CpbocXep?: number;
    CpluuKho?: number;
    TgluuKhoTb?: number;
    CpluuKhoTotal: number;
    GiaVonTaiKho?: number;
    MaKh: string;
    TenKh: string;
    MaKhuVuc: string;
    DiaChiGh: string;
    KhuVucGh: string;
    SoLuong?: number = 1000;
    Cpgiao?: number;
    GiaBanHienHanh?: number;
    GiaBanDuKien?: number = 0;
    CpquanLyTt?: number;
    CpquanLyTtTotal?: number;
    CpquanLyGt?: number;
    CpquanLyGtTotal?: number = 0;
    CptaiChinh?: number;
    CptaiChinhTotal?: number = 0;
    GiaVonGiaoToiKh?: number;
    HanThanhToan?: number;
    LoiNhuanDuKien?: number;
    IsActive?: number;
    IsDelete?: number;
    CreatedBy?: number;
    CreatedDate?: Date;
    ModifyBy?: number;
    ModifyDate?: Date;
    IdColorGiaCif: number;
    IdColorTyGia: number;
    IdColorCpveKho: number;
    IdColorCpluuKho: number;
    IdColorCpbocXep: number;
    IdColorTgluuKhoTb: number;
    IdColorSoLuong: number;
    IdColorCpgiao: number;
    IdColorCpquanLyTt: number;
    IdColorCpquanLyGt: number;
    IdColorCptaiChinh: number;
    IdColorCpquanLyGtTotal: number;
    IdColorGiaBanDuKien: number;
    StoreId?: number;
}