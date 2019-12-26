using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class AreaList
    {
        public int Id { get; set; }
        public string MaKhuVuc { get; set; }
        public string TenKhuVuc { get; set; }
        public string GhiChu { get; set; }
        public string SearchString { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public int? SortOrder { get; set; }
    }
}
