export class CustomerDeliveryAddress {
    Id: number;
    CusId: number;
    IdKhuVuc?: number;
    DeliveryAdr: string = "";
    MaKhuVuc: string = "";
    TenKhuVuc: string = "";
    CreatedDate?: Date;
    CreatedBy?: number;
    ModifyDate?: Date;
    ModifyBy?: number;
    IsActive?: boolean = true;
    IsDelete?: boolean = false;
    IsDefault?: boolean = false;
    SortOrder?: number;
    SearchString: string;
    Notes: string;
}