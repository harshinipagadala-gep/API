using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GEP.SMART.Requisition.API.Middleware
{
    [ExcludeFromCodeCoverage]
    public class ExceptionMiddleware : MiddlewareBase
    {
        private RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> dependecyResolverlogger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            next = next ?? throw new ArgumentNullException(nameof(next));
            dependecyResolverlogger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            await HandleInvoke(context, next, dependecyResolverlogger, (ctx, nxt, logger) =>
            {
                return nxt(ctx);
            });

            return;
        }
    }
}
