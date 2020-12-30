using GEP.SMART.Requisition.BusinessEntities;
using GEP.SMART.Requisition.BusinessObjects.Interfaces;
using GEP.SMART.Settings.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using enums = GEP.SMART.Requisition.BusinessEntities.Enums;

namespace GEP.SMART.Requisition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequisitionAccessTypeController : ControllerBase
    {
        public readonly IRequisitionManager requisitionManager;
        public const int DocumentTypeId = 7;
        public RequisitionAccessTypeController(IRequisitionManager requisitionManager, IRequestHeaders requestHeaders)
        {
            this.requisitionManager = requisitionManager;
            this.requisitionManager.UserContext = requestHeaders.Context;
        }

        [HttpGet("/api/RequisitionAccessType/GetAccessTypeForComments")]
        public async Task<IActionResult> GetAccessTypeForComments(long DocumentCode = 0, long ContactCode = 0, long PartnerCode = 0)
        {
            if (ContactCode == 0)
                ContactCode = this.requisitionManager.UserContext.ContactCode;

            if (PartnerCode == 0)
                PartnerCode = this.requisitionManager.UserContext.BuyerPartnerCode;

            var oAccessType = "" + (int)enums.AccessType.BUYERANDPARTNER;

            var Result = await requisitionManager
                                .GetUserActivities(ContactCode, PartnerCode)
                                .ToActionResultAsync<string>();

            var userActivities = ((ObjectResult)Result).Value.ToString();
            if (!string.IsNullOrEmpty(userActivities))
            {
                //Returns if vendor buyer user
                if (DocumentTypeId == (int)SettingDocumentType.Requisition && userActivities.IndexOf(UserActivityStatus.VENDOR_BUYER_USER) > -1)
                    return new OkObjectResult(oAccessType);

                var isBuyer = (userActivities.IndexOf(UserActivityStatus.CREATE_ORDER) > -1 ||
                               userActivities.IndexOf(UserActivityStatus.FLIP_REQUISITIONS_TO_ORDER) > -1) ? true : false;

                var isAP = (userActivities.IndexOf(UserActivityStatus.CREATE_INVOICE) > -1 ||
                            userActivities.IndexOf(UserActivityStatus.CREATE_INVOICE_BY_FLIPPING_ORDER) > -1) ? true : false;

                var isRequester = (userActivities.IndexOf(UserActivityStatus.CREATE_REQUISITION) > -1) ? true : false;

                if (!this.requisitionManager.UserContext.IsSupplier)
                {
                    oAccessType += "," + (int)enums.AccessType.BUYER;

                    if (isAP)
                        oAccessType += "," + (int)enums.AccessType.APUSERSONLY;
                    if (isBuyer)
                        oAccessType += "," + (int)enums.AccessType.BUYERSONLY;
                    if (isRequester)
                        oAccessType += "," + (int)enums.AccessType.REQUESTERSONLY;
                }
            }

            if (DocumentCode > 0)
            {
                var documentStakeHoldersresult = await requisitionManager
                                                .GetDocumentStakeholderDetails(DocumentCode)
                                                .ToActionResultAsync<List<DocumentStakeHolder>>();
                var documentStakeHolders = (List<DocumentStakeHolder>)((ObjectResult)documentStakeHoldersresult).Value;

                if (documentStakeHolders != null
                    && documentStakeHolders.Count() > 0
                    && !this.requisitionManager.UserContext.IsSupplier)
                {
                    var approverUser = documentStakeHolders
                                            .FirstOrDefault(a => a.ContactCode == this.requisitionManager.UserContext.ContactCode &&
                                                                    a.StakeholderTypeInfo == StakeholderType.Approver);
                    if (approverUser != null)
                        oAccessType += "," + (int)enums.AccessType.APPROVER;
                }
            }

            return new OkObjectResult(oAccessType);
        }
    }

}