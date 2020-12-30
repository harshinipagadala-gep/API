using Gep.Cumulus.CSM.Entities;
using Gep.Cumulus.Partner.Entities;
using GEP.Cumulus.Logging;
using GEP.SMART.Configuration;
using GEP.SMART.Requisition.API.Helpers;
using GEP.SMART.Requisition.BusinessEntities;
using GEP.SMART.Requisition.BusinessEntities.Enums;
using GEP.SMART.Requisition.BusinessObjects.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GEP.SMART.Settings.Entities;

namespace GEP.SMART.Requisition.API.Controllers
{
    [ApiController]
    public class RequisitionController : ControllerBase
    {
        public readonly IRequestHeaders requestHeaders;
        public static readonly ILog Log = Logger.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        public readonly IRequisitionManager requisitionManager;
        public ISettingManager settingManager;

        public RequisitionController(IRequisitionManager requisitionManager, IRequestHeaders requestHeaders, ISettingManager settingManager)
        {
            this.requisitionManager = requisitionManager;
            this.requestHeaders = requestHeaders;
            this.settingManager = settingManager;
            this.requisitionManager.UserContext = requestHeaders.Context;
            this.settingManager.UserContext = requestHeaders.Context;
        }

        private UserExecutionContext UserExecutionContext
        {
            get
            {
                return this.requestHeaders.Context;
            }
        }

