using GEP.SMART.Requisition.BusinessEntities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GEP.SMART.Requisition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequisitionSettingsController : ControllerBase
    {
        public GEP.SMART.Settings.Entities.ISettingManager settingManager;
        private IDataAccessControl dataAccessControl;

        public RequisitionSettingsController(GEP.SMART.Settings.Entities.ISettingManager settingManager, IRequestHeaders requestHeaders, IDataAccessControl accessControl)
        {
            this.settingManager = settingManager;
            this.settingManager.UserContext = requestHeaders.Context;

            this.dataAccessControl = accessControl;
        }

        [HttpPost("/api/RequisitionSettings/Settings")]

        public async Task<IActionResult> Settings(string edc = "", long lobId = 0, int subAppCode = 0, string sourceSystem = "")
        {
            var documentCode = GEP.SMART.Requisition.BusinessObjects.CoreEncryptionHelper.Decrypt(edc, this.settingManager.UserContext.ContactCode);
            Settings.Entities.SettingInput settingInput = new Settings.Entities.SettingInput()
            {
                documentCode = documentCode,
                documentType = 7,
                lobId = lobId,
                sourceSystem = sourceSystem,
                subAppCode = subAppCode
            };

            var docType = (Settings.Entities.SettingDocumentType)settingInput.documentType;
            var result = await settingManager.GetSettings(docType, settingInput.lobId, settingInput.documentCode, settingInput.subAppCode, settingInput.sourceSystem);

            result = await settingManager.ParseSettings(result);

            return new OkObjectResult(result);
        }

        [HttpGet("/api/RequisitionSettings/GetSettings")]

        public async Task<IActionResult> GetSettings(int documentType = 7, long documentCode = 0, long lobId = 0, int subAppCode = 0, string sourceSystem = "")
        {
            this.dataAccessControl.CheckDACAccess(documentCode, 104);

            Settings.Entities.SettingInput settingInput = new Settings.Entities.SettingInput()
            {
                documentCode = documentCode,
                documentType = documentType,
                lobId = lobId,
                sourceSystem = sourceSystem,
                subAppCode = subAppCode
            };

            var docType = (Settings.Entities.SettingDocumentType)settingInput.documentType;
            var result = await settingManager.GetSettings(docType, settingInput.lobId, settingInput.documentCode, settingInput.subAppCode, settingInput.sourceSystem);
            return new OkObjectResult(result);
        }

        [HttpGet("/api/RequisitionSettings/GetSettingsAfterParsing")]

        public async Task<IActionResult> GetSettingsAfterParsing(int documentType = 7, long documentCode = 0, long lobId = 0, int subAppCode = 0, string sourceSystem = "")
        {

            this.dataAccessControl.CheckDACAccess(documentCode, 105);

            Settings.Entities.SettingInput settingInput = new Settings.Entities.SettingInput()
            {
                documentCode = documentCode,
                documentType = documentType,
                lobId = lobId,
                sourceSystem = sourceSystem,
                subAppCode = subAppCode
            };

            var docType = (Settings.Entities.SettingDocumentType)settingInput.documentType;
            var result = await settingManager.GetSettings(docType, settingInput.lobId, settingInput.documentCode, settingInput.subAppCode, settingInput.sourceSystem);

            result = await settingManager.ParseSettings(result);

            return new OkObjectResult(result);
        }
    }
}