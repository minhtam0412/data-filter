using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class ShippedPrice
    {
        public int Id { get; set; }
        public string MaDoGiaVc { get; set; }
        public string DienGiai { get; set; }
        public double? GiaVc { get; set; }
        public string GhiChu { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string SearchString { get; set; }
        public int? SortOrder { get; set; }
        public int? StoreId { get; set; }
    }
}
