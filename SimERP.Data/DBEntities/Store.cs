using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class Store
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
