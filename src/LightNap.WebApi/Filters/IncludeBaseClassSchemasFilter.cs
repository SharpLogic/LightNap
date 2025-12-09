using LightNap.Core.Users.Dto.Request;
using LightNap.Core.Users.Dto.Response;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LightNap.WebApi.Filters
{
    public class IncludeBaseClassSchemasFilter : ISchemaFilter
    {
        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            // Ensure these DTOs are always included so we get the full inheritance hierarchy in Swagger
            // and frontend types generated for convenience
            var typesToInclude = new[]
            {
                typeof(PublicSearchUsersRequestDto),
                typeof(PrivilegedSearchUsersRequestDto),
                typeof(AdminSearchUsersRequestDto),
                typeof(PublicUserDto),
                typeof(PrivilegedUserDto),
                typeof(AdminUserDto),
            };

            if (typesToInclude.Contains(context.Type))
            {
            }
        }
    }
}
