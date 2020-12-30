using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GEP.SMART.Requisition.API.Controllers
{
    [ApiController]
    [ExcludeFromCodeCoverage]
    public class HealthCheckController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("api/HealthCheck")]
        public string HealthCheck()
        {
            return "Hello From HealthCheck Method";
        }

    }
}