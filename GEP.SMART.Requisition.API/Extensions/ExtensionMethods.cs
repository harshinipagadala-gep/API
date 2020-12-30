using GEP.SMART.Requisition.BusinessEntities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace GEP.SMART.Requisition.API.Controllers
{
    [ExcludeFromCodeCoverage]
    public static class ExtensionMethods
    {
        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<MethodResult<T>> methodResult)
        {
            ActionResult actionResult = null;

            switch (await methodResult)
            {

                case SuccessMethodResult<T> successResult:
                    actionResult = new OkObjectResult(successResult.Result);
                    break;

                case ErrorMethodResult<T> errorResult when errorResult?.FirstError?.ErrorType == ErrorType.IncorrectInput:
                    actionResult = new BadRequestObjectResult(errorResult.ErrorList);
                    break;

                case ErrorMethodResult<T> errorResultServer when errorResultServer?.FirstError?.ErrorType == ErrorType.RemoteSystemError:
                    actionResult = new ObjectResult(errorResultServer.ErrorList)
                    {
                        StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
                    };
                    break;

                case ErrorMethodResult<T> errorResultServer when errorResultServer?.FirstError?.ErrorType == ErrorType.Unauthorized:
                    actionResult = new ObjectResult(errorResultServer.ErrorList)
                    {
                        StatusCode = (int)System.Net.HttpStatusCode.Unauthorized
                    };
                    break;

                case ErrorMethodResult<T> errorResultServer when errorResultServer?.FirstError?.ErrorType == ErrorType.NotFound:
                    actionResult = new NotFoundObjectResult(errorResultServer.ErrorList);
                    break;

                // TODO is there a better way to encapsulate the result then ObjectResult.
                // ObjectResult handles the case of formatting the result automatically

                case ErrorMethodResult<T> errorResultServer when errorResultServer?.FirstError?.ErrorType == ErrorType.SystemError:
                    actionResult = new ObjectResult(errorResultServer.ErrorList)
                    {
                        StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
                    };
                    break;


                case ErrorMethodResult<T> errorResultServer:
                    actionResult = new ObjectResult(errorResultServer.ErrorList)
                    {
                        StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
                    };
                    break;

                default:
                    actionResult = new ObjectResult(methodResult.Result);
                    break;
            }

            return actionResult;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source) action(element);
        }

        public static int Count<T>(this IEnumerable<T> source)
        {
            int index = default(int);
            source.ForEach(i => index++);
            return index;
        }

    }

}
