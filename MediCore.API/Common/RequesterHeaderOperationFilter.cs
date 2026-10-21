using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MediCore.API.Common
{
    //public class RequesterHeaderOperationFilter : IOperationFilter
    //{
    //    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    //    {
    //        operation.Parameters ??= new List<OpenApiParameter>();

    //        operation.Parameters.Add(new OpenApiParameter
    //        {
    //            Name = "X-User-Id",
    //            In = ParameterLocation.Header,
    //            Required = false,
    //            Schema = new OpenApiSchema { Type = "string", Format = "uuid" },
    //            Description = "TEMP: the current requester's UserId (Guid) — e.g. the Doctor's Doctors.UserId column."
    //        });

    //        operation.Parameters.Add(new OpenApiParameter
    //        {
    //            Name = "X-User-Role",
    //            In = ParameterLocation.Header,
    //            Required = false,
    //            Schema = new OpenApiSchema { Type = "string" },
    //            Description = "TEMP: the current requester's role, e.g. Doctor / Patient / Pharmacist."
    //        });
    //    }
    //}
}
