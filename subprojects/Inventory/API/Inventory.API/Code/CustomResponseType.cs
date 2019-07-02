using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace PerkinElmer.COE.Inventory.API.Code
{
    public class CustomResponseType : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.produces.Contains("text/json"))
            {
                operation.produces.Remove("text/json");
            }
            if (!operation.produces.Contains("application/json"))
            {
                operation.produces.Add("application/json");
            }
            if (!operation.produces.Contains("application/xml"))
            {
                operation.produces.Add("application/xml");
            }
            if (!operation.produces.Contains("image/png"))
            {
                operation.produces.Add("image/png");
            }
        }
    }
}