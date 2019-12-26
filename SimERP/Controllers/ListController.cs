using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimERP.Business;
using SimERP.Business.Models.MasterData.ListDTO;
using SimERP.Business.Utils;
using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using SimERP.Business.Interfaces.List;
using SimERP.Business.Models.System;
using SimERP.Data.DBEntities;
using SimERP.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Internal;
using SimERP.Business.Businesses.DataFilter;
using SimERP.Business.Businesses.List;
using SimERP.Business.Interfaces.DataFilter;
using AttachFile = SimERP.Business.Models.MasterData.ListDTO.AttachFile;
using Function = SimERP.Business.Models.MasterData.ListDTO.Function;
using ProductDetail = SimERP.Business.Models.MasterData.ListDTO.ProductDetail;
using ShippedPrice = SimERP.Data.DBEntities.ShippedPrice;
using Tax = SimERP.Business.Models.MasterData.ListDTO.Tax;

namespace SimERP.Controllers
{
    public partial class ListController : BaseController
    {
        #region Variables

        private IHttpContextAccessor httpContextAccessor { get; }
        private IPageList pageListBO;
        private IRoleList roleListBO;
        private ITax taxBO;
        private IAggregateCosts aggregatecostsBO;
        private IAreaList areaListBO;
        private IShippedPrice shippedpriceBO;
        private IProductDetail productDetaiBO;
        private IProductCifprice productCifpriceBO;
        private ICustomerDetail customerDetailBO;
        private IStore storeBO;
        private IReportColumnView reportColumnViewBO;
        private IReportTotal reportTotalBO;

        #endregion Variables

        #region Contructor

