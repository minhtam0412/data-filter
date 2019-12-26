using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class Sales
    {
        public int Id { get; set; }
        public string GiaId { get; set; }
        public int CusId { get; set; }
        public int ProductId { get; set; }
        public int? PayType { get; set; }
        public double? GiaNgoaiTe { get; set; }
        public string LoaiNgoaiTe { get; set; }
        public double? GiaDong { get; set; }
        public double? ThueVat { get; set; }
        public DateTimeOffset? ThoiGianBan { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int? SortOrder { get; set; }
        public string SearchString { get; set; }
        public string Notes { get; set; }
        public int? HanTt { get; set; }
    }
}
