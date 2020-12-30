using GEP.SMART.Requisition.BusinessEntities;
using GEP.SMART.Requisition.BusinessObjects.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GEP.SMART.Requisition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequisitionItemDetailsController : ControllerBase
    {
        private IRequisitionManager requisitionManager;
        private IDataAccessControl dataAccessControl;

        public RequisitionItemDetailsController(IRequisitionManager requisitionManager, IRequestHeaders requestHeaders, IDataAccessControl accessControl)
        {
            this.requisitionManager = requisitionManager;
            this.requisitionManager.UserContext = requestHeaders.Context;

            this.dataAccessControl = accessControl;
        }

        [HttpGet("/api/RequisitionItemDetails/{requisitionId}")]       

        public async Task<IActionResult> RequisitionItemDetails(long requisitionId)
        {
            this.dataAccessControl.CheckDACAccess(requisitionId, 103);

            return await requisitionManager
                .GetItemDetails(requisitionId)
                .ToActionResultAsync();

        }

        [HttpGet("/api/RequisitionItemDetails/AdditionalMasterData/")]        

        public async Task<IActionResult> AdditionalMasterData(long accessControlEntityDetailCode, long lobEntityDetailCode, long partnerId)
        {
            return await requisitionManager
                .GetItemDetailsMaster(accessControlEntityDetailCode, lobEntityDetailCode, partnerId)
                .ToActionResultAsync();
        }                      

        [HttpGet("/api/RequisitionItemDetails/GetAllRequisitionItemAccountingDetails/")]

        public async Task<IActionResult> GetAllRequisitionItemAccountingDetails(long documentCode)
        {
            return await requisitionManager
                .GetAllRequisitionItemAccountingDetails(documentCode)
                .ToActionResultAsync();
        }
    }
}