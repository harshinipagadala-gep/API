using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace GEP.SMART.Requisition.API
{
    [ExcludeFromCodeCoverage]
    public static class RequisitionMiddleWare
    {
        public static IApplicationBuilder UseReqMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestHandler>();
        }
    }
}
