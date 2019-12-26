using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class CitiMart
    {
        public int Id { get; set; }
        public int? CusId { get; set; }
        public string Nhom { get; set; }
        public string NgMua { get; set; }
        public string TenTat { get; set; }
        public string DiaChiGiao { get; set; }
        public string MaKhuVuc { get; set; }
        public string DiaChi { get; set; }
        public string TenDvi { get; set; }
        public string Msthue { get; set; }
        public string DchiEmail { get; set; }
        public bool? IsActive { get; set; }
    }
}
