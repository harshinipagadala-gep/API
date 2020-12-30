using GEP.SMART.Requisition.BusinessEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Diagnostics.CodeAnalysis;

namespace GEP.SMART.Requisition.API.Controllers.Filters
{
    [ExcludeFromCodeCoverage]
    public class HandleUnexpectedErrorAttribute : ExceptionFilterAttribute
    {

        ILogger<HandleUnexpectedErrorAttribute> logger;
        public HandleUnexpectedErrorAttribute(ILogger<HandleUnexpectedErrorAttribute> logger)
        {
            this.logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            ErrorCollection result = new ErrorCollection();
            int statusCode = (int)HttpStatusCode.BadRequest;

            Error error = null;

            if (context.Exception.InnerException is CustomException customException)
            {
                error = new FieldError()
                {
                    ErrorField = context.Exception.Message,
                    ErrorType = ErrorType.CustomException,
                    ErrorMessage = customException.customMessage
                };
            }
            else if (context.Exception is ArgumentException argumentException)
            {
                error = new FieldError()
                {
                    ErrorField = argumentException.ParamName,
                    ErrorType = ErrorType.IncorrectInput,
                    ErrorMessage = argumentException.Message
                };
            }
            else
            {
                error = new Error()
                {
                    ErrorType = ErrorType.SystemError,
                    ErrorMessage = context.Exception.Message
                };
                statusCode = (int)HttpStatusCode.InternalServerError;
            }

            result.AddError(error);

            //result.Errors.ForEach(err => logger.LogError(err.ErrorMessage, err.ErrorType));

            context.ExceptionHandled = true;

            context.Result = new ObjectResult(result)
            {
                StatusCode = statusCode
            };
        }
    }
}
