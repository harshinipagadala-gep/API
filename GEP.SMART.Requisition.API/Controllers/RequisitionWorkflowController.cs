using Gep.Cumulus.CSM.Entities;
using Gep.Cumulus.Partner.Entities;
using GEP.Cumulus.Logging;
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

namespace GEP.SMART.Requisition.API.Controllers
{
    [ApiController]
    public class RequisitionWorkflowController : ControllerBase
    {
        public readonly IRequestHeaders requestHeaders;
        public static readonly ILog Log = Logger.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        public readonly IRequisitionManager requisitionManager;

        public RequisitionWorkflowController(IRequisitionManager requisitionManager, IRequestHeaders requestHeaders)
        {
            this.requisitionManager = requisitionManager;
            this.requestHeaders = requestHeaders;

            this.requisitionManager.UserContext = requestHeaders.Context;
        }

        private UserExecutionContext UserExecutionContext
        {
            get
            {
                return this.requestHeaders.Context;
            }
        }

        [HttpGet("/api/Req/Workflow/GetDocumentApprovalCycle")]
        public Dictionary<string, object> GetDocumentApprovalCycle(long documentCode, int documentType)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                var HasAccessToViewEntity = requisitionManager.HasAccessToViewEntity(documentCode).Result.Result;
                string serviceurl = ScriptUrlHelper.GetWorkflowRestServiceUrl() + "/GetDocumentApprovalCycle";
                new RestURLHelper().CreateHttpWebRequest(serviceurl, this.UserExecutionContext, "POST", requestHeaders.JWTtoken);
                Dictionary<string, object> odict = new Dictionary<string, object>();

                odict.Add("documentCode", documentCode);
                odict.Add("documentType", documentType);

                result.Add("documentApprovalCycle", new RestURLHelper().GetHttpWebResponse(odict));
                result.Add("HasAccessToViewEntity", HasAccessToViewEntity);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in GetContractIngerationDetails Method in RequisitionController (for GEP.SMART.Requisition.API)", ex);
                throw;
            }
            return result;
        }

    }

}


