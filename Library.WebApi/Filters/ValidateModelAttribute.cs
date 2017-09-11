using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Library.WebApi.Filters
{
	/// <summary>
	/// Checks ModelState and returns a Bad Request response (400) if invalid
	/// This means that the controller action will not be called.
	/// </summary>
	public class ValidateModelAttribute : ActionFilterAttribute
	{
		/// <inheritdoc />
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			var modelState = actionContext.ModelState;

			if (!modelState.IsValid)
			{
				actionContext.Response = actionContext.Request
					.CreateErrorResponse(HttpStatusCode.BadRequest, modelState);
			}
		}
	}
}