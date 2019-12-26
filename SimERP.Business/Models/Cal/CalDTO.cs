using SimERP.Data.DBEntities;
using System.Collections.Generic;

namespace SimERP.Business.Models.Cal
{
    // DTO load thông tin sản phẩm lên combobox
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductNamePlan { get; set; }
    }

    // DTO load thông tin khách hàng lên combobox
    public class CustomerDTO
    {
        public int CusId { get; set; }
        public string CustomerCode { get; set; }
        public string Namecus { get; set; }
        public string Fullnamecus { get; set; }
    }
}