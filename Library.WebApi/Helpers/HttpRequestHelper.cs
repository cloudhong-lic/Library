using System.Net.Http;
using System.Web;

namespace Library.WebApi.Helpers
{
	public static class HttpRequestHelper
	{
		/// <summary>
		/// Creates a HttpRequestMessage out of HttpRequestBase
		/// http://aspnetwebstack.codeplex.com/SourceControl/changeset/view/4764b0111b91#src/System.Web.Http.WebHost/HttpControllerHandler.cs
		/// </summary>
		/// <param name="message">The message to set headers on.</param>
		/// <param name="request">The request with headers to clone.</param>
		public static HttpRequestMessage ToHttpRequestMessage(this HttpRequestBase request)
		{
			HttpMethod httpmethod = null;

			switch (request.HttpMethod)
			{
				case ("GET"):
					httpmethod = HttpMethod.Get;
					break;

				case ("POST"):
					httpmethod = HttpMethod.Post;
					break;

				case ("PUT"):
					httpmethod = HttpMethod.Put;
					break;

				case ("DELETE"):
					httpmethod = HttpMethod.Delete;
					break;

				case ("HEAD"):
					httpmethod = HttpMethod.Head;
					break;

				case ("OPTIONS"):
					httpmethod = HttpMethod.Options;
					break;

				case ("TRACE"):
					httpmethod = HttpMethod.Trace;
					break;

				default:
					return null;
			}
			var message = new HttpRequestMessage(httpmethod, request.Url);

			foreach (string headerName in request.Headers)
			{
				string[] headerValues = request.Headers.GetValues(headerName);
				if (!message.Headers.TryAddWithoutValidation(headerName, headerValues))
				{
					//No need to send headers via the content if we don't have any contents!
					if (message.Content != null)
					{
						message.Content.Headers.TryAddWithoutValidation(headerName, headerValues);
					}
				}
			}
			return message;
		}
	}
}