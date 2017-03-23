using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Library.WebApi
{
	public class FriendlyHttpResponseException : HttpResponseException
	{
		/// <summary>
		///     Constructs a HttpResponseException
		/// </summary>
		/// <param name="statusCode">The http status code of the exception that has occured</param>
		/// <param name="reason">A short phrase (under 512 characters) typically matching the status code</param>
		/// <param name="content">
		///     More information - usually a stacktrace or a data dump that might exceed 512 characters. If this
		///     is not set the reason will be used instead
		/// </param>
		public FriendlyHttpResponseException(HttpStatusCode statusCode, string reason, string content = null)
			: base(new HttpResponseMessage
			{
				StatusCode = statusCode,
				//Reason phrase doesn't like new line characters - and internally will crash if the reason is over 512 characters.
				//https://msdn.microsoft.com/en-us/library/system.web.httpresponse.statusdescription(v=vs.110).aspx (the string gets piped through to HttpResponse.StatusDescription at some stage)
				ReasonPhrase = Regex.Replace(reason.Length > 512 ? reason.Substring(0, 512) : reason, @"\r\n?|\n", string.Empty),
				Content = statusCode == HttpStatusCode.NoContent
					? null
					: content != null
						? new StringContent(string.Format("{0} {1} - {2} \r\n {3}", (int) statusCode, statusCode, reason, content))
						: new StringContent(string.Format("{0} {1} - {2}", (int) statusCode, statusCode, reason))
			})
		{
		}
	}
}