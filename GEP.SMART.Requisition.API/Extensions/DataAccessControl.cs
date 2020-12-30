using Gep.Cumulus.CSM.Entities;
using GEP.Cumulus.Logging;
using GEP.SMART.Requisition.BusinessEntities;
using GEP.SMART.Security.ClaimsManagerCore;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using GEP.SMART.Settings.Entities;
using System.Linq;

namespace GEP.SMART.Requisition.API
{
    public interface IDataAccessControl
    {
        bool CheckDACAccess(long documentCode, int value);
    }

    [ExcludeFromCodeCoverage]

    public class DataAccessControl : IDataAccessControl
    {
        private ISettingManager settingManager;
        private const string UsecaseName = "Search-DataAccessControl";
        private static readonly ILog Log = Logger.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        private UserExecutionContext userExecutionContext { get; set; }
        private string JWTToken { get; set; }

        int[] _enableDAC;
        protected int[] EnableDAC
        {
            get
            {
                if (_enableDAC == null)
                {
                    var reqSettingDetails = settingManager.GetSettings(SettingDocumentType.Requisition).Result;
                    var dac = reqSettingDetails.DocumentSettings.ContainsKey("EnableDAC");
                    if (dac)
                    {
                        var EnableDAC = Convert.ToString(reqSettingDetails.DocumentSettings["EnableDAC"] ?? string.Empty);
                        if (!string.IsNullOrEmpty(EnableDAC))
                        {
                            _enableDAC = EnableDAC.Split(',').Select(n => Convert.ToInt32(n)).ToArray(); 
                        }
                    }                   
                }
                return _enableDAC;
            }
        }

        public DataAccessControl(IRequestHeaders requestHeaders, ISettingManager settingManager)
        {
            this.JWTToken = requestHeaders.JWTtoken;
            this.userExecutionContext = requestHeaders.Context;

            this.settingManager = settingManager;
            this.settingManager.UserContext = requestHeaders.Context;

        }
        public bool CheckDACAccess(long documentCode, int value)
        {
            if (documentCode == 0 || SmartClaimsManager.IsSystemUser)
            {
                return true;
            }

            var enable = EnableDAC != null ? EnableDAC.Contains(value) : false;
            if (enable)
            {
                return CheckAccess(documentCode);
            }
            else
            {
                return true;
            }
        }


        private bool CheckAccess(long requisitionId)
        {
            try
            {                
                var IsValid = Smart.Platform.SearchCoreIntegretor.Helpers.Helpers.TemplateHelpers.Concrete.DataAccessControlHelper
                    .DocumentAccessCheck(requisitionId, 7, userExecutionContext, JWTToken);

                if (!IsValid)
                {
                    throw new UnauthorizedAccessException("Access to the document denied");
                }
                return IsValid;
            }
            catch (UnauthorizedAccessException ex)
            {
                LogNewRelicAppForException(ex, "DocumentCode:" + requisitionId.ToString());
                throw;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in CheckAccess in DAC (for GEP.SMART.Requisition.WebAPI), DocumentCode = " + requisitionId.ToString(), ex);
                throw;
            }
        }

        private void LogNewRelicAppForException(Exception ex, string message)
        {
            var eventAttributes = new Dictionary<string, object>();
            eventAttributes.Add("Exception", ex.Message);
            eventAttributes.Add("StackTrace", ex.StackTrace);
            eventAttributes.Add("Message", message);
            string innerException = string.Empty;
            if (ex.InnerException != null)
            {
                innerException = ex.InnerException.Message;
            }
            eventAttributes.Add("innerException", innerException);
            NewRelic.Api.Agent.NewRelic.RecordCustomEvent("RequisitionCoreAPI_DAC", eventAttributes);
        }
    }
}
