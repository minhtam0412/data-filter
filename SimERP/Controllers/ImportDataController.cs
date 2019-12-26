using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using SimERP.Business;
using SimERP.Business.Businesses.List;
using SimERP.Business.Interfaces.List;
using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Business.Utils;
using SimERP.Utils;

namespace SimERP.Controllers
{
    public class ImportDataController : BaseController
    {
        private IHttpContextAccessor httpContextAccessor { get; }
        private IProductCifprice productCifpriceBO;

        public ImportDataController(IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(
            httpContextAccessor, mapper)
        {
            this.ControllerName = "ImportData";
            this.httpContextAccessor = httpContextAccessor;

            this.productCifpriceBO = this.productCifpriceBO ?? new ProductCifpriceBO();
        }

        [Authorize]
        [HttpPost]
        [Route("api/import/cifprice")]
        public ResponeResult ImportCifPrice()
        {
            ReqListSearch objReqListSearch = new ReqListSearch();
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListSearch.AuthenParams,
                    objReqListSearch.AuthenParams.ClientUserName, objReqListSearch.AuthenParams.ClientPassword,
                    objReqListSearch.AuthenParams.Sign);
                string messageText = string.Empty;
                var folderAccessName = Path.Combine("Upload", "Product");
                //var folderAccessName = System.Configuration.ConfigurationManager.AppSettings["DataPath"];
                folderAccessName = Path.Combine(Directory.GetCurrentDirectory(), folderAccessName);
                var dataFileAccessNameCIFPrice =
                    System.Configuration.ConfigurationManager.AppSettings["DataAccessFileNameCIFPrice"];
                var dtbCIFPrice = Global.ProcessFile(folderAccessName, dataFileAccessNameCIFPrice, "GiaCIF", ref messageText);
                bool rslUpdate = productCifpriceBO.CompareAllData(ref dtbCIFPrice, ref messageText);

                if (rslUpdate)
                {
                    repData.MessageText = messageText;
                    repData.RepData = rslUpdate;
                }
                else
                {
                    if (string.IsNullOrEmpty(messageText))
                    {
                        this.AddResponeError(ref repData, productCifpriceBO.getMsgCode(),
                            productCifpriceBO.GetMessage(this.productCifpriceBO.getMsgCode(), this.LangID));
                    }
                    else
                    {
                        repData.IsOk = rslUpdate;
                        repData.MessageText = messageText;
                    }
                }


                return repData;
            }
            catch (Exception ex)
            {
                this.responeResult = this.CreateResponeResultError(MsgCodeConst.Msg_RequestDataInvalid,
                    MsgCodeConst.Msg_RequestDataInvalidText, ex.Message, null);
                Logger.Error("EXCEPTION-CALL API", ex);
                return responeResult;
            }
        }
    }
}