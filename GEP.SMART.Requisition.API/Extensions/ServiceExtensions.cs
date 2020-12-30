using GEP.SMART.Requisition.BusinessEntities;
using GEP.SMART.Requisition.BusinessObjects;
using GEP.SMART.Requisition.BusinessObjects.Interfaces;
using GEP.SMART.Requisition.DataAccessObjects;
using GEP.SMART.Requisition.DataAccessObjects.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace GEP.SMART.Requisition.API
{
    [ExcludeFromCodeCoverage]
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IRequestHeaders, RequestHeaders>();
            services.AddTransient<IRequisitionManager, RequisitionManager>();
            services.AddTransient<IRequisitionDAO, RequisitionDAO>();
            services.AddTransient<IURLHelper, URLHelper>();
            services.AddTransient<IDataAccessControl, DataAccessControl>();
            services.AddTransient<GEP.SMART.Settings.Entities.ISettingManager, GEP.SMART.Settings.SettingManager>();
            services.AddTransient<GEP.SMART.Settings.Entities.ISettingDAO, GEP.SMART.Settings.SettingDAO>();
            services.AddTransient<IWorkflowServiceManager, WorkflowServiceManager>();
            services.AddTransient<IRequisitionManagerForBudget, RequisitionManagerForBudget>();
            services.AddTransient<IRequisitionDAOForBudget, RequisitionDAOForBudget>();
            services.AddHttpClient<IHttpClientHelper, HttpClientHelper>();

            return services;
        }      
        public static string EncryptURL(string querystring)
        {
            byte[] base64EncodedBytes = Encoding.UTF8.GetBytes(querystring);           
            var result = Utils.Base64UrlTokenEncode(base64EncodedBytes);
            return result;
        }      
    }

    [ExcludeFromCodeCoverage]
    public static class Utils
    {
        private static readonly Regex InvalidBase64UrlTokens = new Regex(
            @"[^=a-z0-9]",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static string Base64UrlTokenEncode(byte[] data)
        {
            var padding = 0;
            var base64String = Convert.ToBase64String(data);
            return InvalidBase64UrlTokens.Replace(base64String, m => {
                switch (m.Value)
                {
                    case "+": return "-";
                    case "=":
                        padding++;
                        return "";
                    default: return "_";
                }
            }) + padding;
        }
    }
}
