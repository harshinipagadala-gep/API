using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;

namespace GEP.SMART.Requisition.API.Middleware
{
    [ExcludeFromCodeCoverage]
    public static class ExceptionMiddlewareExtension
    {
        public static IApplicationBuilder UseExceptionMiddleware(
          this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
