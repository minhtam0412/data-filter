using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class ReportColumnView
    {
        public string Id { get; set; }
        public int UserId { get; set; }
        public string ColumnCode { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public int ViewType { get; set; }
        public string ColumnName { get; set; }
    }
}
