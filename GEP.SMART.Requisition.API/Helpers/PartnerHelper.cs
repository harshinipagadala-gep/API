using System.Diagnostics.CodeAnalysis;
using Gep.Cumulus.CSM.Entities;
using Gep.Cumulus.Partner.Entities;
using GEP.Cumulus.Logging;
using GEP.SMART.Configuration;
using GEP.SMART.Requisition.BusinessEntities;
using GEP.SMART.Requisition.BusinessObjects;
using GEP.SMART.Security.ClaimsManagerCore;
using log4net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GEP.SMART.Requisition.API.Helpers
{
    [ExcludeFromCodeCoverage]

    public class PartnerHelper
    {
        private static readonly ILog Log = Logger.GetLog(MethodBase.GetCurrentMethod().DeclaringType);

        public UserExecutionContext GetUserContext(long ContactCode, string jwtToken, string ContextJSON, long BuyerPartnerCode)
        {
            UserExecutionContext userExecutionContext = null;
            try
            {
                // Call WebAPI using Platform APIs (User Management), below classes exists into RequisitionService solution
                string UserManagementServiceURL = "/usermanagement/api/UserManagement/";
                var serviceURL = string.Concat(MultiRegionConfig.GetConfig(CloudConfig.APIMBaseURL), UserManagementServiceURL);
                var headers = new Dictionary<string, string>
                                {
                                    { "UserExecutionContext", ContextJSON },
                                    { "Ocp-Apim-Subscription-Key", MultiRegionConfig.GetConfig(CloudConfig.APIMSubscriptionKey) },
                                    { "BPC", BuyerPartnerCode.ToString() },
                                    { "RegionID", MultiRegionConfig.GetConfig(CloudConfig.PrimaryRegion) },
                                    { "Authorization", jwtToken },
                                    { "GEPSmartTransactionId", Guid.NewGuid().ToString() },
                                    { "TransactionId", Guid.NewGuid().ToString() },
                                    { "AppName", System.Environment.GetEnvironmentVariable("NewRelic.AppName") },
                                    { "UseCase", "RequestHandler" }
                };

                var result = ExecutePost(serviceURL + "GetUserDetailsWithLOBMappingByContactCode", ContactCode, headers);
                var getUserDetailsWithLOBMappingByContactCodeResponse = JsonConvert.DeserializeObject<GetUserDetailsWithLOBMappingByContactCodeResponse>(result);
                var tmpUserContext = Map(getUserDetailsWithLOBMappingByContactCodeResponse);
                userExecutionContext = SetUserExecutionContext(tmpUserContext);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return userExecutionContext;
        }

        private UserExecutionContext SetUserExecutionContext(UserContext userContext)
        {
            UserExecutionContext userExecutionContext = new UserExecutionContext();

            userExecutionContext.CompanyName = "BuyerSqlConn";
            userExecutionContext.ClientID = userContext.Partners.Any() ? userContext.Partners[0].PartnerCode : 0;
            userExecutionContext.ContactCode = userContext.ContactCode;
            userExecutionContext.Product = GEPSuite.eCompany;
            userExecutionContext.EntityId = 0;
            userExecutionContext.EntityType = string.Empty;
            userExecutionContext.LoggerCode = "EC101";
            userExecutionContext.Culture = userContext.CultureCode;
            userExecutionContext.UserId = userContext.UserId;
            userExecutionContext.UserName = userContext.UserName;
            userExecutionContext.DefaultCurrencyCode = userContext.DefaultCurrencyCode;

            if (GEP.SMART.Security.ClaimsManagerCore.SmartClaimsManager.IsAuthenticated())
            {
                var activities = GEP.SMART.Security.ClaimsManagerCore.SmartClaimsManager.Activities;
                userExecutionContext.IsAdmin = activities.Contains(CommonConstants.ADMIN_ACTIVITY_CODE) ? true : false;
            }
            userExecutionContext.IsSupplier = userContext.TypeOfUser == Gep.Cumulus.Partner.Entities.User.UserType.Buyer ? false : true;

            if (!userExecutionContext.IsSupplier)
            {
                var belongingLOBDts = userContext.GetDefaultBelongingUserLOBMapping();
                var servingLOBDtls = userContext.GetDefaultServingUserLOBMapping();
                userExecutionContext.BelongingEntityDetailCode = belongingLOBDts != null ? belongingLOBDts.EntityDetailCode : 0;
                userExecutionContext.ServingEntityDetailCode = servingLOBDtls != null ? servingLOBDtls.EntityDetailCode : 0;
                userExecutionContext.BelongingEntityId = belongingLOBDts != null ? belongingLOBDts.EntityId : 0;
                userExecutionContext.ServingEntityId = servingLOBDtls != null ? servingLOBDtls.EntityId : 0;
                userExecutionContext.ShipToLocationId = userContext.ShipToLocationId;
            }

            return userExecutionContext;
        }

        public UserExecutionContext CreateUserExecutionContextFromJWT()
        {
            return new UserExecutionContext()
            {
                BuyerPartnerCode = SmartClaimsManager.BuyerPartnerCode,
                ClientID = SmartClaimsManager.BuyerPartnerCode,
                UserName = SmartClaimsManager.UserName,
                Culture = SmartClaimsManager.CultureCode,
                ContactCode = SmartClaimsManager.ContactCode,
                UserId = Convert.ToInt32(SmartClaimsManager.UserId),
                BuyerPartnerName = SmartClaimsManager.BuyerPartnerName,
                ClientName = SmartClaimsManager.BuyerPartnerName,
                CompanyName = "BuyerSqlConn"
            };
        }

        private string ExecutePost(string path, object body, Dictionary<string, string> headers, int timeOut = 50000)
        {
            string response = string.Empty;
            var content = JsonConvert.SerializeObject(body);
            try
            {
                Uri address = new Uri(path);
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                SetRequest(request, headers);
                request.Timeout = timeOut;
                request.Method = "POST";
                request.ContentType = "application/json";
                if (!string.IsNullOrEmpty(content))
                {
                    byte[] byteData = UTF8Encoding.UTF8.GetBytes(content);
                    request.ContentLength = byteData.Length;
                    using (Stream postStream = request.GetRequestStream())
                    {
                        postStream.Write(byteData, 0, byteData.Length);
                    }
                }
                else
                {
                    request.ContentLength = 0;
                }
                using (HttpWebResponse result = request.GetResponse() as HttpWebResponse)
                {
                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        StreamReader reader = new StreamReader(result.GetResponseStream());
                        response = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(Log, "Error occured in RequestHandler ExecutePost for URL:" + path + "  and exception: " + ex.Message, ex);
                throw;
            }
            finally
            {

            }

            return response;
        }

        private void SetRequest(HttpWebRequest request, Dictionary<string, string> headers)
        {
            foreach (var item in headers)
            {
                request.Headers.Add(item.Key, item.Value);
            }
        }

        private UserContext Map(GetUserDetailsWithLOBMappingByContactCodeResponse input)
        {
            UserContext output = new UserContext();
            if (input != null)
            {
                output = new UserContext()
                {
                    UserId = input.UserDetailWithUserPreferences.UserId,
                    UserName = input.UserDetailWithUserPreferences.UserName,
                    CultureCode = input.UserDetailWithUserPreferences.CultureCode,
                    FirstName = input.UserDetailWithUserPreferences.FirstName,
                    LastName = input.UserDetailWithUserPreferences.LastName,
                    EmailAddress = input.UserDetailWithUserPreferences.EmailAddress,
                    ContactCode = input.UserDetailWithUserPreferences.ContactCode,
                    TimeZone = input.UserDetailWithUserPreferences.TimeZone,
                    ShipToLocationId = input.UserDetailWithUserPreferences.ShipToLocationId,
                    TypeOfUser = (User.UserType)input.UserDetailWithUserPreferences.UserType,
                    ChangePassword = input.UserDetailWithUserPreferences.ChangePassword,
                    HideWelComeDialogBox = input.UserDetailWithUserPreferences.HideWelComeDialogBox,
                    DefaultCurrencyCode = input.UserDetailWithUserPreferences.DefaultCurrencyCode,
                    Partners = new List<PartnerUserContext>(),
                    UserLOBMapping = new List<Gep.Cumulus.Partner.Entities.UserLOBMapping>()
                };
                foreach (var code in input.PartnerCodes)
                {
                    output.Partners.Add(new PartnerUserContext() { PartnerCode = code });
                }
                foreach (var a in input.UserLOBMapping)
                {
                    output.UserLOBMapping.Add(new Gep.Cumulus.Partner.Entities.UserLOBMapping()
                    {
                        ContactLOBMappingId = a.ContactLOBMappingId,
                        EntityCode = a.EntityCode,
                        EntityDetailCode = a.EntityDetailCode,
                        EntityDisplayName = a.EntityDisplayName,
                        EntityId = a.EntityId,
                        IsDefault = a.IsDefault,
                        PreferenceLobType = (Gep.Cumulus.Partner.Entities.PreferenceLOBType)a.PreferenceLobType
                    });
                }
            }
            return output;
        }

    }
}
