using System;
using System.Collections.Generic;

namespace SimERP.Data.DBEntities
{
    public partial class CustomerDetail
    {
        public int CusId { get; set; }
        public string CustomerCode { get; set; }
        public string NameCus { get; set; }
        public string NameCusFs { get; set; }
        public string TenTat { get; set; }
        public string FullnameCus { get; set; }
        public string NguoiMua { get; set; }
        public string Khktoan { get; set; }
        public string Adress { get; set; }
        public string DeliveryAdr { get; set; }
        public int? IdKhuVuc { get; set; }
        public string MaKhuVuc { get; set; }
        public string KhuVuc { get; set; }
        public string DirectorName { get; set; }
        public string ContactName { get; set; }
        public string NgLienhe { get; set; }
        public string LienHeCvu { get; set; }
        public string NguoiKy { get; set; }
        public string DtlienHe { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string TaxCode { get; set; }
        public short? HanTt { get; set; }
        public string SoTaiKhoan { get; set; }
        public string Document { get; set; }
        public string Giogiao { get; set; }
        public string Type { get; set; }
        public int? Nvid { get; set; }
        public string Manager { get; set; }
        public string Employee { get; set; }
        public bool? Na { get; set; }
        public bool? DangBan { get; set; }
        public string NoiGuiCongno { get; set; }
        public string CccongNo { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTimeOffset? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public string SearchString { get; set; }
        public int? SortOrder { get; set; }
        public int? PaymentExpired { get; set; }
    }
}
