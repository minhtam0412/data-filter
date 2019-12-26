using AutoMapper;
using SimERP.Business.Models.MasterData.ListDTO;
using SaleInvoice = SimERP.Business.Models.Voucher.Sale.SaleInvoice;

namespace SimERP.Business.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Data.DBEntities.SaleInvoice, SaleInvoice>();
            CreateMap<Data.DBEntities.ProductDetail, ProductDetail>();
        }
    }
}