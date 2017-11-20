using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Library.WebApi.Jwt.Helpers
{
	/// <summary>
	/// Create a response with status of unauthorized and the details about the request that failed.
	/// </summary>
	public class AuthenticationFailureResult : IHttpActionResult
	{
		/// <summary>
		/// Response will have information about the request that failed
		/// </summary>
		/// <param name="jsonContent"></param>
		/// <param name="request"></param>
		public AuthenticationFailureResult(object jsonContent, HttpRequestMessage request)
		{
			JsonContent = jsonContent;
			Request = request;
		}

		/// <summary>
		/// Request that failed
		/// </summary>
		public HttpRequestMessage Request { get; }

		/// <summary>
		/// Json that was received
		/// </summary>
		public object JsonContent { get; }

		/// <inheritdoc />
		public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			return Task.FromResult(Execute());
		}

		private HttpResponseMessage Execute()
		{
			return new HttpResponseMessage(HttpStatusCode.Unauthorized)
			{
				RequestMessage = Request,
				Content = new ObjectContent(JsonContent.GetType(), JsonContent, new JsonMediaTypeFormatter())
			};
		}
	}
}