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
    [Route("api/[controller]")]
    [ApiController]
    public class RequisitionAdditionalFieldsController : ControllerBase
    {
        public readonly IRequestHeaders requestHeaders;
        public static readonly ILog Log = Logger.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        public readonly IRequisitionManager requisitionManager;

        public RequisitionAdditionalFieldsController(IRequisitionManager requisitionManager, IRequestHeaders requestHeaders)
        {
            this.requisitionManager = requisitionManager;
            this.requestHeaders = requestHeaders;
            this.requisitionManager.UserContext = requestHeaders.Context;
        }

        [HttpPost("/api/Req/GetRequisitionAdditionalFieldsData")]
        public async Task<IActionResult> GetRequisitionAdditionalFieldsData([FromBody] JObject data)
        {
            
            long RequisitionID = data.SelectToken("RequisitionID").ToObject<long>();
            Int16 LevelType = data.SelectToken("LevelType").ToObject<Int16>();
            string RequisitionItemIDs = data.SelectToken("RequisitionItemIDs").ToObject<string>();
      int FlipDocumentType = 0;
      if (data.SelectToken("FlipDocumentType") != null)
       FlipDocumentType = data.SelectToken("FlipDocumentType").ToObject<int>();
      
      return await requisitionManager
                      .GetRequisitionAdditionalFieldsData(RequisitionID, LevelType, RequisitionItemIDs, FlipDocumentType)
                      .ToActionResultAsync();
        }

    }
}