using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class AggregateCosts
    {
        public int Id { get; set; }
        public string LoaiCp { get; set; }
        public double? ChiPhi { get; set; }
        public string DienGiai { get; set; }
        public DateTimeOffset? ApplyDate { get; set; }
        public string Notes { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string SearchString { get; set; }
        public int? SortOrder { get; set; }
    }
}
