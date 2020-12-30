using GEP.Cumulus.Logging;
using GEP.SMART.Configuration;
using GEP.SMART.Requisition.BusinessEntities;
using GEP.SMART.Requisition.BusinessObjects.Interfaces;
using GEP.SMART.Settings.Entities;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GEP.SMART.Requisition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequisitionBudgetController : ControllerBase
    {
        public static readonly ILog Log = Logger.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        private IRequisitionManagerForBudget requisitionManagerForBudget;
        public ISettingManager settingManager;
        public readonly IRequisitionManager requisitionManager;

        public RequisitionBudgetController(IRequisitionManagerForBudget requisitionManagerForBudget,IRequestHeaders requestHeaders,
                                           ISettingManager settingManager, IRequisitionManager requisitionManager) 
        {
            this.requisitionManagerForBudget = requisitionManagerForBudget;
            this.settingManager = settingManager;
            this.requisitionManager = requisitionManager;
            this.requisitionManagerForBudget.UserContext = requestHeaders.Context;            
            this.settingManager.UserContext = requestHeaders.Context;
            this.requisitionManager.UserContext = requestHeaders.Context;
        }

        [HttpPost("/api/RequisitionBudget/GetDocumentStatusFromDocumentCode")]
        public async Task<IActionResult> GetDocumentStatusFromDocumentCode([FromBody] JObject data)
        {
            try
            {               
                List<long> lstDocumentCodes = data.SelectToken("lstDocumentCodes").ToObject<List<long>>();
                if (lstDocumentCodes != null && lstDocumentCodes.Count > 0)
                {
                    return await requisitionManagerForBudget
                       .GetDocumentStatusFromDocumentCode(lstDocumentCodes)
                       .ToActionResultAsync();
                }                
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in GetDocumentStatusFromDocumentCode Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
            return new ObjectResult(new SuccessMethodResult<Dictionary<long, string>>(new Dictionary<long, string>()));
        }

        [HttpPost("/api/RequisitionBudget/GetRequisitionDetailsForControllerDashboard")]
        public async Task<IActionResult> GetRequisitionDetailsForControllerDashboard([FromBody] JObject data)
        {
            try
            {
                List<long> lstDocumentCodes = data.SelectToken("lstDocumentCodes").ToObject<List<long>>();
                List<string> lstBudgetAaccountingEntities = data.SelectToken("lstBudgetAaccountingEntities").ToObject<List<string>>();

                if (lstDocumentCodes != null && lstDocumentCodes.Count > 0)
                {
                   var requisitionListWithURL = PrepareRequisitionURL(lstDocumentCodes);
                    
                    return await requisitionManagerForBudget
                       .GetRequisitionDetailsForControllerDashboard(requisitionListWithURL, lstBudgetAaccountingEntities).ToActionResultAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in GetDocumentStatusFromDocumentCode Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
            return new ObjectResult(new SuccessMethodResult<List<RequisitionEntityForBudget>>(new List<RequisitionEntityForBudget>()));
        }

        [HttpGet("/api/RequisitionBudget/GetApprovalPendingRequisitionsForBudget")]
        public async Task<IActionResult> GetApprovalPendingRequisitionsForBudget()
        {
            try
            {               
                return await requisitionManagerForBudget
                    .GetApprovalPendingRequisitionsForBudget()
                    .ToActionResultAsync();                
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in GetApprovalPendingRequisitionsForBudget Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }            
        }

        private bool IsRequisitionRedirectionToNG5Enabled()
        {
            bool isRequisitionRedirectionEnabled = false;

            Settings.Entities.Settings settings = settingManager
                .GetSettings(SettingDocumentType.Smart2RedirectionSettings, 0, 0, (int)SubAppCodes.Portal).Result;

            if (settings != null && settings.PortalSettings != null && settings.PortalSettings.Count > 0)
            {
                var enableRequisitionRedirectionToNG5 = settings.DocumentSettings.Where(x => x.Key.Equals("EnableRequisitionRedirectionToNG5", StringComparison.InvariantCultureIgnoreCase));

                if (enableRequisitionRedirectionToNG5.Count() > 0)
                    isRequisitionRedirectionEnabled = Convert.ToBoolean(enableRequisitionRedirectionToNG5.FirstOrDefault().Value);
            }

            return isRequisitionRedirectionEnabled;
        }

        private Dictionary<long, string> PrepareRequisitionURL(List<long> lstDocumentCodes)
        {
            Dictionary<long, string> keyValuePairs = new Dictionary<long, string>();
            string encryptedBPC = ServiceExtensions.EncryptURL(this.requisitionManager.UserContext.BuyerPartnerCode.ToString());

            if (lstDocumentCodes != null && lstDocumentCodes.Count > 0)
            {
                foreach (var item in lstDocumentCodes)
                {
                    if (IsRequisitionRedirectionToNG5Enabled())
                    {
                        keyValuePairs.Add(item, MultiRegionConfig.GetConfig(CloudConfig.SmartBaseURL) + "P2P/Index?dd=" +
                        ServiceExtensions.EncryptURL("bpc=" + this.requisitionManager.UserContext.BuyerPartnerCode.ToString() +
                        "&dc=" + item) + "&oloc=263#/req5");
                    }
                    else
                    {
                        keyValuePairs.Add(item, MultiRegionConfig.GetConfig(CloudConfig.SmartBaseURL) + "P2P/Index?dd=" +
                        ServiceExtensions.EncryptURL("bpc=" + this.requisitionManager.UserContext.BuyerPartnerCode.ToString() + "&dc=" + item)
                        + "&oloc=227#&c=" + encryptedBPC + "/requisitions/"+
                        ServiceExtensions.EncryptURL("bpc=" + this.requisitionManager.UserContext.BuyerPartnerCode.ToString() + "&dc=" + item));
                    }
                }
            }
            return keyValuePairs; 
        }
    }
}
