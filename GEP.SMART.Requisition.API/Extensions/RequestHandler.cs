using Gep.Cumulus.CSM.Entities;
using GEP.Cumulus.Logging;
using GEP.SMART.Requisition.API.Helpers;
using GEP.SMART.Requisition.BusinessEntities;
using GEP.SMART.Security.ClaimsManagerCore;
using log4net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GEP.SMART.Requisition.API
{
    [ExcludeFromCodeCoverage]

    public class RequestHandler
    {
        private static readonly ILog Log = Logger.GetLog(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly RequestDelegate _next;
        private const string swaggerJsonPath = "/swagger";
        private const string healthCheckPath = "/HealthCheck";

        public RequestHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IRequestHeaders requestHeaders)
        {
            try
            {
                if (!httpContext.Request.Path.Value.Contains(healthCheckPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!httpContext.Request.Path.Value.Contains(swaggerJsonPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var isAuthenticated = SmartClaimsManager.IsAuthenticated();
                        if (isAuthenticated)
                        {
                            var appName = httpContext.Request.Headers.FirstOrDefault(x => x.Key.ToLower() == "appname").Value.ToString();
                            if (string.IsNullOrEmpty(appName))
                            {
                                // if AppName is not available in request header, then read from configurations
                                appName = System.Environment.GetEnvironmentVariable("NewRelic.AppName") ?? "RequisitionCoreAPI";
                            }

                            var useCase = httpContext.Request.Headers.FirstOrDefault(x => x.Key.ToLower() == "usecase").Value.ToString();
                            if (string.IsNullOrEmpty(useCase))
                            {
                                // if useCase is not available in request header, then set value
                                useCase = "RequisitionCoreAPI";
                            }

                            var transactionId = httpContext.Request.Headers.FirstOrDefault(x => x.Key.ToLower() == "transactionid").Value.ToString();
                            if (string.IsNullOrEmpty(transactionId))
                            {
                                // if transactionId is not available in request header, then set value
                                transactionId = (Guid.NewGuid()).ToString();
                            }

                            //var activities = SmartClaimsManager.Activities;
                            string token = GEP.SMART.Security.ClaimsManagerCore.JwtTokenHelper.CreateJwtTokenFromClaimsIdentity();
                            if (!token.Contains("Bearer"))
                            {
                                token = "Bearer " + token;
                            }

                            // Check if user execution context is there, read from headers
                            var userexecutioncontext = httpContext.Request.Headers.FirstOrDefault(x => x.Key.ToLower() == "userexecutioncontext").Value.ToString();
                            if (!string.IsNullOrEmpty(userexecutioncontext))
                            {
                                var userExecution = JObject.Parse(userexecutioncontext).ToObject(typeof(UserExecutionContext)) as UserExecutionContext;
                                requestHeaders.Set(userexecutioncontext, userExecution, token, appName, useCase, transactionId);
                            }
                            else
                            {
                                // When UserExecutionContext is not coming in header, we will need to build that using token
                                PartnerHelper partnerHelper = new PartnerHelper();
                                var tmpUserExecutionContext = partnerHelper.CreateUserExecutionContextFromJWT();

                                // Set complete context using partner service
                                if (tmpUserExecutionContext.ContactCode > 0)
                                {
                                    string tmpContextJSON = Newtonsoft.Json.JsonConvert.SerializeObject(tmpUserExecutionContext);
                                    var userExecutionContext = partnerHelper.GetUserContext(tmpUserExecutionContext.ContactCode, token, tmpContextJSON, tmpUserExecutionContext.BuyerPartnerCode);
                                    userExecutionContext.BuyerPartnerCode = tmpUserExecutionContext.BuyerPartnerCode;
                                    userExecutionContext.ClientName = tmpUserExecutionContext.ClientName;
                                    string contextJSON = Newtonsoft.Json.JsonConvert.SerializeObject(userExecutionContext);
                                    requestHeaders.Set(contextJSON, userExecutionContext, token, appName, useCase, transactionId);
                                }
                                else
                                {
                                    string contextJSON = Newtonsoft.Json.JsonConvert.SerializeObject(tmpUserExecutionContext);
                                    requestHeaders.Set(contextJSON, tmpUserExecutionContext, token, appName, useCase, transactionId);
                                }
                            }

                            // Set current principle object, this code needs to be removed later as Core is not supporting it in new releases
                            Thread.CurrentPrincipal = SmartClaimsManager.Current.User;
                        }
                        else
                        {
                            httpContext.Response.StatusCode = 401;
                            await httpContext.Response.WriteAsync("Access to Requisition Core Web API is denied. Please check the user session is alive or valid token is passed.");
                            return;
                        }
                    }
                }
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await httpContext.Response.WriteAsync("Either Requisition Core Web API is not available or \n Error :" + ex.Message);
            }


        }
    }
}