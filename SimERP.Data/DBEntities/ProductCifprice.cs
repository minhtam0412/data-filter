using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class ProductCifprice
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public decimal? Cifnorth { get; set; }
        public string LoaiTienNorth { get; set; }
        public DateTimeOffset? EffectiveDateNorth { get; set; }
        public decimal? Cifsouth { get; set; }
        public string LoaiTien { get; set; }
        public DateTimeOffset? EffectiveDate { get; set; }
        public int? IsDefault { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public int IsActive { get; set; }
        public int IsDelete { get; set; }
        public int? SortOrder { get; set; }
        public string SearchString { get; set; }
        public string Notes { get; set; }
        public bool IsImport { get; set; }
        public int IsDefaultNorth { get; set; }
    }
}
