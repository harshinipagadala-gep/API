using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GEP.SMART.Requisition.API.Middleware
{
    [ExcludeFromCodeCoverage]
    public abstract class MiddlewareBase
    {
        public async Task HandleInvoke(HttpContext context, RequestDelegate next, ILogger logger, Func<HttpContext, RequestDelegate, ILogger, Task> handleInvoke)
        {
            try
            {
                 await handleInvoke(context, next, logger);
            }
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    logger.LogError("The response has already started, the http status code middleware will not be executed.");
                    throw;
                }

                logger.LogError("Request Body:" + context.Request.Body.ToString());
                logger.LogError("Exception Message:" + ex.Message);
                logger.LogError("Exception StackTrace:" + ex.StackTrace);

                context.Response.Clear();

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                context.Response.ContentType = "text/json";

                await context.Response.WriteAsync(ex.Message);
            }
            return;
        }
    }
}
