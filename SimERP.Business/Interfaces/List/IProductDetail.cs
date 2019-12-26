using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Data;
using System.Collections.Generic;
using ProductDetail = SimERP.Business.Models.MasterData.ListDTO.ProductDetail;

namespace SimERP.Business.Interfaces.List
{
    public interface IProductDetail : IRepository<ProductDetail>
    {
        List<ProductDetail> GetData(ReqListSearch reqListSearch);

        bool Save(ProductDetail rowData, bool isNew);

        bool DeleteProductDetail(int id);

        bool UpdateSortOrder(int upID, int downID);

        Data.DBEntities.ProductDetail GetInfo(ReqListSearch reqListSearch);
    }
}