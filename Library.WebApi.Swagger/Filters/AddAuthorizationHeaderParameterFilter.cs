using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using Swashbuckle.Swagger;

namespace Library.WebApi.Swagger.Filters
{
	/// <summary>
	/// Used to add an input parameter to endpoints to accept a JWT token. Pilfered from https://github.com/domaindrivendev/Swashbuckle/issues/290
	/// Call config.OperationFilter<AddAuthorizationHeaderParameterFilter>(); within your .EnableSwagger(config => {} ); block to enable. Note: Only applies it to
	/// methods that have the Authorize attribute applied
	/// </summary>
	public class AddAuthorizationHeaderParameterFilter : IOperationFilter
	{
		public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
		{
			var filterPipeline = apiDescription.ActionDescriptor.GetFilterPipeline();
			var isAuthorized = filterPipeline
				.Select(filterInfo => filterInfo.Instance)
				.Any(filter => filter is IAuthorizationFilter);

			var allowAnonymous = apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

			if (isAuthorized && !allowAnonymous)
			{
				operation.parameters.Add(
					new Parameter
					{
						name = "Authorization",
						@default = "Bearer JWTTokenGoesHere",
						@in = "header",
						description = "access token",
						required = true,
						type = "string"
					});
			}
		}
	}
}