using GEP.SMART.Requisition.API.Models;
using GEP.SMART.Requisition.BusinessEntities;
using GEP.SMART.Requisition.BusinessObjects.Interfaces;
using GEP.SMART.Settings.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GEP.SMART.Requisition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequisitionBasicDetailsController : ControllerBase
    {
        private IRequisitionManager requisitionManager;
        private ISettingManager settingManager;
        private IDataAccessControl dataAccessControl;

        public RequisitionBasicDetailsController(IRequisitionManager requisitionManager, IRequestHeaders requestHeaders, ISettingManager settingManager, IDataAccessControl accessControl)
        {
            this.requisitionManager = requisitionManager;
            this.requisitionManager.UserContext = requestHeaders.Context;
            this.settingManager = settingManager;
            this.settingManager.UserContext = requestHeaders.Context;

            this.dataAccessControl = accessControl;

        }

        [HttpGet("/api/RequisitionBasicDetailsWithSettings/{requisitionId}")]
        public async Task<IActionResult> RequisitionBasicDetailsWithSettings(long requisitionId)
        {
            this.dataAccessControl.CheckDACAccess(requisitionId, 101);
            var basicDetailsTask = await requisitionManager.GetRequisitionBasicDetailsById(requisitionId);

            BasicDetailsWithSettings basicDetailsWithSettings = new BasicDetailsWithSettings(basicDetailsTask.Result);

            basicDetailsWithSettings.Settings = await settingManager
                .GetSettings(SettingDocumentType.Requisition, basicDetailsTask.Result.documentLOB.entityDetailCode);

            var helper = new ControllerHelper(this.requisitionManager);
            BusinessEntities.MethodResult<System.Data.DataTable> mappedTaxItemsTask = await helper.GetMappedTaxItems(basicDetailsWithSettings.BasicDetails, basicDetailsWithSettings.Settings);

            basicDetailsWithSettings.BasicDetails.MappedTaxItems = mappedTaxItemsTask.Result;

            SetActiveVersion(basicDetailsWithSettings.BasicDetails);

            return new OkObjectResult(basicDetailsWithSettings);
        }

        [HttpGet("/api/RequisitionBasicDetails/{requisitionId}")]

        public async Task<IActionResult> RequisitionBasicDetails(long requisitionId)
        {
            this.dataAccessControl.CheckDACAccess(requisitionId, 102);

            var basicDetailsTask = await requisitionManager
                .GetRequisitionBasicDetailsById(requisitionId);

            var basicDetails = basicDetailsTask.Result;

            var settings = await settingManager
               .GetSettings(SettingDocumentType.Requisition, basicDetails.documentLOB.entityDetailCode);

            var helper = new ControllerHelper(this.requisitionManager);

            var mappedTaxItemsTask = await helper.GetMappedTaxItems(basicDetails, settings);
            basicDetails.MappedTaxItems = mappedTaxItemsTask.Result;

            SetActiveVersion(basicDetails);

            // Get default Shiptolocation
            await SetDefaultShipToLocation(basicDetails, settings);

            // Set default billto location
            await SetDefaultBillToLocation(basicDetails, settings);

            // Set EnableThirdPartyTaxCalculation
            await SetThirdPartyTaxIntegration(basicDetails);

            await SetDefaultShippingMethods(basicDetails, settings);

            return new JsonResult(
                       basicDetails,
                       new JsonSerializerSettings
                       {
                           Formatting = Formatting.Indented,
                           DateFormatHandling = DateFormatHandling.IsoDateFormat,
                           DateTimeZoneHandling = DateTimeZoneHandling.Utc
                       })
            { StatusCode = (int)System.Net.HttpStatusCode.OK };
        }

        private async Task SetThirdPartyTaxIntegration(BusinessEntities.BasicDetails basicDetails)
        {
            var orderSetttings = await settingManager
               .GetSettingsFromSettingsComponent(SettingDocumentType.Order, this.requisitionManager.UserContext.ContactCode, 107, "", basicDetails.documentLOB.entityDetailCode);

            basicDetails.enbaleThirdPartyTaxIntegration = "0";
            if (orderSetttings != null && orderSetttings.lstSettings != null)
            {
                var objEnableThirdPartySetting = orderSetttings.lstSettings.FirstOrDefault(x => x.FieldName == "EnableThirdPartyTaxCalculation");
                if (objEnableThirdPartySetting != null)
                {
                    basicDetails.enbaleThirdPartyTaxIntegration = objEnableThirdPartySetting.FieldValue != null ? Convert.ToString(objEnableThirdPartySetting.FieldValue) : "0";
                }
            }
        }

        private async Task SetDefaultShippingMethods(BusinessEntities.BasicDetails basicDetails, Settings.Entities.Settings settings)
        {
            long defaultentitydetailcode = 0;
            long lOBEntityDetailCode = 0;

            if (basicDetails.documentLOB != null)
            {
                lOBEntityDetailCode = basicDetails.documentLOB.entityDetailCode;
            }

            long DefaultHeaderEntityFromCommonSettingsForShippingMethods = 0;

            var EntityMappedToShippingMethods = settings.CommonSettings["EntityMappedToShippingMethods"];
            if (EntityMappedToShippingMethods != null)
            {
                string s = EntityMappedToShippingMethods.ToString();
                long.TryParse(s, out DefaultHeaderEntityFromCommonSettingsForShippingMethods);
                if (DefaultHeaderEntityFromCommonSettingsForShippingMethods > 0)
                {
                    var defaultHeaderEntityForShippingMethods = basicDetails.headerSplitAccountingFields.Where(x => x.EntityTypeId == DefaultHeaderEntityFromCommonSettingsForShippingMethods).FirstOrDefault();
                    if (defaultHeaderEntityForShippingMethods != null)
                    {
                        defaultentitydetailcode = defaultHeaderEntityForShippingMethods.EntityDetailCode;
                        if (lOBEntityDetailCode == 0) lOBEntityDetailCode = defaultHeaderEntityForShippingMethods.LOBEntityDetailCode;
                    }
                }
            }
            if (basicDetails.headerSplitAccountingFields != null && basicDetails.headerSplitAccountingFields.Count > 0)
            {

                if (defaultentitydetailcode == 0) defaultentitydetailcode = basicDetails.headerSplitAccountingFields[0].EntityDetailCode;
                if (lOBEntityDetailCode == 0) lOBEntityDetailCode = basicDetails.headerSplitAccountingFields[0].LOBEntityDetailCode;               
            }

            var getShippingMethodsDetailsTask = await requisitionManager.GetShippingMethods(defaultentitydetailcode, lOBEntityDetailCode);
            basicDetails.ShippingMethods = getShippingMethodsDetailsTask.Result;
        }

        private async Task SetDefaultBillToLocation(BusinessEntities.BasicDetails basicDetails, Settings.Entities.Settings settings)
        {
            long defaultentitydetailcode = 0;
            long lOBEntityDetailCode = 0;

            long DefaultHeaderEntityFromCommonSettingsForBillToLocation = 0;
            BusinessEntities.SplitAccountingFields DefaultHeaderEntityForBillToLocation = null;
            var EntityMappedToBillToLocation = settings.CommonSettings["EntityMappedToBillToLocation"];
            if (EntityMappedToBillToLocation != null)
            {
                string s = EntityMappedToBillToLocation.ToString();
                long.TryParse(s, out DefaultHeaderEntityFromCommonSettingsForBillToLocation);
                if (DefaultHeaderEntityFromCommonSettingsForBillToLocation > 0)
                {
                    DefaultHeaderEntityForBillToLocation = basicDetails.headerSplitAccountingFields.Where(x => x.EntityTypeId == DefaultHeaderEntityFromCommonSettingsForBillToLocation).FirstOrDefault();
                    if (DefaultHeaderEntityForBillToLocation != null)
                    {
                        defaultentitydetailcode = DefaultHeaderEntityForBillToLocation.EntityDetailCode;
                        lOBEntityDetailCode = DefaultHeaderEntityForBillToLocation.LOBEntityDetailCode;
                    }
                }

                if (basicDetails.headerSplitAccountingFields != null && basicDetails.headerSplitAccountingFields.Count > 0)
                {

                    if (defaultentitydetailcode == 0) defaultentitydetailcode = basicDetails.headerSplitAccountingFields[0].EntityDetailCode;
                    if (lOBEntityDetailCode == 0) lOBEntityDetailCode = basicDetails.headerSplitAccountingFields[0].LOBEntityDetailCode;                    
                }
            }

            var getBillToLocationDetailsTask = await this.requisitionManager
            .GetListofBillToLocDetails("", 0, 0, defaultentitydetailcode, true, lOBEntityDetailCode);

            var billToLocations = getBillToLocationDetailsTask.Result;
            if (billToLocations != null && billToLocations.Count > 0)
            {
                basicDetails.UserDefaultBillToLocation = billToLocations.FirstOrDefault();
            }

        }

        private async Task SetDefaultShipToLocation(BusinessEntities.BasicDetails basicDetails, Settings.Entities.Settings settings)
        {
            if (requisitionManager.UserContext.ShipToLocationId > 0)
            {
                long DefaultHeaderEntityFromCommonSettingsForShipToLocation = 0;
                long DefaultHeaderEntityForShipToLocation = 0;
                var EntityMappedToShipToLocation = settings.CommonSettings["EntityMappedToShipToLocation"];
                if (EntityMappedToShipToLocation != null)
                {
                    string s = EntityMappedToShipToLocation.ToString();
                    long.TryParse(s, out DefaultHeaderEntityFromCommonSettingsForShipToLocation);
                    if (DefaultHeaderEntityFromCommonSettingsForShipToLocation > 0)
                    {
                        DefaultHeaderEntityForShipToLocation = basicDetails.headerSplitAccountingFields.Where(x => x.EntityTypeId == DefaultHeaderEntityFromCommonSettingsForShipToLocation).Select(x => x.EntityDetailCode).FirstOrDefault();
                    }
                }
                var lobEntityDetailCode = basicDetails.documentLOB.entityDetailCode;

                // Call manager to get shiptolocation based on above parameters
                var shipToList = await requisitionManager
                    .GetListofShipToLocDetails(string.Empty, 0, 0, true, requisitionManager.UserContext.ShipToLocationId, lobEntityDetailCode, DefaultHeaderEntityForShipToLocation);

                var shipto = shipToList.Result.FirstOrDefault();

                basicDetails.UserDefaultShipToLocation = shipto;
            }
        }

        private void SetActiveVersion(BusinessEntities.BasicDetails basicDetails)
        {
            if (basicDetails.parentDocumentCode > 0)
            {
                basicDetails.ActiveVersion = ServiceExtensions.EncryptURL("bpc=" + this.requisitionManager.UserContext.BuyerPartnerCode + "&dc=" + basicDetails.parentDocumentCode);
            }

            if (basicDetails.changeRequisitionDocumentCode > 0)
            {
                basicDetails.ChangedVersion = ServiceExtensions.EncryptURL("bpc=" + this.requisitionManager.UserContext.BuyerPartnerCode + "&dc=" + basicDetails.changeRequisitionDocumentCode);
            }
        }

        [HttpGet("/api/RequisitionBasicDetails/RequisitionMaster")]
        public async Task<IActionResult> RequisitionMaster()
        {
            return await requisitionManager
                .GetRequisitionMaster()
                .ToActionResultAsync();
        }
    }
}
