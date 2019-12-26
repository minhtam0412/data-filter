export class ProductCIFPrice {
  Id = 0;
  ProductId = 0;
  Cifnorth: number;
  Cifsouth: number;
  LoaiTien: string;
  EffectiveDate: Date;
  IsDefault = 0;
  CreatedDate: Date;
  CreatedBy: number;
  ModifyDate: Date;
  ModifyBy: number;
  IsActive: number;
  IsDelete: number;
  SortOrder: number;
  SearchString: string;
  Notes: string;
  LoaiTienNorth: string;
  EffectiveDateNorth: Date;
  IsDefaultNorth = 0;

  // custom properties
  TyGia: number;
  TyGiaNorth: number;
}
