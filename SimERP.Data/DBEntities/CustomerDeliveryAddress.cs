using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class CustomerDeliveryAddress
    {
        public int Id { get; set; }
        public int CusId { get; set; }
        public int? IdKhuVuc { get; set; }
        public string DeliveryAdr { get; set; }
        public string MaKhuVuc { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsDefault { get; set; }
        public int? SortOrder { get; set; }
        public string SearchString { get; set; }
        public string Notes { get; set; }
    }
}
