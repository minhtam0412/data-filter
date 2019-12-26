using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimERP.Business;
using SimERP.Business.Businesses.Cal;
using SimERP.Business.Interfaces.Cal;
using SimERP.Business.Models.Cal;
using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Business.Utils;
using SimERP.Data.DBEntities;

namespace SimERP.Controllers
{
    public class CalController : BaseController
    {
        #region Variables

        private IHttpContextAccessor httpContextAccessor { get; }
        private ICachedData cachedDataBO;

        #endregion Variables

        #region Constructor

        public CalController(IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(httpContextAccessor,
            mapper)
        {
            this.ControllerName = "Cal";
            this.httpContextAccessor = httpContextAccessor;
            this.cachedDataBO = this.cachedDataBO ?? new CachedBO();
        }

        #endregion

        #region Get Cache Data
        [Authorize]
        [HttpPost]
        [Route("api/cal/productcache")]
        public ResponeResult GetProductCache([FromBody] ReqListSearch reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = this.cachedDataBO.GetProductCache();
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.cachedDataBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, cachedDataBO.getMsgCode(),
                        cachedDataBO.GetMessage(this.cachedDataBO.getMsgCode(), this.LangID));

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

        [Authorize]
        [HttpPost]
        [Route("api/cal/customercache")]
        public ResponeResult GetCustomerCache([FromBody] ReqListSearch reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = this.cachedDataBO.GetCustomerCache();
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.cachedDataBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, cachedDataBO.getMsgCode(),
                        cachedDataBO.GetMessage(this.cachedDataBO.getMsgCode(), this.LangID));

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

        [Authorize]
        [HttpPost]
        [Route("api/cal/arealistcache")]
        public ResponeResult GetCustomerDeliveryAddress([FromBody] ReqListSearch reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                reqData.SearchString = ReplaceUnicode(reqData.SearchString);
                var dataResult = this.cachedDataBO.GetAreaListCache(Convert.ToInt32(reqData.SearchString));
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.cachedDataBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, cachedDataBO.getMsgCode(),
                        cachedDataBO.GetMessage(this.cachedDataBO.getMsgCode(), this.LangID));

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

        [Authorize]
        [HttpPost]
        [Route("api/cal/amountdefine")]
        public ResponeResult GetAmountDefine([FromBody] ReqListSearch reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = this.cachedDataBO.GetAmountDefine();
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.cachedDataBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, cachedDataBO.getMsgCode(),
                        cachedDataBO.GetMessage(this.cachedDataBO.getMsgCode(), this.LangID));

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

        [Authorize]
        [HttpPost]
        [Route("api/cal/pricecalculator")]
        public ResponeResult GetPricecalculatorData([FromBody] ReqListSearch objReqListSearch)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListSearch.AuthenParams,
                    objReqListSearch.AuthenParams.ClientUserName, objReqListSearch.AuthenParams.ClientPassword,
                    objReqListSearch.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = cachedDataBO.GetData(this._session.UserID);
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.cachedDataBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, cachedDataBO.getMsgCode(),
                        cachedDataBO.GetMessage(this.cachedDataBO.getMsgCode(), this.LangID));

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

        [Authorize]
        [HttpPost]
        [Route("api/cal/saveshippedprice")]
        public ActionResult<ResponeResult> SaveShippedPrice([FromBody] ReqListAdd objReqListAdd)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListAdd.AuthenParams, objReqListAdd.AuthenParams.ClientUserName,
                    objReqListAdd.AuthenParams.ClientPassword, objReqListAdd.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                PriceSpreadsheets objData = JsonConvert.DeserializeObject<PriceSpreadsheets>(objReqListAdd.RowData.ToString());
                if (objReqListAdd.IsNew)
                {
                    objData.CreatedBy = this._session.UserID;
                    objData.CreatedDate = DateTimeOffset.Now;
                }
                else
                {
                    objData.ModifyBy = this._session.UserID;
                    objData.ModifyDate = DateTimeOffset.Now; ;
                }

                var dataResult = cachedDataBO.Save(objData, objReqListAdd.IsNew);
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, cachedDataBO.getMsgCode(),
                        cachedDataBO.GetMessage(this.cachedDataBO.getMsgCode(), this.LangID));

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

        [Authorize]
        [HttpPost]
        [Route("api/cal/deleteshippedprice")]
        public ActionResult<ResponeResult> DeleteShippedPrice([FromBody] ReqListDelete objReqListDelete)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListDelete.AuthenParams,
                    objReqListDelete.AuthenParams.ClientUserName, objReqListDelete.AuthenParams.ClientPassword,
                    objReqListDelete.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult = this.cachedDataBO.Delete(Convert.ToInt32(objReqListDelete.ID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, this.cachedDataBO.getMsgCode(),
                        this.cachedDataBO.GetMessage(this.cachedDataBO.getMsgCode(), this.LangID));

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
        #endregion
    }
}