        [HttpPost("/api/Req/GetContractIngerationDetails")]
        public string GetContractIngerationDetails([FromBody]JObject data)
        {
            string serviceurl;
            string result = string.Empty;
            try
            {
                serviceurl = ServiceUrlHelper.getP2PRestURL + "/contractrestservice/GetContractIngerationDetails";
                result = new RestURLHelper().InvokeRestUrls(this.UserExecutionContext, serviceurl, data.ToString(), "POST", requestHeaders.JWTtoken);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in GetContractIngerationDetails Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
            return result;
        }

        [HttpPost("/api/Req/GetContractDetailsForRequisition")]
        public string GetContractDetailsForRequisition([FromBody] JObject data)
        {
            string serviceurl;
            string result = string.Empty;
            try
            {
                serviceurl = ServiceUrlHelper.getP2PRestURL + "/contractrestservice/GetcontractDetailsForRequisition";
                result = new RestURLHelper().InvokeRestUrls(this.UserExecutionContext, serviceurl, data.ToString(), "POST", requestHeaders.JWTtoken);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in GetContractDetailsForRequisition Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
            return result;
        }


        [HttpPost("/api/Req/ValidateContractUtilizations")]
        public string ValidateContractUtilizations([FromBody] JObject data)
        {
            string serviceurl;
            string result = string.Empty;

            try
            {
                serviceurl = ServiceUrlHelper.getP2PRestURL + "/contractrestservice/ValidateContractUtilizations";
                result = new RestURLHelper().InvokeRestUrls(this.UserExecutionContext, serviceurl, data.ToString(), "POST", requestHeaders.JWTtoken);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in ValidateContractUtilizations Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
            return result;
        }


        [HttpPost("/api/Req/GetTierPricingDetailsForOtherDocument")]
        public string GetTierPricingDetailsForOtherDocument([FromBody] JObject data)
        {
            string serviceurl;
            string result = string.Empty;

            try
            {
                serviceurl = ServiceUrlHelper.getCatalogRestURL + "/GetTierPricingDetailsForOtherDocument";
                result = new RestURLHelper().InvokeRestUrls(this.UserExecutionContext, serviceurl, data.ToString(), "POST", requestHeaders.JWTtoken);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in GetTierPricingDetailsForOtherDocument Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
            return result;
        }


        [HttpPost("/api/Req/GetLineItems")]
        public string GetLineItems([FromBody] JObject data)
        {
            string serviceurl;
            string result = string.Empty;

            try
            {
                serviceurl = ServiceUrlHelper.getCatalogRestURL + "/GetLineItems";
                result = new RestURLHelper().InvokeRestUrls(this.UserExecutionContext, serviceurl, data.ToString(), "POST", requestHeaders.JWTtoken);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in GetLineItems Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
            return result;
        }

        [HttpPost("/api/Req/IsReviewPendingForUser")]
        public string IsReviewPendingForUser([FromBody] JObject data)
        {
            string serviceurl;
            string result = string.Empty;
            long documentCode = data.SelectToken("documentCode").ToObject<long>();
            long contactCode = data.SelectToken("contactCode").ToObject<long>();
            int wfDocTypeId = data.SelectToken("wfDocTypeId").ToObject<int>();
            try
            {
                serviceurl = ScriptUrlHelper.GetWorkflowRestServiceUrl() + "/IsReviewPendingForUser";
                new RestURLHelper().CreateHttpWebRequest(serviceurl, this.UserExecutionContext, "POST", requestHeaders.JWTtoken);
                Dictionary<string, object> odict = new Dictionary<string, object>();
                odict.Add("documentCode", documentCode);
                odict.Add("contactCode", contactCode);
                odict.Add("wfDocTypeId", wfDocTypeId);
                result = new RestURLHelper().GetHttpWebResponse(odict);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in IsReviewPendingForUser Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
        }


        [HttpPost("/api/Req/ModifyAllApprovalDetails")]
        public string saveAllApprovalDetails([FromBody]JObject data)
        {
            string result = String.Empty;
            string serviceurl;
            ApprovalDetails approvalDetails = null;
            decimal documentAmount;
            try
            {
                approvalDetails = data.SelectToken("approvalDetails").ToObject<ApprovalDetails>();
                documentAmount = data.SelectToken("documentAmount").ToObject<decimal>();
                if (approvalDetails.staticApproval != null && approvalDetails.staticApproval.Any())
                {
                    approvalDetails.staticApproval.ForEach(k => k.ApproverDetails = new List<ApproverDetails>());
                }
                if (approvalDetails.userDefinedApproval != null && approvalDetails.userDefinedApproval.Any())
                {
                    approvalDetails.userDefinedApproval.ForEach(k =>
                    {
                        k.ApproverDetails = new List<ApproverDetails>();
                        k.WorkflowSettings.RemoveAll(setting => setting.SettingName != "GroupId" && setting.SettingName != "SpecificUsers" && setting.SettingName != "SkipCallBackAction");
                    });
                }
                serviceurl = ScriptUrlHelper.GetWorkflowRestServiceUrl() + "/SaveAllApprovalDetails";
                new RestURLHelper().CreateHttpWebRequest(serviceurl, this.UserExecutionContext, "PATCH", requestHeaders.JWTtoken);

                Dictionary<string, object> odict = new Dictionary<string, object>();
                odict.Add("approvalDetails", approvalDetails);
                odict.Add("documentAmount", documentAmount);
                result = new RestURLHelper().GetHttpWebResponse(odict);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in saveAllApprovalDetails Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
            return result;
        }

        [HttpPost("/api/Req/getNewDocumentInstanceId")]
        public string createDocumentInstanceId([FromBody]JObject data)
        {
            string result = String.Empty;
            string serviceurl;
            long documentCode = data.SelectToken("documentCode").ToObject<long>();
            int documentType = data.SelectToken("documentType").ToObject<int>();
            try
            {
                serviceurl = ScriptUrlHelper.GetWorkflowRestServiceUrl() + "/CreateDocumentInstanceId";
                new RestURLHelper().CreateHttpWebRequest(serviceurl, this.UserExecutionContext, "POST", requestHeaders.JWTtoken);

                Dictionary<string, object> odict = new Dictionary<string, object>();
                odict.Add("documentCode", documentCode);
                odict.Add("documentType", documentType);
                result = new RestURLHelper().GetHttpWebResponse(odict);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in getNewDocumentInstanceId Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
            return result;
        }

        [HttpPost("/api/Req/GetSpendControlDocumentAccountingDetailsByLineNumber")]
        public string GetSpendControlDocumentAccountingDetailsByLineNumber([FromBody]JObject data)
        {
            string serviceurl;
            string result = string.Empty;

            try
            {
                serviceurl = ServiceUrlHelper.getP2PRestURL + "/contractrestservice/GetAccountingDetailsByLineNumber";
                result = new RestURLHelper().InvokeRestUrls(this.UserExecutionContext, serviceurl, data.ToString(), "POST", requestHeaders.JWTtoken);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in GetSpendControlDocumentAccountingDetailsByLineNumber Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw ex;
            }
            return result;
        }

        [HttpPost("/api/Req/EncodeDocumentCode")]
        public string EncodeDocumentCode([FromBody]JObject data)
        {
            var url = string.Empty;
            var documentCode = data.SelectToken("documentCode");
            var buyerPartnerCode = requisitionManager.UserContext.BuyerPartnerCode;

            url = ServiceExtensions.EncryptURL($"dc={documentCode}&bpc={buyerPartnerCode}");

            return url;
        }

        [HttpGet("/api/Req/CheckRequisitionCatalogItemAccess")]
        public async Task<IActionResult> CheckRequisitionCatalogItemAccess(long newrequisitionId, string requisitionIds)
        {
            return await requisitionManager
                    .CheckRequisitionCatalogItemAccess(newrequisitionId, requisitionIds)
                    .ToActionResultAsync();
        }

        [HttpGet("/api/Req/CheckCatalogItemsAccessForContactCode")]
        public async Task<IActionResult> CheckCatalogItemsAccessForContactCode(long requesterId, string catalogItems)
        {
            return await requisitionManager
                    .CheckCatalogItemsAccessForContactCode(requesterId, catalogItems)
                    .ToActionResultAsync();
        }

        [HttpGet("/api/Req/GetAllUOMs")]
        public async Task<IActionResult> GetAllUOMs(string term, int pageIndex = 0, int pageSize = 1000)
        {
            return await requisitionManager
                    .GetAllUOMs(term, pageIndex, pageSize)
                    .ToActionResultAsync();
        }

        [HttpGet("/api/Req/GetUserConfigurationsDetails")]
        public async Task<IActionResult> GetUserConfigurationsDetails(long contactCode, int documentType = 7)
        {
            return await requisitionManager
                    .GetUserConfigurationsDetails(contactCode, documentType)
                    .ToActionResultAsync();
        }

        [HttpPost("/api/Req/SaveUserConfigurations")]
        public async Task<IActionResult> SaveUserConfigurations([FromBody]JObject data)
        {
            BusinessEntities.UserConfiguration userConfig = data.SelectToken("userConfig").ToObject<BusinessEntities.UserConfiguration>();

            return await requisitionManager
                    .SaveUserConfigurations(userConfig)
                    .ToActionResultAsync();
        }

        [HttpGet("/api/Req/GetAllSplitAccountingControlValues")]
        public async Task<IActionResult> GetAllSplitAccountingControlValues(int entityTypeId, string term, int parentEntityCode = 0, long LOBEntityDetailCode = 0, int preferenceLOBType = -1, int PageIndex = 0, int PageSize = 10)
        {
            return await requisitionManager
                  .GetAllSplitAccountingControlValues(entityTypeId, term, parentEntityCode, LOBEntityDetailCode, preferenceLOBType, PageIndex, PageSize)
                  .ToActionResultAsync();
        }

        [HttpGet("/api/Req/GetSupplierLocationByLocationType")]
        public async Task<IActionResult> GetSupplierLocationByLocationType(string partnerCode, long accessControlEntityDetailCode, int locationType, List<long> BUIds = null)
        {
            return await requisitionManager
                  .GetSupplierLocationByLocationType(partnerCode, accessControlEntityDetailCode, locationType, BUIds = null)
                  .ToActionResultAsync();
        }

        [HttpPost("/api/Req/GetOrderingLocationByPartnerCode")]
        public async Task<IActionResult> GetOrderingLocationByPartnerCode([FromBody]JObject data)
        {
            long partnerCode = data.SelectToken("partnerCode").ToObject<long>();
            long accessControlEntityDetailCode = data.SelectToken("accessControlEntityDetailCode").ToObject<long>();
            List<long> BUIds = data.SelectToken("BUIds").ToObject<List<long>>();


            return await requisitionManager
                  .GetOrderingLocationByPartnerCodeAsync(partnerCode, accessControlEntityDetailCode, BUIds)
                  .ToActionResultAsync();

        }

        [HttpPost("/api/Req/GetUserActivitiesByContactCode")]
        public async Task<IActionResult> GetUserActivities([FromBody]JObject data)
        {
            long ContactCode = data.SelectToken("contactCode").ToObject<long>();
            long PartnerCode = data.SelectToken("partnerCode").ToObject<long>();

            return await requisitionManager
                  .GetUserActivities(ContactCode, PartnerCode)
                  .ToActionResultAsync();
        }

        [HttpGet("/api/Req/GetTab")]
        public async Task<IActionResult> GetTab(long formId)
        {
            return await requisitionManager
                  .GetTab(formId)
                  .ToActionResultAsync();
        }

        [HttpGet("/api/Req/GetAllUserLOBDetails")]
        public async Task<IActionResult> GetAllUserLOBDetails(long ContactCode, int preferenceLOBType)
        {
            var preferenceLOBTypeId = (Gep.Cumulus.Partner.Entities.PreferenceLOBType)preferenceLOBType;
            return await requisitionManager
                  .GetAllUserLOBDetails(ContactCode, preferenceLOBTypeId)
                  .ToActionResultAsync();
        }


        [HttpPost("/api/Req/GetCurrencyAutoSuggest")]
        public async Task<IActionResult> GetCurrencyAutoSuggest([FromBody]JObject data)
        {
            var currencyAutoSuggestFilterModel = data.ToObject<CurrencyAutoSuggestFilterModel>();
            return await requisitionManager
                  .GetCurrencyAutoSuggest(currencyAutoSuggestFilterModel)
                  .ToActionResultAsync();
        }

        [HttpGet("/api/Req/GetCustomAttrFormId")]
        public async Task<IActionResult> GetCustomAttrFormId(int docType, long categoryId = 0, long LOBEntityDetailCode = 0, int entityId = 0, long entityDetailCode = 0, long documentCode = 0, int purchaseTypeId = 0)
        {
            return await requisitionManager.GetCustomAttrFormId(docType
                , new List<Level>() { Level.Header, Level.Item, Level.Distribution }
                , categoryId, LOBEntityDetailCode
                , entityId, entityDetailCode, documentCode, purchaseTypeId)
                .ToActionResultAsync();
        }
        [Route("~/api/Req/GetAllBuyerSuppliersAutoSuggest")]
        [AcceptVerbs("GET")]
        public async Task<IActionResult> GetAllBuyerSuppliersAutoSuggest(string partnerStatus, string term, long LOBEntityDetailCode, string buList = "", int partnerDisplayFormat = 1, string partnerRelationshipTypesToBeRestricted = "", int pageIndex = 1, int pageSize = 10, long PASCode = 0)
        {
            try
            {
                long contactCode = 0;
                var Result = await requisitionManager
                               .GetUserActivities(this.requisitionManager.UserContext.ContactCode, this.requisitionManager.UserContext.BuyerPartnerCode)
                               .ToActionResultAsync<string>();

                if (Result != null)
                {
                    var userActivities = ((ObjectResult)Result).Value.ToString();

                    if (!string.IsNullOrEmpty(userActivities))
                    {
                        if (this.requisitionManager.UserContext != null && userActivities.IndexOf(UserActivityStatus.VENDOR_BUYER_USER) > -1)
                        {
                            contactCode = this.requisitionManager.UserContext.ContactCode;
                        }
                    }
                }
                return await requisitionManager.GetAllBuyerSuppliersAutoSuggest(partnerStatus, partnerDisplayFormat, term, pageIndex, pageSize, buList, LOBEntityDetailCode, partnerRelationshipTypesToBeRestricted, PASCode, contactCode).ToActionResultAsync();

            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occurred in GetAllBuyerSuppliersAutoSuggest API Method", ex);
                throw;
            }

        }
        [HttpGet("/api/Req/GetRequisitionAdditionalFields")]
        public async Task<IActionResult> GetRequisitionAdditionalFields(long requisitionId)
        {
            return await requisitionManager
                      .GetRequisitionAdditionalFields(requisitionId)
                      .ToActionResultAsync();
        }

        [HttpGet("/api/Req/GetReminderNotificationDetails")]
        public async Task<IActionResult> GetReminderNotificationDetails(long documentCode, long actionerId, bool reminderForApproval)
        {
            try
            {
                return await requisitionManager
                              .GetReminderNotificationDetails(documentCode, actionerId, reminderForApproval)
                              .ToActionResultAsync();
            }
            catch (Exception ex)
            {
                string finalJsonObj = string.Empty;
                LogHelper.LogError(Log, string.Format("Error occured in GetReminderNotificationDetails Method in RequisitionController Buyer Partner Code = {0} , Contact Code = {1},  InnerExceptionMessage = {2},  ParameterValues = {3}.",
                   this.UserExecutionContext.BuyerPartnerCode, this.UserExecutionContext.ContactCode, ex.InnerException != null ? ex.InnerException.Message : "null", finalJsonObj), ex);
                throw;
            }
        }

        [HttpPost("/api/Req/GetRequisitionURL")]
        public async Task<IActionResult> GetRequisitionURL([FromBody]JObject data)
        {
            var requisitionId = data.SelectToken("reqId").ToObject<long>();
            var procurmentProfileId = data.SelectToken("ppid").ToObject<long>();
            var orgEntity = data.SelectToken("orgEntity");
            var requisitionNumber = data.SelectToken("requisitionNumber");


            string encryptedBPC = ServiceExtensions.EncryptURL(this.requisitionManager.UserContext.BuyerPartnerCode.ToString());
            string ppidEncrypted = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(procurmentProfileId.ToString()));
            string requisitionUrl = string.Empty;

            if (IsRequisitionRedirectionToNG5Enabled())
            {
                requisitionUrl = MultiRegionConfig.GetConfig(CloudConfig.SmartBaseURL) + "P2P/Index?dd=" +
                ServiceExtensions.EncryptURL("bpc=" + this.requisitionManager.UserContext.BuyerPartnerCode.ToString() + "&dc=" + requisitionId)
                + "&oe=" + orgEntity + "&ppid=" + ppidEncrypted + "&CF=1&oloc=263#/req5";
            }
            else
            {
                requisitionUrl = MultiRegionConfig.GetConfig(CloudConfig.SmartBaseURL) + "P2P/Index?dd=" +
                ServiceExtensions.EncryptURL("bpc=" + this.requisitionManager.UserContext.BuyerPartnerCode.ToString() + "&dc=" + requisitionId)
                + "&oe=" + orgEntity + "&ppid=" + ppidEncrypted + "&CF=1&oloc=227#&c=" + encryptedBPC + "/requisitions/"
                + ServiceExtensions.EncryptURL("bpc=" + this.requisitionManager.UserContext.BuyerPartnerCode.ToString() + "&dc=" + requisitionId);
            }

            return new JsonResult(
                new
                {
                    RequisitionId = (requisitionId == 0 ? "" : requisitionNumber),
                    RequisitionUrl = requisitionUrl
                }, new JsonSerializerSettings());
        }

        [HttpGet("/api/Req/GetUpdatedRequisitionURL")]
        public async Task<IActionResult> GetUpdatedRequisitionURL(long reqId, long ppid, string orgEntity, string requisitionNumber)
        {
            string encryptedBPC = ServiceExtensions.EncryptURL(this.requisitionManager.UserContext.BuyerPartnerCode.ToString());
            string ppidEncrypted = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ppid.ToString()));
            string requisitionUrl = string.Empty;

            if (IsRequisitionRedirectionToNG5Enabled())
            {
                requisitionUrl = MultiRegionConfig.GetConfig(CloudConfig.SmartBaseURL) + "P2P/Index?dd=" +
                ServiceExtensions.EncryptURL("bpc=" + this.requisitionManager.UserContext.BuyerPartnerCode.ToString() + "&dc=" + reqId)
                + "&oe=" + orgEntity + "&ppid=" + ppidEncrypted + "&CF=1&oloc=263#/req5";
            }
            else
            {
                requisitionUrl = MultiRegionConfig.GetConfig(CloudConfig.SmartBaseURL) + "P2P/Index?dd=" +
                ServiceExtensions.EncryptURL("bpc=" + this.requisitionManager.UserContext.BuyerPartnerCode.ToString() + "&dc=" + reqId)
                + "&oe=" + orgEntity + "&ppid=" + ppidEncrypted + "&CF=1&oloc=227#&c=" + encryptedBPC + "/requisitions/"
                + ServiceExtensions.EncryptURL("bpc=" + this.requisitionManager.UserContext.BuyerPartnerCode.ToString() + "&dc=" + reqId);
            }

            return new JsonResult(
                new
                {
                    RequisitionId = (reqId == 0 ? "" : requisitionNumber),
                    RequisitionUrl = requisitionUrl
                }, new JsonSerializerSettings());
        }

        /// <summary>
        /// IsEnableRequisitionRedirectionToNG5
        /// </summary>
        /// <returns></returns>
        private bool IsRequisitionRedirectionToNG5Enabled()
        {
            bool isRequisitionRedirectionEnabled = false;

            GEP.SMART.Settings.Entities.Settings settings = settingManager
                .GetSettings(SettingDocumentType.Smart2RedirectionSettings, 0,0, (int)GEP.SMART.Settings.Entities.SubAppCodes.Portal).Result;

            if (settings != null && settings.PortalSettings.Count > 0)
            {
                var enableRequisitionRedirectionToNG5 = settings.DocumentSettings.Where(x => x.Key.Equals("EnableRequisitionRedirectionToNG5", StringComparison.InvariantCultureIgnoreCase));

                if (enableRequisitionRedirectionToNG5.Count() > 0)
                    isRequisitionRedirectionEnabled = Convert.ToBoolean(enableRequisitionRedirectionToNG5.FirstOrDefault().Value);
            }

            return isRequisitionRedirectionEnabled;
        }

        [HttpPost("/api/Req/GetManagedataPreferredSupplier")]
        public async Task<IActionResult> GetManagedataPreferredSupplier([FromBody]JObject data)
        {
            
            try
            {
                int partnerDisplayFormat = data["partnerDisplayFormat"].ToObject<int>();
                WorkspaceRestInput jsonsupplierSearchinput = data["jsonsupplierSearchinput"].ToObject<WorkspaceRestInput>();
                if (data["pageIndex"] != null)
                {
                    var pageNumber = data["pageIndex"].ToObject<string>();
                    jsonsupplierSearchinput.Filters[1] = "pageNumber:" + pageNumber;
                }
                
                if (data["term"] != null && !string.Equals(data["term"].ToObject<string>(), "@term"))
                {
                    jsonsupplierSearchinput.SearchKeyword = data["term"].ToObject<string>();
                }
                
                jsonsupplierSearchinput.globalSearchText = jsonsupplierSearchinput.SearchKeyword;
                return await requisitionManager.GetManagedataPreferredSupplier(jsonsupplierSearchinput, partnerDisplayFormat)
                    .ToActionResultAsync(); 
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in GetManagedataPreferredSupplier Method in Req Controller", ex);
                throw;
            }
            
        }
    [HttpGet("/api/Req/GetTaxJurisdcitionsByShiptoLocationIds")]
    public async Task<IActionResult> GetTaxJurisdcitionsByShiptoLocationIds(string shipToLocationIds)
    {
      return await requisitionManager
                .GetTaxJurisdcitionsByShiptoLocationIds(shipToLocationIds)
                .ToActionResultAsync();
    }
        [HttpGet("/api/Req/GetInterfaceStatusDetails")]
        public async Task<IActionResult> getInterfaceStatusDetails(long requisitionId)
        {
            return await requisitionManager
                      .getInterfaceStatusDetails(requisitionId)
                      .ToActionResultAsync();
        }

        [HttpPost("/api/Req/GetPartnerCurrencies")]
        public async Task<IActionResult> GetPartnerCurrencies([FromBody]JObject data)
        {
            string partners = data["partners"].ToObject<string>();
            return await requisitionManager.GetPartnerCurrencies(partners).ToActionResultAsync();
        }

    }
}


