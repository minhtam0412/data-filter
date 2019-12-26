export class Sales {
    Id: number;
    CusId: number
    ProductId: number;
    ProductCode: string;
    ProductName: string;
    GiaNgoaiTe?: number;
    LoaiNgoaiTe: string = "USD";
    GiaDong?: number;
    ThueVat?: number;
    ThoiGianBan?: Date;
    CreatedDate?: Date;
    CreatedBy?: number;
    ModifyDate?: Date;
    ModifyBy?: number;
    IsActive?: boolean;
    IsDelete: boolean;
    SortOrder?: number;
    SearchString: string;
    Notes: string;
    HanTt?: number;
    GiaId: string;
    PayType: number = 1;
}