        public ListController(IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(httpContextAccessor,
            mapper)
        {
            this.ControllerName = "List";
            this.httpContextAccessor = httpContextAccessor;
            this.pageListBO = this.pageListBO ?? new PageListBO();
            this.roleListBO = this.roleListBO ?? new RoleListBO();
            this.taxBO = this.taxBO ?? new TaxBO();
            this.aggregatecostsBO = this.aggregatecostsBO ?? new AggregatecostsBO();
            this.areaListBO = this.areaListBO ?? new AreaListBO();
            this.shippedpriceBO = this.shippedpriceBO ?? new ShippedPriceBO();
            this.productDetaiBO = this.productDetaiBO ?? new ProductDetailBO();
            this.productCifpriceBO = this.productCifpriceBO ?? new ProductCifpriceBO();
            this.customerDetailBO = this.customerDetailBO ?? new CustomerDetailBO();
            this.storeBO = this.storeBO ?? new StoreBO();
            this.reportColumnViewBO = this.reportColumnViewBO ?? new ReportColumnViewBO();
            this.reportTotalBO = this.reportTotalBO ?? new ReportTotalBO();
        }

        #endregion Contructor

        #region PageList

        [Authorize]
        [HttpPost]
        [Route("api/list/pagelist")]
        public ResponeResult GetPageListData([FromBody] JObject reqData)
        {
            ReqListSearch reqSerach = new ReqListSearch();
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData, reqSerach.AuthenParams.ClientUserName,
                    reqSerach.AuthenParams.ClientPassword, "");
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = pageListBO.GetData(ReplaceUnicode(reqData["dataserach"]["SearchString"].ToString()),
                    reqData["moduleID"].ToString() == ""
                        ? (int?)null
                        : Convert.ToInt32(reqData["moduleID"].ToString()),
                    reqData["dataserach"]["IsActive"].ToString() == ""
                        ? (bool?)null
                        : Convert.ToBoolean(reqData["dataserach"]["IsActive"].ToString()),
                    Convert.ToInt32(reqData["dataserach"]["StartRow"].ToString()),
                    Convert.ToInt32(reqData["dataserach"]["MaxRow"].ToString()));
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.pageListBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, pageListBO.getMsgCode(),
                        pageListBO.GetMessage(this.pageListBO.getMsgCode(), this.LangID));

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
        [Route("api/list/savepagelist")]
        public ActionResult<ResponeResult> SavePageList([FromBody] ReqListAdd reqData)
        {
            try
            {
                int pageID = -1;
                string message = "";
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                if (reqData.IsNew)
                    reqData.RowData.CreatedDate = DateTimeOffset.Now;
                else
                    reqData.RowData.ModifyDate = DateTimeOffset.Now;

                var dataResult = pageListBO.Save(JsonConvert.DeserializeObject<Page>(reqData.RowData.ToString()),
                    reqData.IsNew, ref pageID);

                //---save list page function
                if (pageID != -1)
                {
                    if (!reqData.IsNew)
                    {
                        pageListBO.DeleteListPagePermission(pageID, ref message);
                    }

                    PageList pageList = JsonConvert.DeserializeObject<PageList>(reqData.RowData.ToString());
                    foreach (Function item in pageList.lstFunction)
                    {
                        if (item.IsCheck)
                        {
                            if (!pageListBO.checkIssuePermission(pageID, item.FunctionId))
                                dataResult = pageListBO.SaveListPageFunction(pageID, item.FunctionId);
                        }
                    }
                }

                if (dataResult)
                {
                    repData.RepData = dataResult;
                    repData.MessageText = message;
                }

                else
                    this.AddResponeError(ref repData, pageListBO.getMsgCode(),
                        pageListBO.GetMessage(this.pageListBO.getMsgCode(), this.LangID));

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
        [Route("api/list/deletepagelist")]
        public ActionResult<ResponeResult> DeletePageList([FromBody] ReqListDelete reqData)
        {
            try
            {
                string message = "";
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult = pageListBO.DeletePageList(Convert.ToInt32(reqData.ID), ref message);

                if (dataResult)
                {
                    repData.RepData = dataResult;
                    repData.MessageText = message;
                }
                else
                    this.AddResponeError(ref repData, pageListBO.getMsgCode(),
                        pageListBO.GetMessage(this.pageListBO.getMsgCode(), this.LangID));

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
        [Route("api/list/updateSortOrderPageList")]
        public ActionResult<ResponeResult> UpdateSortOrderPageList([FromBody] ReqListUpdateSortOrder reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult =
                    pageListBO.UpdateSortOrder(Convert.ToInt32(reqData.UpID), Convert.ToInt32(reqData.DownID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, pageListBO.getMsgCode(),
                        pageListBO.GetMessage(this.pageListBO.getMsgCode(), this.LangID));

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
        [Route("api/list/getlistmodule")]
        public ResponeResult GetListModule([FromBody] ReqListSearch reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = pageListBO.GetListModule();
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                }
                else
                    this.AddResponeError(ref repData, pageListBO.getMsgCode(),
                        pageListBO.GetMessage(this.pageListBO.getMsgCode(), this.LangID));

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
        [Route("api/list/getlistfunction")]
        public ResponeResult GetListFunction([FromBody] ReqListSearch reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = pageListBO.GetListFunction();
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                }
                else
                    this.AddResponeError(ref repData, pageListBO.getMsgCode(),
                        pageListBO.GetMessage(this.pageListBO.getMsgCode(), this.LangID));

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

        #region RoleList

        [Authorize]
        [HttpPost]
        [Route("api/list/rolelist")]
        public ResponeResult GetRoleListData([FromBody] ReqListSearch reqData)
        {
            ReqListSearch reqSerach = new ReqListSearch();
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = roleListBO.GetData(ReplaceUnicode(reqData.SearchString), reqData.IsActive,
                    reqData.StartRow, reqData.MaxRow);
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.roleListBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, roleListBO.getMsgCode(),
                        roleListBO.GetMessage(this.roleListBO.getMsgCode(), this.LangID));

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
        [Route("api/list/loadpagelistrole")]
        public ResponeResult LoadPageListRole([FromBody] JObject reqData)
        {
            ReqListSearch reqSerach = new ReqListSearch();
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData, reqSerach.AuthenParams.ClientUserName,
                    reqSerach.AuthenParams.ClientPassword, "");
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = roleListBO.LoadPageListRole(reqData["moduleID"].ToString() == ""
                    ? (int?)null
                    : Convert.ToInt32(reqData["moduleID"].ToString()));
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.roleListBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, roleListBO.getMsgCode(),
                        roleListBO.GetMessage(this.roleListBO.getMsgCode(), this.LangID));

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
        [Route("api/list/saverolelist")]
        public ActionResult<ResponeResult> SaveRoleList([FromBody] ReqListAdd reqData)
        {
            try
            {
                int roleId = -1;
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                if (reqData.IsNew)
                    reqData.RowData.CreatedDate = DateTimeOffset.Now;
                else
                    reqData.RowData.ModifyDate = DateTimeOffset.Now;

                var dataResult =
                    roleListBO.Save(JsonConvert.DeserializeObject<Data.DBEntities.Role>(reqData.RowData.ToString()),
                        reqData.IsNew, ref roleId);

                //---save list role permission
                if (roleId != -1)
                {
                    if (!reqData.IsNew)
                    {
                        roleListBO.DeleteListRolePermission(roleId);
                    }

                    Business.Models.MasterData.ListDTO.Role roleList =
                        JsonConvert.DeserializeObject<Business.Models.MasterData.ListDTO.Role>(
                            reqData.RowData.ToString());
                    string[] lstPermission = roleList.LstPermission.Split(';');
                    foreach (string item in lstPermission)
                    {
                        dataResult = roleListBO.SaveListRoleFunction(roleId, Convert.ToInt32(item));
                    }
                }

                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, roleListBO.getMsgCode(),
                        roleListBO.GetMessage(this.roleListBO.getMsgCode(), this.LangID));

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
        [Route("api/list/deleterolelist")]
        public ActionResult<ResponeResult> DeleteRoleList([FromBody] ReqListDelete reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult = roleListBO.Delete(Convert.ToInt32(reqData.ID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, roleListBO.getMsgCode(),
                        roleListBO.GetMessage(this.roleListBO.getMsgCode(), this.LangID));

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
        [Route("api/list/updateSortOrderRoleList")]
        public ActionResult<ResponeResult> UpdateSortOrderRoleList([FromBody] ReqListUpdateSortOrder reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult =
                    roleListBO.UpdateSortOrder(Convert.ToInt32(reqData.UpID), Convert.ToInt32(reqData.DownID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, roleListBO.getMsgCode(),
                        roleListBO.GetMessage(this.roleListBO.getMsgCode(), this.LangID));

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

        #region Tax

        [HttpPost]
        [Route("api/list/tax")]
        public ResponeResult GetTaxData([FromBody] ReqListSearch objReqListSearch)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListSearch.AuthenParams,
                    objReqListSearch.AuthenParams.ClientUserName, objReqListSearch.AuthenParams.ClientPassword,
                    objReqListSearch.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = taxBO.GetData(ReplaceUnicode(objReqListSearch.SearchString), objReqListSearch.StartRow,
                    objReqListSearch.MaxRow);
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.taxBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, taxBO.getMsgCode(),
                        taxBO.GetMessage(this.taxBO.getMsgCode(), this.LangID));

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

        [HttpPost]
        [Route("api/list/savetax")]
        public ActionResult<ResponeResult> SaveTax([FromBody] ReqListAdd objReqListAdd)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListAdd.AuthenParams, objReqListAdd.AuthenParams.ClientUserName,
                    objReqListAdd.AuthenParams.ClientPassword, objReqListAdd.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                Tax taxData = JsonConvert.DeserializeObject<Tax>(objReqListAdd.RowData.ToString());
                if (objReqListAdd.IsNew)
                {
                    taxData.CreatedBy = this._session.UserID;
                }
                else
                {
                    taxData.ModifyBy = this._session.UserID;
                }

                var dataResult = taxBO.Save(taxData, objReqListAdd.IsNew);
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, taxBO.getMsgCode(),
                        taxBO.GetMessage(this.taxBO.getMsgCode(), this.LangID));

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

        [HttpPost]
        [Route("api/list/deletetax")]
        public ActionResult<ResponeResult> DeleteTax([FromBody] ReqListDelete objReqListDelete)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListDelete.AuthenParams,
                    objReqListDelete.AuthenParams.ClientUserName, objReqListDelete.AuthenParams.ClientPassword,
                    objReqListDelete.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult = taxBO.DeleteTax(Convert.ToInt32(objReqListDelete.ID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, taxBO.getMsgCode(),
                        taxBO.GetMessage(this.taxBO.getMsgCode(), this.LangID));

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

        [HttpPost]
        [Route("api/list/updateSortOrderTax")]
        public ActionResult<ResponeResult> UpdateSortOrderTax([FromBody] ReqListUpdateSortOrder reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult = taxBO.UpdateSortOrder(Convert.ToInt32(reqData.UpID), Convert.ToInt32(reqData.DownID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, taxBO.getMsgCode(),
                        taxBO.GetMessage(this.taxBO.getMsgCode(), this.LangID));

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

        #endregion Tax

        #region Aggregatecosts

        [Authorize]
        [HttpPost]
        [Route("api/list/aggregatecosts")]
        public ResponeResult GetAggregatecostsData([FromBody] ReqListSearch objReqListSearch)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListSearch.AuthenParams,
                    objReqListSearch.AuthenParams.ClientUserName, objReqListSearch.AuthenParams.ClientPassword,
                    objReqListSearch.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = aggregatecostsBO.GetData(ReplaceUnicode(objReqListSearch.SearchString),
                    objReqListSearch.StartRow,
                    objReqListSearch.MaxRow);
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.aggregatecostsBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, aggregatecostsBO.getMsgCode(),
                        aggregatecostsBO.GetMessage(this.aggregatecostsBO.getMsgCode(), this.LangID));

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
        [Route("api/list/saveaggregatecosts")]
        public ActionResult<ResponeResult> SaveAggregatecosts([FromBody] ReqListAdd objReqListAdd)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListAdd.AuthenParams, objReqListAdd.AuthenParams.ClientUserName,
                    objReqListAdd.AuthenParams.ClientPassword, objReqListAdd.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                AggregateCosts objData =
                    JsonConvert.DeserializeObject<AggregateCosts>(objReqListAdd.RowData.ToString());
                if (objReqListAdd.IsNew)
                {
                    objData.CreatedBy = this._session.UserID;
                    objData.CreatedDate = DateTimeOffset.Now;
                }
                else
                {
                    objData.ModifyBy = this._session.UserID;
                    objData.ModifyDate = DateTimeOffset.Now;
                }

                var dataResult = aggregatecostsBO.Save(objData, objReqListAdd.IsNew);

                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, aggregatecostsBO.getMsgCode(),
                        aggregatecostsBO.GetMessage(this.aggregatecostsBO.getMsgCode(), this.LangID));

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
        [Route("api/list/deleteaggregatecosts")]
        public ActionResult<ResponeResult> DeleteAggregatecosts([FromBody] ReqListDelete objReqListDelete)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListDelete.AuthenParams,
                    objReqListDelete.AuthenParams.ClientUserName, objReqListDelete.AuthenParams.ClientPassword,
                    objReqListDelete.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult = aggregatecostsBO.Delete(Convert.ToInt32(objReqListDelete.ID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, aggregatecostsBO.getMsgCode(),
                        aggregatecostsBO.GetMessage(this.aggregatecostsBO.getMsgCode(), this.LangID));

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
        [Route("api/list/updateSortOrderAggregateCosts")]
        public ActionResult<ResponeResult> UpdateSortOrderAggregatecosts([FromBody] ReqListUpdateSortOrder reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult =
                    aggregatecostsBO.UpdateSortOrder(Convert.ToInt32(reqData.UpID), Convert.ToInt32(reqData.DownID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, aggregatecostsBO.getMsgCode(),
                        aggregatecostsBO.GetMessage(this.aggregatecostsBO.getMsgCode(), this.LangID));

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

        #region AreaList

        [Authorize]
        [HttpPost]
        [Route("api/list/arealist")]
        public ResponeResult GetAreaListData([FromBody] ReqListSearch objReqListSearch)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckAuthen();
                if (repData == null || !repData.IsOk)
                    return repData;

                objReqListSearch.SearchString = ReplaceUnicode(objReqListSearch.SearchString);
                var dataResult = this.areaListBO.GetData(objReqListSearch);
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.areaListBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, areaListBO.getMsgCode(),
                        areaListBO.GetMessage(this.areaListBO.getMsgCode(), this.LangID));

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
        [Route("api/list/savearealist")]
        public ActionResult<ResponeResult> SaveAreaList([FromBody] ReqListAdd objReqListAdd)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckAuthen();
                if (repData == null || !repData.IsOk)
                    return repData;

                AreaList rowData = JsonConvert.DeserializeObject<AreaList>(objReqListAdd.RowData.ToString());
                if (rowData != null)
                {
                    rowData.SearchString =
                        ReplaceUnicode(rowData.MaKhuVuc + " " + rowData.TenKhuVuc);
                    if (objReqListAdd.IsNew)
                    {
                        rowData.CreatedBy = this._session.UserID;
                    }
                    else
                    {
                        rowData.ModifyBy = this._session.UserID;
                    }

                    var dataResult = this.areaListBO.Save(rowData, objReqListAdd.IsNew);
                    if (dataResult)
                        repData.RepData = dataResult;
                    else
                        this.AddResponeError(ref repData, this.areaListBO.getMsgCode(),
                            this.areaListBO.GetMessage(this.areaListBO.getMsgCode(), this.LangID));

                    return repData;
                }
                else
                {
                    this.responeResult = this.CreateResponeResultError(MsgCodeConst.Msg_RequestDataInvalid,
                        MsgCodeConst.Msg_RequestDataInvalidText, "Lỗi tham số gọi API", null);
                    Logger.Error("EXCEPTION-CALL API", new Exception("Lỗi tham số gọi API"));
                    return this.responeResult;
                }
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
        [Route("api/list/deletearealist")]
        public ActionResult<ResponeResult> DeleteAreaList([FromBody] ReqListDelete objReqListDelete)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListDelete.AuthenParams,
                    objReqListDelete.AuthenParams.ClientUserName, objReqListDelete.AuthenParams.ClientPassword,
                    objReqListDelete.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult = this.areaListBO.DeleteAreaList(Convert.ToInt32(objReqListDelete.ID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, this.areaListBO.getMsgCode(),
                        this.areaListBO.GetMessage(this.areaListBO.getMsgCode(), this.LangID));

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
        [Route("api/list/updatesortarealist")]
        public ActionResult<ResponeResult> UpdateSortOrderAreaList([FromBody] ReqListUpdateSortOrder reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult =
                    this.areaListBO.UpdateSortOrder(Convert.ToInt32(reqData.UpID), Convert.ToInt32(reqData.DownID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, areaListBO.getMsgCode(),
                        areaListBO.GetMessage(this.areaListBO.getMsgCode(), this.LangID));

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

        #region ShippedPrice

        [Authorize]
        [HttpPost]
        [Route("api/list/shippedprice")]
        public ResponeResult GetShippedPriceData([FromBody] ReqListSearch objReqListSearch)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListSearch.AuthenParams,
                    objReqListSearch.AuthenParams.ClientUserName, objReqListSearch.AuthenParams.ClientPassword,
                    objReqListSearch.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = shippedpriceBO.GetData(ReplaceUnicode(objReqListSearch.SearchString),
                    objReqListSearch.StartRow,
                    objReqListSearch.MaxRow);
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.shippedpriceBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, shippedpriceBO.getMsgCode(),
                        shippedpriceBO.GetMessage(this.shippedpriceBO.getMsgCode(), this.LangID));

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
        [Route("api/list/saveshippedprice")]
        public ActionResult<ResponeResult> SaveShippedPrice([FromBody] ReqListAdd objReqListAdd)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListAdd.AuthenParams, objReqListAdd.AuthenParams.ClientUserName,
                    objReqListAdd.AuthenParams.ClientPassword, objReqListAdd.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                Business.Models.MasterData.ListDTO.ShippedPrice objData =
                    JsonConvert.DeserializeObject<Business.Models.MasterData.ListDTO.ShippedPrice>(objReqListAdd.RowData
                        .ToString());
                if (objReqListAdd.IsNew)
                {
                    objData.CreatedBy = this._session.UserID;
                    objData.CreatedDate = DateTimeOffset.Now;
                }
                else
                {
                    objData.ModifyBy = this._session.UserID;
                    objData.ModifyDate = DateTimeOffset.Now;
                }

                var dataResult = shippedpriceBO.Save(objData, objReqListAdd.IsNew);

                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, shippedpriceBO.getMsgCode(),
                        shippedpriceBO.GetMessage(this.shippedpriceBO.getMsgCode(), this.LangID));

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
        [Route("api/list/deleteshippedprice")]
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

                var dataResult = shippedpriceBO.Delete(Convert.ToInt32(objReqListDelete.ID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, shippedpriceBO.getMsgCode(),
                        shippedpriceBO.GetMessage(this.shippedpriceBO.getMsgCode(), this.LangID));

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
        [Route("api/list/updateSortOrderShippedPrice")]
        public ActionResult<ResponeResult> UpdateSortOrderShippedPrice([FromBody] ReqListUpdateSortOrder reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult =
                    shippedpriceBO.UpdateSortOrder(Convert.ToInt32(reqData.UpID), Convert.ToInt32(reqData.DownID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, shippedpriceBO.getMsgCode(),
                        shippedpriceBO.GetMessage(this.shippedpriceBO.getMsgCode(), this.LangID));

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

        #region ProductDetail

        [Authorize]
        [HttpPost]
        [Route("api/list/productdetail")]
        public ResponeResult GetProductDetailData([FromBody] ReqListSearch objReqListSearch)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckAuthen();
                if (repData == null || !repData.IsOk)
                    return repData;

                objReqListSearch.SearchString = ReplaceUnicode(objReqListSearch.SearchString);
                var dataResult = this.productDetaiBO.GetData(objReqListSearch);
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.productDetaiBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, productDetaiBO.getMsgCode(),
                        productDetaiBO.GetMessage(this.productDetaiBO.getMsgCode(), this.LangID));

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
        [Route("api/list/saveproductdetail")]
        public ActionResult<ResponeResult> SaveProductDetail([FromBody] ReqListAdd objReqListAdd)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckAuthen();
                if (repData == null || !repData.IsOk)
                    return repData;

                ProductDetail rowData = JsonConvert.DeserializeObject<ProductDetail>(objReqListAdd.RowData.ToString());
                if (rowData != null)
                {
                    rowData.SearchString =
                        ReplaceUnicode(rowData.ProductCode + " " + rowData.ProductName);
                    if (objReqListAdd.IsNew)
                    {
                        rowData.CreatedBy = this._session.UserID;
                    }
                    else
                    {
                        rowData.ModifyBy = this._session.UserID;
                    }

                    var dataResult = this.productDetaiBO.Save(rowData, objReqListAdd.IsNew);
                    if (dataResult)
                        repData.RepData = dataResult;
                    else
                        this.AddResponeError(ref repData, this.productDetaiBO.getMsgCode(),
                            this.productDetaiBO.GetMessage(this.productDetaiBO.getMsgCode(), this.LangID));

                    return repData;
                }
                else
                {
                    this.responeResult = this.CreateResponeResultError(MsgCodeConst.Msg_RequestDataInvalid,
                        MsgCodeConst.Msg_RequestDataInvalidText, "Lỗi tham số gọi API", null);
                    Logger.Error("EXCEPTION-CALL API", new Exception("Lỗi tham số gọi API"));
                    return this.responeResult;
                }
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
        [Route("api/list/deleteproductdetail")]
        public ActionResult<ResponeResult> DeleteProductDetail([FromBody] ReqListDelete objReqListDelete)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListDelete.AuthenParams,
                    objReqListDelete.AuthenParams.ClientUserName, objReqListDelete.AuthenParams.ClientPassword,
                    objReqListDelete.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult = this.productDetaiBO.DeleteProductDetail(Convert.ToInt32(objReqListDelete.ID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, this.productDetaiBO.getMsgCode(),
                        this.productDetaiBO.GetMessage(this.productDetaiBO.getMsgCode(), this.LangID));

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
        [Route("api/list/updatesortproductdetail")]
        public ActionResult<ResponeResult> UpdateSortOrdeProductDetail([FromBody] ReqListUpdateSortOrder reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult =
                    this.productDetaiBO.UpdateSortOrder(Convert.ToInt32(reqData.UpID), Convert.ToInt32(reqData.DownID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, productDetaiBO.getMsgCode(),
                        productDetaiBO.GetMessage(this.productDetaiBO.getMsgCode(), this.LangID));

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
        [Route("api/list/getproductdetailinfo")]
        public ResponeResult GetProductDetailInfo([FromBody] ReqListSearch objReqListSearch)
        {
            ProductDetail rowData = null;
            try
            {
                //Check security & data request
                var repData = this.CheckAuthen();
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult = this.productDetaiBO.GetInfo(objReqListSearch);

                if (dataResult != null)
                {
                    rowData = this.mapper.Map<ProductDetail>(dataResult);

                    #region Load info List CIF Price

                    ReqListSearch reqListSearch = new ReqListSearch();
                    reqListSearch.SearchString = Convert.ToString(rowData.ProductId);

                    var lsttCifprices = this.productCifpriceBO.GetData(reqListSearch);
                    rowData.ListProductCIFPrice = lsttCifprices;

                    #endregion

                    repData.RepData = rowData;
                }
                else
                    this.AddResponeError(ref repData, productDetaiBO.getMsgCode(),
                        productDetaiBO.GetMessage(this.productDetaiBO.getMsgCode(), this.LangID));

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

        #region CustomerDetail

        [Authorize]
        [HttpPost]
        [Route("api/list/customerdetail")]
        public ResponeResult GetCustomerDetailData([FromBody] JObject reqData)
        {
            ReqListSearch reqSerach = new ReqListSearch();
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData, reqSerach.AuthenParams.ClientUserName,
                    reqSerach.AuthenParams.ClientPassword, "");
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = customerDetailBO.GetData(
                    ReplaceUnicode(reqData["dataserach"]["SearchString"].ToString()),
                    reqData["areaId"].ToString() == "" ? (int?)null : Convert.ToInt32(reqData["areaId"].ToString()),
                    Convert.ToInt32(reqData["dataserach"]["StartRow"].ToString()),
                    Convert.ToInt32(reqData["dataserach"]["MaxRow"].ToString()));
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.customerDetailBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, customerDetailBO.getMsgCode(),
                        customerDetailBO.GetMessage(this.customerDetailBO.getMsgCode(), this.LangID));

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
        [Route("api/list/savecustomerdetail")]
        public ActionResult<ResponeResult> SaveCustomerDetail([FromBody] ReqListAdd objReqListAdd)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListAdd.AuthenParams, objReqListAdd.AuthenParams.ClientUserName,
                    objReqListAdd.AuthenParams.ClientPassword, objReqListAdd.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                Business.Models.MasterData.ListDTO.CustomerDetail objData =
                    JsonConvert.DeserializeObject<Business.Models.MasterData.ListDTO.CustomerDetail>(
                        objReqListAdd.RowData.ToString());
                if (objReqListAdd.IsNew)
                {
                    objData.CreatedBy = this._session.UserID;
                    objData.CreatedDate = DateTimeOffset.Now;

                    foreach (Business.Models.MasterData.ListDTO.CustomerDeliveryAddress item in objData
                        .lstCustomerDeliveryAddress)
                    {
                        item.CreatedBy = this._session.UserID;
                        item.CreatedDate = DateTimeOffset.Now;
                    }

                    foreach (Business.Models.MasterData.ListDTO.Sales item in objData.lstSales)
                    {
                        item.CreatedBy = this._session.UserID;
                        item.CreatedDate = DateTimeOffset.Now;
                    }
                }
                else
                {
                    objData.ModifyBy = this._session.UserID;
                    objData.ModifyDate = DateTimeOffset.Now;

                    foreach (Business.Models.MasterData.ListDTO.Sales item in objData.lstSales)
                    {
                        item.ModifyBy = this._session.UserID;
                        item.ModifyDate = DateTimeOffset.Now;
                    }
                }

                var dataResult = customerDetailBO.Save(objData, objReqListAdd.IsNew);

                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, customerDetailBO.getMsgCode(),
                        customerDetailBO.GetMessage(this.customerDetailBO.getMsgCode(), this.LangID));

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
        [Route("api/list/deletecustomerdetail")]
        public ActionResult<ResponeResult> DeleteCustomerDetail([FromBody] ReqListDelete objReqListDelete)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListDelete.AuthenParams,
                    objReqListDelete.AuthenParams.ClientUserName, objReqListDelete.AuthenParams.ClientPassword,
                    objReqListDelete.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult = customerDetailBO.Delete(Convert.ToInt32(objReqListDelete.ID), this._session.UserID);
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, customerDetailBO.getMsgCode(),
                        customerDetailBO.GetMessage(this.customerDetailBO.getMsgCode(), this.LangID));

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
        [Route("api/list/updateSortOrderCustomerDetail")]
        public ActionResult<ResponeResult> UpdateSortOrderCustomerDetail([FromBody] ReqListUpdateSortOrder reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult =
                    customerDetailBO.UpdateSortOrder(Convert.ToInt32(reqData.UpID), Convert.ToInt32(reqData.DownID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, customerDetailBO.getMsgCode(),
                        customerDetailBO.GetMessage(this.customerDetailBO.getMsgCode(), this.LangID));

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
        [Route("api/list/productcache")]
        public ResponeResult GetProductCache([FromBody] ReqListSearch reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var dataResult = this.customerDetailBO.GetProductCache();
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.customerDetailBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, customerDetailBO.getMsgCode(),
                        customerDetailBO.GetMessage(this.customerDetailBO.getMsgCode(), this.LangID));

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

        #region Store

        [Authorize]
        [HttpPost]
        [Route("api/list/store")]
        public ResponeResult GetStoretData([FromBody] ReqListSearch objReqListSearch)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckAuthen();
                if (repData == null || !repData.IsOk)
                    return repData;

                objReqListSearch.SearchString = ReplaceUnicode(objReqListSearch.SearchString);
                var dataResult = this.storeBO.GetData(objReqListSearch);
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.storeBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, storeBO.getMsgCode(),
                        storeBO.GetMessage(this.storeBO.getMsgCode(), this.LangID));

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
        [Route("api/list/savestore")]
        public ActionResult<ResponeResult> SaveStore([FromBody] ReqListAdd objReqListAdd)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckAuthen();
                if (repData == null || !repData.IsOk)
                    return repData;

                Store rowData = JsonConvert.DeserializeObject<Store>(objReqListAdd.RowData.ToString());
                if (rowData != null)
                {
                    if (objReqListAdd.IsNew)
                    {
                        rowData.CreatedBy = this._session.UserID;
                    }
                    else
                    {
                        rowData.ModifyBy = this._session.UserID;
                    }

                    var dataResult = this.storeBO.Save(rowData, objReqListAdd.IsNew);
                    if (dataResult)
                        repData.RepData = dataResult;
                    else
                        this.AddResponeError(ref repData, this.storeBO.getMsgCode(),
                            this.storeBO.GetMessage(this.storeBO.getMsgCode(), this.LangID));

                    return repData;
                }
                else
                {
                    this.responeResult = this.CreateResponeResultError(MsgCodeConst.Msg_RequestDataInvalid,
                        MsgCodeConst.Msg_RequestDataInvalidText, "Lỗi tham số gọi API", null);
                    Logger.Error("EXCEPTION-CALL API", new Exception("Lỗi tham số gọi API"));
                    return this.responeResult;
                }
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
        [Route("api/list/deletestore")]
        public ActionResult<ResponeResult> DeleteStore([FromBody] ReqListDelete objReqListDelete)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(objReqListDelete.AuthenParams,
                    objReqListDelete.AuthenParams.ClientUserName, objReqListDelete.AuthenParams.ClientPassword,
                    objReqListDelete.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult = this.storeBO.DeleteStore(Convert.ToInt32(objReqListDelete.ID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, this.storeBO.getMsgCode(),
                        this.storeBO.GetMessage(this.storeBO.getMsgCode(), this.LangID));

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
        [Route("api/list/updatesortstore")]
        public ActionResult<ResponeResult> UpdateSortOrderStore([FromBody] ReqListUpdateSortOrder reqData)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;

                var dataResult =
                    this.storeBO.UpdateSortOrder(Convert.ToInt32(reqData.UpID), Convert.ToInt32(reqData.DownID));
                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, storeBO.getMsgCode(),
                        storeBO.GetMessage(this.storeBO.getMsgCode(), this.LangID));

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

        #region ReportColumnView

        [HttpPost]
        [Route("api/list/reportcolumnview")]
        public ResponeResult GeReportColumnViewData([FromBody] ReqListSearch objReqListSearch)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckAuthen();
                if (repData == null || !repData.IsOk)
                    return repData;

                objReqListSearch.SearchString = ReplaceUnicode(objReqListSearch.SearchString);
                var dataResult = this.reportColumnViewBO.GetData(objReqListSearch, this._session.UserID);
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.areaListBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, reportColumnViewBO.getMsgCode(),
                        reportColumnViewBO.GetMessage(this.reportColumnViewBO.getMsgCode(), this.LangID));

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

        [HttpPost]
        [Route("api/list/savereportcolumnview")]
        public ActionResult<ResponeResult> SaveReportColumnView([FromBody] ReqListAdd reqData)
        {
            try
            {
                int roleId = -1;
                //Check security & data request
                var repData = this.CheckSign(reqData.AuthenParams, reqData.AuthenParams.ClientUserName,
                    reqData.AuthenParams.ClientPassword, reqData.AuthenParams.Sign);
                if (repData == null || !repData.IsOk)
                    return repData;
                var lstColumn = JsonConvert.DeserializeObject<ReportColumnView[]>(reqData.RowData.ToString());
                var dataResult = reportColumnViewBO.Save(lstColumn, this._session.UserID);

                if (dataResult)
                    repData.RepData = dataResult;
                else
                    this.AddResponeError(ref repData, reportColumnViewBO.getMsgCode(),
                        reportColumnViewBO.GetMessage(this.reportColumnViewBO.getMsgCode(), this.LangID));

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

        [HttpPost]
        [Route("api/list/reporttotal")]
        public ResponeResult GeReportTotalData([FromBody] ReqListSearch objReqListSearch)
        {
            try
            {
                //Check security & data request
                var repData = this.CheckAuthen();
                if (repData == null || !repData.IsOk)
                    return repData;

                objReqListSearch.SearchString = ReplaceUnicode(objReqListSearch.SearchString);
                var dataResult = this.reportTotalBO.GetData(objReqListSearch);
                if (dataResult != null)
                {
                    repData.RepData = dataResult;
                    repData.TotalRow = this.areaListBO.TotalRows;
                }
                else
                    this.AddResponeError(ref repData, reportTotalBO.getMsgCode(),
                        reportTotalBO.GetMessage(this.reportTotalBO.getMsgCode(), this.LangID));

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