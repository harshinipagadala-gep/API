using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GEP.SMART.Requisition.API.Controllers.Filters
{
    [ExcludeFromCodeCoverage]
    public class SwaggerBuyerPartnerCodeFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<IParameter>();

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "BuyerPartnerCode",
                In = "header",
                Type = "int",
                Required = true 
            });
        }
    }
}
